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
public class RemoteKiller: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
		HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
		Tools.GetStaticKeyword("Knife")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(6, ValueProp.Move),
        new CardsVar(3)
     ];
	public RemoteKiller() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue) .FromCard(this) .Targeting(cardPlay.Target).Execute(choiceContext);
		await PowerCmd.Apply<KnifePower>(Owner.Creature,this.DynamicVars.Cards.IntValue,Owner.Creature,this);
		CardModel card = CreateClone();
		CardCmd.ApplyKeyword(card,CardKeyword.Ethereal);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, addedByPlayer: true), 1.6f);
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(2);
		this.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
