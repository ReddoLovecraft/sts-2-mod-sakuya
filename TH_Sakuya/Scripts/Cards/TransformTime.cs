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
public class TransformTime: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
         this.EnergyHoverTip,
		 Tools.GetStaticKeyword("Tsp")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new EnergyVar(1),
        new CardsVar(6)
     ];
	public TransformTime() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await PlayerCmd.LoseEnergy(this.DynamicVars.Energy.IntValue,Owner);
		TimeStopPointSystem.Gain(this.Owner,this.DynamicVars.Cards.IntValue);
	}
	protected override void OnUpgrade()
	{
	    this.DynamicVars.Energy.UpgradeValueBy(1);
		this.DynamicVars.Cards.UpgradeValueBy(6);
	}
}

}
