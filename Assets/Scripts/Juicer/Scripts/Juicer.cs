using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Pelumi.Juicer
{
    public static class Juicer
    {
        #region Core
        private class JuicerMonoBehaviour : MonoBehaviour { }

        private static JuicerMonoBehaviour JuicerController;

        private static void Init()
        {
            if (JuicerController == null)
            {
                GameObject gameObject = new GameObject("JuicerController");
                JuicerController = gameObject.AddComponent<JuicerMonoBehaviour>();
            }
        }

        public static CoroutineHandle StartCoroutine(IEnumerator enumerator)
        {
            Init();
            return JuicerController.RunCoroutine(enumerator);
        }

        public static void StopCoroutine(CoroutineHandle coroutine)
        {
            Init();
            JuicerController.StopCoroutine(coroutine);
        }

        public static CoroutineHandle RunCoroutine(this MonoBehaviour owner, IEnumerator coroutine)
        {
            return new CoroutineHandle(owner, coroutine);
        }
        #endregion

        #region Generic
        public static JuicerRuntime To(float start, Action<float> setter, float end, float duration)
        {
            Init();

            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => start);
            juicerTargetParam.Set(start, setter, end);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime To(Vector3 start, Action<Vector3> setter, Vector3 end, float duration)
        {
            Init();

            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => start);
            juicerTargetParam.Set(start, setter, end);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }
        #endregion

        #region Transform - Scale
        public static JuicerRuntime JuicyScale(this Transform transform, Vector3 to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.localScale);
            juicerTargetParam.Set(transform.localScale, (value) => transform.localScale = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyScale(this Transform transform, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.localScale.x);
            juicerTargetParam.Set(transform.localScale.x, (value) => transform.localScale = new Vector3(value, value, value), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyScaleX(this Transform transform, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.localScale.x);
            juicerTargetParam.Set(transform.localScale.x, (value) => transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyScaleY(this Transform transform, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.localScale.y);
            juicerTargetParam.Set(transform.localScale.y, (value) => transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyScaleZ(this Transform transform, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.localScale.z);
            juicerTargetParam.Set(transform.localScale.z, (value) => transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, value), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }
        #endregion

        #region Transform - World Position
        public static JuicerRuntime JuicyMove(this Transform transform, Vector3 to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.position);
            juicerTargetParam.Set(transform.position, (value) => transform.position = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyMoveX(this Transform transform, float to, float duration, JuicerRuntimeParam juicerRuntimeParam = null)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.position.x);
            juicerTargetParam.Set(transform.position.x, (value) => transform.position = new Vector3(value, transform.position.y, transform.position.z), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyMoveY(this Transform transform, float to, float duration, JuicerRuntimeParam juicerRuntimeParam = null)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.position.y);
            juicerTargetParam.Set(transform.position.y, (value) => transform.position = new Vector3(transform.position.x, value, transform.position.z), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyMoveZ(this Transform transform, float to, float duration, JuicerRuntimeParam juicerRuntimeParam = null)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.position.z);
            juicerTargetParam.Set(transform.position.z, (value) => transform.position = new Vector3(transform.position.x, transform.position.y, value), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        #endregion

        #region Transform - Local Position

        public static JuicerRuntime JuicyLocalMove(this Transform transform, Vector3 to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.position);
            juicerTargetParam.Set(transform.localPosition, (value) => transform.localPosition = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyLocalMoveX(this Transform transform, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.localPosition);
            juicerTargetParam.Set(transform.localPosition.x, (value) => transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyLocalMoveY(this Transform transform, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.localPosition.y);
            juicerTargetParam.Set(transform.localPosition.y, (value) => transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyLocalMoveZ(this Transform transform, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.localPosition.z);
            juicerTargetParam.Set(transform.localPosition.z, (value) => transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }
        #endregion

        #region Transform - World Rotation
        public static JuicerRuntime JuicyRotate(this Transform transform, Vector3 to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.eulerAngles);
            juicerTargetParam.Set(transform.eulerAngles, (value) => transform.eulerAngles = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyRotateQuaternion(this Transform transform, Quaternion to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.rotation);
            juicerTargetParam.Set(transform.rotation, (value) => transform.rotation = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }
        #endregion

        #region Transform - Local Rotation
        public static JuicerRuntime JuicyLocalRotate(this Transform transform, Vector3 to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.localEulerAngles);
            juicerTargetParam.Set(transform.localEulerAngles, (value) => transform.localEulerAngles = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyLocalRotateQuaternion(this Transform transform, Quaternion to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => transform.localRotation);
            juicerTargetParam.Set(transform.localRotation, (value) => transform.localRotation = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }
        #endregion

        #region Material
        public static JuicerRuntime JuicyColour(this Material material, Color to, float duration, JuicerRuntimeParam juicerRuntimeParam = null)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => material.color);
            juicerTargetParam.Set(material.color, (value) => material.color = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyAlpha(this Material material, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            Color color = material.color;
            juicerTargetParam.SetCurrentValue(() => material.color);
            juicerTargetParam.Set(color.a, (value) => material.color = new Color(color.r, color.g, color.b, value), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }
        #endregion

        #region UI
        public static JuicerRuntime JuicyAlpha(this CanvasGroup canvasGroup, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => canvasGroup.alpha);
            juicerTargetParam.Set(canvasGroup.alpha, (value) => canvasGroup.alpha = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyFillAmount(this Image image, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => image.fillAmount);
            juicerTargetParam.Set(image.fillAmount, (value) => image.fillAmount = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyColour(this TMP_Text textMeshPro, Color to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => textMeshPro.color);
            juicerTargetParam.Set(textMeshPro.color, (value) => textMeshPro.color = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyAlpha(this TMP_Text TextMeshPro, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            Color color = TextMeshPro.color;
            juicerTargetParam.SetCurrentValue(() => TextMeshPro.color);
            juicerTargetParam.Set(color.a, (value) => TextMeshPro.color = new Color(color.r, color.g, color.b, value), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyText(this TMP_Text textMeshPro, string to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => textMeshPro.text);
            juicerTargetParam.Set(textMeshPro.text, (value) => textMeshPro.text = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyTextNumber(this TMP_Text textMeshPro, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            float.TryParse(textMeshPro.text, out float currentAmount);
            juicerTargetParam.SetCurrentValue(() => textMeshPro.text.TextToFloat());
            juicerTargetParam.Set(currentAmount, (value) => textMeshPro.text = value.ToString("F0"), to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyScroll(this ScrollRect scrollRect, Vector3 to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => scrollRect.content.localPosition);
            juicerTargetParam.Set(scrollRect.content.localPosition, (value) => scrollRect.content.localPosition = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyValue(this Slider slider, float to, float duration)
        {
            Init();
            JuicerTargetParam juicerTargetParam = new JuicerTargetParam();
            juicerTargetParam.SetCurrentValue(() => slider.value);
            juicerTargetParam.Set(slider.value, (value) => slider.value = value, to);
            JuicerRuntime juicerRuntime = new JuicerRuntime(duration, juicerTargetParam);
            return juicerRuntime;
        }
        #endregion
    }
}