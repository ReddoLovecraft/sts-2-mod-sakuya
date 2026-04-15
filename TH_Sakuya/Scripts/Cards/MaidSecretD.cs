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
public class MaidSecretD : SakuyaCardModel
{
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
         Tools.GetStaticKeyword("Knife"),
  });
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(8)];
	public MaidSecretD() : base(1, CardType.Skill, CardRarity.Basic, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (base.Owner.Character is SakuyaCharacter)
		{
			 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		}
		await PowerCmd.Apply<KnifePower>( base.Owner.Creature, base.DynamicVars.Cards.IntValue,Owner.Creature,this);

	}
	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(4);
	}
}


}
