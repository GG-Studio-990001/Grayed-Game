using UnityEngine;

namespace Runtime.CH3.DnD
{
    public class DiceRoll : MonoBehaviour
    {
        [SerializeField] private float _maxRandomForceValue, _startRollingForce;
        public int DiceFaceNum;
        Rigidbody _body;
        private float _forceX, _forceY, _forceZ;

        private void Awake()
        {
            Initialize();
        }

        public void RollDice()
        {
            _body.isKinematic = false;
            _forceX = Random.Range(0, _maxRandomForceValue);
            _forceY = Random.Range(0, _maxRandomForceValue);
            _forceZ = Random.Range(0, _maxRandomForceValue);

            _body.AddForce(Vector3.up * _startRollingForce);
            _body.AddTorque(_forceX, _forceY, _forceZ);
        }

        private void Initialize()
        {
            DiceFaceNum = -1;
            _body = GetComponent<Rigidbody>();
            _body.isKinematic = true;
            transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
        }
    }
}