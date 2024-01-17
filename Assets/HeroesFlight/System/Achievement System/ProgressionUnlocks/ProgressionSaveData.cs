using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace HeroesFlight.System.Achievement_System.ProgressionUnlocks
{
    [Serializable]
    public class ProgressionSaveData
    {
        [FormerlySerializedAs("UnlockedId")] public List<string> UnlockedIds = new List<string>();
        
    }
}