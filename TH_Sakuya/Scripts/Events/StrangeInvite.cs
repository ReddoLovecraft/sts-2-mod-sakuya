using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH_Sakuya.Scrpits.Cards;
using TH_Sakuya.Scrpits.Potions;

namespace TH_Sakuya.Scripts.Events;

public sealed class StrangeInvite : CustomEventModel
{
	 public override string? CustomInitialPortraitPath => "res://TH_Sakuya/ArtWorks/Events/strangeinvite.png";
	public override bool IsAllowed(IRunState runState)
	{
		return runState.CurrentActIndex is 0 or 1;
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		Player owner = base.Owner!;
		bool hasCurse = PileType.Deck.GetPile(owner).Cards.Any((CardModel c) => c is BloodCurse);

		if (hasCurse)
		{
			return new EventOption[]
			{
				new EventOption(this, Refuse, $"{Id.Entry}.pages.INITIAL.options.REFUSE"),
				new EventOption(this, Drink, $"{Id.Entry}.pages.INITIAL.options.DRINK",HoverTipFactory.FromPotion<BloodBarPotion>())
			};
		}
		return new EventOption[]
		{
			new EventOption(this, Refuse, $"{Id.Entry}.pages.INITIAL.options.REFUSE"),
			new EventOption(this, Eat, $"{Id.Entry}.pages.INITIAL.options.EAT",HoverTipFactory.FromCard<BloodCurse>())
		};
	}

	private async Task Refuse()
	{
		Player owner = base.Owner!;
		await CreatureCmd.SetCurrentHp(owner.Creature, System.Math.Max(0, owner.Creature.CurrentHp - 15));
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.REFUSE.description"));
	}

	private async Task Eat()
	{
		Player owner = base.Owner!;
		await CreatureCmd.SetCurrentHp(owner.Creature, owner.Creature.MaxHp);
		CardModel card = owner.RunState.CreateCard(ModelDb.Card<BloodCurse>(), owner);
		CardPileAddResult result = await CardPileCmd.Add(card, PileType.Deck);
		CardCmd.PreviewCardPileAdd(result, 2f);
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.EAT.description"));
	}

	private async Task Drink()
	{
		Player owner = base.Owner!;
		for (int i = 0; i < 3; i++)
		{
			await PotionCmd.TryToProcure<BloodBarPotion>(owner);
		}
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.DRINK.description"));
	}
}
