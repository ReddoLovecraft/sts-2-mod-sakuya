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
public class LatePrepare: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
		 Tools.GetStaticKeyword("Knife"),
		 Tools.GetStaticKeyword("Tsp")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(6)
     ];
	 protected override bool IsPlayable => TimeStopPointSystem.Get(Owner) > 0;
	 protected override bool ShouldGlowGoldInternal => TimeStopPointSystem.Get(Owner) >=this.DynamicVars.Cards.IntValue;
	public LatePrepare() : base(0, CardType.Skill, CardRarity.Common, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (base.Owner.Character is SakuyaCharacter)
		{
			 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		}
	     int cost=Math.Min(this.DynamicVars.Cards.IntValue,TimeStopPointSystem.Get(Owner));
		 if(await TimeStopPointSystem.TrySpend(Owner,cost))
		 {
			await PowerCmd.Apply<KnifePower>(Owner.Creature, cost,Owner.Creature,this);
		 }
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Cards.UpgradeValueBy(6);
	}
}

}
