using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class SelfDisolve : SakuyaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Unpowered|ValueProp.Unblockable)];
	public SelfDisolve() : base(1, CardType.Status, CardRarity.Status, TargetType.None)
	{
	}
	public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
	{
		await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars.Damage.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, this);
	}
		public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
	{
		if (card == this)
		{ 
			if(Owner.Creature.HasPower<TimeStopPower>()&&!Owner.Creature.HasPower<SakuyaWorldPower>())
			{
				await PowerCmd.Remove<TimeStopPower>(Owner.Creature);
				await PowerCmd.Apply<CannotTimeStopPower>(Owner.Creature,1,null,this);
			}
		
		}
	}

}

}
