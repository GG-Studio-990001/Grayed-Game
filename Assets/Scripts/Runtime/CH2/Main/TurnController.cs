using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TurnController : MonoBehaviour
{
    [SerializeField] DialogueRunner _dialogueRunner;
    private List<Dictionary<string, object>> _data = null;
    private int _turn = 0;
    private string _location = null;

    private void Awake()
    {
        _data = CSVReader.Read("BIC_Move");
    }

    public void AdvanceTurnAndMoveLocation(string location)
    {
        _turn++;
        _location = location;
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

    private void DisplayAvailableLocations()
    {
        // 현재 다이얼로그가 끝나고 이동 가능한 장소 출력
    }
}
