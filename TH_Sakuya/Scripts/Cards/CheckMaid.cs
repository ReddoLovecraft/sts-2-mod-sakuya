using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
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
public class CheckMaid: SakuyaCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Retain];
	public CheckMaid() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
	    await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await KillCreature(Owner.Creature.CombatState.HittableEnemies);
		
	}
	public async Task KillCreature(IReadOnlyList<Creature> creatures)
	{
		if (creatures.Count == 0)
		{
			return;
		}
		CombatState combatState = creatures.First().CombatState;
		foreach (Creature creature in creatures)
		{
			int deadline=IsUpgraded?Owner.Creature.MaxHp:Owner.Creature.CurrentHp;
			if(creature.CurrentHp<=deadline)
			await CreatureCmd.Kill(creature);
		}
		await Hook.AfterDiedToDoom(combatState, creatures);
	}
	protected override void OnUpgrade()
	{
		this.AddKeyword(CardKeyword.Innate);
	}
}

}
