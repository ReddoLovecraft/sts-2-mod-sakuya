using Godot;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scrpits.Cards;
using Patchouib.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class PowerMovementStrengthPower :CustomTempStrengthPower
{
	public override AbstractModel OriginModel => ModelDb.Card<PowerMovement>();
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/PMP332.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/PMP364.png";
      
}

}

