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
public class MaidMaster: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
  });
	public MaidMaster() : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
		CardModel card = (await CardSelectCmd.FromHand(choiceContext, base.Owner, prefs, (CardModel c) => !c.Keywords.Contains(CardKeyword.Unplayable), this)).FirstOrDefault();
		if (card != null)
		{
			CardCmd.ApplyKeyword(card, CardKeyword.Exhaust);
			await CardCmd.AutoPlay(choiceContext, card, null);
		}
	}
	protected override void OnUpgrade()
	{
		this.EnergyCost.UpgradeBy(-1);
	}
}

}
