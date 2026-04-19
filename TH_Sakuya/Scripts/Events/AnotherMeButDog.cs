using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH_Sakuya.Scrpits.Relics;
using TH_Sakuya.Scripts.Main;
using MegaCrit.Sts2.Core.HoverTips;

namespace TH_Sakuya.Scripts.Events;

public sealed class AnotherMeButDog : CustomEventModel
{
	 public override string? CustomInitialPortraitPath => "res://TH_Sakuya/ArtWorks/Events/dogsakuya.png";
	public override bool IsAllowed(IRunState runState)
	{
		return runState.CurrentActIndex == 0 && runState.Players.All((Player p) => p.Character is SakuyaCharacter);
	}

	public override LocString InitialDescription => new LocString(LocTable, $"{Id.Entry}.pages.INITIAL.description");

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new EventOption[]
		{
			new EventOption(this, Adopt, $"{Id.Entry}.pages.INITIAL.options.BUY_POTION",HoverTipFactory.FromRelic<DogSakuya>()),
			new EventOption(this, Ignore, $"{Id.Entry}.pages.INITIAL.options.BUY_HEAL")
		};
	}

	private async Task Adopt()
	{
		RelicModel relic = ModelDb.Relic<DogSakuya>().ToMutable();
		await RelicCmd.Obtain(relic, base.Owner!);
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.BUY_POTION.description"));
	}

	private Task Ignore()
	{
		SetEventFinished(new LocString(LocTable, $"{Id.Entry}.pages.BUY_HEAL.description"));
		return Task.CompletedTask;
	}
}
