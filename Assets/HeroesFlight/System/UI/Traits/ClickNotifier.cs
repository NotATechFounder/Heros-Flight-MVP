using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HeroesFlight.System.UI.Traits
{
    public class ClickNotifier : MonoBehaviour, IPointerClickHandler
    {
        public event Action OnClicked;
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke();
        }

    }
}