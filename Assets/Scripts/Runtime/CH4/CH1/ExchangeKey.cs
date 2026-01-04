using TMPro;
using UnityEngine;

namespace CH4.CH1
{
    public class ExchangeKey : MonoBehaviour
    {
        [SerializeField] private ResourceController _resourceController;
        [SerializeField] private ButtonInteractableController _buttonInteractableController;
        [SerializeField] private GameObject _targetPanel;
        [SerializeField] private TextMeshProUGUI _fishTxt;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("열쇠당");
                if (_targetPanel != null)
                    _targetPanel.SetActive(true);
                _fishTxt.text = _resourceController.Fish.ToString() + "/7";
                _buttonInteractableController.CheckPurchaseKeyBtn();
            }
        }
    }
}