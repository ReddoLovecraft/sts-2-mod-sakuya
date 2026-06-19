using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
public class ClockWork : CustomRelicModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [(HoverTipFactory.ForEnergy(this))];
    public override RelicRarity Rarity => RelicRarity.Common;
	public override string PackedIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Sakuya/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[1]
	{
		new EnergyVar(1)
	});
       public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != CombatSide.Player)
            {
                return;
            }
           if (Owner.PlayerCombatState.Energy > 0) 
            { 
			  if(Owner.GetRelic<SakuyaWatch>() != null)
			  {
				Owner.GetRelic<SakuyaWatch>().SetCounter(Owner.GetRelic<SakuyaWatch>().DisplayAmount + 1);
			  }
			  else if(Owner.GetRelic<SakuyaLunaDial>() != null)
			  {
				Owner.GetRelic<SakuyaLunaDial>().SetCounter(Owner.GetRelic<SakuyaLunaDial>().DisplayAmount + 1);
			  }
            }
        }
}
}
