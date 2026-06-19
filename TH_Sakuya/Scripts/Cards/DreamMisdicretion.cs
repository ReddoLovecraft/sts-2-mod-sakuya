using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class DreamMisdirection: SakuyaCardModel
{
	 	public override bool GainsBlock => true;
		 public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
         HoverTipFactory.FromPower<VulnerablePower>(),
         HoverTipFactory.FromPower<WeakPower>()
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(2)
     ];
	public DreamMisdirection() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies)
        {
            if(mos.IsAlive)
			{
            await PowerCmd.Apply<VulnerablePower>(choiceContext,  mos, base.DynamicVars.Cards.IntValue,Owner.Creature,this);
            await PowerCmd.Apply<WeakPower>(choiceContext,  mos, base.DynamicVars.Cards.IntValue,Owner.Creature,this);
			await CreatureCmd.LoseBlock(mos, mos.Block);
			}
        }
		
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
