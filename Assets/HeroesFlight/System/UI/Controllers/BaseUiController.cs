using UnityEngine;

namespace HeroesFlight.System.UI.Controllers
{
    public abstract  class BaseUiController : MonoBehaviour,IUiController
    {
        Canvas m_Canvas;
        public virtual void Init()
        {
            m_Canvas = GetComponent<Canvas>();
        }

        public virtual void Show()
        {
            m_Canvas.enabled = true;
        }

        public virtual void Hide()
        {
            m_Canvas.enabled = false;
        }
    }
}