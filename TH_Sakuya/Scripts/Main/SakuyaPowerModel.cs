using BaseLib.Abstracts;

namespace TH_Sakuya.Scripts.Main
{
    public abstract class SakuyaPowerModel : CustomPowerModel
    {
        public virtual void Trigger()
        {
            Flash();
        }
    }
    
}
