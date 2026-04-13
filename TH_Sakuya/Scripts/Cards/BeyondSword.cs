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
public class BeyondSword: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
         base.EnergyHoverTip,
		 Tools.GetStaticKeyword("Knife")
  });
    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<KnifePower>();
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(6, ValueProp.Move),
        new CardsVar(6),
		new EnergyVar(1)
     ];
	public BeyondSword() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
     	await DamageCmd.Attack(DynamicVars.Damage.BaseValue) .FromCard(this) .Targeting(cardPlay.Target).Execute(choiceContext);
       	if(ShouldGlowGoldInternal)
		{
			if(cardPlay.Target!=null&&cardPlay.Target.IsAlive&&cardPlay.Target.IsHittable)
			await (Owner.Creature.GetPower<KnifePower>()).ThrowKnife(choiceContext,cardPlay.Target,KnifeType.AnyEnemy,1,this.DynamicVars.Cards.IntValue);
		await PlayerCmd.GainEnergy(1,Owner);
		}
		if (base.Owner.Character is SakuyaCharacter)
		{
			 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		}
		 await PowerCmd.Apply<KnifePower>(Owner.Creature, base.DynamicVars.Cards.IntValue,Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(2);
		this.DynamicVars.Cards.UpgradeValueBy(2);
	}
}

}
