using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class PuzzleMenu : BaseMenu<PuzzleMenu>
    {
        public event Action OnPuzzleSolved;

        [SerializeField] private AdvanceButton closeButton;

        [Header("Puzzle Menu")]
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private PuzzlePiece[] puzzlePieces;
        [SerializeField] private PuzzleSO puzzleSO;



        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnComplected(CloseMenu);


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
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

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
    }
}
