using UnityEngine;
using GoogleSheetsToUnity;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Runtime.CH2;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Runtime.CH2
{
    [Serializable]
    public struct ItemData
    {
        public int id;
        public string name;
        [TextArea] public string description;

        public ItemData(int id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }
    }

    [CreateAssetMenu(fileName = "Reader", menuName = "Scriptable Object/StoryBranchReader", order = int.MaxValue)]
    public class StoryBranchReader : DataReaderBase
    {
        [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")][SerializeField] public List<ItemData> DataList = new List<ItemData>();

        internal void UpdateStats(List<GSTU_Cell> list, int itemID)
        {
            int id = 0;
            string name = null, description = null;

            for (int i = 0; i < list.Count; i++)
            {
                switch (list[i].columnId)
                {
                    case "id":
                        {
                            id = int.Parse(list[i].value);
                            break;
                        }
                    case "name":
                        {
                            name = list[i].value;
                            break;
                        }
                    case "description":
                        {
                            description = list[i].value;
                            break;
                        }
                }
            }

            DataList.Add(new ItemData(id, name, description));
        }
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(StoryBranchReader))]
public class StoryBranchReaderEditor : Editor
{
    StoryBranchReader data;

    void OnEnable()
    {
        data = (StoryBranchReader)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("\n\n스프레드 시트 읽어오기");

        if (GUILayout.Button("데이터 읽기(API 호출)"))
        {
            UpdateStats(UpdateMethodOne);
            data.DataList.Clear();
        }
    }

    void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(data.SheetId, data.WorkSheetName), callback, mergedCells);
    }

    void UpdateMethodOne(GstuSpreadSheet ss)
    {
        for (int i = data.StartRow; i <= data.EndRow; ++i)
        {
            data.UpdateStats(ss.rows[i], i);
        }

        EditorUtility.SetDirty(target);
    }
}
#endif