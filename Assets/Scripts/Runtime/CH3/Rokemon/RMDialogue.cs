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

        public void StartNextDialogue(string oldSkill, string newSkill)
        {
            _runner.VariableStorage.SetValue("$DeletedSkill", oldSkill);
            _runner.VariableStorage.SetValue("$NewSkill", newSkill);
            
            _runner.StartDialogue("GetNewSkill");

            _profileBlock.SetActive(false);
            _rMController.ResetRemoveInfo();
        }
    }
}