using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using TH_Sakuya.Scrpits.Cards;

namespace TH_Sakuya.Scripts.Main
{
	public class SakuyaCharacter : PlaceholderCharacterModel
	{
		public override Color NameColor => new Color("8ca6c2ff");
		public override Color EnergyLabelOutlineColor => new Color("8ca6c2ff");
		public override Color DialogueColor => new Color("8ca6c2ff");
		public override Color MapDrawingColor => new Color("a5c6f9ff");
		public override Color RemoteTargetingLineColor => new Color("96b9faff");
		public override Color RemoteTargetingLineOutline => new Color("5c68ffff");
		public override CharacterGender Gender => CharacterGender.Feminine;
		public override bool ShouldAlwaysShowStarCounter => true;
		public override int StartingHp => 80;
        public override string CustomVisualPath => "res://TH_Sakuya/ArtWorks/Character/sakuya.tscn";
        public override string CustomTrailPath => "res://TH_Sakuya/ArtWorks/VFX/SakuyaCardTrail.tscn";
        public override string CustomIconTexturePath => "res://TH_Sakuya/ArtWorks/Character/sakuya_icon.png";
		public override string CustomIconPath => "res://TH_Sakuya/ArtWorks/Character/sakuya_icon.tscn";
        public override string CustomEnergyCounterPath => "res://TH_Sakuya/ArtWorks/Character/sakuya_energy_counter.tscn";
        // 篝火休息动画。
        public override string CustomRestSiteAnimPath => "res://TH_Sakuya/ArtWorks/Character/sakuyarest.tscn";
        // 商店人物动画。
        public override string CustomMerchantAnimPath => "res://TH_Sakuya/ArtWorks/Character/sakuya_merchant.tscn";
        public override string CustomArmPointingTexturePath => "res://TH_Sakuya/ArtWorks/Character/multiplayer_hand_sakuya_point.png";
		public override string CustomArmRockTexturePath => "res://TH_Sakuya/ArtWorks/Character/multiplayer_hand_sakuya_rock.png";
		public override string CustomArmPaperTexturePath => "res://TH_Sakuya/ArtWorks/Character/multiplayer_hand_sakuya_paper.png";
		public override string CustomArmScissorsTexturePath => "res://TH_Sakuya/ArtWorks/Character/multiplayer_hand_sakuya_scissors.png";
        public override string CustomCharacterSelectBg => "res://TH_Sakuya/ArtWorks/Character/Sakuya_bg.tscn";
        public override string CustomCharacterSelectIconPath => "res://TH_Sakuya/ArtWorks/Character/char_select_sakuya.png";
	    public override string CustomCharacterSelectLockedIconPath => "res://TH_Sakuya/ArtWorks/Character/char_select_sakuya_locked.png";
	    public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/silent_transition_mat.tres";
		public override string CustomMapMarkerPath => "res://TH_Sakuya/ArtWorks/Character/map_marker_sakuya.png";
		// 攻击音效
		public override string CustomAttackSfx => SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/attack.wav");
		// 施法音效
		public override string CustomCastSfx => SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/cast.wav");
		// 死亡音效
		public override string CustomDeathSfx => SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/die.ogg");
	    public override string CharacterSelectSfx  => SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/characterselect.wav");
		public override string CharacterTransitionSfx => SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/transition.wav");
		public override CardPoolModel CardPool => ModelDb.CardPool<SakuyaCardPool>();
		public override RelicPoolModel RelicPool => ModelDb.RelicPool<SakuyaRelicPool>();
		public override PotionPoolModel PotionPool => ModelDb.PotionPool<SakuyaPotionPool>();

		// 初始卡组
		public override IEnumerable<CardModel> StartingDeck => [
			 ModelDb.Card<Strike>(),
			 ModelDb.Card<Strike>(),
			 ModelDb.Card<Strike>(),
			 ModelDb.Card<Strike>(),
			 ModelDb.Card<Defend>(),
			 ModelDb.Card<Defend>(),
			 ModelDb.Card<Defend>(),
			 ModelDb.Card<Defend>(),
			 ModelDb.Card<MaidSecret>(),
			 ModelDb.Card<Sweep>()
	];

		// 初始遗物
		public override IReadOnlyList<RelicModel> StartingRelics => [
		    ModelDb.Relic<SakuyaWatch>()
	];

		// 攻击建筑师的攻击特效列表
		public override List<string> GetArchitectAttackVfx() => [
		"vfx/vfx_attack_blunt",
		"vfx/vfx_heavy_blunt",
		"vfx/vfx_attack_slash",
		"vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
		];
		public override CreatureAnimator GenerateAnimator(MegaSprite controller)
		{
			AnimState animState = new AnimState("Idle", isLooping: true);
			AnimState animState2 = new AnimState("Cast");
			AnimState animState3 = new AnimState("Attack");
			AnimState animState4 = new AnimState("Hit");
			AnimState state = new AnimState("die");
			AnimState animState5 = new AnimState("relaxed_loop", isLooping: true);
			AnimState animState6 = new AnimState("Summon");
            AnimState animState7 = new AnimState("TimeStop");
            AnimState animState8 = new AnimState("Knife");
            animState6.NextState = animState;
            animState7.NextState = animState;
            animState8.NextState = animState;
            animState2.NextState = animState;
			animState3.NextState = animState;
			animState4.NextState = animState;
			animState5.AddBranch("Idle", animState);
			CreatureAnimator creatureAnimator = new CreatureAnimator(animState, controller);
			creatureAnimator.AddAnyState("Idle", animState);
			creatureAnimator.AddAnyState("Dead", state);
			creatureAnimator.AddAnyState("Hit", animState4);
			creatureAnimator.AddAnyState("Attack", animState3);
			creatureAnimator.AddAnyState("Cast", animState2);
			creatureAnimator.AddAnyState("relaxed_loop", animState5);
			creatureAnimator.AddAnyState("Summon",animState6);
			creatureAnimator.AddAnyState("TimeStop", animState7);
			creatureAnimator.AddAnyState("Knife", animState8);
            return creatureAnimator;
		}
		private int cnt=0;
		public int UsedKnivesCount()
		{
		return cnt;
		}
		public void ResetUsedKnivesCount()
		{
			cnt=0;
		}
		public void AddUsedKnivesCount(int amount)
		{
			cnt+=amount;
		}
	}
}
