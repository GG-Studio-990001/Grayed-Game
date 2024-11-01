using Runtime.CH2.Location;
using Runtime.ETC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH2.Main
{
    public class TurnController2 : MonoBehaviour
    {
        [SerializeField] private DialogueRunner _dialogueRunner;
        [SerializeField] private LocationSelectionUI2 _locationSelectionUI;
        private List<Dictionary<string, object>> _data = new();
        private List<string> _visitedLocations = new();

        private void Awake()
        {
            _data = CSVReader.Read("CH2Branch");
            _locationSelectionUI.TurnController = this;
        }

        public void GetInitialLocation()
        {
            // CH2 시작시 최초 1회 호출
            // 현재는 Turn 0 기준, 추후 진행도에 따라 변경되도록
            Managers.Data.CH2.Turn = 0;

            // 중간부터 이어하려면 Turn뿐만 아니라 위치도 알아야함
            // 이 부분은 임시
            List<string> loc = GetAvailableLocations();
            if (loc.Count != 1)
            {
                Debug.LogError("Location is not unique.");
            }

            Managers.Data.CH2.Location = loc[0];

            _locationSelectionUI.FadeIn();
            StartDialogue();
        }

        public void AdvanceTurnAndMoveLocation(string location)
        {
            Managers.Data.CH2.Location = location;

            _locationSelectionUI.MoveLocation();
            StartDialogue();
        }

        private void StartDialogue()
        {
            string showLocations = "ShowLocations"; // 리팩터링 필요 (옮기기)
            string dialogueName;

            if (_visitedLocations.Contains(Managers.Data.CH2.Location))
            {
                Debug.Log("이미 방문함");
                dialogueName = showLocations;
            }
            else
            {
                dialogueName = GetDialogueName();
                _visitedLocations.Add(Managers.Data.CH2.Location);
            }

            if (dialogueName == "O")
                dialogueName = showLocations;

            _dialogueRunner.StartDialogue(dialogueName);
        }

        private string GetDialogueName()
        {
            // 현재 턴수와 장소에 맞는 다이얼로그 이름 가져오기

            int progress = Managers.Data.CH2.Turn;
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
                    string dialogueName = row[$"{progress}"].ToString();
                    return dialogueName;
                }
            }

            // 장소가 없거나 진행도를 찾을 수 없는 경우 null 반환
            return null;
        }

        private List<string> GetAvailableLocations()
        {
            int progress = Managers.Data.CH2.Turn;
            Debug.Log(Managers.Data.CH2.Turn + "에서 갈 수 있는 곳");
            // 이동 가능한 장소 리스트 가져오기
            List<string> loc = new();

            foreach (var row in _data)
            {
                // 첫 번째 열(장소) 데이터를 가져옴
                string location = row.ElementAt(0).Value.ToString(); // 첫 번째 열

                // 진행도 상태 가져오기 (Managers.Data.CH2.Progress에 해당하는 열)
                string progressState = row[$"{progress}"].ToString();
                // Debug.Log($"{location} {progressState}");

                // 진행도에서 'x'가 아닌 값이면 이동 가능 장소로 리스트에 추가
                // 현재 있는 위치가 아니어야 함
                if (progressState != "x" && location != Managers.Data.CH2.Location)
                {
                    loc.Add(location);
                }
            }

            return loc; // 이동 가능한 장소 리스트 반환
        }

        public void DisplayAvailableLocations()
        {
            _locationSelectionUI.SetLocationOptions(GetAvailableLocations());
        }

        public void NextProgress()
        {
            Managers.Data.CH2.Turn++;
            _visitedLocations.Clear();
        }
    }
}