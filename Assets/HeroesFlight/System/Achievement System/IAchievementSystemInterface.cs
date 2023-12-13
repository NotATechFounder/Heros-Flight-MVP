using HeroesFlight.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAchievementSystemInterface : SystemInterface
{
    public QuestRewardHandler questRewardHandler { get; }

    public void InjectUiConnection();
}
