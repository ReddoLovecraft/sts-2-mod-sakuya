using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
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
public class PerfectSquare: SakuyaCardModel	
{
	public override bool GainsBlock => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
         Tools.GetStaticKeyword("TimeStop"),
		 Tools.GetStaticKeyword("Stop")
  });
   protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<TimeStopPower>();
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new BlockVar(10, ValueProp.Move),
        new CardsVar(2)
     ];
	public PerfectSquare() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		if(Owner.Creature.HasPower<TimeStopPower>())
		{
			IEnumerable<CardModel> enumerable = PileType.Draw.GetPile(base.Owner).Cards.Where((CardModel c) => c.IsUpgradable).TakeRandom(base.DynamicVars.Cards.IntValue, base.Owner.RunState.Rng.CombatCardSelection);
			foreach (CardModel item in enumerable)
			{
			CardCmd.Upgrade(item);
			CardCmd.Preview(item);
			}
		}
		await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
		 CardModel cardModel;
		do
		{
			cardModel = await CardPileCmd.Draw(choiceContext, base.Owner);
		}
		while (cardModel != null && cardModel.IsUpgraded && CardPile.GetCards(base.Owner, PileType.Hand).Count() < 10);

	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Cards.UpgradeValueBy(1);
		base.DynamicVars.Block.UpgradeValueBy(2);
	}
}

}
