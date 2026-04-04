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
using System.Linq;
using System.Threading.Tasks;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;
using Patchoulib.Scrpits.Main;

[Pool(typeof(SakuyaRelicPool))]
    public class SakuyaWatch : CustomRelicModel, IRightCilckable
    {
        private const int MaxTimeStopCount = 12;
        private int _timeStopCount = MaxTimeStopCount;
        private bool _shouldRefillOnNextTurnStart;
        private bool _hasGrantedFirstTimeStopTspThisCombat;

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

        

        public override Task BeforeCombatStart()
        {
            ResetCounter();
            _shouldRefillOnNextTurnStart = false;
            _hasGrantedFirstTimeStopTspThisCombat = false;
            TimeStopPointSystem.InitForCombat(Owner);
            return Task.CompletedTask;
        }

        public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room)
        {
            _shouldRefillOnNextTurnStart = false;
            _hasGrantedFirstTimeStopTspThisCombat = false;
            return Task.CompletedTask;
        }

        public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner)
            {
                return Task.CompletedTask;
            }

            if (_shouldRefillOnNextTurnStart)
            {
                _shouldRefillOnNextTurnStart = false;
                ResetCounter();
            }

            if (!player.Creature.HasPower<TimeStopPower>())
            {
                int max = TimeStopPointSystem.GetMax(player);
                TimeStopPointSystem.Gain(player, max / 4);
            }

            if (player.Creature.HasPower<TimeStopPower>())
            {
                DecrementCounterAndMaybeEndTurn(player);
            }

            return Task.CompletedTask;
        }

        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (Owner == null || cardPlay.Card.Owner != Owner)
            {
                return Task.CompletedTask;
            }
            if (!Owner.Creature.HasPower<TimeStopPower>())
            {
                return Task.CompletedTask;
            }
            DecrementCounterAndMaybeEndTurn(Owner);
            return Task.CompletedTask;
        }

        private async Task ToggleTimeStop(Player player)
        {
            if (player.Creature.HasPower<TimeStopPower>())
            {
                await PowerCmd.Remove<TimeStopPower>(player.Creature);
            }
            else
            {
                TryGrantFirstTimeStopTsp(player);
                await PowerCmd.Apply<TimeStopPower>(player.Creature, 1m, player.Creature, cardSource: null, silent: true);
            }
        }

        private void TryGrantFirstTimeStopTsp(Player player)
        {
            if (_hasGrantedFirstTimeStopTspThisCombat)
            {
                return;
            }
            if (player.Creature.CombatState == null)
            {
                return;
            }
            int gained = CalculateEnemyAttackIntentTotal(player);
            if (gained <= 0)
            {
                _hasGrantedFirstTimeStopTspThisCombat = true;
                return;
            }
            TimeStopPointSystem.Gain(player, gained);
            _hasGrantedFirstTimeStopTspThisCombat = true;
        }

        private static int CalculateEnemyAttackIntentTotal(Player player)
        {
            var combatState = player.Creature.CombatState;
            if (combatState == null)
            {
                return 0;
            }
            var targets = combatState.PlayerCreatures;
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

        private void ResetCounter()
        {
            _timeStopCount = MaxTimeStopCount;
            InvokeDisplayAmountChanged();
        }

        private void DecrementCounterAndMaybeEndTurn(Player player)
        {
            if (_timeStopCount <= 0)
            {
                return;
            }
            _timeStopCount -= 1;
            InvokeDisplayAmountChanged();
            if (_timeStopCount == 0)
            {
                _shouldRefillOnNextTurnStart = true;
                TaskHelper.RunSafely(PowerCmd.Remove<TimeStopPower>(player.Creature));
                PlayerCmd.EndTurn(player, canBackOut: false);
            }
        }

        public Task OnRightClick(PlayerChoiceContext context)
        {
            if (Owner?.Creature == null)
            {
                return Task.CompletedTask;
            }
            return ToggleTimeStop(Owner);
        }
}
