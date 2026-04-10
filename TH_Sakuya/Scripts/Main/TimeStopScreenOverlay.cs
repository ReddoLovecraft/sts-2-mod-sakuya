using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System.Collections.Generic;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scripts.Main;

public static class TimeStopScreenOverlay
{
	private const string OverlayName = "TimeStopGrayscaleOverlay";
	private const string ShaderPath = "res://TH_Sakuya/ArtWorks/VFX/screen_grayscale.gdshader";

	private static ColorRect? _overlay;
	private static ShaderMaterial? _mat;
	private static readonly Dictionary<NCreature, SavedZ> _savedZ = new Dictionary<NCreature, SavedZ>();

	public static void ApplyIfNeeded()
	{
		if (GodotObject.IsInstanceValid(_overlay))
		{
			RefreshExemptCreatures();
			return;
		}
		NCombatRoom? room = NCombatRoom.Instance;
		if (room == null || room.SceneContainer == null)
		{
			return;
		}
		if (room.SceneContainer.GetNodeOrNull<ColorRect>(OverlayName) != null)
		{
			return;
		}

		Shader shader = GD.Load<Shader>(ShaderPath);
		_mat = new ShaderMaterial { Shader = shader };

		ColorRect overlay = new ColorRect
		{
			Name = OverlayName,
			MouseFilter = Control.MouseFilterEnum.Ignore,
			Material = _mat,
			ZAsRelative = false,
			ZIndex = 5000
		};
		overlay.AnchorLeft = 0f;
		overlay.AnchorTop = 0f;
		overlay.AnchorRight = 1f;
		overlay.AnchorBottom = 1f;
		overlay.OffsetLeft = 0f;
		overlay.OffsetTop = 0f;
		overlay.OffsetRight = 0f;
		overlay.OffsetBottom = 0f;

		room.SceneContainer.AddChildSafely(overlay);
		_overlay = overlay;
		RefreshExemptCreatures();
	}

	public static void Restore()
	{
		foreach (var kv in _savedZ)
		{
			if (GodotObject.IsInstanceValid(kv.Key))
			{
				kv.Value.Restore(kv.Key);
			}
		}
		_savedZ.Clear();
		if (GodotObject.IsInstanceValid(_overlay))
		{
			_overlay.QueueFreeSafely();
		}
		_overlay = null;
		_mat = null;
	}

	public static void RefreshExemptCreatures()
	{
		if (!GodotObject.IsInstanceValid(_overlay))
		{
			return;
		}
		NCombatRoom? room = NCombatRoom.Instance;
		if (room == null)
		{
			return;
		}
		int overlayZ = _overlay.ZIndex;

		foreach (NCreature node in room.CreatureNodes)
		{
			if (node == null || node.Entity == null)
			{
				continue;
			}
			bool exempt = node.Entity.HasPower<SakuyaClock>();
			if (exempt)
			{
				if (!_savedZ.ContainsKey(node))
				{
					_savedZ[node] = SavedZ.Capture(node);
				}
				node.ZAsRelative = false;
				node.ZIndex = overlayZ + 1;
			}
			else if (_savedZ.TryGetValue(node, out SavedZ saved))
			{
				saved.Restore(node);
				_savedZ.Remove(node);
			}
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
