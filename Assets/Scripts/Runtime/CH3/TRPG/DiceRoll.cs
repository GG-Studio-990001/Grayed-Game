using UnityEngine;

namespace Runtime.CH3.TRPG
{
    public class DiceRoll : MonoBehaviour
    {
        private Vector3 _initPos;
        public int DiceFaceNum;
        Rigidbody _body;
        [SerializeField] private float _forceX, _forceY, _forceZ;
        [SerializeField] private Vector3[] _presets;

        private void Awake()
        {
            Physics.gravity = new Vector3(0, -9.81f * 3, 0); // TODO: CH3씬에 합칠 때 2.5D에 중력 영향 안가도록 설정

            _body = GetComponent<Rigidbody>();
            _initPos = transform.position;

            Initialize();
        }

        public void RollDice()
        {
            Initialize();
            SetValue();

            Debug.Log(this.name + " Vector3" + transform.localEulerAngles);

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
            int val = Random.Range(0, 10);
            Quaternion rot = Quaternion.Euler(_presets[val]);
            transform.SetPositionAndRotation(_initPos, rot);
            Debug.Log(this.name + " 목표값: " + val);
        }
    }
}