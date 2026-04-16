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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using Patchouib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scrpits.Relics
{
[Pool(typeof(SakuyaRelicPool))]
public class Honor : CustomRelicModel,IRightCilckable
{
    public override RelicRarity Rarity => RelicRarity.Rare;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MinionPower>()];
	public override string PackedIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Sakuya/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
    int amount=0;
	[SavedProperty]
    public  int Counter
    {
        get{return counter;}
        set
        {
            AssertMutable();
			counter=value;
			InvokeDisplayAmountChanged();
        }
    }
	[SavedProperty]
    public  int Amount
    {
        get{return amount;}
        set
        {
            AssertMutable();
			amount=value;
			InvokeDisplayAmountChanged();
        }
    }
	int counter=0;
	public override bool ShowCounter => true;
    public override int DisplayAmount => counter;
	 public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom)
		{
		   base.Status = RelicStatus.Active;
		   switch(room.RoomType)
		   {
				case RoomType.Boss:
					amount=9;
					break;
				case RoomType.Elite:
					amount=6;
					break;
				default:
					amount=3;
					break;
		   }
		}
		else
		{
			base.Status = RelicStatus.Normal;
		}
	}
    public async Task OnRightClick(PlayerChoiceContext context)
    {
		if(counter<=0)
		return;
        int cnt=counter;
		ClearCounter();
		int increment=Owner.Creature.MaxHp-Owner.Creature.CurrentHp;
		if(increment<cnt)
		{
		  await CreatureCmd.Heal(Owner.Creature,increment);
          await CreatureCmd.GainMaxHp(Owner.Creature,cnt-increment);
		}
		else
		{
			await CreatureCmd.Heal(Owner.Creature,cnt); 
		}
	}
		

	public void AddCounter(int value)
	{
		counter+=value;
		InvokeDisplayAmountChanged();
	}
	public void ClearCounter()
	{
		counter=0;
		InvokeDisplayAmountChanged();
	}
	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (target.Side != base.Owner.Creature.Side)
		{
			Flash();
			if(!target.HasPower<MinionPower>())
			AddCounter(amount);
		}
	}
}
}
