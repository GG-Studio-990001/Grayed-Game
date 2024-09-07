using Runtime.ETC;
using System;
using UnityEngine;

namespace Runtime.CH1.Pacmom
{
    public class Dust : MonoBehaviour
    {
        [NonSerialized] public PMController GameController;
        public MovementWithEyes Movement { get; set; }
        [field:SerializeField] public int DustID { get; private set; }

        private void Awake()
        {
            Movement = GetComponent<MovementWithEyes>();
        }

        private void Start()
        {
            Movement.ResetState();
        }

        private void FixedUpdate()
        {
            Movement.Move();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer(GlobalConst.PacmomStr))
            {
                if (GameController == null)
                    return;

                GameController.DustEaten(this);
            }
        }
    }
}