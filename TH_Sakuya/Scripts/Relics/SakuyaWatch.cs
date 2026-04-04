using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using TH_Sakuya.Scripts.Main;

[Pool(typeof(SakuyaRelicPool))]
    public class SakuyaWatch : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override string PackedIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
        protected override string PackedIconOutlinePath => $"res://TH_Sakuya/ArtWorks/Relics/Outlines/{Id.Entry}.png";
        protected override string BigIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
       
        // protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        // {
       
        // });

    }


