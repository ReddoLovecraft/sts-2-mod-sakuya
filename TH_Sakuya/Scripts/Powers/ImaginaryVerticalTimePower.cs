using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class ImaginaryVerticalTimePower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override bool IsInstanced => true;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/IVTP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/IVTP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("TimeStop")];
	 public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
			if(Owner.Player.GetRelic<SakuyaWatch>()!=null)
			{
				Owner.Player.GetRelic<SakuyaWatch>().SetCounter(Amount);
			}
			else if(Owner.Player.GetRelic<SakuyaLunaDial>()!=null)
			{
				Owner.Player.GetRelic<SakuyaLunaDial>().SetCounter(Amount);
			}
			await PowerCmd.Remove(this);
        }

}
}



