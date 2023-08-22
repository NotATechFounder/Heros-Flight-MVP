using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class VFXData
    {
        [SerializeField] string autoattackNormal;
        [SerializeField] string autoattackCrit;
        [SerializeField] string ultNormal;
        [SerializeField] string ultCrit;
        [SerializeField] string ultVfx;


        public string AutoattackNormal => autoattackNormal;
        public string AutoattackCrit => autoattackCrit;
        public string UltNormal => ultNormal;
        public string UltCrit => ultCrit;
        public string UltVfx => ultVfx;
    }
}