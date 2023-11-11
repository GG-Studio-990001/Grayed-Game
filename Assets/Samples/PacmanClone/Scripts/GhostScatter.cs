using UnityEngine;

public class GhostScatter : GhostBehavior
{
    private void OnDisable()
    {
        this.ghost.chase.Enable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled && !this.ghost.frightend.enabled)
        {
            int index = Random.Range(0, node.availableDirections.Count);

            if (node.availableDirections[index] == -this.ghost.movement.direction && node.availableDirections.Count > 1)
            {
                // cuz we dont wanna back track
                index++; // 랜덤으로 새로 뽑을바엔 +1

                if (index >= node.availableDirections.Count)
                    index = 0;
            }

            this.ghost.movement.SetDirection(node.availableDirections[index]);
        }
    }
}
