using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using Patchouib.Scrpits.Main;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;
using Patchoulib.Scrpits.Main;
using MegaCrit.Sts2.Core.Models;
using TH_Sakuya.Scrpits.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using TH_Sakuya.Scrpits.Relics;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using TH_Sakuya.Scripts.GameActions;

[Pool(typeof(SakuyaRelicPool))]
    public class SakuyaWatch : CustomRelicModel, IRightCilckable
    {
        private const int MaxTimeStopCount = 12;
        private int _timeStopCount = MaxTimeStopCount;
        private bool _shouldRefillOnNextTurnStart;

        public override RelicRarity Rarity => RelicRarity.Starter;
        public override string PackedIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
        protected override string PackedIconOutlinePath => $"res://TH_Sakuya/ArtWorks/Relics/Outlines/{Id.Entry}.png";
        protected override string BigIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";

        public override bool ShowCounter => true;
        public override int DisplayAmount => _timeStopCount;
       
         protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
         {
              Tools.GetStaticKeyword("TimeStop"),
              Tools.GetStaticKeyword("Tsp")
         });

        

        public override async Task BeforeCombatStart()
        {
            ResetCounter();
            SfxCmd.Play(SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/entercombat.wav"));
            if(Owner.Creature.Player.Character is SakuyaCharacter sc)
            {
                sc.ResetUsedKnivesCount();
            }
            _shouldRefillOnNextTurnStart = false;
            await TimeStopPointSystem.InitForCombat(Owner);
        }

        public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room)
        {
            _shouldRefillOnNextTurnStart = false;
            return Task.CompletedTask;
        }

        public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner)
            {
                return;
            }

            if (_shouldRefillOnNextTurnStart)
            {
                _shouldRefillOnNextTurnStart = false;
                ResetCounter();
            }

            if (!player.Creature.HasPower<TimeStopPower>())
            {
                int max = TimeStopPointSystem.GetMax(player);
                await TimeStopPointSystem.Gain(player, max / 8);
            }

            if (player.Creature.HasPower<TimeStopPower>())
            {
                await DecrementCounterAndMaybeEndTurn(player);
            }
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (Owner == null || cardPlay.Card.Owner != Owner)
            {
                return;
            }
            if (!Owner.Creature.HasPower<TimeStopPower>())
            {
                return;
            }
            await DecrementCounterAndMaybeEndTurn(Owner);
        }

        internal async Task ToggleTimeStop(Player player, int? firstGrantTspOverride = null)
        {
            if (player.Creature.HasPower<CannotTimeStopPower>())
            {
                return;
            }
            if (player.Creature.HasPower<TimeStopPower>())
            {
                if(player.Creature.HasPower<SakuyaWorldPower>())
                return;
                if(Owner.Creature.HasPower<OverMindPower>())
                {
                OverMindPower omp = Owner.Creature.GetPower<OverMindPower>();
                await omp.TriggerOverMind(true);
                }
                await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
                List<CardModel> list = new List<CardModel>();
                list.AddRange(PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c is FinishHomework));
                list.AddRange(PileType.Draw.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c is FinishHomework));
                list.AddRange(PileType.Discard.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c is FinishHomework));
                await PowerCmd.Remove<TimeStopPower>(player.Creature);
				bool hasAliveEnemy = player.Creature.CombatState?.HittableEnemies.Any(e => e != null && e.IsAlive) ?? false;
				if (!hasAliveEnemy)
				{
					return;
				}
                if(list.Count>0)
                foreach (CardModel card in list)
                {
                   CardModel cardModel = player.Creature.CombatState.CreateCard<Sweep>(base.Owner);
                    if (card.IsUpgraded)
                    {
                    CardCmd.Upgrade(cardModel);
                    }
                    await CardCmd.Transform(card, cardModel);
                }
                
            }
            else
            {
                await TryGrantFirstTimeStopTsp(player, firstGrantTspOverride);
                SfxCmd.Play(SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/entertimestop.wav"));
                await CreatureCmd.TriggerAnim(base.Owner.Creature, "TimeStop", base.Owner.Character.CastAnimDelay);
                await PowerCmd.Apply<TimeStopPower>(player.Creature, 1m, player.Creature, cardSource: null, silent: true);
                await TriggerWhenEnterTimeStop(player);
            }
        }
        private async Task TriggerWhenEnterTimeStop(Player player)
        {
            if(Owner.Creature.HasPower<OverMindPower>())
            {
                OverMindPower omp = Owner.Creature.GetPower<OverMindPower>();
                await omp.TriggerOverMind();
            }
                List<CardModel> list =
                [
                    .. PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c is Sweep),
                    .. PileType.Draw.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c is Sweep),
                    .. PileType.Discard.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c is Sweep),
                ];
                 if(list.Count>0)
                foreach (CardModel card in list)
                {
                   CardModel cardModel = player.Creature.CombatState.CreateCard<FinishHomework>(base.Owner);
                    if (card.IsUpgraded)
                    {
                    CardCmd.Upgrade(cardModel);
                    }
                    await CardCmd.Transform(card, cardModel);
                }
                 List<CardModel> list2 =
                 [
                     .. PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c is Circle),
                     .. PileType.Draw.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c is Circle),
                     .. PileType.Discard.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c is Circle),
                 ];
                 if(list2.Count>0)
                foreach (Circle c in list2)
                {
                   await c.MoveUpperCardPile();
                }
                if(Owner.Creature.HasPower<OverMindPower>())
                {
                    
                }
        }
        private async Task TryGrantFirstTimeStopTsp(Player player, int? firstGrantTspOverride)
        {
            if (player?.Creature == null || player.Creature.CombatState == null)
            {
                return;
            }
            if (player.Creature.HasPower<TimeStopFirstGrantPower>())
            {
                return;
            }
            int gained = firstGrantTspOverride ?? CalculateEnemyAttackIntentTotal(player);
            if (gained > 0)
            {
                await TimeStopPointSystem.Gain(player, gained);
            }
            await PowerCmd.Apply<TimeStopFirstGrantPower>(player.Creature, 1m, player.Creature, cardSource: null, silent: true);
        }

        private static int CalculateEnemyAttackIntentTotal(Player player)
        {
            if (player?.Creature?.CombatState == null)
            {
                return 0;
            }
            var combatState = player.Creature.CombatState;
            List<Creature> targets = [player.Creature];

            int sum = 0;
            foreach (Creature enemy in combatState.HittableEnemies)
            {
                if (enemy == null || !enemy.IsAlive || enemy.Monster == null)
                {
                    continue;
                }
                foreach (AttackIntent intent in enemy.Monster.NextMove.Intents.OfType<AttackIntent>())
                {
                    sum += intent.GetTotalDamage(targets, enemy);
                }
            }
            return sum;
        }

        public void ResetCounter()
        {
            _timeStopCount = MaxTimeStopCount;
            InvokeDisplayAmountChanged();
        }
        public void SetCounter(int count)
        {
            if(count>12)count=12;
            _timeStopCount = count;
            InvokeDisplayAmountChanged();
        }

        public async Task DecrementCounterAndMaybeEndTurn(Player player)
        {
            if (_timeStopCount <= 0)
            {
                return;
            }
            if(Owner.GetRelic<ClockHand>()!=null)
            {
                Rng rng = player.RunState.Rng.CombatCardGeneration;
                int randomNumber = rng.NextInt(1, 11);
                if(randomNumber>=5)
                return;
            }
            if(Owner.Creature.HasPower<ReverseTimePower>()&&Owner.Creature.HasPower<TimeStopPower>())
            {
                _timeStopCount += 1;
                if(_timeStopCount>MaxTimeStopCount)
                {
                    _timeStopCount=MaxTimeStopCount;
                }
                InvokeDisplayAmountChanged();
                return;
            }
            _timeStopCount -= 1;
            InvokeDisplayAmountChanged();
             //能力逻辑
            if(player.Creature.HasPower<MoonNightPower>())
            {
                int cnt=player.Creature.GetPowerAmount<MoonNightPower>();
                await TimeStopPointSystem.Gain(player, cnt);
            }
            //以上
            if (_timeStopCount == 0)
            {
                _shouldRefillOnNextTurnStart = true;
                if(!player.Creature.HasPower<SakuyaWorldPower>())
                {
                    await PowerCmd.Remove<TimeStopPower>(player.Creature);
                }
                SfxCmd.Play(SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/timestop.wav"));
                bool hasAliveEnemy = player.Creature.CombatState?.HittableEnemies.Any(e => e != null && e.IsAlive) ?? false;
                if (hasAliveEnemy)
                {
                    PlayerCmd.EndTurn(player, canBackOut: false);
                }
            }
        }

        public Task OnRightClick(PlayerChoiceContext context)
        {
            if (Owner?.Creature == null)
            {
                return Task.CompletedTask;
            }
            if (context is GameActionPlayerChoiceContext)
            {
                return ToggleTimeStop(Owner, null);
            }
            if (RunManager.Instance.IsInProgress && RunManager.Instance.NetService.Type != NetGameType.Singleplayer)
            {
                RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new TimeStopToggleAction(Owner));
                return Task.CompletedTask;
            }
            return ToggleTimeStop(Owner, null);
        }
}
