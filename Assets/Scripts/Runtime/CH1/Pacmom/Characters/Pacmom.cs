using Runtime.ETC;
using System;
using UnityEngine;

namespace Runtime.CH1.Pacmom
{
    public class Pacmom : MonoBehaviour
    {
        [NonSerialized] public PMController Controller;
        public MovementWithFlipAndRotate Movement { get; set; }
        [SerializeField] private GameObject _vacuum;

        private void Awake()
        {
            Movement = GetComponent<MovementWithFlipAndRotate>();
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
                if (Controller == null)
                    return;

                Controller.RapleyEaten();
            }
        }
    }
}
