using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.CH1.Pacmom
{
    public class PMShader : MonoBehaviour
    {
        [SerializeField]
        private Volume _volume;
        private Bleed _bleed;

        private void Start()
        {
            if (_volume != null)
            {
                _volume.profile.TryGet(out _bleed);
            }

            _bleed.active = true;
        }

        public void ChangeBleedAmount()
        {
            _bleed.bleedAmount.value = 10f;
        }
    }
}