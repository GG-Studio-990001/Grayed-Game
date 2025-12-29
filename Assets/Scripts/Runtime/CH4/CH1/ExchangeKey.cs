using TMPro;
using UnityEngine;

namespace CH4.CH1
{
    public class ExchangeKey : MonoBehaviour
    {
        [SerializeField] private ResourceController resourceController;
        [SerializeField] private GameObject targetPanel;
        [SerializeField] private TextMeshProUGUI fishTxt;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("열쇠당");
                if (targetPanel != null)
                    targetPanel.SetActive(true);
                fishTxt.text = resourceController.Fish.ToString() + "/7";
            }
        }
    }
}