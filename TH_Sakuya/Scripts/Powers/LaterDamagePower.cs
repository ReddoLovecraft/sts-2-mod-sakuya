using Godot;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class LaterDamagePower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Debuff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/LDP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/LDP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("TimeStop")];

}
}



