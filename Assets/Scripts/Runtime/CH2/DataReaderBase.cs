using UnityEngine;

namespace Runtime.CH2
{
    public class DataReaderBase : ScriptableObject
    {
        [Header("시트의 주소")]
        [SerializeField] public string SheetId = ""; // 1VRmG8Bv0I-4TSoCNACvhbK5IHR1_2LsF
        [Header("스프레드 시트의 시트 이름")]
        [SerializeField] public string WorkSheetName = ""; // StoryBranch
        [Header("읽기 시작할 행 번호")]
        [SerializeField] public int StartRow = 2;
        [Header("읽을 마지막 행 번호")]
        [SerializeField] public int EndRow = 4; // 18
    }
}