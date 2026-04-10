using Godot;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class SilverAcutePower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Single;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/SAP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/SAP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("Knife")];

}
}



