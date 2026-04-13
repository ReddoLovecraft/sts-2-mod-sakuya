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
public class InflationSquare: SakuyaCardModel
{
	 public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Ethereal];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
  });
	public InflationSquare() : base(3, CardType.Skill, CardRarity.Rare, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		 List<CardModel> list = PileType.Hand.GetPile(base.Owner).Cards.ToList();
		int cardCount = list.Count;
		foreach (CardModel item in list)
		{
			CardModel card = item.CreateClone();
			CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
			card.EnergyCost.SetThisTurn(0);
			if(IsUpgraded)
			await CardCmd.Discard(choiceContext, item);
			else
			await CardCmd.Exhaust(choiceContext, item);
			CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true), 0.8f);
		}
	}
	protected override void OnUpgrade()
	{
		this.RemoveKeyword(CardKeyword.Ethereal);
	}
}

}
