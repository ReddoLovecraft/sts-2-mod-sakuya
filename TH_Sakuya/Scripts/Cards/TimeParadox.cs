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
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class TimeParadox: SakuyaCardModel
{
 	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
	public TimeParadox() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		 await PowerCmd.Apply<TimeParadoxPower>(Owner.Creature, 1,Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
		this.RemoveKeyword(CardKeyword.Ethereal);
		this.AddKeyword(CardKeyword.Retain);
	}
}

}
