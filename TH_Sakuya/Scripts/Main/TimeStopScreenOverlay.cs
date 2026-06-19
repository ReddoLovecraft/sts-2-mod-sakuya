using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scripts.Main;

public static class TimeStopScreenOverlay
{
	private const string OverlayRootName = "TimeStopGrayscaleOverlay";
	private const string SceneOverlayName = "TimeStopGrayscaleOverlay_scene";
	private const string BackVfxOverlayName = "TimeStopGrayscaleOverlay_back-vfx";
	private const string CombatVfxOverlayName = "TimeStopGrayscaleOverlay_combat-vfx";
	private const string OverlayRectName = "Rect";
	private const string ShaderPath = "res://TH_Sakuya/ArtWorks/VFX/screen_grayscale.gdshader";
	private const int OverlayZIndex = -8;
	private const int ExemptCreatureZIndex = OverlayZIndex + 1;
	// #region debug-point A:config
	private const string DebugEnvPath = ".dbg/timestop-toggle-crash.env";
	private const string DebugFallbackUrl = "http://127.0.0.1:7777/event";
	private const string DebugSessionId = "timestop-toggle-crash";
	private const string DebugRunId = "pre-fix";
	private static readonly System.Net.Http.HttpClient _debugHttp = new System.Net.Http.HttpClient();
	// #endregion

	private static BackBufferCopy? _sceneOverlayRoot;
	private static ColorRect? _sceneOverlayRect;
	private static BackBufferCopy? _backVfxOverlayRoot;
	private static ColorRect? _backVfxOverlayRect;
	private static BackBufferCopy? _combatVfxOverlayRoot;
	private static ColorRect? _combatVfxOverlayRect;
	private static ShaderMaterial? _mat;
	private static readonly Dictionary<NCreature, SavedZ> _savedZ = new Dictionary<NCreature, SavedZ>();

	// #region debug-point A:report
	private static void ReportDebug(string hypothesisId, string location, string msg, object data)
	{
		Task.Run(async () =>
		{
			try
			{
				string url = DebugFallbackUrl;
				string sessionId = DebugSessionId;
				if (File.Exists(DebugEnvPath))
				{
					foreach (string line in File.ReadAllLines(DebugEnvPath))
					{
						if (line.StartsWith("DEBUG_SERVER_URL=", StringComparison.Ordinal))
						{
							url = line["DEBUG_SERVER_URL=".Length..];
						}
						else if (line.StartsWith("DEBUG_SESSION_ID=", StringComparison.Ordinal))
						{
							sessionId = line["DEBUG_SESSION_ID=".Length..];
						}
					}
				}
				string payload = JsonSerializer.Serialize(new
				{
					sessionId,
					runId = DebugRunId,
					hypothesisId,
					location,
					msg = "[DEBUG] " + msg,
					data,
					ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
				});
				using System.Net.Http.StringContent content = new System.Net.Http.StringContent(payload, Encoding.UTF8, "application/json");
				await _debugHttp.PostAsync(url, content);
			}
			catch
			{
			}
		});
	}
	// #endregion

	public static void ApplyIfNeeded()
	{
		PruneInvalidState();
		// #region debug-point A:apply-enter
		ReportDebug("A", "TimeStopScreenOverlay.ApplyIfNeeded", "enter", new
		{
			overlayCount = CountValidOverlays(),
			roomExists = NCombatRoom.Instance != null
		});
		// #endregion
		NCombatRoom? room = NCombatRoom.Instance;
		if (room == null)
		{
			return;
		}
		CleanupLegacyOverlays(room);
		EnsureShaderMaterial();
		int createdCount = EnsureOverlays(room);
		UpdateOverlayRectsIfPossible();
		RefreshExemptCreatures();
		// #region debug-point A:create
		ReportDebug("A", "TimeStopScreenOverlay.ApplyIfNeeded", "ensured-overlay-root", new
		{
			createdCount,
			overlayCount = CountValidOverlays(),
			sceneParent = _sceneOverlayRoot?.GetParent()?.Name.ToString(),
			backVfxParent = _backVfxOverlayRoot?.GetParent()?.Name.ToString(),
			combatVfxParent = _combatVfxOverlayRoot?.GetParent()?.Name.ToString(),
			sceneContainerZ = room.SceneContainer?.ZIndex,
			backVfxZ = room.BackCombatVfxContainer?.ZIndex,
			combatVfxZ = room.CombatVfxContainer?.ZIndex
		});
		// #endregion
	}

	public static void Restore()
	{
		PruneInvalidState();
		// #region debug-point E:restore
		ReportDebug("E", "TimeStopScreenOverlay.Restore", "restore-begin", new
		{
			savedCount = _savedZ.Count,
			overlayCount = CountValidOverlays()
		});
		// #endregion
		foreach (var kv in _savedZ)
		{
			if (GodotObject.IsInstanceValid(kv.Key))
			{
				kv.Value.Restore(kv.Key);
			}
		}
		_savedZ.Clear();
		SetOverlayVisibility(false);
	}

