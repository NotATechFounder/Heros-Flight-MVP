using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class EntryBase
    {
        [SerializeField] int index;
        public int Index => index;
    }
}