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
public class DevilMaid: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
         HoverTipFactory.FromPower<StrengthPower>()
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new DynamicVar("Power", 2),
        new CardsVar(2)
     ];
	public DevilMaid() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_bloody_impact");
		await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars.Cards.IntValue, ValueProp.Unblockable | ValueProp.Unpowered, this);
		await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["Power"].IntValue,Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars["Power"].UpgradeValueBy(2);
	}
}

}
