using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using Plugins.Audio_System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class GodsBenevolencePuzzleMenu : BaseMenu<GodsBenevolencePuzzleMenu>
    {
        public Func< GodBenevolenceType, GodsBenevolenceVisualData> GetRandomBenevolenceVisualSO;
        public event Action<GodBenevolenceType> OnPuzzleSolved;
        public event Action OnPuzzleFailed;

        [SerializeField] private int countDownTime = 60;
        [SerializeField] private TextMeshProUGUI countDownText;
        [SerializeField] private AdvanceButton closeButton;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private GameObject blocker;
        [SerializeField] private Image timerFill;

        [Header("Puzzle Menu")]
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private PuzzlePiece[] puzzlePieces;

        [Header("Debug")]
        [SerializeField] private GodBenevolenceType selectedBenevolence;
        [SerializeField] private GodsBenevolenceVisualData selectedBenevolenceVisual;

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
            selectedBenevolence = GodBenevolenceType.Ares;

            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

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

            if (GetRandomBenevolenceVisualSO == null)
            {
                Debug.LogError("GetRandomBenevolenceVisualSO is null");
                return; 
            }

            selectedBenevolenceVisual = GetRandomBenevolenceVisualSO(selectedBenevolence);
            selectedBenevolence = selectedBenevolenceVisual.BenevolenceType;

            for (int i = 0; i < puzzlePieces.Length; i++)
            {
                puzzlePieces[i].ShuffleRotation();
                puzzlePieces[i].SetSprite(selectedBenevolenceVisual.BenevolencePuzzle[i]);
            }

            blocker.SetActive(false);

            openEffectBG.Start();
            openEffectBG.SetOnCompleted(() =>
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
                timerFill.fillAmount = current / countDownTime;

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
                AudioManager.PlaySoundEffect("RotateLastTile",SoundEffectCategory.UI);
                PuzzleSolved();
            }
            else
            {
                AudioManager.PlaySoundEffect("RotateTiles",SoundEffectCategory.UI);
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

            AudioManager.PlaySoundEffect(selectedBenevolenceVisual.CompletedSfxKey,SoundEffectCategory.UI);    

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
