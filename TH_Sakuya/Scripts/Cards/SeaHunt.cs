using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
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
public class SeaHunt: SakuyaCardModel
{
	public override bool GainsBlock => true;
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
       HoverTipFactory.Static(StaticHoverTip.Fatal)
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(24, ValueProp.Unblockable|ValueProp.Move),
        new CardsVar(6)
     ];
	public SeaHunt() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
		int value=cardPlay.Target.Block;
		await CreatureCmd.LoseBlock(cardPlay.Target,value);
		AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_bite", null, "blunt_attack.mp3")
			.Execute(choiceContext);
		if(cardPlay.Target!=null&&cardPlay.Target.IsAlive)
		{
			await CreatureCmd.GainBlock(cardPlay.Target,new BlockVar(value,ValueProp.Unpowered),null);
		}
		if (shouldTriggerFatal && attackCommand.Results.Any((DamageResult r) => r.WasTargetKilled))
		{
			await CreatureCmd.GainMaxHp(base.Owner.Creature, base.DynamicVars.Cards.IntValue);
		}
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(8);
		base.DynamicVars.Cards.UpgradeValueBy(2);
	}
}

}
