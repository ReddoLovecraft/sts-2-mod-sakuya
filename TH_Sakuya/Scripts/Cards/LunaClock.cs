using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Sakuya.Scripts.Main;
using TH_Sakuya.Scripts.Powers;

namespace TH_Sakuya.Scrpits.Cards
{
[Pool(typeof(SakuyaCardPool))]
public class LunaClock: SakuyaCardModel
{
	   public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Ethereal];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
		 Tools.GetStaticKeyword("TimeStop")
  });
    
	public LunaClock() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		if(Owner.GetRelic<SakuyaWatch>!=null)
		{
		  SfxCmd.Play(SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/timestop.wav"));
          Owner.GetRelic<SakuyaWatch>().ResetCounter();
		}
		else if(Owner.GetRelic<SakuyaLunaDial>()!=null)
		{
		  SfxCmd.Play(SakuyaInit.ToModSfxPath("TH_Sakuya/ArtWorks/SFX/timestop.wav"));
          Owner.GetRelic<SakuyaLunaDial>().ResetCounter();
		}
	}
	protected override void OnUpgrade()
	{
		this.RemoveKeyword(CardKeyword.Ethereal);
	}
}

}
