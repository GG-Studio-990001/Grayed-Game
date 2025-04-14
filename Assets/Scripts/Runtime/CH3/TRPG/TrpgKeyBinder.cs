using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH3.TRPG
{
    public class TrpgKeyBinder : MonoBehaviour
    {
        [SerializeField] private LineView line;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.TrpgKeyBinding(line);
        }
    }
}