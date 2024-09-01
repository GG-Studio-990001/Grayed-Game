using UnityEngine;

namespace Runtime.CH1.Pacmom
{
    public class Prey : AI
    {
        [SerializeField] private Transform _enemy;
        [SerializeField] private InGameDialogue _dialogue;
        private Dust _dust;
        private bool _dustTalked = false;

        private void Awake()
        {
            _dust = GetComponent<Dust>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<Step>(out _))
                return;

            Step step = other.GetComponent<Step>();

            float distanceFromEnemy = (_enemy.position - transform.position).sqrMagnitude;
            BeingChased(distanceFromEnemy); 
            
            Vector2 direction;
            if (distanceFromEnemy < 36f)
            {
                direction = RunAwayFromEnemy(step);
            }
            else
            {
                direction = MoveRandomly(step);
            }

            Movement.SetNextDirection(direction);
        }

        private void BeingChased(float distance)
        {
            if (distance < 20f)
            {
                if (!_dustTalked)
                {
                    _dialogue.ChasedDialogue(_dust.DustID);
                    _dustTalked = true;
                }
            }
            else
            {
                _dustTalked = false;
            }
        }

        private Vector2 RunAwayFromEnemy(Step step)
        {
            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue;

            foreach (Vector2 availableDirection in step.AvailableDirections)
            {
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);
                float newDistance = (_enemy.position - newPosition).sqrMagnitude;

                if (newDistance > maxDistance)
                {
                    maxDistance = newDistance;
                    direction = availableDirection;
                }
            }

            return direction;
        }
    }
}