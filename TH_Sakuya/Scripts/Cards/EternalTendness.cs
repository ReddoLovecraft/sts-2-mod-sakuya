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
public class EternalTendness: SakuyaCardModel
{
	 public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Eternal];
	  public override bool GainsBlock => true;
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
         HoverTipFactory.FromPower<StrengthPower>()
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new BlockVar(10m,ValueProp.Move),
        new CardsVar(1)
     ];
	public EternalTendness() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		 await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<StrengthPower>(mos, -this.DynamicVars.Cards.IntValue,Owner.Creature,this);
        }
	}
	protected override void OnUpgrade()
	{
		 base.DynamicVars.Block.UpgradeValueBy(2);
		 base.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
