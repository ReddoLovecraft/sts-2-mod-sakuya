using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
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
using MegaCrit.Sts2.Core.HoverTips;

namespace TH_Sakuya.Scripts.Events;

public sealed class LostHunter : CustomEventModel
{
	 public override string? CustomInitialPortraitPath => "res://TH_Sakuya/ArtWorks/Events/losthunter.png";
	public override bool IsAllowed(IRunState runState)
	{
		return runState.CurrentActIndex == 1 && runState.Players.All((Player p) => p.Character is SakuyaCharacter);
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new EventOption[]
		{
			new EventOption(this, Ignore, $"{Id.Entry}.pages.INITIAL.options.IGNORE"),
			new EventOption(this, Kill, $"{Id.Entry}.pages.INITIAL.options.KILL"),
			new EventOption(this, Lead, $"{Id.Entry}.pages.INITIAL.options.LEAD",HoverTipFactory.FromCard<ThanksNote>())
		};
	}

	private Task Ignore()
	{
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.IGNORE.description"));
		return Task.CompletedTask;
	}

	private async Task Kill()
	{
		Player owner = base.Owner!;
		await CreatureCmd.SetCurrentHp(owner.Creature, System.Math.Max(0, owner.Creature.CurrentHp - 10));
		RelicModel relic = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Rare).ToMutable();
		await RelicCmd.Obtain(relic, owner);
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.KILL.description"));
	}

	private async Task Lead()
	{
		Player owner = base.Owner!;
		CardModel card = owner.RunState.CreateCard(ModelDb.Card<ThanksNote>(), owner);
		CardPileAddResult result = await CardPileCmd.Add(card, PileType.Deck);
		CardCmd.PreviewCardPileAdd(result, 2f);
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.LEAD.description"));
	}
}
