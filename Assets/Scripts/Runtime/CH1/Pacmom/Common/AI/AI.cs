using UnityEngine;

namespace Runtime.CH1.Pacmom
{
    public class AI : MonoBehaviour
    {
        public Movement Movement;

        public Vector2 MoveRandomly(Step step)
        {
            int index = Random.Range(0, step.AvailableDirections.Count);

            if (step.AvailableDirections[index] == -1 * Movement.Direction && step.AvailableDirections.Count > 1)
            {
                index++;

                if (index >= step.AvailableDirections.Count)
                    index = 0;
            }

            return step.AvailableDirections[index];
        }
    }
}