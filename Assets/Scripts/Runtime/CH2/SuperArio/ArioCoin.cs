using UnityEngine;

public class ArioCoin : MonoBehaviour
{
    private System.Random random = new System.Random();

    public void RandomCoin()
    {
        // random 객체를 사용하여 값을 추출
        if (random.Next(0, 100) <= 10)
        {
            gameObject.SetActive(true);
        }
    }
}