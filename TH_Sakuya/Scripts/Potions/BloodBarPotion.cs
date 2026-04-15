using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;
using TH_Sakuya.Scrpits.Cards;

namespace TH_Sakuya.Scrpits.Potions;
[Pool(typeof(SakuyaPotionPool))]
public sealed class BloodBarPotion : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override bool CanBeGeneratedInCombat => true;

    public override IEnumerable<IHoverTip> ExtraHoverTips => 
    [HoverTipFactory.ForEnergy(this),HoverTipFactory.FromCard<BloodCurse>(),HoverTipFactory.FromPower<BloodCursePower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[2]
    {
        new EnergyVar(3),
        new DynamicVar("HealPercent", 25m)
    });
    public override string? CustomPackedImagePath => "res://TH_Sakuya/ArtWorks/Potions/BLOOD_BAR_POTION.png";
    public override string? CustomPackedOutlinePath => "res://TH_Sakuya/ArtWorks/Potions/Outlines/BLOOD_BAR_POTION.png";
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
       await PlayerCmd.GainEnergy(3,Owner);
       await CreatureCmd.Heal(Owner.Creature,Owner.Creature.MaxHp/4);
       if(Owner.HasPower<BloodCursePower>())
       {
          Owner.Creature.GetPower<BloodCursePower>().dec(1);
       }
       else
            await TaskHelper.RunSafely(CardPileCmd.AddCurseToDeck<BloodCurse>(Owner));
    }
}
