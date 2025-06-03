using UnityEngine;
using UnityEngine.UI;

namespace Runtime.InGameSystem
{
    public abstract class FadeController : MonoBehaviour
    {
        [SerializeField] protected Image fadeImage;

        protected float FadeDuration { get; set; } = 1f;

        public virtual void SetFadeImg()
        {
            fadeImage.color = new Color(0, 0, 0, 1);
        }

        public abstract void StartFadeIn();
        public abstract void StartFadeOut();
    }
}
