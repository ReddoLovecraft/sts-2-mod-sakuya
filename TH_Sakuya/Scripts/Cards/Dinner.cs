using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
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
public class Dinner: SakuyaCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
  });
  protected override bool HasEnergyCostX => true;
     protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
		new DamageVar(8, ValueProp.Move)
     ];
	public Dinner() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int num = ResolveEnergyXValue();
		bool flag=true;
		if (num > 0)
		{
			Color color = new Color(0.973f, 0.001f, 0.12f);
			double num2 = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2 : 0.3);
			NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, 0.8 + (double)Mathf.Min(8, num) * num2));
			SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
			NRun.Instance?.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(color, color));
			if(num>=2)
			flag=false;
		}
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this)
			.TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_giant_horizontal_slash")
			.Execute(choiceContext);
		if(flag)
		{
           await CardCmd.Exhaust(choiceContext,this);
		}
		else
		{
           await CreatureCmd.GainMaxHp(Owner.Creature,num);
		}
	}
	protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(3);
	}
}

}
