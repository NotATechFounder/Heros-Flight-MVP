using System;

namespace HeroesFlight.Common
{
    public interface IAttackController
    {
        event Action<IHealthController> OnAttackTarget;
        event Action OnStopAttack;
    }
}