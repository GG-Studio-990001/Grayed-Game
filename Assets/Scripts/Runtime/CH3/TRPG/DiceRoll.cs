using UnityEngine;

namespace Runtime.CH3.DnD
{
    public class DiceRoll : MonoBehaviour
    {
        // [SerializeField] private float _maxRandomForceValue, _startRollingForce;
        private Vector3 _initPos;
        private Quaternion _initRot;
        public int DiceFaceNum;
        Rigidbody _body;
        [SerializeField] private float _forceX, _forceY, _forceZ;

        private void Awake()
        {
            //_initRot = transform.rotation;
            _initPos = transform.position;
            Initialize();
        }

        public void RollDiceTest()
        {
            Initialize();
            _body.isKinematic = false;
            _body.AddForce(350, 500, -150);
            _body.AddTorque(0, 0, -20);
        }

        public void RollDiceTest2()
        {
            Initialize();
            _body.isKinematic = false;
            _body.AddForce(500, 500, -50);
            _body.AddTorque(0, 0, -10);
        }

        public void RollDice()
        {
            //_body.iskinematic = false;
            //_forcex = random.range(0, _maxrandomforcevalue);
            //_forcey = random.range(0, _maxrandomforcevalue);
            //_forcez = random.range(0, _maxrandomforcevalue);

            //_body.addforce(vector3.up * _startrollingforce);
            //_body.addtorque(_forcex, _forcey, _forcez);
        }

        private void Initialize()
        {
            DiceFaceNum = -1;
            _body = GetComponent<Rigidbody>();
            _body.isKinematic = true;
            // transform.SetPositionAndRotation(_initPos, _initRot);
            transform.position = _initPos;
            transform.rotation = new Quaternion(Random.Range(0, 30), 0, 0, 0);
        }
    }
}