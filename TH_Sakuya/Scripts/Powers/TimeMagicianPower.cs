using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class TimeMagicianPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override bool IsInstanced => true;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/TMP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/TMP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("Tsp"),HoverTipFactory.ForEnergy(this)];
	protected override IEnumerable<DynamicVar> CanonicalVars => [(new EnergyVar(1))];
	public void InitPower()
	{
        base.DynamicVars.Energy.BaseValue=Amount/6;
	}
	   public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
			for(int i=0;i<base.DynamicVars.Energy.IntValue;i++)
			{
				if(TimeStopPointSystem.TrySpend(player,6))
				{
					await PlayerCmd.GainEnergy(1,player);
				}
				else
				{
					break;
				}
			}
        }

}
}



