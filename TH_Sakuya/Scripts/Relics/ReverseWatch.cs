using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Patchouib.Scrpits.Main;
using TH_Sakuya.Scripts.GameActions;
using TH_Sakuya.Scripts.Main;
using MegaCrit.Sts2.Core.Combat;

namespace TH_Sakuya.Scrpits.Relics
{
[Pool(typeof(SakuyaRelicPool))]
public sealed class ReverseWatch : CustomRelicModel, IRightCilckable
{
	private sealed class TurnSnapshot
	{
		public int Hp { get; init; }
		public int Energy { get; init; }
		public List<CardModel> Hand { get; init; } = new();
		public List<CardModel> Draw { get; init; } = new();
		public List<CardModel> Discard { get; init; } = new();
		public List<(ModelId id, int amount)> Powers { get; init; } = new();
	}

	private TurnSnapshot? _snapshot;
	private int _cooldown;

	public override bool ShowCounter => true;
	public override int DisplayAmount => _cooldown;

	public override string PackedIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
	protected override string PackedIconOutlinePath => $"res://TH_Sakuya/ArtWorks/Relics/Outlines/{Id.Entry}.png";
	protected override string BigIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
	public override RelicRarity Rarity => RelicRarity.Event;

	public override Task BeforeCombatStart()
	{
		_snapshot = null;
		_cooldown = 0;
		InvokeDisplayAmountChanged();
		return Task.CompletedTask;
	}

	public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room)
	{
		_snapshot = null;
		_cooldown = 0;
		InvokeDisplayAmountChanged();
		return Task.CompletedTask;
	}

	public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != base.Owner || base.Owner?.Creature == null)
		{
			return Task.CompletedTask;
		}

		if (_cooldown > 0)
		{
			_cooldown -= 1;
			InvokeDisplayAmountChanged();
		}

		_snapshot = TakeSnapshot(base.Owner);
		return Task.CompletedTask;
	}

	public Task OnRightClick(PlayerChoiceContext context)
	{
		if (base.Owner?.Creature == null || base.Owner.PlayerCombatState == null)
		{
			return Task.CompletedTask;
		}
		if (!CombatManager.Instance.IsInProgress)
		{
			return Task.CompletedTask;
		}

		if (context is GameActionPlayerChoiceContext)
		{
			return TryRewind();
		}
		if (RunManager.Instance.IsInProgress && RunManager.Instance.NetService.Type != NetGameType.Singleplayer)
		{
			RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new ReverseWatchRewindAction(base.Owner));
			return Task.CompletedTask;
		}
		return TryRewind();
	}

	internal async Task TryRewind()
	{
		if (_cooldown > 0)
		{
			return;
		}
		if (_snapshot == null)
		{
			return;
		}
		if (base.Owner?.Creature == null || base.Owner.PlayerCombatState == null)
		{
			return;
		}

		Flash();
		await RestoreSnapshot(base.Owner, _snapshot);
		_cooldown = 1;
		InvokeDisplayAmountChanged();
	}

	private static TurnSnapshot TakeSnapshot(Player player)
	{
		return new TurnSnapshot
		{
			Hp = player.Creature.CurrentHp,
			Energy = player.PlayerCombatState.Energy,
			Hand = PileType.Hand.GetPile(player).Cards.ToList(),
			Draw = PileType.Draw.GetPile(player).Cards.ToList(),
			Discard = PileType.Discard.GetPile(player).Cards.ToList(),
			Powers = player.Creature.Powers.Select((PowerModel p) => (p.Id, p.Amount)).ToList()
		};
	}

	private static async Task RestoreSnapshot(Player player, TurnSnapshot snapshot)
	{
		await CreatureCmd.SetCurrentHp(player.Creature, System.Math.Max(0, snapshot.Hp));
		await PlayerCmd.SetEnergy(snapshot.Energy, player);

		List<PowerModel> currentPowers = player.Creature.Powers.ToList();
		foreach (PowerModel power in currentPowers)
		{
			await PowerCmd.Remove(power);
		}
		foreach ((ModelId id, int amount) in snapshot.Powers)
		{
			if (amount == 0)
			{
				continue;
			}
			PowerModel power = ModelDb.GetById<PowerModel>(id).ToMutable();
			await PowerCmd.Apply(power, player.Creature, amount, null, null, silent: true);
		}

		CardPile hand = PileType.Hand.GetPile(player);
		CardPile draw = PileType.Draw.GetPile(player);
		CardPile discard = PileType.Discard.GetPile(player);

		HashSet<CardModel> snapshotCards = snapshot.Hand.Concat(snapshot.Draw).Concat(snapshot.Discard).ToHashSet();

		List<CardModel> currentHand = hand.Cards.ToList();
		List<CardModel> currentDraw = draw.Cards.ToList();
		List<CardModel> currentDiscard = discard.Cards.ToList();

		List<CardModel> extrasHand = currentHand.Where((CardModel c) => !snapshotCards.Contains(c)).ToList();
		List<CardModel> extrasDraw = currentDraw.Where((CardModel c) => !snapshotCards.Contains(c)).ToList();
		List<CardModel> extrasDiscard = currentDiscard.Where((CardModel c) => !snapshotCards.Contains(c)).ToList();

		List<CardModel> desiredHand = snapshot.Hand.Concat(extrasHand).ToList();
		List<CardModel> desiredDraw = snapshot.Draw.Concat(extrasDraw).ToList();
		List<CardModel> desiredDiscard = snapshot.Discard.Concat(extrasDiscard).ToList();

		Dictionary<CardModel, PileType> desiredPile = new Dictionary<CardModel, PileType>();
		foreach (CardModel c in desiredHand)
		{
			desiredPile[c] = PileType.Hand;
		}
		foreach (CardModel c in desiredDraw)
		{
			desiredPile[c] = PileType.Draw;
		}
		foreach (CardModel c in desiredDiscard)
		{
			desiredPile[c] = PileType.Discard;
		}

		HashSet<CardModel> allCards = desiredPile.Keys.ToHashSet();
		foreach (CardModel c in snapshotCards)
		{
			allCards.Add(c);
		}

		foreach (CardModel card in allCards)
		{
			if (!desiredPile.TryGetValue(card, out PileType targetPileType))
			{
				continue;
			}

			PileType? currentPileType = card.Pile?.Type;
			if (currentPileType == targetPileType)
			{
				continue;
			}

			await CardPileCmd.Add(card, targetPileType, CardPilePosition.Top, skipVisuals: false);
		}

		for (int i = desiredHand.Count - 1; i >= 0; i--)
		{
			if (desiredHand[i].Pile?.Type == PileType.Hand)
			{
				hand.MoveToTopInternal(desiredHand[i]);
			}
		}
		for (int i = desiredDraw.Count - 1; i >= 0; i--)
		{
			if (desiredDraw[i].Pile?.Type == PileType.Draw)
			{
				draw.MoveToTopInternal(desiredDraw[i]);
			}
		}
		for (int i = desiredDiscard.Count - 1; i >= 0; i--)
		{
			if (desiredDiscard[i].Pile?.Type == PileType.Discard)
			{
				discard.MoveToTopInternal(desiredDiscard[i]);
			}
		}

		hand.InvokeContentsChanged();
		draw.InvokeContentsChanged();
		discard.InvokeContentsChanged();

		hand.InvokeCardAddFinished();
		draw.InvokeCardAddFinished();
		discard.InvokeCardAddFinished();
	}
}
}
