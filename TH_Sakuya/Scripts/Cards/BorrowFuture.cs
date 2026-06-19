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
public class BorrowFuture: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
         base.EnergyHoverTip
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new EnergyVar(2)
     ];
	public BorrowFuture() : base(0, CardType.Skill, CardRarity.Common, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		 await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue,Owner.Creature.Player);
		 await PowerCmd.Apply<BorrowFuturePower>(choiceContext, Owner.Creature, base.DynamicVars.Energy.IntValue,Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Energy.UpgradeValueBy(1);
	}
}

}
