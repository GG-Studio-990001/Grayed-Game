using UnityEngine;

namespace Runtime.CH3.Rokemon
{
    public class RMController : MonoBehaviour
    {
        [SerializeField] private Assigner _assigner;
        [SerializeField] private GameObject _profilePage;
        [SerializeField] private GameObject _assignPage;
        [SerializeField] private GameObject _savePanel;
        private int _selectedSkill = -1; // 0부터 3까지, 프로필은 -1
        private int _clickedSkill = -1; // 0부터 3까지, 프로필은 -1

        public void SkillBtnToggle(int idx)
        {
            if (_selectedSkill == -1 && idx != -1) // 할당창 활성화
            {
                _selectedSkill = idx;
                _profilePage.SetActive(false);
                _assignPage.SetActive(true);
                _assigner.UpdateAssignPage(_selectedSkill);
            }
            else // 할당창 활성화 중 스킬 버튼을 눌렀다면
            {
                ChangeTab(idx);
            }
        }

        private void ChangeTab(int idx) // 프로필 활성화 또는 어사인 새로고침
        {
            _clickedSkill = idx;

            if (_assigner.HasChanged())
            {
                // 변동 있으면 새로 클릭한 정보를 저장 후 저장패널 활성화
                _savePanel.SetActive(true);
            }
            else
            {
                // 변동이 없을 경우
                if (_clickedSkill == -1 || _clickedSkill == _selectedSkill) // 그냥 할당창 끄기
                {
                    _selectedSkill = -1;
                    _profilePage.SetActive(true);
                    _assignPage.SetActive(false);
                }
                else // 할당창 새로고침
                {
                    _selectedSkill = _clickedSkill;
                    _assigner.UpdateAssignPage(_clickedSkill);
                }
            }
        }

        public void CloseAssignPage()
        {
            // 변동이 있어도 무시
            _selectedSkill = -1;
            _profilePage.SetActive(true);
            _assignPage.SetActive(false);
        }

        public void CloseAndSaveAssignPage()
        {
            ChangeTab(-1);
        }

        public void SaveRpOkBtn()
        {
            _savePanel.SetActive(false);
            // 저장 코드 추가
            _assigner.SaveRp();

            if (_clickedSkill == -1 || _clickedSkill == _selectedSkill) // 할당창 끄기
            {
                _selectedSkill = -1;
                _profilePage.SetActive(true);
                _assignPage.SetActive(false);
            }
            else // 할당창 새로고침
            {
                _selectedSkill = _clickedSkill;
                _assigner.UpdateAssignPage(_clickedSkill);
            }
        }

        public void SaveRpCancelBtn()
        {
            _savePanel.SetActive(false);
        }
    }
}