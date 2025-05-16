using UnityEngine;

namespace Runtime.CH3.TRPG
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
            Physics.gravity = new Vector3(0, -9.81f * 3, 0); // TODO: CH3씬에 합칠 때 2.5D에 중력 영향 안가도록 설정

            // _initRot = transform.rotation;
            _initPos = transform.position;
            Initialize();
        }

        public void RollDice()
        {
            // Initialize();
            
            Debug.Log(this.name + " " + transform.localEulerAngles);
            _body.isKinematic = false;

            float t = 4f;
            _body.AddForce(_forceX * t, _forceY * t, _forceZ * t);
        }

        public void Initialize()
        {
            DiceFaceNum = -1;
            _body = GetComponent<Rigidbody>();
            _body.isKinematic = true;

            // transform.position = _initPos;
            // Quaternion rot = Quaternion.Euler(Random.Range(0, 181), Random.Range(0, 181), Random.Range(0, 181));
            Quaternion rot = Quaternion.Euler(Random.Range(0, 2) == 0 ? 90 : -90, 0, 72 * Random.Range(1, 6));
            // Quaternion rot = Quaternion.Euler(0, 0, 0);
            transform.SetPositionAndRotation(_initPos, rot);
        }
    }
}