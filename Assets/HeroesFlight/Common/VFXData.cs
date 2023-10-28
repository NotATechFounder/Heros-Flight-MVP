using System;
using Pelumi.ObjectPool;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class VFXData
    {
        [SerializeField] Particle autoattackNormal;
        [SerializeField] Particle autoattackCrit;
        [SerializeField] Particle ultNormal;
        [SerializeField] Particle ultCrit;
       


        public Particle AutoattackNormal => autoattackNormal;
        public Particle AutoattackCrit => autoattackCrit;
        public Particle UltNormal => ultNormal;
        public Particle UltCrit => ultCrit;
       
    }
}