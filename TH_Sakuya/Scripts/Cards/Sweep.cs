using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public sealed class Sweep : SakuyaCardModel
{
    public override bool GainsBlock => true;
          protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
  {
         Tools.GetStaticKeyword("TimeStop"),
         HoverTipFactory.Static(StaticHoverTip.Transform),
         HoverTipFactory.FromCard<FinishHomework>(base.IsUpgraded)
  });

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> { new BlockVar(7m, ValueProp.Move) ,new CardsVar(1)};

    public Sweep()
        : base(1, CardType.Skill, CardRarity.Basic, TargetType.None)
    {
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, Owner.Creature.Player);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2);
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}

}
