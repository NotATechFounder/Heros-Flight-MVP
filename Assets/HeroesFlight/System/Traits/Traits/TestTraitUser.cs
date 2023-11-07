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
            traitHandler = new TraitHandler();
             traitHandler.TryUnlockFeat("HpBoost");
             traitHandler.TryUnlockFeat("SomeTrait");
            menu.InitTree(traitHandler.GetFeatTreeData());
          //  menu.Show();
            menu.Open();
        }
    }
}