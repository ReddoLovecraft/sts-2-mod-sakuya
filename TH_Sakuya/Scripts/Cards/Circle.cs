using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class Circle : SakuyaCardModel
{
	  protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
         Tools.GetStaticKeyword("TimeStop"),
  });
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move),new CardsVar(1)];
	public Circle() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue) .FromCard(this) .Targeting(cardPlay.Target).Execute(choiceContext);
		await CardPileCmd.Draw(choiceContext, 1, Owner.Creature.Player);
	}
	public async Task MoveUpperCardPile()
	{
          CardPile? pile = base.Pile;
		if (pile != null)
		{
			switch (pile.Type)
			{
				case PileType.Draw:
					await CardPileCmd.Add(this, PileType.Discard);
					break;
				case PileType.Discard:
					await CardPileCmd.Add(this, PileType.Hand);
					break;
				case PileType.Hand:
					await CardPileCmd.Add(this, PileType.Draw);
					break;
				default:
						break;
			}
		}
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(3); 
	}
}
}

