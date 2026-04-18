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
public class StarKnife: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 Tools.GetStaticKeyword("Knife")
  });
    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<KnifePower>();
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(4, ValueProp.Move),
        new CardsVar(2)
     ];

	public StarKnife() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int cnt=0;
		if(Owner.Creature.HasPower<KnifePower>())
		{
			cnt=Owner.Creature.GetPowerAmount<KnifePower>();
		}
			ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue+cnt).WithHitCount(this.DynamicVars.Cards.IntValue).FromCard(this)
			.Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_starry_impact")
			.Execute(choiceContext);
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
