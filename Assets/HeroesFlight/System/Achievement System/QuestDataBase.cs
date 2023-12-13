using ScriptableObjectDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Database", menuName = "Quest/Quest Database")]
public class QuestDataBase : ScriptableObjectDatabase<QuestSO>
{

 # if UNITY_EDITOR
    [ContextMenu("Set Name")]
    public void SetInfo()
    {
        foreach (QuestSO quest in Items)
        {
            quest.SetQuestInfo(quest.name);
            UnityEditor.EditorUtility.SetDirty(quest);
        }
    }
 #endif
}
