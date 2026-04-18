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
public class ParallelBrane: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 Tools.GetStaticKeyword("Knife")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new BlockVar(5,ValueProp.Move),
        new CardsVar(1)
     ];
	public ParallelBrane() : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (base.Owner.Character is SakuyaCharacter)
		{
			 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		}
		await CreatureCmd.GainBlock(Owner.Creature,this.DynamicVars.Block,cardPlay);
		 await PowerCmd.Apply<KnifePower>(Owner.Creature,4,Owner.Creature,this);
		 (await PowerCmd.Apply<KnivesNextTurnPower>(Owner.Creature,this.DynamicVars.Cards.IntValue,Owner.Creature,this)).SetKnifeCount(4);
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
