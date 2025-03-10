using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH3.Rokemon
{
    public class RMDialogue : DialogueViewBase
    {
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [Header("=Else=")]
        [SerializeField] private LineView _lineView;

        private void Awake()
        {
            // _runner.AddCommandHandler("", );
        }
    }
}