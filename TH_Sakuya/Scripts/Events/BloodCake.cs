using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH_Sakuya.Scrpits.Cards;
using TH_Sakuya.Scrpits.Relics;
using TH_Sakuya.Scripts.Main;
using MegaCrit.Sts2.Core.HoverTips;

namespace TH_Sakuya.Scripts.Events;

public sealed class BloodCake : CustomEventModel
{
	 public override string? CustomInitialPortraitPath => "res://TH_Sakuya/ArtWorks/Events/bloodcake.png";
	public override bool IsAllowed(IRunState runState)
	{
		return runState.Players.All((Player p) => p.Character is SakuyaCharacter && p.Gold >= 200);
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		Player owner = base.Owner!;
		EventOption buy = owner.Gold >= 200
			? new EventOption(this, Buy, $"{Id.Entry}.pages.INITIAL.options.BUY",HoverTipFactory.FromRelic<ScarletCake>())
			: new EventOption(this, null, $"{Id.Entry}.pages.INITIAL.options.BUY_LOCKED");

		return new EventOption[]
		{
			buy,
			new EventOption(this, Collect, $"{Id.Entry}.pages.INITIAL.options.COLLECT",HoverTipFactory.FromCard<RedThanBlood>())
		};
	}

	private async Task Buy()
	{
		Player owner = base.Owner!;
		await PlayerCmd.LoseGold(200, owner, GoldLossType.Spent);
		RelicModel relic = ModelDb.Relic<ScarletCake>().ToMutable();
		await RelicCmd.Obtain(relic, owner);
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.BUY.description"));
	}

	private async Task Collect()
	{
		Player owner = base.Owner!;
		List<CardModel> strikes = PileType.Deck.GetPile(owner).Cards.Where((CardModel c) => c.Rarity==CardRarity.Basic&&c.Tags.Contains(CardTag.Strike)).ToList();
		if (strikes.Count > 0)
		{
			await CardPileCmd.RemoveFromDeck(strikes);
		}

		List<CardPileAddResult> results = new List<CardPileAddResult>();
		for (int i = 0; i < 4; i++)
		{
			CardModel card = owner.RunState.CreateCard(ModelDb.Card<RedThanBlood>(), owner);
			results.Add(await CardPileCmd.Add(card, PileType.Deck));
		}
		CardCmd.PreviewCardPileAdd(results, 2f);
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.COLLECT.description"));
	}
}