	public static void Reset()
	{
		PruneInvalidState();
		SetOverlayVisibility(false);
		_savedZ.Clear();
		_sceneOverlayRoot = null;
		_sceneOverlayRect = null;
		_backVfxOverlayRoot = null;
		_backVfxOverlayRect = null;
		_combatVfxOverlayRoot = null;
		_combatVfxOverlayRect = null;
	}

	public static void RefreshExemptCreatures()
	{
		PruneInvalidSavedCreatures();
		if (!HasVisibleOverlays())
		{
			return;
		}
		NCombatRoom? room = NCombatRoom.Instance;
		if (room == null)
		{
			return;
		}
		int exemptCount = 0;
		int creatureCount = 0;
		foreach (NCreature node in room.CreatureNodes)
		{
			if (node == null || node.Entity == null)
			{
				continue;
			}
			creatureCount++;
			bool exempt = node.Entity.HasPower<SakuyaClock>();
			if (exempt)
			{
				exemptCount++;
				if (!_savedZ.ContainsKey(node))
				{
					_savedZ[node] = SavedZ.Capture(node);
				}
				node.ZAsRelative = false;
				node.ZIndex = ExemptCreatureZIndex;
			}
			else if (_savedZ.TryGetValue(node, out SavedZ saved))
			{
				saved.Restore(node);
				_savedZ.Remove(node);
			}
		}
		// #region debug-point B:refresh
		ReportDebug("B", "TimeStopScreenOverlay.RefreshExemptCreatures", "refresh-exempt-creatures", new
		{
			creatureCount,
			exemptCount,
			savedCount = _savedZ.Count,
			overlayCount = CountValidOverlays()
		});
		// #endregion
	}

	private static void UpdateOverlayRectsIfPossible()
	{
		UpdateOverlayRectIfPossible(_sceneOverlayRoot, _sceneOverlayRect);
		UpdateOverlayRectIfPossible(_backVfxOverlayRoot, _backVfxOverlayRect);
		UpdateOverlayRectIfPossible(_combatVfxOverlayRoot, _combatVfxOverlayRect);
	}

	private static void UpdateOverlayRectIfPossible(BackBufferCopy? overlayRoot, ColorRect? overlayRect)
	{
		if (!GodotObject.IsInstanceValid(overlayRoot) || !GodotObject.IsInstanceValid(overlayRect))
		{
			return;
		}
		Vector2 viewportSize = overlayRoot.GetViewportRect().Size;
		overlayRect.SetDeferred(Control.PropertyName.Size, viewportSize);
	}

	private static void EnsureShaderMaterial()
	{
		if (_mat != null)
		{
			return;
		}
		Shader shader = GD.Load<Shader>(ShaderPath);
		_mat = new ShaderMaterial { Shader = shader };
	}

	private static int EnsureOverlays(NCombatRoom room)
	{
		int createdCount = 0;
		if (EnsureOverlay(room.SceneContainer, SceneOverlayName, ref _sceneOverlayRoot, ref _sceneOverlayRect))
		{
			createdCount++;
		}
		if (EnsureOverlay(room.BackCombatVfxContainer, BackVfxOverlayName, ref _backVfxOverlayRoot, ref _backVfxOverlayRect))
		{
			createdCount++;
		}
		if (EnsureOverlay(room.CombatVfxContainer, CombatVfxOverlayName, ref _combatVfxOverlayRoot, ref _combatVfxOverlayRect))
		{
			createdCount++;
		}
		return createdCount;
	}

