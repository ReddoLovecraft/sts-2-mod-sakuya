using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class TreasureHuntPower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Single;
	public override bool IsInstanced => true;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/THP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/THP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MinionPower>()];
	private int cnt=0;
	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (target.Side != base.Owner.Side)
		{
			Flash();
			cnt++;
		}
	}
 		public override Task AfterCombatEnd(CombatRoom room)
        {
			for(int i=0;i<cnt;i++)
            room.AddExtraReward(base.Owner.Player, new RelicReward(Owner.Player));
            return Task.CompletedTask;
        }


}
}



