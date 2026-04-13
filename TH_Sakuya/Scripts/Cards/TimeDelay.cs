using System.Reflection.Metadata.Ecma335;
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
public class TimeDelay: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
        Tools.GetStaticKeyword("Tsp"),
		 Tools.GetStaticKeyword("TimeStop")
  });
     protected override bool ShouldGlowGoldInternal => IsPlayable;
        protected override bool IsPlayable
        {
            get
            {
				int cnt=0;
                if(Owner.Creature.HasPower<TimeStopPower>()&&!IsUpgraded)
					cnt+=6;
				return TimeStopPointSystem.Get(Owner) > 18+cnt;
            }
        }

        protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(18)
     ];
	public TimeDelay() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
	   if(TimeStopPointSystem.TrySpend(Owner,this.DynamicVars.Cards.IntValue))
	   { 
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		if(Owner.GetRelic<SakuyaWatch>()!=null)
		{
            Owner.GetRelic<SakuyaWatch>().SetCounter(Owner.GetRelic<SakuyaWatch>().DisplayAmount+2);
		}
	   }
	}
	protected override void OnUpgrade()
	{
		this.EnergyCost.UpgradeBy(-1);
	}
}

}
