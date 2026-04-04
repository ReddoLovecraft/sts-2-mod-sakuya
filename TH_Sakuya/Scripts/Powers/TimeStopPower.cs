using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers;

public sealed class TimeStopPower : SakuyaPowerModel
{
	private static readonly object _renderLock = new object();
	private static float _prevExposure = 1f;
	private static float _prevBrightness = 1f;
	private static float _prevContrast = 1f;
	private static float _prevSaturation = 1f;
	private static bool _renderApplied;

	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.None;

	protected override bool IsVisibleInternal => false;

	public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		ApplyRenderIfLocalPlayer();
		await Task.CompletedTask;
	}

	public override async Task AfterRemoved(Creature oldOwner)
	{
		RestoreRenderIfLocalPlayer();
		if (oldOwner.CombatState != null)
		{
			await TriggerDelayedDamage(oldOwner.CombatState);
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

		Creature applier = dealer ?? Owner;
		decimal appliedAmount = amount;
		TaskHelper.RunSafely(PowerCmd.Apply<LaterDamagePower>(target, appliedAmount, applier, cardSource, silent: true));
		return 0m;
	}

	private static async Task TriggerDelayedDamage(CombatState combatState)
	{
		List<Creature> creatures = combatState.Creatures.Where(c => c != null && c.IsAlive).ToList();
		foreach (Creature creature in creatures)
		{
			LaterDamagePower? laterDamage = creature.GetPower<LaterDamagePower>();
			if (laterDamage == null)
			{
				continue;
			}
			int amount = Math.Max(0, laterDamage.Amount);
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
		if (Owner == null || Owner.Player == null || NGame.Instance == null)
		{
			return;
		}
		if (!LocalContext.IsMe(Owner.Player))
		{
			return;
		}
		lock (_renderLock)
		{
			if (_renderApplied)
			{
				return;
			}
			WorldEnvironment worldEnvironment = NGame.Instance.ActivateWorldEnvironment();
			_prevExposure = worldEnvironment.Environment.TonemapExposure;
			_prevBrightness = worldEnvironment.Environment.AdjustmentBrightness;
			_prevContrast = worldEnvironment.Environment.AdjustmentContrast;
			_prevSaturation = worldEnvironment.Environment.AdjustmentSaturation;
			worldEnvironment.Environment.TonemapExposure = 1f;
			worldEnvironment.Environment.AdjustmentEnabled = true;
			worldEnvironment.Environment.AdjustmentBrightness = 0.95f;
			worldEnvironment.Environment.AdjustmentContrast = 1.1f;
			worldEnvironment.Environment.AdjustmentSaturation = 0f;
			_renderApplied = true;
		}
	}

	private void RestoreRenderIfLocalPlayer()
	{
		if (Owner == null || Owner.Player == null || NGame.Instance == null)
		{
			return;
		}
		if (!LocalContext.IsMe(Owner.Player))
		{
			return;
		}
		lock (_renderLock)
		{
			if (!_renderApplied)
			{
				return;
			}
			WorldEnvironment worldEnvironment = NGame.Instance.ActivateWorldEnvironment();
			worldEnvironment.Environment.TonemapExposure = _prevExposure;
			worldEnvironment.Environment.AdjustmentBrightness = _prevBrightness;
			worldEnvironment.Environment.AdjustmentContrast = _prevContrast;
			worldEnvironment.Environment.AdjustmentSaturation = _prevSaturation;
			_renderApplied = false;
		}
	}
}
