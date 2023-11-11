using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 8.0f;

    protected override void Eat()
    {
        FindObjectOfType<PacmanGameManager>().PowerPelletEaten(this);
    }
}
