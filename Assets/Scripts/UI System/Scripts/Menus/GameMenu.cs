using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public class GameMenu : BaseMenu<GameMenu>
    {
        JuicerRuntime _openEffect;
        JuicerRuntime _closeEffect;

        public override void OnCreated()
        {
            _openEffect = _canvasGroup.JuicyAlpha(1, 0.5f);
            _openEffect.SetOnStart(() => _canvasGroup.alpha = 0);

            _closeEffect = _canvasGroup.JuicyAlpha(0, 0.5f);
            _closeEffect.SetOnStart(() => _canvasGroup.alpha = 1);
            _closeEffect.SetOnComplected(CloseMenu);
        }

        public override void OnOpened()
        {
            _openEffect.Start();
        }

        public override void OnClosed()
        {
            _closeEffect.Start();
        }
    }
}
