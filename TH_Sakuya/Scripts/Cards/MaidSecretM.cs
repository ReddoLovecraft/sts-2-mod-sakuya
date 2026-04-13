using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class MaidSecretM: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
         HoverTipFactory.FromPower<VulnerablePower>(),
         HoverTipFactory.FromPower<WeakPower>()
  });
   protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new DynamicVar("Power", 2),
        new CardsVar(1),
     ];
	public MaidSecretM() : base(0, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
	     await PowerCmd.Apply<VulnerablePower>( cardPlay.Target, base.DynamicVars["Power"].IntValue,Owner.Creature,this);
	     await PowerCmd.Apply<WeakPower>( cardPlay.Target, base.DynamicVars["Power"].IntValue,Owner.Creature,this);
		 await CardPileCmd.Draw(choiceContext,base.DynamicVars.Cards.IntValue,Owner.Creature.Player);
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars["Power"].UpgradeValueBy(1);
		base.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
