using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Runs;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class SilverSpace: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
  {
        Tools.GetStaticKeyword("TimeStop"),
        Tools.GetStaticKeyword("Stop"),
		Tools.GetStaticKeyword("Tsp")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(8, ValueProp.Move)
     ];
	public SilverSpace() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
	{
	}
	  private static int CalculateEnemyAttackIntentTotal(Player player)
        {
            if (player?.Creature?.CombatState == null)
            {
                return 0;
            }
            var combatState = player.Creature.CombatState;
            List<Creature> targets = [player.Creature];

            int sum = 0;
            foreach (Creature enemy in combatState.HittableEnemies)
            {
                if (enemy == null || !enemy.IsAlive || enemy.Monster == null)
                {
                    continue;
                }
                foreach (AttackIntent intent in enemy.Monster.NextMove.Intents.OfType<AttackIntent>())
                {
                    sum += intent.GetTotalDamage(targets, enemy);
                }
            }
            return sum;
        }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
		if(!Owner.Creature.HasPower<TimeStopPower>())
	    await TimeStopPointSystem.Gain(Owner,attackCommand.Results.Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage));
		else
		{
			if (RunManager.Instance.IsInProgress
				&& !string.Equals(RunManager.Instance.NetService.Type.ToString(), "Singleplayer", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			int cnt=CalculateEnemyAttackIntentTotal(Owner);
			await TimeStopPointSystem.Gain(Owner,cnt);
		}
	
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(4);
	}
}

}
