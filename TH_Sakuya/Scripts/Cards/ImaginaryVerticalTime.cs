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
public class ImaginaryVerticalTime: SakuyaCardModel
{
	 public override bool GainsBlock => true;
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 Tools.GetStaticKeyword("TimeStop")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new BlockVar(10m, ValueProp.Move)
     ];
	public ImaginaryVerticalTime() : base(2, CardType.Skill, CardRarity.Rare, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		 await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
		 await PowerCmd.Apply<BlockNextTurnPower>(Owner.Creature,Owner.Creature.Block,Owner.Creature, this);
		 int cnt=0;
		 if(Owner.GetRelic<SakuyaWatch>()!=null)
		 {
			 cnt=Owner.GetRelic<SakuyaWatch>().DisplayAmount;
			await PowerCmd.Apply<ImaginaryVerticalTimePower>(Owner.Creature, cnt,Owner.Creature,this);
		 }
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Block.UpgradeValueBy(5);
	}
}

}
