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
public class WeaponCatch: SakuyaCardModel
{
	 public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
         HoverTipFactory.FromPower<StrengthPower>()
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(2)
     ];
	public WeaponCatch() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (base.Owner.Character is SakuyaCharacter)
		{
			 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		}
        foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies)
        {
			if(mos.IsAlive)
			{
    			await PowerCmd.Apply<StrengthPower>(choiceContext, mos, -this.DynamicVars.Cards.IntValue,Owner.Creature,this);
				await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, this.DynamicVars.Cards.IntValue,Owner.Creature,this);
			}
        }
	}
	protected override void OnUpgrade()
	{
	  	this.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
