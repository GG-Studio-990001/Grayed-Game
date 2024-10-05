using UnityEngine;

namespace Runtime.CH1.SubB._3_Match
{
    public class ThreeMatchPuzzleClearCheck : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Managers.Data.CH1.Is3MatchCleared = true;
                Managers.Data.SaveGame();
            }
        }
    }
}
