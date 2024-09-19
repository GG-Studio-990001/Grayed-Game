using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Common
{
    public class TranslatorActivator : MonoBehaviour
    {
        [SerializeField] private Image[] _TransCoverImg;
        [SerializeField] private Sprite _activateSpr;

        public void ActiveTranslator(int idx)
        {
            _TransCoverImg[idx].sprite = _activateSpr;
        }
    }
}
