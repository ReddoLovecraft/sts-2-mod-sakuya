using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
public class DualVanish: SakuyaCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(2)
     ];
	public DualVanish() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
	    CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 0,2);
		List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(base.Owner).Cards
			orderby c.Rarity, c.Id
			select c).ToList();
		foreach(CardModel card in await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, prefs))
		{
			if(card!=null)
			await CardPileCmd.Add(card, PileType.Hand);
		}
		cardsIn = (from c in PileType.Discard.GetPile(base.Owner).Cards
			orderby c.Rarity, c.Id
			select c).ToList();
		foreach(CardModel card in await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, prefs))
		{
			if(card!=null)
			await CardPileCmd.Add(card, PileType.Hand);
		}
	}
	protected override void OnUpgrade()
	{
		this.EnergyCost.UpgradeBy(-1);
	}
}

}
