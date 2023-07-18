using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using System.Text;

namespace Pelumi.Juicer
{
    public enum AnimationCurveType
    {
        Linear,
        EaseInOut,
        Constant
    }

    public enum TimeMode
    {
        Unscaled,
        Scaled
    }

    public enum LoopType
    {
        Restart,
        Yoyo,
        Incremental
    }

    public enum TextAnimationMode
    {
        ClearOldText,
        AppendNewText,
        ReplaceText
    }

    public static class JuicerCore
    {
        public static IEnumerator Do<T>(T startingValue, Action<T> valueToModify, T destination, JuicerRuntimeParam juicerRuntimeParam, JuicerRuntimeController juicerRuntimeController)
        {
            float currentDelay = juicerRuntimeController.StartDelay;

            while (currentDelay > 0.0f)
            {
                if (!juicerRuntimeController.IsPaused) currentDelay -= juicerRuntimeParam.TimeMode == TimeMode.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }

            juicerRuntimeController.OnStart();

            float i = 0.0f;
            float rate = 1.0f / juicerRuntimeController.Duration;
            bool forward = true; // Flag to indicate the animation direction
            int loopCount = 0;

            T previousDestination = destination;

            while (true)
            {
                if (!juicerRuntimeController.IsPaused)
                {
                    float timeScale = juicerRuntimeParam.TimeMode == TimeMode.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                    i += forward ? timeScale * rate : -timeScale * rate; // Increment or decrement based on direction

                    valueToModify.Invoke(GetValue(startingValue, destination, juicerRuntimeParam, i));

                    juicerRuntimeController.Process(i);

                    if ((forward && i >= 1.0f) || (!forward && i <= 0.0f))
                    {
                        if(juicerRuntimeParam.LoopCount == 0 || (juicerRuntimeParam.LoopCount > 0 && loopCount >= juicerRuntimeParam.LoopCount))
                        {
                            juicerRuntimeController.OnCompleted();
                            yield break;
                        }
                        else
                        {
                            loopCount++;
                            juicerRuntimeController.OnCompleteStep();

                            switch (juicerRuntimeParam.GetLoopType)
                            {
                                case LoopType.Restart:

                                    i = 0.0f;
                                    forward = true;

                                    break;

                                case LoopType.Yoyo:

                                    forward = !forward;
                                    break;

                                case LoopType.Incremental:

                                    previousDestination = destination;
                                    destination = AddValue( destination , SubtractValue(destination, startingValue));
                                    startingValue = previousDestination;
                                    i = 0.0f;

                                    break;
                                default: break;
                            }
                        }
                    }
                }
                yield return null;
            }
        }

        public static T GetValue<T>(T startingValue, T destination, JuicerRuntimeParam juicerRuntimeParam, float i)
        {
            switch (startingValue)
            {
                case float floatValue:
                    return (T)(object)(Mathf.Lerp(floatValue, (float)(object)destination, GetEaseValue(juicerRuntimeParam, i)));

                case Vector3 vectorValue:
                    return (T)(object)Vector3.Lerp(vectorValue, (Vector3)(object)destination, GetEaseValue(juicerRuntimeParam, i));

                case Quaternion quaternionValue:
                    return (T)(object)Quaternion.Lerp(quaternionValue, (Quaternion)(object)destination, GetEaseValue(juicerRuntimeParam, i));

                case Color colorValue:
                    return (T)(object)Color.Lerp(colorValue, (Color)(object)destination, GetEaseValue(juicerRuntimeParam, i));

                case Vector4 vectorValue:
                    return (T)(object)Vector4.Lerp(vectorValue, (Vector4)(object)destination, GetEaseValue(juicerRuntimeParam, i));

                case string stringValue:
                        return (T)(object)StringLerp(stringValue, (string)(object)destination, juicerRuntimeParam, i);

                default:
                    Debug.LogError("Type not supported");
                    return default(T);
            }
        }

        public static T ConvertToType<T>( T value)
        {
            switch (value)
            {
                case float floatValue:
                    return (T)(object)(float)(object)0;

                case Vector3 vectorValue:
                    return (T)(object)(Vector3)(object)Vector3.zero;

                case Quaternion quaternionValue:
                    return (T)(object)(Quaternion)(object)Quaternion.identity;

                case Color colorValue:
                    return (T)(object)(Color)(object)value;

                case Vector4 vectorValue:
                    return (T)(object)(Vector4)(object)Vector4.zero ;

                case string stringValue:
                    return (T)(object)(string)(object)"";

                default:
                    Debug.LogError("Type not supported");
                    return default(T);
            }
        }

        public static T AddValue<T>(T first, T second)
        {
            switch (first)
            {
                case float floatValue:
                    return (T)(object)(floatValue + (float)(object)second);

                case Vector3 vectorValue:
                    //object secondVectorValue = (object)second;
                    //if (secondVectorValue == null)
                    //{
                    //    secondVectorValue = Vector3.zero;
                    //}

                    return (T)(object)(vectorValue + (Vector3)(object)second);

                case Quaternion quaternionValue:
                    return (T)(object)AddQuaternion(quaternionValue, (Quaternion)(object)second);

                case Color colorValue:
                    return (T)(object)(colorValue + (Color)(object)second);

                case Vector4 vectorValue:
                    return (T)(object)(vectorValue + (Vector4)(object)second);

                case string stringValue:
                    return (T)(object)(stringValue);

                default:
                    Debug.LogError("Type not supported :" + typeof(T));
                    return default(T);
            }
        }

