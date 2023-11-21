using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEditor;
using UnityEngine;

public class SPineMeshanalyzer : MonoBehaviour
{
    [ContextMenu("Check Bones")]
    public  void LogVisuals()
    {
        var skeleton = Selection.activeObject as GameObject;
         var spineSkeleton=  skeleton.GetComponent<SkeletonAnimation>();
         foreach (var slot in spineSkeleton.skeleton.Slots)
         {
             Debug.Log(slot.Data.Name);
         }

        var bone= spineSkeleton.skeleton.FindSlot("Face");
        bone.A = 1;
    }
}
