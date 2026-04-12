using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class SoulSculpture: SakuyaCardModel
{
	 protected override IEnumerable<DynamicVar> CanonicalVars =>
	 [
		new DamageVar(4, ValueProp.Move),
		new CardsVar(10)
	 ];
	public SoulSculpture() : base(4, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
			Color color = new Color(0.973f, 0.001f, 0.12f);
			double num2 = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2 : 0.3);
			NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, 0.8 + 8 * num2));
			SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
			NRun.Instance?.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(color, color));
			foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies) 
		{
			if (mos.IsAlive) 
			{
			await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(this.DynamicVars.Cards.IntValue).FromCard(this)
			.Targeting(mos)
			.WithHitVfxNode((Creature t) => NScratchVfx.Create(t, goingRight: true))
			.Execute(choiceContext);
			}
		}
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(2);
	}
}

}
