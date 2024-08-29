using Runtime.ETC;
using System;
using UnityEngine;

namespace Runtime.CH1.Pacmom
{
    public class Pacmom : MonoBehaviour
    {
        [NonSerialized]
        public PMController GameController;
        public MovementWithFlipAndRotate Movement { get; set; }
        private AI _ai;
        [SerializeField]
        private GameObject _vacuum;

        private void Awake()
        {
            Movement = GetComponent<MovementWithFlipAndRotate>();
            _ai = GetComponent<AI>();
        }

        private void Start()
        {
            SetSpriteRotation();
            Movement.ResetState();
        }

        private void SetSpriteRotation()
        {
            Movement.SpriteRotation.SetCanRotate(true);
        }

        private void FixedUpdate()
        {
            Movement.Move();
        }

        public void VacuumMode(bool isVacuum)
        {
            Movement.SetRotateZ();

            Movement.SpriteRotation.SetCanRotate(!isVacuum);
            _vacuum.SetActive(isVacuum);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer(GlobalConst.PlayerStr))
            {
                if (GameController == null)
                    return;

                GameController.RapleyEaten();
            }
        }
    }
}
