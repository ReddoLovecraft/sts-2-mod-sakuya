using Godot;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using MegaCrit.Sts2.Core.Entities.Creatures;
using System.Threading.Tasks;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class SakuyaClock : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Single;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/SC32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/SC64.png";

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        TimeStopScreenOverlay.RefreshExemptCreatures();
        return Task.CompletedTask;
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        TimeStopScreenOverlay.RefreshExemptCreatures();
        return Task.CompletedTask;
    }
}
}
