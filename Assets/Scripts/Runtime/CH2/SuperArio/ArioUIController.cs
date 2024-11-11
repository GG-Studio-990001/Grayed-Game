using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArioUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text restartText;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private Image itemImg;
    [SerializeField] private ArioHeartsUI hearts;

    public void ActiveRestartText(bool isRestart)
    {
        if(isRestart)
            restartText.gameObject.SetActive(true);
        else
            restartText.gameObject.SetActive(false);
    }
    
    public void ChangeHeartUI(int count)
    {
        hearts.ChangeHeartUI(count);
    }

    public void ChangeStageText(string text)
    {
        stageText.text = text;
    }

    public void ChangeCoinText(string text)
    {
        coinText.text = text;
    }

    public void ChangeItemSprite(bool isUse)
    {
        itemImg.enabled = !isUse;
    }
}
