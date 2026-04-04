using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers;

public sealed class LaterDamagePower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Debuff;
	public override PowerStackType StackType => PowerStackType.Counter;
}

