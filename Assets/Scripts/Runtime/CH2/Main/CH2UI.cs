using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlasticPipe.PlasticProtocol.Messages.Serialization.ItemHandlerMessagesSerialization;

public class CH2UI : MonoBehaviour
{
    [NonSerialized] public TurnController TurnController;
    [SerializeField] private TextMeshProUGUI _locationTxt;
    [SerializeField] private Transform _locationOptions;
    [SerializeField] private GameObject _optionBtnPrefab;

    public void SetLocationTxt(string text)
    {
        _locationTxt.text = text;
    }

    public void SetLocationOptions(List<string> loc)
    {
        MakeOptions(loc.Count);
        _locationOptions.gameObject.SetActive(true);

        for (int i = 0; i < loc.Count; i++)
        {
            if (i >= _locationOptions.childCount)
            {
                Debug.LogError("Not enough buttons created.");
                return;
            }

            Button btn = _locationOptions.GetChild(i).GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            int index = i; // Local copy of i for the closure
            btn.onClick.AddListener(() => TurnController.AdvanceTurnAndMoveLocation(loc[index]));

            TextMeshProUGUI btnTxt = btn.GetComponentInChildren<TextMeshProUGUI>();
            btnTxt.text = loc[index];
        }
    }

    private void MakeOptions(int cnt)
    {
        int child = _locationOptions.childCount;

        if (child == cnt)
            return;

        if (child > cnt)
        {
            for (int i = 0; i < child - cnt; i++)
            {
                Destroy(_locationOptions.GetChild(i).gameObject);
            }
        }
        else
        {
            for (int i = 0; i < cnt - child; i++)
            {
                Instantiate(_optionBtnPrefab, _locationOptions.position, _locationOptions.rotation, _locationOptions);
            }
        }
    }
}
