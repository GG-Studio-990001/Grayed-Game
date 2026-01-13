using System.Collections;
using UnityEngine;
using Runtime.ETC;

namespace Runtime.CH3.TRPG
{
    [RequireComponent(typeof(Rigidbody))]
    public class DiceRoll : MonoBehaviour
    {
        private Vector3 _initPos;
        public int DiceFaceNum;

        private Rigidbody _body;

        [SerializeField] private float _forceX;
        [SerializeField] private float _forceY;
        [SerializeField] private float _forceZ;
        [SerializeField] private Vector3[] _presets;

        private bool _hasHitGround;

        private void Awake()
        {
            Physics.gravity = new Vector3(0, -9.81f * 3, 0); // TODO: CH3씬에 합칠 때 2.5D에 중력 영향 안가도록 설정

            _body = GetComponent<Rigidbody>();
            _initPos = transform.position;

            Initialize();
        }

        public void DiceInit()
        {
            Initialize();
            SetValue();
        }

        public void RollDice()
        {
            _hasHitGround = false;
            _body.isKinematic = false;

            float t = 4f;
            _body.AddForce(_forceX * t, _forceY * t, _forceZ * t);
        }

        private void Initialize()
        {
            DiceFaceNum = -1;
            _body.isKinematic = true;
        }

        private void SetValue()
        {
            int val = Random.Range(0, _presets.Length);
            Quaternion rot = Quaternion.Euler(_presets[val]);
            transform.SetPositionAndRotation(_initPos, rot);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasHitGround) return;
            if (_body.isKinematic) return;
            if (!collision.gameObject.CompareTag("Ground")) return;

            _hasHitGround = true;
            StartCoroutine(PlayDiceLandingSFX());
        }

        private IEnumerator PlayDiceLandingSFX()
        {
            float STOP_VELOCITY_THRESHOLD = 0.05f;

            // 첫 착지 효과음 (1 ~ 3)
            PlaySFX(Random.Range(1, 3 + 1));

            // 0.n초 후에도 착지하지 않았다면
            yield return new WaitForSeconds(0.2f);

            if (_body.velocity.sqrMagnitude > STOP_VELOCITY_THRESHOLD * STOP_VELOCITY_THRESHOLD)
            {
                PlaySFX(Random.Range(4, 5 + 1));

                // 또 0.n초 후에도 착지하지 않았다면
                yield return new WaitForSeconds(0.4f);

                if (_body.velocity.sqrMagnitude > STOP_VELOCITY_THRESHOLD * STOP_VELOCITY_THRESHOLD)
                {
                    PlaySFX(Random.Range(6, 7 + 1));
                }
            }
        }

        private void PlaySFX(int index)
        {
            Managers.Sound.Play(Sound.SFX, $"CH3/CoC/Dice/CH3_SFX_CoC_Dice_{index}");
        }
    }
}