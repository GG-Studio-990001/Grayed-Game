using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TurnController : MonoBehaviour
{
    [SerializeField] private DialogueRunner _dialogueRunner;
    [SerializeField] private CH2UI _ch2Ui;
    private List<Dictionary<string, object>> _data = new();
    [SerializeField] private int _turn = 0;
    [SerializeField] string _location = null;

    private void Awake()
    {
        _data = CSVReader.Read("BIC_Move");
        _ch2Ui.TurnController = this;
    }

    public void AdvanceTurnAndMoveLocation(string location)
    {
        _turn++;
        _location = location;
        _ch2Ui.SetLocationTxt(_location);
        InitiateDialogue();
    }

    private void InitiateDialogue()
    {
        _dialogueRunner.StartDialogue(FetchDialogueName());
    }

    private string FetchDialogueName()
    {
        // 현재 턴수와 장소에 맞는 다이얼로그 출력
        foreach (var row in _data)
        {
            if (row.ContainsKey("턴수") && (int)row["턴수"] == _turn)
            {
                if (row.ContainsKey(_location))
                {
                    return row[_location].ToString();
                }
            }
        }
        return null;
    }

    private List<string> GetAvailableLocations()
    {
        List<string> loc = new();

        foreach (var row in _data)
        {
            if (row.ContainsKey("턴수") && (int)row["턴수"] == _turn + 1)
            {
                foreach (var col in row)
                {
                    if (col.Value is string value && value != "이동 불가" && value != (_turn + 1).ToString())
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
