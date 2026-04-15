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
public sealed class RedKingPotion : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override bool CanBeGeneratedInCombat => true;

    public override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("TimeStop")];
    public override string? CustomPackedImagePath => "res://TH_Sakuya/ArtWorks/Potions/RED_KING_POTION.png";
    public override string? CustomPackedOutlinePath => "res://TH_Sakuya/ArtWorks/Potions/Outlines/RED_KING_POTION.png"; 
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
       if(Owner.GetRelic<SakuyaWatch>() !=null)
       {
          Owner.GetRelic<SakuyaWatch>().SetCounter(Owner.GetRelic<SakuyaWatch>().DisplayAmount+10);
       }
       else if(Owner.GetRelic<SakuyaLunaDial>() !=null)
       {
          Owner.GetRelic<SakuyaLunaDial>().SetCounter(Owner.GetRelic<SakuyaLunaDial>().DisplayAmount+10);
       }
    }
}
