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
public class BattlePaper: SakuyaCardModel
{
	  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Innate];
	public override bool GainsBlock => true;
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new BlockVar(7, ValueProp.Move),
        new CardsVar(1)
     ];
	public BattlePaper() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CardPileCmd.Draw(choiceContext,base.DynamicVars.Cards.IntValue,Owner);
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Block.UpgradeValueBy(1);
		base.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
