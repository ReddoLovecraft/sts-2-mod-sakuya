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
public class SilverPerfectMaid: SakuyaCardModel
{
     
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(7, ValueProp.Move)
     ];
	public SilverPerfectMaid() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue) .FromCard(this) .Targeting(cardPlay.Target).Execute(choiceContext);
		if (base.IsUpgraded)
		{
			foreach (CardModel item in PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c.IsUpgradable))
			{
				CardCmd.Upgrade(item);
			}
			return;
		}
		CardModel cardModel = await CardSelectCmd.FromHandForUpgrade(choiceContext, base.Owner, this);
		if (cardModel != null)
		{
			CardCmd.Upgrade(cardModel);
		}
	}
	protected override void OnUpgrade()
	{
		
	}
}

}
