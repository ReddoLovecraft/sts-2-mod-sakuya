using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scrpits.Potions;
[Pool(typeof(SakuyaPotionPool))]
public sealed class RedTea : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override bool CanBeGeneratedInCombat => false;

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
    protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[1]
    {
        new EnergyVar(1)
    });
    public override string? CustomPackedImagePath => "res://TH_Sakuya/ArtWorks/Potions/RED_TEA.png";
    public override string? CustomPackedOutlinePath => "res://TH_Sakuya/ArtWorks/Potions/Outlines/RED_TEA.png"; 
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
       await CreatureCmd.Heal(Owner.Creature,6);
       await CardPileCmd.Draw(choiceContext,1,Owner);
       await PlayerCmd.GainEnergy(1,Owner);
    }
}
