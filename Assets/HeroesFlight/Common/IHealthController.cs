using System;

namespace HeroesFlight.Common
{
    public interface IHealthController
    {
        event Action OnBeingDamaged;
    }
}