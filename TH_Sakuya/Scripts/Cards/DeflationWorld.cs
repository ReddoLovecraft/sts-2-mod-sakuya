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
public class DeflationWorld: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 Tools.GetStaticKeyword("TimeStop")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(1)
     ];
	public DeflationWorld() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int muti=12;
	    if(Owner.GetRelic<SakuyaWatch>()!=null)
	    {
	        muti-=Owner.GetRelic<SakuyaWatch>().DisplayAmount;
	    }
	    else if(Owner.GetRelic<SakuyaLunaDial>()!=null)
	    {
	        muti-=Owner.GetRelic<SakuyaLunaDial>().DisplayAmount;
	    }
		int totaldmg=this.DynamicVars.Cards.IntValue;
		for(int i=0;i<muti;i++)
		{
			totaldmg*=2;
		}
		await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature.CombatState.HittableEnemies,totaldmg, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this);

	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
