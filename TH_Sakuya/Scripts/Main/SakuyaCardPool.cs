using BaseLib.Abstracts;
using Godot;

namespace TH_Sakuya.Scripts.Main
{
	public class SakuyaCardPool : CustomCardPoolModel
{
	public override string Title => "TH_Sakuya";

 	public override Color ShaderColor => new Color("a2c3feff");
	public override Color DeckEntryCardColor => new Color("a2c3feff");
  	public override string? BigEnergyIconPath => "res://TH_Sakuya/ArtWorks/Character/card_orb.png";
	public override string? TextEnergyIconPath => "res://TH_Sakuya/ArtWorks/Character/cost_orb.png";
	public override bool IsColorless => false;
}
}
