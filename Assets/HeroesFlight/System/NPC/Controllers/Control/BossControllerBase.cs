using System;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Control
{
    public class BossControllerBase : MonoBehaviour,BossControllerInterface
    {
        public event Action<float> OnHpChange;
        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}