using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class ArioHeartsUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] _children;
    
        public void ChangeHeartUI(int count)
        {
            switch (count)
            {
                case 0:
                    _children[0].SetActive(false);
                    _children[1].SetActive(false);
                    _children[2].SetActive(false);
                    break;
                case 1:
                    _children[0].SetActive(false);
                    _children[1].SetActive(false);
                    _children[2].SetActive(true);
                    break;
                case 2:
                    _children[0].SetActive(false);
                    _children[1].SetActive(true);
                    _children[2].SetActive(true);
                    break;
                case 3:
                    _children[0].SetActive(true);
                    _children[1].SetActive(true);
                    _children[2].SetActive(true);
                    break;
            }
        }
    }
}