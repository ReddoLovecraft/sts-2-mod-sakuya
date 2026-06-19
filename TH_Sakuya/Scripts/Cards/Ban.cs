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
public class Ban: SakuyaCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
         HoverTipFactory.FromPower<StrengthPower>()
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(5)
     ];
	public Ban() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies)
		{
			if(mos.IsAlive)
			{
				if(mos.Monster.IntendsToAttack)
				{
 					await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, base.DynamicVars.Cards.IntValue,Owner.Creature,this);
				}
				await PowerCmd.Apply<PowerDirectionPower>(choiceContext, mos, 2,Owner.Creature,this);
			}
		}
		
	}
	protected override void OnUpgrade()
	{
		this.RemoveKeyword(CardKeyword.Exhaust);
	}
}

}
