
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pelumi.Juicer
{
    public abstract class JuicerRuntimeBase
    {

    }

    public class JuicerDelay : JuicerRuntimeBase, IHasDuration
    {
        private float _delay;

        public JuicerDelay(float delay)
        {
            _delay = delay;
        }

        public float Duration => _delay;
    }

    public class JuicerCallBack : JuicerRuntimeBase
    {
        private JuicerEventHandler _callBack;

        public JuicerCallBack(JuicerEventHandler callBack)
        {
            _callBack = callBack;
        }

        public void Invoke()
        {
            _callBack?.Invoke();
        }
    }

    public class JuicerRuntime : JuicerRuntimeBase, IHasDuration
    {
        private JuicerRuntimeParam _runtimeParam = new JuicerRuntimeParam();
        private JuicerRuntimeController _juicerRuntimeController;
        private CoroutineHandle _coroutine;
        private JuicerTargetParam _juicerTargetParam;

        public float Duration => _juicerRuntimeController.Duration;

        public JuicerRuntime (float duration, JuicerTargetParam juicerTargetParam)
        {
            _juicerRuntimeController = new JuicerRuntimeController(duration);
            _juicerTargetParam = juicerTargetParam;
        }

        public JuicerRuntime Start(Action preStart = null)
        {
            if (_coroutine != null && !_coroutine.IsDone)
            {
                Resume();
            }
            else
            {
                preStart?.Invoke();
                _juicerTargetParam?.ResetStartValue();
                _coroutine = Juicer.StartCoroutine(JuicerCore.Do(_juicerTargetParam.StartingValue, _juicerTargetParam.ValueModifyEvent, _juicerTargetParam.DestinationValue, _runtimeParam, _juicerRuntimeController));
            }    
            return this;
        }

        public JuicerRuntime ChangeDuration(float newDuration)
        {
            _juicerRuntimeController.SetDuration(newDuration);
            return this;
        }

        public JuicerRuntime ChangeDesination<T>(T destination)
        {
            _juicerTargetParam.SetDestination(destination);
            return this;
        }

        public JuicerRuntime SetDelay(float delay)
        {
            _juicerRuntimeController.SetStartDelay(delay);
            return this;
        }

        public JuicerRuntime SetOnStart(JuicerEventHandler onStart)
        {
            _juicerRuntimeController.JuicerEvent.SetOnStart(onStart);
            return this;
        }

        public JuicerRuntime SetOnTick(JuicerEventHandler onTick)
        {
            _juicerRuntimeController.JuicerEvent.SetOnUpdate(onTick);
            return this;
        }

        public JuicerRuntime SetOnComplected(JuicerEventHandler onComplected)
        {
            _juicerRuntimeController.JuicerEvent.SetOnComplected(onComplected);
            return this;
        }

        public JuicerRuntime SetOnStepComplete(JuicerEventHandler onStepComplete)
        {
            _juicerRuntimeController.JuicerEvent.SetOnStepComplete(onStepComplete);
            return this;
        }

        public JuicerRuntime AddTimeEvent(float time, JuicerEventHandler action)
        {
            _juicerRuntimeController.JuicerEvent.AddTimeEvent(time, action);
            return this;
        }

        public JuicerRuntime SetLoop(int loopCount, LoopType loopType = LoopType.Yoyo)
        {
            _runtimeParam.LoopCount = loopCount;
            _runtimeParam.GetLoopType = loopType;
            return this;
        }

        public JuicerRuntime SetTimeMode(TimeMode timeMode)
        {
            _runtimeParam.TimeMode = timeMode;
            return this;
        }

        public JuicerRuntime SetEase(Ease curveType)
        {
            _runtimeParam.EaseType = curveType;
            return this;
        }

        public JuicerRuntime SetTextAnimationMode(TextAnimationMode textAnimationMode)
        {
            _runtimeParam.TextAnimationMode = textAnimationMode;
            return this;
        }

        public JuicerRuntime SetEase(AnimationCurve customCurve)
        {
            _runtimeParam.CustomEase = customCurve;
            return this;
        }

        public bool IsFinished()
        {
            return _coroutine.IsDone;
        }

        public bool IsComplectedRound()
        {
            return _juicerRuntimeController.IsCompletedStep;
        }

        public void Pause()
        {
            _juicerRuntimeController.Pause();
        }

        public void Resume()
        {
            _juicerRuntimeController.Resume();
        }

        public void Stop()
        {
            if (_coroutine != null)
            {
                Debug.Log("Stop Juicer");
                Juicer.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
    }

    public class JuicerRuntimeParam
    {
        private LoopType _loopType = LoopType.Restart;
        private TimeMode _timeMode = TimeMode.Scaled;
        private Ease _ease = Ease.Linear;
        private TextAnimationMode textAnimationMode;
        private AnimationCurve _customEase;
        private EasingFunction.Function easeFunction = null;
        private int _loopCount = 0;

        public LoopType GetLoopType { get => _loopType; set => _loopType = value;}
        public TimeMode TimeMode { get => _timeMode; set => _timeMode = value;}
        public Ease EaseType { get => _ease; set => _ease = value;}
        public TextAnimationMode TextAnimationMode { get => textAnimationMode; set => textAnimationMode = value;}
        public AnimationCurve CustomEase { get => _customEase; set => _customEase = value;}
        public int LoopCount { get => _loopCount; set => _loopCount = value;}

        public EasingFunction.Function EaseFunction
        {
            get
            {
                if (easeFunction == null)
                {
                    easeFunction = EasingFunction.GetEasingFunction(_ease);
                }
                return easeFunction;
            }
        }
    }

    public class JuicerRuntimeController
    {
        private float _startDelay;
        protected float _duration;
        private bool _isPaused;
        private bool _isCompletedStep;
        private JuicerRuntimeEvent _juicerEvent = new JuicerRuntimeEvent();

        public JuicerRuntimeEvent JuicerEvent => _juicerEvent;
        public float StartDelay => _startDelay;
        public float Duration => _duration;
        public bool IsPaused => _isPaused;
        public bool IsCompletedStep => _isCompletedStep;

        public JuicerRuntimeController (float duration)
        {
            _duration = duration;
        }

        public void SetStartDelay(float startDelay)
        {
            _startDelay = startDelay;
        }

        public void SetDuration(float duration)
        {
            _duration = duration;
        }

        public void SetIsPaused(bool isPaused)
        {
            _isPaused = isPaused;
        }

        public void Process(float timeline)
        {
            _juicerEvent.InvokeOnUpdate();
            _juicerEvent.ProcessTimelineEvents(timeline);
        }

        public void OnStart()
        {
            _juicerEvent.InvokeOnStart();
        }

        public void OnCompleted()
        {
            _juicerEvent.InvokeOnComplected();
            _juicerEvent.ResetTimelineEvents();
        }

        public void OnCompleteStep()
        {
            _isCompletedStep = true;
            _juicerEvent.InvokeOnStepComplete();
            _juicerEvent.ResetTimelineEvents();
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }
    }

    public class JuicerRuntimeEvent
    {
        private List<JuicerTimelineEvent> timelineEvents = new List<JuicerTimelineEvent>();
        private JuicerEventHandler OnStart;
        private JuicerEventHandler OnTick;
        private JuicerEventHandler OnComplected;
        private JuicerEventHandler OnStepComplete;

        public void InvokeOnStart()
        {
            OnStart?.Invoke();
        }

        public void InvokeOnUpdate()
        {
            OnTick?.Invoke();
        }

        public void InvokeOnComplected()
        {
            OnComplected?.Invoke();
        }

        public void InvokeOnStepComplete()
        {
            OnStepComplete?.Invoke();
        }

        public void SetOnStart(JuicerEventHandler onStart)
        {
            OnStart = onStart;
        }

        public void SetOnUpdate(JuicerEventHandler onTick)
        {
            OnTick = onTick;
        }

        public void SetOnComplected(JuicerEventHandler onFinished)
        {
            OnComplected = onFinished;
        }

        public void SetOnStepComplete(JuicerEventHandler onStepComplete)
        {
            OnStepComplete = onStepComplete;
        }

        public void AddTimeEvent(float time, JuicerEventHandler action)
        {
            timelineEvents.Add(new JuicerTimelineEvent(time, action));
        }

        public void ProcessTimelineEvents(float timeline)
        {
            if (timelineEvents == null) return;
            for (int i = 0; i < timelineEvents.Count; i++)
            {
                if (!timelineEvents[i].IsTriggered && timeline >= timelineEvents[i].Time)
                {
                    timelineEvents[i].Invoke();
                }
            }
        }

        public void ResetTimelineEvents()
        {
            if (timelineEvents == null) return;
            for (int i = 0; i < timelineEvents.Count; i++)
            {
                timelineEvents[i].Reset();
            }
        }
    }

    public class JuicerTimelineEvent
    {
        private JuicerEventHandler _action;
        private float _time;
        private bool _isTriggered;

        public JuicerEventHandler Action => _action;
        public float Time => _time;
        public bool IsTriggered => _isTriggered;

        public JuicerTimelineEvent(float time, JuicerEventHandler action)
        {
            _time = time;
            _action = action;
            _isTriggered = false;
        }

        public void Invoke()
        {
            _action.Invoke();
            _isTriggered = true;
        }

        public void Reset()
        {
            _isTriggered = false;
        }
    }

    public class JuicerSequencer
    {
        private List<JuicerRuntimeBase> juicerRuntimeBases = new List<JuicerRuntimeBase>();
        private CoroutineHandle _coroutine;
        private JuicerRuntime _currentJuicerRuntime;
        private Action _OnStarted;
        private Action _OnComplected;
        private bool _isPaused;
        private float _delay;

        public bool IsPaused => _isPaused;

        public void AppendCallback(JuicerEventHandler action)
        {
            juicerRuntimeBases.Add(new JuicerCallBack(action));
        }

        public void Append(JuicerRuntime juicerRuntime)
        {
            juicerRuntimeBases.Add(juicerRuntime);
        }

        public void Delay(float delay)
        {
            juicerRuntimeBases.Add(new JuicerDelay(delay));
        }

        public void Run()
        {
            DisableInfiniteLoop();
            _coroutine = Juicer.StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            _OnStarted?.Invoke();
            foreach (JuicerRuntimeBase juicerRuntimeBase in juicerRuntimeBases)
            {
                switch (juicerRuntimeBase)
                {
                    case JuicerRuntime juicerRuntime:

                        _currentJuicerRuntime = juicerRuntime;
                        juicerRuntime.Start();
                        yield return new WaitUntilJuicerComplectedOrStep(juicerRuntime);

                        break;

                    case JuicerDelay juicerDelay:

                        _currentJuicerRuntime = null;
                        _delay = juicerDelay.Duration;
                        while (_delay > 0)
                        {
                            if (!_isPaused)
                            {
                                _delay -= Time.deltaTime;
                            }

                            yield return null;
                        }
                        break;

                    case JuicerCallBack juicerCallBack:

                        _currentJuicerRuntime = null;
                        juicerCallBack.Invoke();

                        break;

                    default: break;
                }
            }
            _OnComplected?.Invoke();
        }

        public void Pause()
        {
            _isPaused = true;
            _currentJuicerRuntime?.Pause();
        }

        public void Resume()
        {

            _currentJuicerRuntime?.Resume();
        }

        public void SetOnStarted(Action onStarted)
        {
            _OnStarted = onStarted;
        }

        public void SetOnComplected(Action onComplected)
        {
            _OnComplected = onComplected;
        }

        public float Duration()
        {
            float duration = 0;

            foreach (JuicerRuntimeBase juicerRuntimeBase in juicerRuntimeBases)
            {
                if (juicerRuntimeBase is IHasDuration hasDuration)
                    duration += hasDuration.Duration;
            }
            return duration;
        }

        public void Stop()
        {
            Juicer.StopCoroutine(_coroutine);
            _currentJuicerRuntime.Stop();
        }

        private void DisableInfiniteLoop()
        {
            foreach (JuicerRuntimeBase juicerRuntimeBase in juicerRuntimeBases)
            {
                if (juicerRuntimeBase is JuicerRuntime validJuicerRuntime)
                    validJuicerRuntime.SetLoop(0);
            }
        }
    }

    public class JuicerTargetParam
    {
        private object _startingValue;
        private object _destinationValue;
        private Action<object> _valueModifyEvent;
        private Func<object> _setStartValue;

        public object StartingValue => _startingValue;
        public Action<object> ValueModifyEvent => _valueModifyEvent;
        public object DestinationValue => _destinationValue;

        public void Set<T>(T _startingValue, Action<T> _valueModifyEvent, T _destinationValue)
        {
            this._startingValue = _startingValue;
            this._valueModifyEvent = (object value) => _valueModifyEvent((T)value);
            this._destinationValue = _destinationValue;
        }

        public void SetDestination<T>(T _destinationValue)
        {
            this._destinationValue = _destinationValue;
        }

        public void SetCurrentValue(Func<object> getStartingValueFunc)
        {
            _setStartValue = getStartingValueFunc;
        }

        public void ResetStartValue()
        {
            _startingValue = _setStartValue?.Invoke();
        }
    }

    public delegate void JuicerEventHandler();

    public interface IHasDuration
    {
        float Duration { get; }
    }
}