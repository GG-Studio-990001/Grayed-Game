using Runtime.CH2.Location;
using Runtime.ETC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH2.Main
{
    public class TurnController : MonoBehaviour
    {
        [SerializeField] private DialogueRunner _dialogueRunner;
        [SerializeField] private LocationUIController _locationUiController;
        private List<Dictionary<string, object>> _data = new();
        private List<string> _visitedLocations = new();
        private const string _nobody = "Nobody"; // Nobody를 상수로 이동

        private void Awake()
        {
            _data = CSVReader.Read("CH2Branch");
            _locationUiController.TurnController = this;
        }

        public void GetInitialLocation()
        {
            int turn = Managers.Data.CH2.Turn;
            string dialogueKey = (turn == 1 || turn == 2 || turn == 4 || turn == 6) ? $"Turn{turn}_S" : $"Turn{turn}";

            _dialogueRunner.StartDialogue(dialogueKey);
        }

        public void SetLocation(string loc)
        {
            Managers.Data.CH2.Location = loc;
        }

        public void StartDialogue()
        {
            string dialogueName = _nobody;

            if (_visitedLocations.Contains(Managers.Data.CH2.Location))
            {
                Debug.Log("이미 방문함");
                _locationUiController.MoveLocation();
                _dialogueRunner.StartDialogue(dialogueName);
                return;
            }

            dialogueName = GetDialogueName();
            _visitedLocations.Add(Managers.Data.CH2.Location);

            if (string.IsNullOrEmpty(dialogueName))
            {
                Debug.Log("출력할 다이얼로그 없음");
                _locationUiController.MoveLocation();
                dialogueName = _nobody;
            }

            _dialogueRunner.StartDialogue(dialogueName);
        }

        private string GetDialogueName()
        {
            // 현재 턴수와 장소에 맞는 다이얼로그 이름 가져오기
            int turn = Managers.Data.CH2.Turn;
            Debug.Log(Managers.Data.CH2.Turn + " 시작");

            for (int i = 0; i < _data.Count; i++)
            {
                var row = _data[i];

                // 해당 행의 첫 번째 열(장소)을 확인
                string location = row.ElementAt(0).Value.ToString();

                // 입력된 장소 이름과 일치하는지 확인
                if (location == Managers.Data.CH2.Location)
                {
                    // 진행도에 해당하는 셀 값을 가져와 반환
                    string dialogueName = row[$"{turn}"].ToString();
                    return dialogueName;
                }
            }

            // 장소가 없거나 진행도를 찾을 수 없는 경우 null 반환
            return null;
        }

        private List<string> GetAvailableLocations()
        {
            // 기본적으로 4개의 위치이며, 여기서 현재 위치를 제거하고 Turn이 4보다 작다면 '달러 동상'도 제거
            List<string> loc = new()
            {
                "Entrance",
                "Square",
                "Temple",
                "Statue"
            };

            for (int i = loc.Count - 1; i >= 0; i--)
            {
                if (Managers.Data.CH2.Turn < 4 && i == 3)
                {
                    loc.RemoveAt(3);
                }
                else if (loc[i] == Managers.Data.CH2.Location)
                {
                    loc.RemoveAt(i);
                    break;
                }
            }
            
            return loc; // 이동 가능한 장소 리스트 반환
        }

        public void DisplayAvailableLocations()
        {
            _locationUiController.SetLocationOptions(GetAvailableLocations());
        }

        public void NextTurn()
        {
            Managers.Data.SaveGame();
            _visitedLocations.Clear();
            Managers.Data.CH2.Turn++;
            Debug.Log("CH2 Turn: " + Managers.Data.CH2.Turn);
        }
    }
}