using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH_Sakuya.Scrpits.Cards;
using TH_Sakuya.Scripts.Main;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;

namespace TH_Sakuya.Scripts.Events;

public sealed class LostFriend : CustomEventModel
{
	 public override string? CustomInitialPortraitPath => "res://TH_Sakuya/ArtWorks/Events/lostfriend.png";
	public override bool IsAllowed(IRunState runState)
	{
		bool actAllowed = runState.CurrentActIndex is 0 or 2;
		return actAllowed && runState.Players.All((Player p) => p.Character is SakuyaCharacter);
	}

	public override LocString InitialDescription => new LocString(LocTable, "TH_SAKUYA-MEET_SAKUYA.pages.INITIAL.description");

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		Player owner = base.Owner!;
		bool hasThanksNote = PileType.Deck.GetPile(owner).Cards.Any((CardModel c) => c is ThanksNote);
		EventOption showOption = hasThanksNote
			? new EventOption(this, ShowThanksNote, "TH_SAKUYA-MEET_SAKUYA.pages.INITIAL.options.SHOW",HoverTipFactory.FromCard<ThanksNote>())
			: new EventOption(this, null, "TH_SAKUYA-MEET_SAKUYA.pages.INITIAL.options.SHOW_LOCKED");

		return new EventOption[]
		{
			showOption,
			new EventOption(this, Kill, "TH_SAKUYA-MEET_SAKUYA.pages.INITIAL.options.KILL"),
			new EventOption(this, Ignore, "TH_SAKUYA-MEET_SAKUYA.pages.INITIAL.options.IGNORE")
		};
	}

	private async Task ShowThanksNote()
	{
		Player owner = base.Owner!;
		CardModel? note = PileType.Deck.GetPile(owner).Cards.FirstOrDefault((CardModel c) => c is ThanksNote);
		if (note != null)
		{
			await CardPileCmd.RemoveFromDeck(note);
		}
		await PlayerCmd.GainGold(300, owner);
		SetEventFinished(new LocString(LocTable, "TH_SAKUYA-MEET_SAKUYA.pages.SHOW.description"));
	}

	private async Task Kill()
	{
		Player owner = base.Owner!;
		bool success = base.Rng.NextBool();
		if (success)
		{
			for (int i = 0; i < 5; i++)
			{
				RelicModel relic = RelicFactory.PullNextRelicFromFront(owner).ToMutable();
				await RelicCmd.Obtain(relic, owner);
			}
			SetEventFinished(new LocString(LocTable, "TH_SAKUYA-MEET_SAKUYA.pages.KILL_SUCCESS.description"));
			return;
		}

		int loss = owner.Creature.MaxHp / 2;
		await CreatureCmd.SetCurrentHp(owner.Creature, System.Math.Max(0, owner.Creature.CurrentHp - loss));
		SetEventFinished(new LocString(LocTable, "TH_SAKUYA-MEET_SAKUYA.pages.KILL_FAILED.description"));
	}

	private Task Ignore()
	{
		SetEventFinished(new LocString(LocTable, "TH_SAKUYA-MEET_SAKUYA.pages.IGNORE.description"));
		return Task.CompletedTask;
	}
}
