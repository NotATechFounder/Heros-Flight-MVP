using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    public class HealthController : MonoBehaviour,IHealthController
    {
        public event Action OnBeingDamaged;
    }

}
