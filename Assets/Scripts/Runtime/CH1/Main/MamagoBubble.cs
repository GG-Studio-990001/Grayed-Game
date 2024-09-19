using UnityEngine;

namespace Runtime.CH1.Main
{
    public class MamagoBubble : MonoBehaviour
    {
        public void CheckMamagoBubble()
        {
            if (Managers.Data.Scene == 5 && Managers.Data.SceneDetail == 1)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}