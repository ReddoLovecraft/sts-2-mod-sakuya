using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using TH_Sakuya.ArtWorks.UI;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(EventCardPool))]
public sealed class LittleKnife : SakuyaCardModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

	protected override IEnumerable<DynamicVar> CanonicalVars =>
	[
		new DamageVar(1,MegaCrit.Sts2.Core.ValueProps.ValueProp.Move),
		new CardsVar(6)
	];

	public LittleKnife() : base(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		decimal hitCount = base.DynamicVars.Cards.BaseValue;
		decimal damage = base.DynamicVars.Damage.BaseValue;

		NDogSakuyaPet.TryDash();
		await DamageCmd.Attack(damage).FromCard(this).WithHitCount((int)hitCount).TargetingAllOpponents(base.CombatState).Execute(choiceContext);

		foreach (Creature enemy in base.Owner.Creature.CombatState.HittableEnemies)
		{
			if (enemy.IsAlive)
			{
				await PowerCmd.Apply<StrengthPower>(choiceContext, enemy, -hitCount * damage, base.Owner.Creature, this);
			}
		}
	}
}
}
