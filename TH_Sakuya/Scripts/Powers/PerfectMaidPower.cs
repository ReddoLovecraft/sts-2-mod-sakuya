using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class PerfectMaidPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/PMP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/PMP64.png";
	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            await PowerCmd.Decrement(this);
			flag=false;
        }
	private bool flag=false;
	public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
	
		if (target == base.Owner && dealer != null &&dealer!=base.Owner&& (props.IsPoweredAttack_() )&&flag)
		{
			Flash();
			await Owner.GetPower<KnifePower>().ThrowKnife(choiceContext,dealer,KnifeType.AnyEnemy,1,16);
			flag=false;
		}
		
	}
		public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != base.Owner)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack_())
		{
			return 1m;
		}
		if(dealer == null||dealer==base.Owner)
		{
			return 1m;
		}
        if(!Owner.HasPower<KnifePower>()||Owner.GetPowerAmount<KnifePower>()<16)
		{
			return 1m;
		}
		flag=true;
		return 0;
	}
}
}



