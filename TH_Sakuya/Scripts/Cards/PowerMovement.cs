using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class PowerMovement : SakuyaCardModel
{
	  protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
        Tools.GetStaticKeyword("Knife"),
		HoverTipFactory.FromPower<StrengthPower>()
  });
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
	public PowerMovement() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.Self)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await PowerCmd.Apply<PowerMovementPower>(choiceContext, Owner.Creature,base.DynamicVars.Cards.IntValue,Owner.Creature,this);
	}
	
	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(1); 
	}
}
}

