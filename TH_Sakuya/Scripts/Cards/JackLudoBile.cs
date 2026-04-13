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
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class JackLudoBile: SakuyaCardModel
{
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(8, ValueProp.Move),
       
     ];
	public JackLudoBile() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue) .FromCard(this) .Targeting(cardPlay.Target).Execute(choiceContext);
		IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(base.Owner).Cards;
		Rng combatCardSelection = base.Owner.RunState.Rng.CombatCardSelection;
		CardModel cardModel = combatCardSelection.NextItem(cards.Where((CardModel c) => c.CostsEnergyOrStars(includeGlobalModifiers: false)));
		if (cardModel == null)
		{
			combatCardSelection.NextItem(cards.Where((CardModel c) => c.CostsEnergyOrStars(includeGlobalModifiers: true)));
		}
		cardModel?.SetToFreeThisTurn();
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(3);
	}
}

}
