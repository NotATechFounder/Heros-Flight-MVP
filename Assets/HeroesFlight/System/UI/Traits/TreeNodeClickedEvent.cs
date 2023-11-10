using HeroesFlight.System.FileManager.Model;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.UI.Traits
{
    public class TreeNodeClickedEvent
    {
        public TreeNodeClickedEvent(TraitModel currentModel, Vector2 clickPosition)
        {
            TargetModel = currentModel;
            Position = clickPosition;
        }

        public TraitModel TargetModel { get; }
        public Vector2 Position { get; }
    }
}