using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Powers
{
public sealed class KnifePower : SakuyaPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
    public override string? CustomPackedIconPath => "res://TH_Sakuya/ArtWorks/Powers/KP32.png";
    public override string? CustomBigIconPath => "res://TH_Sakuya/ArtWorks/Powers/KP64.png";
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
	private decimal CalculateDamage(decimal dmg)
	{
		decimal result=dmg;
		if(Owner.HasPower<StrengthPower>())
		{
          result+=Owner.GetPowerAmount<StrengthPower>();
		}
		
		 if(Owner.HasPower<SharpPower>())
		 {
			Owner.GetPower<SharpPower>().Trigger();
			result+=Owner.GetPowerAmount<SharpPower>();
		 }
		if(result<0)
		{
			result=0;
		}
		return result;
	}
	public async Task ThrowKnife(PlayerChoiceContext choiceContext,Creature target,KnifeType knifeType,decimal damage,int maxKnifeThrow,int count=1)
	{
		 await CreatureCmd.TriggerAnim(Owner, "Knife", Owner.Player.Character.CastAnimDelay);
		 int cnt=Math.Min(maxKnifeThrow,Amount);
		 if(Owner.HasPower<KnifeMagicianPower>())
		 {
			Owner.GetPower<KnifeMagicianPower>().Trigger();
			TimeStopPointSystem.Gain(Owner.Player, cnt);
		 }	
 		if(Owner.HasPower<PowerMovementPower>())
		 {
		    PowerMovementPower pmp=Owner.GetPower<PowerMovementPower>();
			pmp.Trigger();
			int ct=pmp.Amount;
			await PowerCmd.Apply<PowerMovementStrengthPower>(Owner,ct,Owner,null);
		 }
		if(Owner.HasPower<GradualMovementPower>())
		 {
		  GradualMovementPower gmp=	Owner.GetPower<GradualMovementPower>();
		  gmp.Trigger();
		  int ct=gmp.Amount;
		  await CardPileCmd.Draw(choiceContext,ct,Owner.Player);
		 }



		 DamageVar damageVar=null;
		   decimal dmg=CalculateDamage(damage);
		   if(dmg<=0)
		   {
			return;
		   }
		   KnifeType kt=knifeType;
           if(Owner.HasPower<SilverAcutePower>())
		   {
			 kt=KnifeType.AllEnemies;
		   }
		   if(Owner.HasPower<TunnelEffectPower>())
		   {
			damageVar=new DamageVar(dmg,ValueProp.Unblockable|ValueProp.Unpowered);
		   }
		   else
		   {
			 damageVar=new DamageVar(dmg,ValueProp.Unpowered);
		   }
		   for(int i=0;i<cnt;i++)
		   {
			switch(kt)
			{
				case KnifeType.AllEnemies:
				for(int j=0;j<count;j++)
				{
					 List<Creature> enemies = base.CombatState.Enemies.Where((Creature e) => e.IsAlive).ToList();
					  if(enemies.Count>0)
					 {
						NShivThrowVfx Nvfx=NShivThrowVfx.Create(base.Owner, enemies.Last(), Colors.Silver);
						 if (Nvfx != null)
                		{
                    		NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(Nvfx);
                    		await Cmd.Wait(0.15f);
                		}
						 foreach (Creature item in enemies)
                		{
                    	NShivThrowVfx Nvfx2 = NShivThrowVfx.Create(base.Owner, item, Colors.Silver);
                    	if (Nvfx2 != null)
                    	{
                    	    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(Nvfx2);
                   	    }
                		}
						await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, damageVar, base.Owner);
					 }
				}
					break;
				case KnifeType.AnyEnemy:
				if(target==null)
				return;
				for(int j=0;j<count;j++)
				{
					NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NShivThrowVfx.Create(base.Owner, target, Colors.Silver));
					await CreatureCmd.Damage(choiceContext,target, damageVar, base.Owner);
				}
				break;
				case KnifeType.RandomEnemy:
					Creature creature = base.Owner.Player.RunState.Rng.CombatTargets.NextItem(base.Owner.CombatState.HittableEnemies);
				if (creature != null)
				{
					for(int j=0;j<count;j++)
				{
					if(creature.IsAlive&&creature.IsHittable)
					{
					NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NShivThrowVfx.Create(base.Owner, creature, Colors.Silver));
					await CreatureCmd.Damage(choiceContext, creature, damageVar, base.Owner);
					}
					else
					{
						break;
					}
				}
				}
					break;
			}
		   }
		   await PowerCmd.ModifyAmount(this,-cnt,null,null);
		}
	
}

}