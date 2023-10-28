using System;
using System.Collections.Generic;

namespace HeroesFlight.System.Combat
{
    [Serializable]
    public class BossAttackPattern
    {
        public List<WarningLine> Lines = new();
    }
}