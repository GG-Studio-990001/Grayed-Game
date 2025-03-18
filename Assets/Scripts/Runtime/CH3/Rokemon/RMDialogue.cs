using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH3.Rokemon
{
    public class RMDialogue : DialogueViewBase
    {
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [Header("=Else=")]
        [SerializeField] private RMController _rMController;
        [SerializeField] private GameObject _profileBlock;
        private int _newSkill = -1;

        private void Awake()
        {
            _runner.AddCommandHandler("ReplaceSkill", ReplaceSkill);
        }

        public void NewSkillDialogue(int idx)
        {
            _newSkill = idx;
            _runner.VariableStorage.SetValue("$NewSkill", _rMController.GetSkillName(idx));

            _runner.StartDialogue("NewSkillAvailable");
        }

        private void ReplaceSkill()
        {
            _rMController.SetRemoveInfo(_newSkill);
            _profileBlock.SetActive(true);
        }

        public void StartNextDialogue(string oldSkill)
        {
            _runner.VariableStorage.SetValue("$DeletedSkill", oldSkill);
            
            _runner.StartDialogue("SkillReplacement");

            _profileBlock.SetActive(false);
            _rMController.ResetRemoveInfo();
        }
    }
}