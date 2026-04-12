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
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(StatusCardPool))]
public class ClockPart: SakuyaCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
  {
         Tools.GetStaticKeyword("TimeStop"),
		 Tools.GetStaticKeyword("Tsp"),
		 Tools.GetStaticKeyword("Stop")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new DynamicVar("Power", 12),
        new CardsVar(2)
     ];
	protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<TimeStopPower>();
	public ClockPart() : base(0, CardType.Status, CardRarity.Status, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if(Owner.Character is SakuyaCharacter)
		{
			TimeStopPointSystem.Gain(Owner, base.DynamicVars["Power"].IntValue);
		}
		await CardPileCmd.Draw(choiceContext,base.DynamicVars.Cards.IntValue,Owner);
		if(Owner.Creature.HasPower<TimeStopPower>())
		{
			if(Owner.GetRelic<SakuyaWatch>()!=null)
			{
                Owner.GetRelic<SakuyaWatch>().SetCounter(Owner.GetRelic<SakuyaWatch>().StackCount+this.DynamicVars.Cards.IntValue);
			}
		}

	}
	protected override void OnUpgrade()
	{
		
	}
}

}
