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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class TransformInstant: SakuyaCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[4]
  {
         HoverTipFactory.FromCard<ClockPart>(),
		 Tools.GetStaticKeyword("TimeStop"),
		 Tools.GetStaticKeyword("Stop"), 
		 HoverTipFactory.Static(StaticHoverTip.Transform)
  });
	public TransformInstant() : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		List<CardModel> list = new List<CardModel>();
		 if(Owner.Creature.HasPower<TimeStopPower>())
		 {
			list =PileType.Discard.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c.IsTransformable && c.Type == CardType.Status).ToList();
		 }
		 else
		 {
			list = PileType.Draw.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c.IsTransformable && c.Type == CardType.Status).ToList();
		 }
		 foreach (CardModel item in list)
		{
			CardModel cardModel = base.CombatState.CreateCard<ClockPart>(base.Owner);
			await CardCmd.Transform(item, cardModel);
		}
	}
	protected override void OnUpgrade()
	{
		this.AddKeyword(CardKeyword.Retain);
		this.EnergyCost.UpgradeBy(-1);
	}
}

}
