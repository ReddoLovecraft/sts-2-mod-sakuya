using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;


namespace TH_Sakuya.Scripts.Main
{
    public abstract class SakuyaCardModel : CustomCardModel
    {
        public override string PortraitPath => $"res://TH_Sakuya/ArtWorks/Cards/{Id.Entry}.png";
        public SakuyaCardModel(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true)
     : base(baseCost, type, rarity, target, showInCardLibrary)
        {
            if (autoAdd)
            {
                CustomContentDictionary.AddModel(GetType());
            }
        }


    }
  
}
