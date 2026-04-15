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
using Patchoulib.Scrpits.Main;
using System.Collections.Generic;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scrpits.Potions;
[Pool(typeof(SakuyaPotionPool))]
public sealed class ManaPotion : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override bool CanBeGeneratedInCombat => true;

    public override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("Tsp")];
    public override string? CustomPackedImagePath => "res://TH_Sakuya/ArtWorks/Potions/MANA_POTION.png";
    public override string? CustomPackedOutlinePath => "res://TH_Sakuya/ArtWorks/Potions/Outlines/MANA_POTION.png"; 
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
       TimeStopPointSystem.Gain(Owner,24);
    }
}
