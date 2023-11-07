using System;
using HeroesFlight.System.Stats.Handlers;
using HeroesFlight.System.UI.Traits;
using UnityEngine;

namespace HeroesFlight.System.Stats.Traits
{
    public class TestTraitUser : MonoBehaviour
    {
        private TraitHandler traitHandler;
        [SerializeField] private TraitTreeMenu menu;
        private void Awake()
        {
            traitHandler = new TraitHandler(new Vector2Int(6,4));
             traitHandler.TryUnlockFeat("HpBoost");
             traitHandler.TryUnlockFeat("SomeTrait");
             traitHandler.ModifyTraitValue("HpBoost",3);
       
            menu.InitTree(traitHandler.GetFeatTreeData());
          //  menu.Show();
            menu.Open();
        }
    }
}