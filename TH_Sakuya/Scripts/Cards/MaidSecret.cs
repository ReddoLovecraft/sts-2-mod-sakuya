using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class MaidSecret: SakuyaCardModel
{
	    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
  {		 HoverTipFactory.FromCard<MaidSecretK>(),
         HoverTipFactory.FromCard<MaidSecretM>(),
         HoverTipFactory.FromCard<MaidSecretD>()
  });
	public MaidSecret() : base(0, CardType.Skill, CardRarity.Basic, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
	  if(IsUpgraded)
	  {
		List<CardModel> list =new List<CardModel>();
	   	list.Add(Owner.Creature.CombatState.CreateCard<MaidSecretD>(Owner.Creature.Player));
	  	list.Add(Owner.Creature.CombatState.CreateCard<MaidSecretK>(Owner.Creature.Player));
	   	list.Add(Owner.Creature.CombatState.CreateCard<MaidSecretM>(Owner.Creature.Player));
	  	await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, addedByPlayer: true);
	  }
	  else
	  {
		  IEnumerable<CardModel> c =[ModelDb.Card<MaidSecretD>(), ModelDb.Card<MaidSecretK>(), ModelDb.Card<MaidSecretM>()];
		 List<CardModel> cards = new List<CardModel>();
		  foreach (var item in c)
		  {
			cards.Add(Owner.Creature.CombatState.CreateCard(item, Owner.Creature.Player));
		  }
		CardModel cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, base.Owner, canSkip: false);
		if (cardModel != null)
		{
		 	await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
		}
	  }
	}
	protected override void OnUpgrade()
	{
	}
}

}
