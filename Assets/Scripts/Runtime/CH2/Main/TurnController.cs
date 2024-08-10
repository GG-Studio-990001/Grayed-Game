using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TurnController : MonoBehaviour
{
    [SerializeField] private DialogueRunner _dialogueRunner;
    [SerializeField] private CH2UI _ch2Ui;
    private List<Dictionary<string, object>> _data = new();

    private void Awake()
    {
        _data = CSVReader.Read("BIC_Move");
        _ch2Ui.TurnController = this;
    }

    public void GetInitialLocation()
    {
        List<string> loc = GetAvailableLocations();
        if (loc.Count != 1)
        {
            Debug.LogError("Location is not unique.");
        }

        AdvanceTurnAndMoveLocation(loc[0]);
    }

    public void AdvanceTurnAndMoveLocation(string location)
    {
        Managers.Data.CH2.Turn++;
        Managers.Data.CH2.Location = location;
        _ch2Ui.SetLocationTxt(Managers.Data.CH2.Location);
        InitiateDialogue();
    }

    private void InitiateDialogue()
    {
        _dialogueRunner.StartDialogue(GetDialogueName());
    }

    private string GetDialogueName()
    {
        // 현재 턴수와 장소에 맞는 다이얼로그 이름 가져오기
        foreach (var row in _data)
        {
            if (row.ContainsKey("Turn") && (int)row["Turn"] == Managers.Data.CH2.Turn)
            {
                if (row.ContainsKey(Managers.Data.CH2.Location))
                {
                    return row[Managers.Data.CH2.Location].ToString();
                }
            }
        }
        return null;
    }

    private List<string> GetAvailableLocations()
    {
        // 이동 가능한 장소 리스트 가져오기
        List<string> loc = new();

        foreach (var row in _data)
        {
            if (row.ContainsKey("Turn") && (int)row["Turn"] == Managers.Data.CH2.Turn + 1)
            {
                foreach (var col in row)
                {
                    if (col.Value is string value && value != "X" && value != (Managers.Data.CH2.Turn + 1).ToString())
                    {
                        loc.Add((string)col.Key);
                    }
                }
            }
        }

        return loc;
    }

    public void DisplayAvailableLocations()
    {
        _ch2Ui.SetLocationOptions(GetAvailableLocations());
    }
}