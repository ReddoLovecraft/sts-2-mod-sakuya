using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TH_Sakuya.Scripts.Events;

public sealed class Library : CustomEventModel
{
	 public override string? CustomInitialPortraitPath => "res://TH_Sakuya/ArtWorks/Events/library.png";
	public override bool IsAllowed(IRunState runState)
	{
		return runState.CurrentActIndex is 0 or 1;
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new EventOption[]
		{
			new EventOption(this, ReadBook, $"{Id.Entry}.pages.INITIAL.options.READ"),
			new EventOption(this, Sleep, $"{Id.Entry}.pages.INITIAL.options.SLEEP")
		};
	}

	private async Task ReadBook()
	{
		Player owner = base.Owner!;
		List<CardModel> libraryCards = owner.Character.CardPool.AllCards.Where((CardModel c) => c.ShouldShowInCardLibrary && c.Type != CardType.Quest).ToList();
		CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1, 1)
		{
			RequireManualConfirmation = true
		};
		CardModel selected = (await CardSelectCmd.FromSimpleGrid(new BlockingPlayerChoiceContext(), libraryCards, owner, prefs)).FirstOrDefault();
		if (selected != null)
		{
			CardModel card = owner.RunState.CreateCard(selected, owner);
			CardPileAddResult result = await CardPileCmd.Add(card, PileType.Deck);
			CardCmd.PreviewCardPileAdd(result, 2f);
		}
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.READ.description"));
	}

	private async Task Sleep()
	{
		await CreatureCmd.Heal(base.Owner!.Creature, 20);
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.SLEEP.description"));
	}
}
