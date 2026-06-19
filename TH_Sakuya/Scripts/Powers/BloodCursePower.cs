using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class BloodCursePower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Debuff;
	public override PowerStackType StackType => PowerStackType.Single;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/BCP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/BCP64.png";
	protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[1]
    {
        new CardsVar(1)
    });

 public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if(base.DynamicVars.Cards.IntValue!=2)
		{
			await Task.CompletedTask;
			return;
		}
		if (!CombatManager.Instance.IsInProgress)
		{
			await Task.CompletedTask;
			return;
		}
		if (target == base.Owner)
		{
			await Task.CompletedTask;
			return;
		}
        if(dealer==null||dealer!=base.Owner)
        {
			await Task.CompletedTask;
			return;
		}
		if(result.UnblockedDamage<=0)
		{
			await Task.CompletedTask;
			return;
		}
        Flash();
		await CreatureCmd.Heal(Owner,result.UnblockedDamage);
        await Task.CompletedTask;
	}	public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!props.IsPoweredAttack_())
		{
			return 1m;
		}
		if (target!=null&&target.IsPlayer && !target.HasPower<BloodCursePower>())
		{
			return 1m;
		}
		this.Flash();
		return (base.DynamicVars.Cards.IntValue==1)?2:1;
	}
	 public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != base.Owner.Side)
            {
                return;
            }
            if(base.DynamicVars.Cards.IntValue==2)
			{
				this.Flash();
				VfxCmd.PlayOnCreatureCenter(Owner, "vfx/vfx_bloody_impact");
				await CreatureCmd.Damage(choiceContext,Owner,new DamageVar(Owner.CurrentHp/4,ValueProp.Unblockable|ValueProp.Unpowered),null,null);
			}
        }
  
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
           if(base.DynamicVars.Cards.IntValue==3)
			{
				this.Flash();
                await PowerCmd.Apply<WeakPower>(choiceContext, Owner, 1, null, null);
                await PowerCmd.Apply<FrailPower>(choiceContext, Owner, 1, null, null);
                await PowerCmd.Apply<VulnerablePower>(choiceContext, Owner, 1, null, null);
				await PlayerCmd.LoseGold(1,Owner.Player);
			}
        }
	public async Task TriggerEffect(PlayerChoiceContext choiceContext)
	{
		this.Flash();
		this.DynamicVars.Cards.UpgradeValueBy(1);
		if(this.DynamicVars.Cards.IntValue>=4)
		{
			await CreatureCmd.Kill(Owner);
		}
	}
	public void dec(int value)
	{
		if(this.DynamicVars.Cards.IntValue-value>=1)
		{
			this.DynamicVars.Cards.UpgradeValueBy(-value);
		}
		else this.DynamicVars.Cards.UpgradeValueBy(-(this.DynamicVars.Cards.IntValue-1));
	}

}
}



