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
using MegaCrit.Sts2.Core.Models;
using TH_Sakuya.Scrpits.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

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
            if(Owner.Creature.Player.Character is SakuyaCharacter)
            {
                SakuyaCharacter.ResetUsedKnivesCount();
            }
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
            if (player.Creature.HasPower<CannotTimeStopPower>())
            {
                return;
            }
            if (player.Creature.HasPower<TimeStopPower>())
            {
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
                TryGrantFirstTimeStopTsp(player);
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

        public void ResetCounter()
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
             //能力逻辑
            if(player.Creature.HasPower<MoonNightPower>())
            {
                int cnt=player.Creature.GetPowerAmount<MoonNightPower>();
                TimeStopPointSystem.Gain(player, cnt);
            }
            //以上
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
