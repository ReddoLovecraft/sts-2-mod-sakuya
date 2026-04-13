using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
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
public class AnotherMurder: SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 this.EnergyHoverTip
  });
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(12, ValueProp.Move),
        new EnergyVar(2)
     ];
	 	private bool WasLastCardPlayedAttack
	{
		get
		{
			CardPlayStartedEntry cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == base.Owner && e.HappenedThisTurn(base.CombatState) && e.CardPlay.Card != this);
			if (cardPlayStartedEntry == null)
			{
				return false;
			}
			return cardPlayStartedEntry.CardPlay.Card.Type == CardType.Attack;
		}
	}
	public AnotherMurder() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
	    await DamageCmd.Attack(DynamicVars.Damage.BaseValue) .FromCard(this) .Targeting(cardPlay.Target).Execute(choiceContext);
		if(WasLastCardPlayedAttack)
		{
			await PlayerCmd.GainEnergy(this.DynamicVars.Energy.IntValue,Owner);
		}
	}
	protected override void OnUpgrade()
	{
	   base.DynamicVars.Damage.UpgradeValueBy(3);
	}
}

}
