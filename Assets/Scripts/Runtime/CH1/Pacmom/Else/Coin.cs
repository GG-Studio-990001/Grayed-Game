using Runtime.ETC;
using System.Collections;
using UnityEngine;

namespace Runtime.CH1.Pacmom
{
    public class Coin : MonoBehaviour
    {
        public PMController Controller;
        private Vector3 _defaultPos;

        private void Start()
        {
            _defaultPos = transform.position;
        }

        public void ResetCoin()
        {
            transform.position = _defaultPos;
            gameObject.SetActive(true);
        }

        private void EatenByRapley()
        {
            gameObject.SetActive(false);

            if (Controller != null)
                Controller.CoinEatenByRapley();
        }

        private void EatenByPacmom()
        {
            gameObject.SetActive(false);

            if (Controller != null)
                Controller.CoinEatenByPacmom();
        }

        private void SuckByVacuum()
        {
            if (gameObject.activeSelf)
                StartCoroutine(nameof(SuckCoin));
        }

        private IEnumerator SuckCoin()
        {
            float duration = 0.2f;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                Vector3 pacomPos = Controller.GetPacmomPos();
                Vector3 newPosition = Vector3.Lerp(_defaultPos, pacomPos, elapsed / duration);
                newPosition.z = transform.position.z;
                this.transform.position = newPosition;

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(GlobalConst.PlayerStr))
            {
                EatenByRapley();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer(GlobalConst.PacmomStr))
            {
                if (other.gameObject.CompareTag(GlobalConst.VacuumStr))
                {
                    SuckByVacuum();
                }
                else
                {
                    EatenByPacmom();
                }
            }
        }
    }
}