	private static bool EnsureOverlay(Node? parent, string overlayName, ref BackBufferCopy? overlayRoot, ref ColorRect? overlayRect)
	{
		if (parent == null)
		{
			overlayRoot = null;
			overlayRect = null;
			return false;
		}
		if (GodotObject.IsInstanceValid(overlayRoot) && GodotObject.IsInstanceValid(overlayRect) && overlayRoot.GetParent() == parent)
		{
			ApplyOverlayProperties(overlayRoot, overlayRect);
			return false;
		}
		if (GodotObject.IsInstanceValid(overlayRoot) && overlayRoot.GetParent() != parent)
		{
			overlayRoot.QueueFreeSafely();
		}
		overlayRoot = null;
		overlayRect = null;
		BackBufferCopy? existingRoot = parent.GetNodeOrNull<BackBufferCopy>(overlayName);
		if (existingRoot != null)
		{
			ColorRect? existingRect = existingRoot.GetNodeOrNull<ColorRect>(OverlayRectName);
			if (existingRect != null)
			{
				overlayRoot = existingRoot;
				overlayRect = existingRect;
				ApplyOverlayProperties(existingRoot, existingRect);
				return false;
			}
			existingRoot.QueueFreeSafely();
		}
		BackBufferCopy newRoot = new BackBufferCopy
		{
			Name = overlayName,
			CopyMode = BackBufferCopy.CopyModeEnum.Viewport,
			ZAsRelative = false,
			ZIndex = OverlayZIndex,
			Visible = true
		};
		ColorRect newRect = new ColorRect
		{
			Name = OverlayRectName,
			Material = _mat,
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		newRoot.AddChildSafely(newRect);
		parent.AddChildSafely(newRoot);
		overlayRoot = newRoot;
		overlayRect = newRect;
		return true;
	}

	private static void ApplyOverlayProperties(BackBufferCopy overlayRoot, ColorRect overlayRect)
	{
		overlayRoot.ZAsRelative = false;
		overlayRoot.ZIndex = OverlayZIndex;
		overlayRoot.CopyMode = BackBufferCopy.CopyModeEnum.Viewport;
		overlayRoot.Visible = true;
		overlayRect.Material = _mat;
		overlayRect.MouseFilter = Control.MouseFilterEnum.Ignore;
	}

	private static void CleanupLegacyOverlays(NCombatRoom room)
	{
		RemoveLegacyRootOverlay(room, OverlayRootName);
		RemoveLegacyColorRect(room.SceneContainer, SceneOverlayName);
		RemoveLegacyColorRect(room.BackCombatVfxContainer, BackVfxOverlayName);
		RemoveLegacyColorRect(room.CombatVfxContainer, CombatVfxOverlayName);
	}

	private static void RemoveLegacyRootOverlay(Node? parent, string name)
	{
		if (parent == null)
		{
			return;
		}
		BackBufferCopy? legacyOverlay = parent.GetNodeOrNull<BackBufferCopy>(name);
		if (legacyOverlay == null)
		{
			return;
		}
		parent.RemoveChildSafely(legacyOverlay);
		legacyOverlay.QueueFreeSafely();
	}

	private static void RemoveLegacyColorRect(Node? parent, string name)
	{
		if (parent == null)
		{
			return;
		}
		ColorRect? legacyOverlay = parent.GetNodeOrNull<ColorRect>(name);
		if (legacyOverlay == null)
		{
			return;
		}
		parent.RemoveChildSafely(legacyOverlay);
		legacyOverlay.QueueFreeSafely();
	}

	private static void PruneInvalidState()
	{
		PruneInvalidOverlay(ref _sceneOverlayRoot, ref _sceneOverlayRect);
		PruneInvalidOverlay(ref _backVfxOverlayRoot, ref _backVfxOverlayRect);
		PruneInvalidOverlay(ref _combatVfxOverlayRoot, ref _combatVfxOverlayRect);
		PruneInvalidSavedCreatures();
	}

	private static void PruneInvalidOverlay(ref BackBufferCopy? overlayRoot, ref ColorRect? overlayRect)
	{
		if (!GodotObject.IsInstanceValid(overlayRoot))
		{
			overlayRoot = null;
			overlayRect = null;
			return;
		}
		if (!GodotObject.IsInstanceValid(overlayRect))
		{
			overlayRoot.QueueFreeSafely();
			overlayRoot = null;
			overlayRect = null;
		}
	}

	private static void PruneInvalidSavedCreatures()
	{
		if (_savedZ.Count == 0)
		{
			return;
		}
		List<NCreature> invalidNodes = new List<NCreature>();
		foreach (NCreature node in _savedZ.Keys)
		{
			if (!GodotObject.IsInstanceValid(node))
			{
				invalidNodes.Add(node);
			}
		}
		foreach (NCreature node in invalidNodes)
		{
			_savedZ.Remove(node);
		}
	}

	private static int CountValidOverlays()
	{
		int count = 0;
		if (GodotObject.IsInstanceValid(_sceneOverlayRoot))
		{
			count++;
		}
		if (GodotObject.IsInstanceValid(_backVfxOverlayRoot))
		{
			count++;
		}
		if (GodotObject.IsInstanceValid(_combatVfxOverlayRoot))
		{
			count++;
		}
		return count;
	}

	private static bool HasVisibleOverlays()
	{
		return IsOverlayVisible(_sceneOverlayRoot)
			|| IsOverlayVisible(_backVfxOverlayRoot)
			|| IsOverlayVisible(_combatVfxOverlayRoot);
	}

	private static bool IsOverlayVisible(BackBufferCopy? overlayRoot)
	{
		return GodotObject.IsInstanceValid(overlayRoot) && overlayRoot.Visible;
	}

	private static void SetOverlayVisibility(bool visible)
	{
		if (GodotObject.IsInstanceValid(_sceneOverlayRoot))
		{
			_sceneOverlayRoot.Visible = visible;
		}
		if (GodotObject.IsInstanceValid(_backVfxOverlayRoot))
		{
			_backVfxOverlayRoot.Visible = visible;
		}
		if (GodotObject.IsInstanceValid(_combatVfxOverlayRoot))
		{
			_combatVfxOverlayRoot.Visible = visible;
		}
	}

	private readonly struct SavedZ
	{
		private readonly int _zIndex;
		private readonly bool _zAsRelative;

		private SavedZ(int zIndex, bool zAsRelative)
		{
			_zIndex = zIndex;
			_zAsRelative = zAsRelative;
		}

		public static SavedZ Capture(CanvasItem item)
		{
			return new SavedZ(item.ZIndex, item.ZAsRelative);
		}

		public void Restore(CanvasItem item)
		{
			item.ZAsRelative = _zAsRelative;
			item.ZIndex = _zIndex;
		}
	}
}
