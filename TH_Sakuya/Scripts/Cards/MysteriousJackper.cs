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
public class MysteriousJackper: SakuyaCardModel
{
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(10, ValueProp.Move),
     ];
	public MysteriousJackper() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue) .FromCard(this) .Targeting(cardPlay.Target).Execute(choiceContext);
		foreach(CardModel card in PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c.EnergyCost.GetWithModifiers(CostModifiers.Local) > 0 && !c.EnergyCost.CostsX))
		{
           card.EnergyCost.AddThisTurn(-1);
		}
	}
	protected override void OnUpgrade()
	{
	   this.EnergyCost.UpgradeBy(-1);
	}
}

}
