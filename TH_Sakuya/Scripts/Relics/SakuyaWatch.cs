using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
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
using System;
using TH_Sakuya.Scripts.GameActions;
using System.IO;
using System.Text;
using System.Text.Json;

[Pool(typeof(SakuyaRelicPool))]
    public class SakuyaWatch : CustomRelicModel, IRightCilckable
    {
        // #region debug-point A:config
        private const string DebugEnvPath = ".dbg/timestop-toggle-crash.env";
        private const string DebugFallbackUrl = "http://127.0.0.1:7777/event";
        private const string DebugSessionId = "timestop-toggle-crash";
        private const string DebugRunId = "pre-fix";
        private static readonly System.Net.Http.HttpClient _debugHttp = new System.Net.Http.HttpClient();
        // #endregion

        private const int MaxTimeStopCount = 12;
        private int _timeStopCount = MaxTimeStopCount;
        private bool _shouldRefillOnNextTurnStart;
        private bool _isToggleInProgress;

        // #region debug-point A:report
        private static void ReportDebug(string hypothesisId, string location, string msg, object data)
        {
            Task.Run(async () =>
            {
                try
                {
                    string url = DebugFallbackUrl;
                    string sessionId = DebugSessionId;
                    if (File.Exists(DebugEnvPath))
                    {
                        foreach (string line in File.ReadAllLines(DebugEnvPath))
                        {
                            if (line.StartsWith("DEBUG_SERVER_URL=", StringComparison.Ordinal))
                            {
                                url = line["DEBUG_SERVER_URL=".Length..];
                            }
                            else if (line.StartsWith("DEBUG_SESSION_ID=", StringComparison.Ordinal))
                            {
                                sessionId = line["DEBUG_SESSION_ID=".Length..];
                            }
                        }
                    }
                    string payload = JsonSerializer.Serialize(new
                    {
                        sessionId,
                        runId = DebugRunId,
                        hypothesisId,
                        location,
                        msg = "[DEBUG] " + msg,
                        data,
                        ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    });
                    using System.Net.Http.StringContent content = new System.Net.Http.StringContent(payload, Encoding.UTF8, "application/json");
                    await _debugHttp.PostAsync(url, content);
                }
                catch
                {
                }
            });
        }
        // #endregion

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
            _isToggleInProgress = false;
            TimeStopScreenOverlay.Reset();
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
            // #region debug-point A:toggle-enter
            ReportDebug("A", "SakuyaWatch.ToggleTimeStop", "toggle-enter", new
            {
                inProgress = _isToggleInProgress,
                isInCombat = CombatManager.Instance.IsInProgress,
                hasTimeStop = player?.Creature?.HasPower<TimeStopPower>() ?? false,
                timeStopCount = _timeStopCount,
                sweepCount = CountCardsInPiles<Sweep>(),
                finishHomeworkCount = CountCardsInPiles<FinishHomework>()
            });
            // #endregion
            if (!CombatManager.Instance.IsInProgress)
            {
                return;
            }
            if (_isToggleInProgress)
            {
                // #region debug-point A:toggle-skip
                ReportDebug("A", "SakuyaWatch.ToggleTimeStop", "toggle-skip-in-progress", new
                {
                    timeStopCount = _timeStopCount
                });
                // #endregion
                return;
            }
            _isToggleInProgress = true;
            try
            {
            if (player.Creature.HasPower<CannotTimeStopPower>())
            {
                return;
            }
            if (player.Creature.HasPower<TimeStopPower>())
            {
                // #region debug-point B:exit-start
                ReportDebug("B", "SakuyaWatch.ToggleTimeStop", "exit-time-stop-start", new
                {
                    finishHomeworkCount = CountCardsInPiles<FinishHomework>(),
                    sweepCount = CountCardsInPiles<Sweep>(),
                    hasOverMind = Owner.Creature.HasPower<OverMindPower>()
                });
                // #endregion
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
                // #region debug-point D:exit-after-remove
                ReportDebug("D", "SakuyaWatch.ToggleTimeStop", "exit-time-stop-after-remove-power", new
                {
                    queuedTransformCount = list.Count,
                    hasTimeStop = player.Creature.HasPower<TimeStopPower>()
                });
                // #endregion
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
                // #region debug-point B:exit-done
                ReportDebug("B", "SakuyaWatch.ToggleTimeStop", "exit-time-stop-finished-transforms", new
                {
                    transformedCount = list.Count,
                    finishHomeworkCount = CountCardsInPiles<FinishHomework>(),
                    sweepCount = CountCardsInPiles<Sweep>()
                });
                // #endregion
                
            }
            else
            {
                // #region debug-point B:enter-start
                ReportDebug("B", "SakuyaWatch.ToggleTimeStop", "enter-time-stop-start", new
                {
                    firstGrantTspOverride,
                    sweepCount = CountCardsInPiles<Sweep>(),
                    finishHomeworkCount = CountCardsInPiles<FinishHomework>()
                });
                // #endregion
                await TryGrantFirstTimeStopTsp(player, firstGrantTspOverride);
                SfxCmd.Play(SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/entertimestop.wav"));
                await CreatureCmd.TriggerAnim(base.Owner.Creature, "TimeStop", base.Owner.Character.CastAnimDelay);
                await PowerCmd.Apply<TimeStopPower>(new ThrowingPlayerChoiceContext(), player.Creature, 1m, player.Creature, cardSource: null, silent: true);
                await TriggerWhenEnterTimeStop(player);
            }
            }
            finally
            {
                // #region debug-point A:toggle-finally
                ReportDebug("A", "SakuyaWatch.ToggleTimeStop", "toggle-finally", new
                {
                    hadTimeStopAtFinally = player?.Creature?.HasPower<TimeStopPower>() ?? false,
                    timeStopCount = _timeStopCount,
                    sweepCount = CountCardsInPiles<Sweep>(),
                    finishHomeworkCount = CountCardsInPiles<FinishHomework>()
                });
                // #endregion
                _isToggleInProgress = false;
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
                // #region debug-point B:enter-transform-begin
                ReportDebug("B", "SakuyaWatch.TriggerWhenEnterTimeStop", "enter-transform-begin", new
                {
                    sweepCount = list.Count,
                    finishHomeworkCount = CountCardsInPiles<FinishHomework>()
                });
                // #endregion
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
                 // #region debug-point B:enter-transform-done
                 ReportDebug("B", "SakuyaWatch.TriggerWhenEnterTimeStop", "enter-transform-done", new
                 {
                     transformedCount = list.Count,
                     circleCount = list2.Count,
                     sweepCount = CountCardsInPiles<Sweep>(),
                     finishHomeworkCount = CountCardsInPiles<FinishHomework>()
                 });
                 // #endregion
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
            await PowerCmd.Apply<TimeStopFirstGrantPower>(new ThrowingPlayerChoiceContext(), player.Creature, 1m, player.Creature, cardSource: null, silent: true);
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

        private int CountCardsInPiles<TCard>() where TCard : CardModel
        {
            if (Owner == null)
            {
                return 0;
            }
            int count = 0;
            count += PileType.Hand.GetPile(Owner).Cards.Count(c => c != null && c is TCard);
            count += PileType.Draw.GetPile(Owner).Cards.Count(c => c != null && c is TCard);
            count += PileType.Discard.GetPile(Owner).Cards.Count(c => c != null && c is TCard);
            return count;
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
            if (!CombatManager.Instance.IsInProgress)
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
