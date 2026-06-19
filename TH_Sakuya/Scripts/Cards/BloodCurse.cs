using BaseLib.Extensions;
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
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(CurseCardPool))]
public class BloodCurse: SakuyaCardModel
{
	public override int MaxUpgradeLevel => 0;

		public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable,CardKeyword.Eternal];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
         HoverTipFactory.FromPower<BloodCursePower>()
  });

	public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
	{
		if (card == this)
		{
			if(Owner.HasPower<BloodCursePower>())
			{
			  await	Owner.Creature.GetPower<BloodCursePower>().TriggerEffect(choiceContext);
			}
			else
			{
				await PowerCmd.Apply<BloodCursePower>(choiceContext, Owner.Creature,1,null,null);
			}
		}
	}
	public BloodCurse() : base(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
	{
	}
	
}

}
