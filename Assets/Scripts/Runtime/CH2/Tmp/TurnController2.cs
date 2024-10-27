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

        private void Awake()
        {
            _data = CSVReader.Read("StoryBranch");
            _locationSelectionUI.TurnController = this;
        }

        public void GetInitialLocation()
        {
            // CH2 시작시 최초 1회 호출
            // 현재는 Progress 0 기준, 추후 진행도에 따라 변경되도록

            Managers.Data.CH2.Progress = 0;

            List<string> loc = GetAvailableLocations();
            if (loc.Count != 1)
            {
                Debug.LogError("Location is not unique.");
            }

            Managers.Data.CH2.Location = loc[0];

            _locationSelectionUI.FadeIn();
            InitiateDialogue();
        }

        public void AdvanceTurnAndMoveLocation(string location)
        {
            Managers.Data.CH2.Location = location;

            _locationSelectionUI.MoveLocation();
            InitiateDialogue();
        }

        private void InitiateDialogue()
        {
            _dialogueRunner.StartDialogue(GetDialogueName());
        }

        private string GetDialogueName()
        {
            int progress = Managers.Data.CH2.Progress;

            Debug.Log(Managers.Data.CH2.Progress);
            // 현재 턴수와 장소에 맞는 다이얼로그 이름 가져오기
            for (int i = 1; i < _data.Count; i++)  // 첫 번째 행(헤더)은 제외
            {
                var row = _data[i];

                // 해당 행의 첫 번째 열(장소)을 확인
                string location = row.ElementAt(0).Value.ToString();

                // 입력된 장소 이름과 일치하는지 확인
                if (location == Managers.Data.CH2.Location)
                {
                    // 진행도에 해당하는 셀 값을 가져와 반환
                    string progressState = row[$"{progress}"].ToString();
                    return progressState;
                }
            }

            // 장소가 없거나 진행도를 찾을 수 없는 경우 null 반환
            return null;
        }

        private List<string> GetAvailableLocations()
        {
            int nextProgress = Managers.Data.CH2.Progress;
            // 이동 가능한 장소 리스트 가져오기
            List<string> loc = new();

            // 첫 번째 행(header)을 제외하고 탐색
            for (int i = 1; i < _data.Count; i++)
            {
                var row = _data[i];

                // 첫 번째 열(장소) 데이터를 가져옴 (Location 열에 해당)
                string location = row.ElementAt(0).Value.ToString(); // 첫 번째 열

                // 진행도 상태 가져오기 (Managers.Data.CH2.Progress에 해당하는 열)
                string progressState = row[$"{nextProgress}"].ToString();

                // 진행도에서 x가 아닌 값이면 이동 가능 장소로 리스트에 추가
                // 현재 있는 위치도 아니어야 함
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
    }
}