        public static T SubtractValue<T>(T first, T second)
        {
            switch (first)
            {
                case float floatValue:
                    return (T)(object)(floatValue - (float)(object)second);

                case Vector3 vectorValue:
                    return (T)(object)(vectorValue - (Vector3)(object)second);

                case Quaternion quaternionValue:
                    return (T)(object)SubtractQuaternion(quaternionValue , (Quaternion)(object)second);

                case Color colorValue:
                    return (T)(object)(colorValue - (Color)(object)second);

                case Vector4 vectorValue:
                    return (T)(object)(vectorValue - (Vector4)(object)second);

                case string stringValue:
                    return (T)(object)(stringValue);

                default:
                    Debug.LogError("Type not supported");
                    return default(T);
            }
        }

        private static Quaternion SubtractQuaternion(Quaternion a, Quaternion b)
        {
            Quaternion difference = Quaternion.Inverse(a) * b; // Perform quaternion subtraction
            return difference;
        }

        private static Quaternion AddQuaternion(Quaternion a, Quaternion b)
        {
            Quaternion sum = a * b;
            return sum;
        }

        public static string StringLerp(string from, string to, JuicerRuntimeParam juicerRuntimeParam, float i)
        {
            int removeCount = Mathf.FloorToInt(Mathf.Lerp(0, from.Length, GetEaseValue(juicerRuntimeParam, i))); // Number of characters to remove
            int addCount = Mathf.FloorToInt(Mathf.Lerp(0, to.Length, GetEaseValue(juicerRuntimeParam, i))); // Number of characters to add

            string trimmedFrom = from.Substring(0, from.Length - removeCount); // Remove characters from the end of 'from'
            string trimmedTo = to.Substring(0, addCount); // Take characters from the start of 'to'

            switch (juicerRuntimeParam.TextAnimationMode)
            {
                case TextAnimationMode.ClearOldText:
                    return trimmedFrom + trimmedTo;

                case TextAnimationMode.AppendNewText:
                    return from + trimmedTo;
    
                case TextAnimationMode.ReplaceText:
                    int remainingCount = Mathf.Max(0, Mathf.Min(addCount, to.Length)); // Number of characters to add from 'to'
                    string addedText = to.Substring(0, remainingCount);
                    return addedText;
            }
            return "";
        }
      
        public static bool HasFinished(float duration, bool forward)
        {
            if ((forward && duration >= 1.0f) || (!forward && duration <= 0.0f))
            {
                return true;
            }
            return false;
        }

        public static float GetEaseValue(JuicerRuntimeParam juicerRuntimeParam, float time)
        {
            if (juicerRuntimeParam.CustomEase != null)
            {
                return juicerRuntimeParam.CustomEase.Evaluate(time);
            }
            else
            {
                return juicerRuntimeParam.EaseFunction(0, 1, time);
            }
        }

        public static T ConvertToTarget<T>(this object _target)
        {
            if (_target is T convertedTarget)
            {
                Debug.Log(convertedTarget.ToString());
                return convertedTarget;
            }

            // Handle other conversion cases if needed
            throw new InvalidCastException($"Cannot convert {_target.GetType().Name} to {typeof(T).Name}");
        }

        public static float TextToFloat(this string text)
        {
            if (float.TryParse(text, out float result))
            {
                return result;
            }
            else
            {
                Debug.LogError($"Cannot convert {text} to float");
                return 0;
            }
        }

        public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
        {
            instance.StopMovement();
            Canvas.ForceUpdateCanvases();
            Vector2 viewportLocalPosition = instance.viewport.localPosition;
            Vector2 childLocalPosition = child.localPosition;
            Vector2 result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0
            );
            return result;
        }
    }

    public class CoroutineHandle : IEnumerator
    {
        public event Action<CoroutineHandle> OnCompleted;
        public bool IsDone { get; private set; }
        public object Current {get;}
        public bool MoveNext() => !IsDone;
        public void Reset() { }

        public CoroutineHandle(MonoBehaviour owner, IEnumerator coroutine)
        {
            owner.StartCoroutine(Wrap(coroutine));
        }

        private IEnumerator Wrap(IEnumerator coroutine)
        {
            yield return coroutine;
            IsDone = true;
            OnCompleted?.Invoke(this);
        }
    }

    public class WaitUntilJuicerComplected : CustomYieldInstruction
    {
        private JuicerRuntime _juicerRuntime;
        public override bool keepWaiting => !_juicerRuntime.IsFinished();

        public WaitUntilJuicerComplected(JuicerRuntime juicerRuntime)
        {
            _juicerRuntime = juicerRuntime;
        }
    }

    public class WaitUntilJuicerComplectedOrStep : CustomYieldInstruction
    {
        private JuicerRuntime _juicerRuntime;
        public override bool keepWaiting => !_juicerRuntime.IsFinished() && !_juicerRuntime.IsComplectedRound();

        public WaitUntilJuicerComplectedOrStep(JuicerRuntime juicerRuntime)
        {
            _juicerRuntime = juicerRuntime;
        }
    }
}