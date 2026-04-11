using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
public class SakuyaWorld: SakuyaCardModel
{
	public override bool GainsBlock => true;
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 Tools.GetStaticKeyword("TimeStop")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(3)
     ];
	public SakuyaWorld() : base(5, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		 List<PowerModel> to_remove=new List<PowerModel>();
        foreach(PowerModel debuff in Owner.Creature.Powers)
        {
			if(debuff.Type==PowerType.Debuff)
            to_remove.Add(debuff);
        }
        for(int i=to_remove.Count-1;i>=0;i--)
        {
			await PowerCmd.Remove(to_remove[i]);
			to_remove.RemoveAt(i);
	 	}
		foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies)
		{
			if(mos.IsAlive)
			{
				await CreatureCmd.LoseBlock(mos, mos.Block);
				foreach(PowerModel power in mos.Powers)
				{
					if(power.Type==PowerType.Buff)
					{
						to_remove.Add(power);
					}
				}
			}
		}
 		for(int i=to_remove.Count-1;i>=0;i--)
        {
			await PowerCmd.Remove(to_remove[i]);
			to_remove.RemoveAt(i);
	 	}
		 if(!Owner.Creature.HasPower<TimeStopPower>())
		 {
            if(Owner.GetRelic<SakuyaWatch>()!=null)
			{
				SakuyaWatch watch = Owner.GetRelic<SakuyaWatch>();
				await watch.OnRightClick(choiceContext);
			}
		 }
		 await PowerCmd.Apply<SakuyaWorldPower>(Owner.Creature, this.DynamicVars.Cards.IntValue,Owner.Creature,this);
			
	}
	protected override void OnUpgrade()
	{
		this.AddKeyword(CardKeyword.Retain);
	}
}

}
