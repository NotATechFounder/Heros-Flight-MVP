using HeroesFlight.System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.System.Achievement_System.ProgressionUnlocks;
using UnityEngine;

public interface IAchievementSystemInterface : SystemInterface
{
    QuestRewardHandler questRewardHandler { get; }
    ProgressionUnlocksHandler UnlocksHandlers { get; }
    void InjectUiConnection();
    void AddQuestProgress<T>(QuestEntry<T> questEntry) where T : QuestBase;
    void AddProgressionProgress<T>(QuestEntry<T> questEntry) where T : QuestBase;
}