using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.CH1.SubB
{
    public class ThreeMatchPuzzleController : MonoBehaviour
    {
        public UnityEvent onClear;
        [field:SerializeField] public int PuzzleNumber { get; private set; }
        [field:SerializeField] public List<Jewelry> Jewelries { get; set; }

        public bool IsClear
        {
            get { return Managers.Data.ThreeMatchPuzzleStageData[PuzzleNumber].isClear; }
            set { Managers.Data.ThreeMatchPuzzleStageData[PuzzleNumber].isClear = value; }
        }

        private ThreeMatchPuzzleLogic _logic;
        
        private void Start()
        {
            if (Jewelries.Count == 0 || Jewelries == null)
            {
                Jewelries = new List<Jewelry>(FindObjectsOfType<Jewelry>());
            }
            
            _logic = new ThreeMatchPuzzleLogic(Jewelries);
            
            foreach (var jewelry in Jewelries)
            {
                jewelry.Controller = this;
            }
            
            PuzzleReset();
        }

        public void PuzzleReset()
        {
            if (IsClear)
            {
                var puzzleData = Managers.Data.ThreeMatchPuzzleStageData[PuzzleNumber];
                
                for (int i = 0; i < Jewelries.Count; i++)
                {
                    Jewelries[i].ResetPosition(puzzleData.jewelryPositions[i]);
                }
                
                return;
            }

            // reset puzzle 이 경우는 인 게임에서 유저가 리셋할 떼
            foreach (var jewelry in Jewelries)
            {
                jewelry.ResetPosition();
            }
        }
        
        public void PuzzleClear()
        {
            if (IsClear)
            {
                return;
            }

            IsClear = true;
            onClear?.Invoke();
            
            // save puzzle data
            var puzzleData = Managers.Data.ThreeMatchPuzzleStageData[PuzzleNumber];
            puzzleData.jewelryPositions.Clear();
            
            foreach (var jewelry in Jewelries)
            {
                puzzleData.jewelryPositions.Add(jewelry.transform.position);
            }
            
            Managers.Data.SaveGame();
            
            Debug.Log("Puzzle Clear");
        }

        public bool ValidateMovement(Jewelry jewelry, Vector2 direction) => _logic.ValidateMovement(jewelry, direction);
        public void CheckMatching() => _logic.CheckMatching();
    }
}
