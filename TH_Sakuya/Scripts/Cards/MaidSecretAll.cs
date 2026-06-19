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
public class MaidSecretAll: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
  {
         HoverTipFactory.FromPower<VulnerablePower>(),
         HoverTipFactory.FromPower<WeakPower>(),
		 Tools.GetStaticKeyword("Knife")
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(16, ValueProp.Move),
        new DynamicVar("Power", 2),
        new CardsVar(16)
     ];
	public MaidSecretAll() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (base.Owner.Character is SakuyaCharacter)
		{
			 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		}
			SfxCmd.Play("event:/sfx/characters/silent/silent_dagger_spray");
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
			.TargetingAllOpponents(base.CombatState)
			.WithAttackerFx(() => NDaggerSprayFlurryVfx.Create(base.Owner.Creature, new Color("#b1ccca"), goingRight: true))
			.BeforeDamage(delegate
			{
				IReadOnlyList<Creature> hittableEnemies = base.CombatState.HittableEnemies;
				foreach (Creature item in hittableEnemies)
				{
					NDaggerSprayImpactVfx child = NDaggerSprayImpactVfx.Create(item, new Color("#b1ccca"), goingRight: true);
					NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
				}
				return Task.CompletedTask;
			})
			.Execute(choiceContext);
        foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext,  mos, base.DynamicVars["Power"].IntValue,Owner.Creature,this);
            await PowerCmd.Apply<WeakPower>(choiceContext,  mos, base.DynamicVars["Power"].IntValue,Owner.Creature,this);
        }
		 await CardPileCmd.Draw(choiceContext,base.DynamicVars["Power"].IntValue,Owner.Creature.Player);
		 await PowerCmd.Apply<KnifePower>(choiceContext, Owner.Creature, base.DynamicVars.Cards.IntValue,Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars["Power"].UpgradeValueBy(1);
	}
}

}
