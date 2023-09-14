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
        public event Action<GodsBenevolenceSO> OnPuzzleSolved;
        public event Action OnPuzzleFailed;

        [SerializeField] private int countDownTime = 60;
        [SerializeField] private TextMeshProUGUI countDownText;
        [SerializeField] private AdvanceButton closeButton;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private GameObject blocker;

        [Header("Puzzle Menu")]
        [SerializeField] private GodsBenevolenceSO[] godsBenevolenceArray;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private PuzzlePiece[] puzzlePieces;

        [Header("Debug")]
        [SerializeField] private GodsBenevolenceSO selectedBenevolence;

        private CountDownTimer countDownTimer;
        private JuicerRuntime countDownTextEffect;
        private JuicerRuntime openEffectBG;
        private JuicerRuntime closeEffectBG;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PuzzleSolved();
            }
        }

        public override void OnCreated()
        {
            selectedBenevolence = godsBenevolenceArray[UnityEngine.Random.Range(0, godsBenevolenceArray.Length)];

            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnComplected(CloseMenu);

            closeButton.onClick.AddListener(PuzzleFailed);

            countDownTimer = new CountDownTimer(this);

            countDownTextEffect = countDownText.transform.JuicyScale(1.5f, 0.15f);
            countDownTextEffect.SetEase(animationCurve);


            for (int i = 0; i < puzzlePieces.Length; i++)
            {
                puzzlePieces[i].OnPuzzlePieceClicked += OnPuzzlePieceClicked;
            }
        }

        public override void OnOpened()
        {
            closeButton.gameObject.SetActive(true);

             GodsBenevolenceSO random = null;
            do
            {
                random = godsBenevolenceArray[UnityEngine.Random.Range(0, godsBenevolenceArray.Length)];
            } while (random == selectedBenevolence);

            selectedBenevolence = random;

            for (int i = 0; i < puzzlePieces.Length; i++)
            {
                puzzlePieces[i].ShuffleRotation();
                puzzlePieces[i].SetSprite(selectedBenevolence.BenevolencePuzzle[i]);
            }

            blocker.SetActive(false);

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
            countDownTimer.Start(countDownTime, (current) =>
            {
                if ((int)current != (int)countDownTimer.GetLastTime)
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
                AudioManager.PlaySoundEffect("RotateLastTile");
                PuzzleSolved();
            }
            else
            {
                AudioManager.PlaySoundEffect("RotateTiles");
            }
        }

        public void PuzzleSolved()
        {
            closeButton.gameObject.SetActive(false);
            StartCoroutine(PuzzleSolvedRoutine());
        }

        private IEnumerator PuzzleSolvedRoutine()
        {
            countDownTimer.Stop();
            blocker.SetActive(true);
            
            yield return new WaitForSeconds(.5f);

            AudioManager.PlaySoundEffect(selectedBenevolence.CompletedSfxKey);    

            yield return new WaitForSeconds(3f);
            OnPuzzleSolved?.Invoke(selectedBenevolence);
            Close();
        }

        private void PuzzleFailed()
        {
            OnPuzzleFailed?.Invoke();
            Close();
        }
    }
}
