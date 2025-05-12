using UnityEngine;

namespace Runtime.CH3.DnD
{
    public class DiceRoll : MonoBehaviour
    {
        private Vector3 _initPos;
        // private Quaternion _initRot;
        public int DiceFaceNum;
        Rigidbody _body;
        [SerializeField] private float _forceX, _forceY, _forceZ;

        private void Awake()
        {
            Physics.gravity = new Vector3(0, -9.81f * 3, 0);

            // _initRot = transform.rotation;
            _initPos = transform.position;
            Initialize();
        }

        public void RollDice()
        {
            Initialize();
            _body.isKinematic = false;

            float t = 4f;
            _body.AddForce(_forceX * t, _forceY * t, _forceZ * t);
        }

        private void Initialize()
        {
            DiceFaceNum = -1;
            _body = GetComponent<Rigidbody>();
            _body.isKinematic = true;

            // transform.position = _initPos;
            Quaternion rot = Quaternion.Euler(Random.Range(0, 181), Random.Range(0, 181), Random.Range(0, 181));
            transform.SetPositionAndRotation(_initPos, rot);
        }
    }
}