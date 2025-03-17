using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH3.Rokemon
{
    public class RMDialogue : DialogueViewBase
    {
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [Header("=Else=")]
        //[SerializeField] private LineView _lineView;
        [SerializeField] private RMController _rMController;
        [SerializeField] private GameObject _profileBlock;

        private void Awake()
        {
            _runner.AddCommandHandler<int>("ChooseSkillToForget", ChooseSkillToForget);
        }

        private void ChooseSkillToForget(int idx)
        {
            _rMController.SetRemoveInfo(idx);
            _profileBlock.SetActive(true);
        }

        public void StartNextDialogue(int delete, int add)
        {
            _runner.VariableStorage.SetValue("$DeletedSkill", delete);
            Debug.Log($"DeletedSkill을 {delete}로 설정");

            switch (add)
            {
                case 4:
                    _runner.StartDialogue("Charm2");
                    break;
                case 5:
                    // _runner.StartDialogue("Charm2");
                    break;
                case 6:
                    // _runner.StartDialogue("Charm2");
                    break;
                default:
                    Debug.LogError($"{add} => Invalid Idx");
                    break;
            }

            _profileBlock.SetActive(false);
            _rMController.ResetRemoveInfo();
        }
    }
}