using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using TH_Sakuya.Scrpits.Cards;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scrpits.Relics
{
[Pool(typeof(SakuyaRelicPool))]
public sealed class DogSakuya : CustomRelicModel
{
	public override string PackedIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
	protected override string PackedIconOutlinePath => $"res://TH_Sakuya/ArtWorks/Relics/Outlines/{Id.Entry}.png";
	protected override string BigIconPath => $"res://TH_Sakuya/ArtWorks/Relics/{Id.Entry}.png";
	public override RelicRarity Rarity => RelicRarity.Event;

	public override async Task AfterObtained()
	{
		if (base.Owner == null)
		{
			return;
		}

		CardModel card = base.Owner.RunState.CreateCard(ModelDb.Card<LittleKnife>(), base.Owner);
		CardPileAddResult result = await CardPileCmd.Add(card, PileType.Deck);
		CardCmd.PreviewCardPileAdd(result, 2f);
	}
}
}
