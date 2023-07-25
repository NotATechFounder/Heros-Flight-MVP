using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class PuzzleMenu : BaseMenu<PuzzleMenu>
    {
        public event Action OnPuzzleSolved;
        public event Action OnPuzzleFailed;

        [SerializeField] private int countDownTime = 60;
        [SerializeField] private TextMeshProUGUI countDownText;
        [SerializeField] private AdvanceButton closeButton;
        [SerializeField] private AnimationCurve animationCurve;

        [Header("Puzzle Menu")]
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private PuzzlePiece[] puzzlePieces;
        [SerializeField] private PuzzleSO puzzleSO;

        private CountDownTimer startTimer;
        private JuicerRuntime countDownTextEffect;
        private JuicerRuntime openEffectBG;
        private JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnComplected(CloseMenu);

            closeButton.onClick.AddListener(PuzzleFailed);

            startTimer = new CountDownTimer(this);

            countDownTextEffect = countDownText.transform.JuicyScale(1.5f, 0.15f);
            countDownTextEffect.SetEase(animationCurve);
        }

        public override void OnOpened()
        {
            for (int i = 0; i < puzzlePieces.Length; i++)
            {
                puzzlePieces[i].OnPuzzlePieceClicked += OnPuzzlePieceClicked;
                puzzlePieces[i].ShuffleRotation();
                puzzlePieces[i].SetSprite(puzzleSO.Sprites[i]);
            }

            openEffectBG.Start();
            openEffectBG.SetOnComplected(() =>
            {
                StartTimer();
            });
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }

        private void StartTimer()
        {
            startTimer.Start(countDownTime, (current) =>
            {
                if ((int)current != (int)startTimer.GetLastTime)
                {
                    countDownTextEffect.Start();
                    countDownText.text = Mathf.CeilToInt(current).ToString();
                }
            }, () =>
            {
                PuzzleFailed();
            });
        }

        private void OnPuzzlePieceClicked(PuzzlePiece puzzlePiece)
        {
            bool isCorrectRotation = true;
            for (int i = 0; i < puzzlePieces.Length; i++)
            {
                if (!puzzlePieces[i].IsCorrectRotation)
                {
                    isCorrectRotation = false;
                    break;
                }
            }
            if (isCorrectRotation)
            {
                OnPuzzleSolved?.Invoke();
                Close();
            }
        }

        private void PuzzleSolved()
        {
            OnPuzzleSolved?.Invoke();
            Close();
        }

        private void PuzzleFailed()
        {
            OnPuzzleFailed?.Invoke();
            Close();
        }
    }
}
