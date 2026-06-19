using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scripts.Powers;

public sealed class TimeStopPower : SakuyaPowerModel
{
	// #region debug-point C:config
	private const string DebugEnvPath = ".dbg/timestop-toggle-crash.env";
	private const string DebugFallbackUrl = "http://127.0.0.1:7777/event";
	private const string DebugSessionId = "timestop-toggle-crash";
	private const string DebugRunId = "pre-fix";
	private static readonly System.Net.Http.HttpClient _debugHttp = new System.Net.Http.HttpClient();
	// #endregion

	// #region debug-point C:report
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

	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.None;
	protected override bool IsVisibleInternal => false;

	public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		// #region debug-point C:after-applied
		ReportDebug("C", "TimeStopPower.AfterApplied", "time-stop-applied", new
		{
			ownerType = Owner?.GetType().Name,
			ownerHasPlayer = Owner?.Player != null,
			applierType = applier?.GetType().Name,
			cardSourceType = cardSource?.GetType().Name
		});
		// #endregion
		ApplyRenderIfLocalPlayer();
		await Task.CompletedTask;
	}

	public override async Task AfterRemoved(Creature oldOwner)
	{
		// #region debug-point E:after-removed
		ReportDebug("E", "TimeStopPower.AfterRemoved", "time-stop-removed", new
		{
			oldOwnerType = oldOwner?.GetType().Name,
			combatStateExists = oldOwner?.CombatState != null
		});
		// #endregion
		RestoreRenderIfLocalPlayer();
		if (oldOwner.CombatState != null)
		{
			// #region debug-point D:delayed-damage-start
			ReportDebug("D", "TimeStopPower.AfterRemoved", "trigger-delayed-damage-start", new
			{
				creatureCount = oldOwner.CombatState.Creatures?.Count() ?? 0
			});
			// #endregion
			await TriggerDelayedDamage(oldOwner.CombatState);
			// #region debug-point D:delayed-damage-done
			ReportDebug("D", "TimeStopPower.AfterRemoved", "trigger-delayed-damage-done", new
			{
				creatureCount = oldOwner.CombatState.Creatures?.Count() ?? 0
			});
			// #endregion
		}
		await Task.CompletedTask;
	}

	public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (amount <= 0m)
		{
			return amount;
		}
		if (Owner == null || Owner.CombatState == null || Owner.IsDead)
		{
			return amount;
		}
		bool isIncomingToOwner = target == Owner;
		bool isOutgoingFromOwner = dealer == Owner && target != Owner;
		bool isSelfDamageFromOwner = dealer == Owner && target == Owner;

		if (!isIncomingToOwner && !isOutgoingFromOwner && !isSelfDamageFromOwner)
		{
			return amount;
		}

		if ((target?.HasPower<SakuyaClock>() ?? false) || (dealer?.HasPower<SakuyaClock>() ?? false))
		{
			return amount;
		}

		Creature applier = dealer ?? Owner;
		decimal appliedAmount = amount;
		TaskHelper.RunSafely(PowerCmd.Apply<LaterDamagePower>(new ThrowingPlayerChoiceContext(), target, appliedAmount, applier, cardSource, silent: true));
		return 0m;
	}

	private static async Task TriggerDelayedDamage(ICombatState combatState)
	{
		List<Creature> creatures = combatState.Creatures.Where(c => c != null && c.IsAlive).ToList();
		// #region debug-point D:delayed-damage-loop
		ReportDebug("D", "TimeStopPower.TriggerDelayedDamage", "delayed-damage-loop-begin", new
		{
			livingCreatureCount = creatures.Count,
			withLaterDamageCount = creatures.Count(c => c.GetPower<LaterDamagePower>() != null)
		});
		// #endregion
		foreach (Creature creature in creatures)
		{
			LaterDamagePower? laterDamage = creature.GetPower<LaterDamagePower>();
			if (laterDamage == null)
			{
				continue;
			}
			int amount = Math.Max(0, laterDamage.Amount);
			// #region debug-point D:delayed-damage-creature
			ReportDebug("D", "TimeStopPower.TriggerDelayedDamage", "delayed-damage-creature", new
			{
				creatureType = creature.GetType().Name,
				amount
			});
			// #endregion
			await PowerCmd.Remove(laterDamage);
			if (amount <= 0)
			{
				continue;
			}
			await CreatureCmd.Damage(new BlockingPlayerChoiceContext(), creature, amount, ValueProp.Unpowered, dealer: null, cardSource: null);
		}
	}

	private void ApplyRenderIfLocalPlayer()
	{
		// #region debug-point C:apply-render
		ReportDebug("C", "TimeStopPower.ApplyRenderIfLocalPlayer", "apply-render-check", new
		{
			ownerExists = Owner != null,
			playerExists = Owner?.Player != null,
			isLocal = Owner?.Player != null && LocalContext.IsMe(Owner.Player),
			gameExists = NGame.Instance != null
		});
		// #endregion
		if (Owner == null || Owner.Player == null || NGame.Instance == null)
		{
			return;
		}
		if (!LocalContext.IsMe(Owner.Player))
		{
			return;
		}
		TimeStopScreenOverlay.ApplyIfNeeded();
	}

	private void RestoreRenderIfLocalPlayer()
	{
		// #region debug-point E:restore-render
		ReportDebug("E", "TimeStopPower.RestoreRenderIfLocalPlayer", "restore-render-check", new
		{
			ownerExists = Owner != null,
			playerExists = Owner?.Player != null,
			isLocal = Owner?.Player != null && LocalContext.IsMe(Owner.Player),
			gameExists = NGame.Instance != null
		});
		// #endregion
		if (Owner == null || Owner.Player == null || NGame.Instance == null)
		{
			return;
		}
		if (!LocalContext.IsMe(Owner.Player))
		{
			return;
		}
		TimeStopScreenOverlay.Restore();
	}
}

public sealed class TimeStopPointPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
	protected override bool IsVisibleInternal => false;

	public override bool ShouldClearBlock(Creature creature)
	{
		return true;
	}
}

public sealed class TimeStopFirstGrantPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Single;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
	protected override bool IsVisibleInternal => false;
}
