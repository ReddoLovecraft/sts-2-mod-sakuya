using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using TH_Sakuya.Scripts.Main;


namespace TH_Sakuya.Scrpits.Relics
{
[Pool(typeof(SakuyaRelicPool))]
public class MaidCloth : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
	public override string PackedIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Sakuya/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
	
	bool flag=true;
       public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
        {
            if (player.Creature.CombatState.RoundNumber > 1)
            {
                flag = false;
                base.Status = RelicStatus.Normal;
            }
            else
            {
                flag=true;
                base.Status = RelicStatus.Active;
            }
        }
	public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
	{
		if (card.Owner != base.Owner)
		{
			return;
		}
		if(!flag)
		{
			return;
		}
        if(card.IsUpgradable)
        {
			this.Flash();
            CardCmd.Upgrade(card);
        }
	}
   
}
}
