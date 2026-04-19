using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH_Sakuya.Scrpits.Relics;
using TH_Sakuya.Scripts.Main;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.HoverTips;

namespace TH_Sakuya.Scripts.Events;

public sealed class TwistedTimeline : CustomEventModel
{
	 public override string? CustomInitialPortraitPath => "res://TH_Sakuya/ArtWorks/Events/timeline.png";
	public override bool IsAllowed(IRunState runState)
	{
		return runState.CurrentActIndex == 2 && runState.Players.All((Player p) => p.Character is SakuyaCharacter);
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new EventOption[]
		{
			new EventOption(this, Future, $"{Id.Entry}.pages.INITIAL.options.FUTURE"),
			new EventOption(this, Now, $"{Id.Entry}.pages.INITIAL.options.NOW"),
			new EventOption(this, Past, $"{Id.Entry}.pages.INITIAL.options.PAST",HoverTipFactory.FromRelic<ReverseWatch>())
		};
	}

	private async Task Future()
	{
		Player owner = base.Owner!;
		List<CardModel> deckCards = PileType.Deck.GetPile(owner).Cards.ToList();

		List<CardTransformation> transforms = deckCards.Where((CardModel c) => c.IsTransformable && c.Type != CardType.Quest).Select((CardModel c) => new CardTransformation(c)).ToList();
		List<CardModel> notTransformable = deckCards.Except(transforms.Select((CardTransformation t) => t.Original)).ToList();

		IEnumerable<CardPileAddResult> results = await CardCmd.Transform(transforms, base.Rng, CardPreviewStyle.EventLayout);
		foreach (CardPileAddResult r in results)
		{
			CardCmd.Upgrade(r.cardAdded, CardPreviewStyle.EventLayout);
		}
		CardCmd.Upgrade(notTransformable, CardPreviewStyle.EventLayout);

		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.FUTURE.description"));
	}

	private async Task Now()
	{
		Player owner = base.Owner!;
		await CreatureCmd.SetCurrentHp(owner.Creature, System.Math.Max(0, owner.Creature.CurrentHp - 5));

		CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 3, 3)
		{
			RequireManualConfirmation = true
		};
		IEnumerable<CardModel> cards = await CardSelectCmd.FromDeckForUpgrade(owner, prefs);
		CardCmd.Upgrade(cards, CardPreviewStyle.EventLayout);
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.NOW.description"));
	}

	private async Task Past()
	{
		Player owner = base.Owner!;
		await CreatureCmd.SetCurrentHp(owner.Creature, System.Math.Max(0, owner.Creature.CurrentHp - 10));
		RelicModel relic = ModelDb.Relic<ReverseWatch>().ToMutable();
		await RelicCmd.Obtain(relic, owner);
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.PAST.description"));
	}
}

