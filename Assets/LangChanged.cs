using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LangChanged : MonoBehaviour
{
    [SerializeField]
    GameObject Log;

    public void ShowText()
    {
        StartCoroutine("ShowLog");
    }

    public IEnumerator ShowLog()
    {
        Log.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        Log.SetActive(false);
    }
}
