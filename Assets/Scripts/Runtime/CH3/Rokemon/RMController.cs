using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH3.Rokemon
{
    public class RMController : MonoBehaviour
    {
        [SerializeField] private RMDialogue _rMDialogue;
        [SerializeField] private Assigner _assigner;
        [SerializeField] private Skill[] _curSkills = new Skill[4];
        [SerializeField] private Skill[] _skills = new Skill[7];
        [SerializeField] private GameObject _profilePage;
        [SerializeField] private GameObject _assignPage;
        [SerializeField] private GameObject _savePanel;
        private bool _isRemoveMode = false;
        private int _addIdx = 0;
        private int _selectedSkill = -1; // 0부터 3까지, 프로필은 -1
        private int _clickedSkill = -1; // 0부터 3까지, 프로필은 -1

        private void Start()
        {
            SetCurSkills();

            _profilePage.SetActive(true);
            _assignPage.SetActive(false);
        }

        private void SetCurSkills()
        {
            Debug.Log("버튼 새로 세팅");
            int active = 0;

            for (int i = 0; i < _skills.Length; i++)
            {
                if (_skills[i].gameObject.activeSelf)
                {
                    _curSkills[active] = _skills[i];

                    // 버튼 컴포넌트 가져오기
                    if (_skills[i].TryGetComponent<Button>(out var btn))
                    {
                        btn.onClick.RemoveAllListeners();
                        int idx = active; // 람다 캡처 방지
                        btn.onClick.AddListener(() => SkillBtnClked(idx));
                    }

                    active++;
                }
            }
        }

        public string GetSkillName(int idx)
        {
            return _skills[idx].SkillName;
        }

        #region 스킬창 클릭
        public void SkillBtnClked(int idx)
        {
            if (_isRemoveMode)
            {
                RemoveSkill(idx);
                return;
            }
            
            SkillBtnToggle(idx);
        }

        private void RemoveSkill(int idx)
        {
            // 제거
            _curSkills[idx].gameObject.SetActive(false);

            // 추가
            _skills[_addIdx].gameObject.SetActive(true);

            // 초기화
            _rMDialogue.StartNextDialogue(_curSkills[idx].SkillName);

            // 세팅
            SetCurSkills();
        }

        public void SetRemoveInfo(int idx)
        {
            _isRemoveMode = true;
            _addIdx = idx;
        }

        public void ResetRemoveInfo()
        {
            _isRemoveMode = false;
            _addIdx = 0;
        }

        private void SkillBtnToggle(int idx)
        {
            if (_selectedSkill == -1 && idx != -1) // 할당창 활성화
            {
                _selectedSkill = idx;
                _curSkills[_selectedSkill].SkillSelected(true);
                _profilePage.SetActive(false);
                _assignPage.SetActive(true);
                _assigner.UpdateAssignPage(_curSkills[_selectedSkill]);
            }
            else // 할당창 활성화 중 스킬 버튼을 눌렀다면
            {
                ChangeTab(idx);
            }
        }

        private void ChangeTab(int idx) // 프로필창 활성화 또는 할당창 새로고침
        {
            _clickedSkill = idx;

            if (_assigner.HasChanged())
            {
                // 변동 있으면 새로 클릭한 정보를 저장 후 저장패널 활성화
                _savePanel.SetActive(true);
            }
            else
            {
                UpdateAssignPage();
            }
        }

        private void UpdateAssignPage()
        {
            if (_clickedSkill == -1 || _clickedSkill == _selectedSkill) // 할당창 끄기
            {
                _curSkills[_selectedSkill].SkillSelected(false);
                _selectedSkill = -1;
                _profilePage.SetActive(true);
                _assignPage.SetActive(false);
            }
            else // 할당창 새로고침
            {
                _curSkills[_selectedSkill].SkillSelected(false);
                _selectedSkill = _clickedSkill;
                _curSkills[_selectedSkill].SkillSelected(true);
                _assigner.UpdateAssignPage(_curSkills[_selectedSkill]);
            }
        }
        #endregion

        #region 할당 페이지의 닫기/확인 버튼
        public void CloseAssignPage()
        {
            // 변동이 있어도 무시
            _assigner.RevertLeftLv();
            _curSkills[_selectedSkill].SkillSelected(false);
            _selectedSkill = -1;
            _profilePage.SetActive(true);
            _assignPage.SetActive(false);
        }

        public void CloseAndSaveAssignPage()
        {
            ChangeTab(-1);
        }
        #endregion

        #region 저장패널의 취소/확인 버튼
        public void SaveRpOkBtn()
        {
            _savePanel.SetActive(false);
            _assigner.SaveLv();

            UpdateAssignPage();
        }

        public void SaveRpCancelBtn()
        {
            _savePanel.SetActive(false);
        }
        #endregion
    }
}