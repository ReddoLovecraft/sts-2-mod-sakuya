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
using MegaCrit.Sts2.Core.ValueProps;
using TH_Sakuya.Scripts.Main;


namespace TH_Sakuya.Scrpits.Relics
{
[Pool(typeof(SakuyaRelicPool))]
public class DoctorLicense : CustomRelicModel
{
  	protected override IEnumerable<IHoverTip> ExtraHoverTips => [(HoverTipFactory.ForEnergy(this)),HoverTipFactory.Static(StaticHoverTip.Block)];
    public override RelicRarity Rarity => RelicRarity.Uncommon;
	public override string PackedIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Sakuya/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!CombatManager.Instance.IsInProgress)
		{
			await Task.CompletedTask;
			return;
		}
		if (target == base.Owner.Creature)
		{
			await Task.CompletedTask;
			return;
		}
        if(dealer==null||dealer!=base.Owner.Creature)
        {
			await Task.CompletedTask;
			return;
		}
		if(result.UnblockedDamage<=0)
		{
			await Task.CompletedTask;
			return;
		}
        Flash();
		VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_bloody_impact");
		await PowerCmd.Apply<DemisePower>( target, 1,null,null);
        await Task.CompletedTask;
	}
}
}
