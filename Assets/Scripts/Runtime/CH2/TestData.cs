using UnityEngine;
using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Runtime.CH2
{
    public class TestData : ScriptableObject
    {
        public string associatedSheet = "";
        public string associatedWorksheet = "";

        public List<string> items = new List<string>();

        public List<string> Names = new List<string>();
        internal void UpdateStats(List<GSTU_Cell> list, string name)
        {
            items.Clear();
            int math = 0, korean = 0, english = 0;
            for (int i = 0; i < list.Count; i++)
            {
                switch (list[i].columnId)
                {
                    case "Math":
                        {
                            math = int.Parse(list[i].value);
                            break;
                        }
                    case "Korean":
                        {
                            korean = int.Parse(list[i].value);
                            break;
                        }
                    case "English":
                        {
                            english = int.Parse(list[i].value);
                            break;
                        }
                }
            }
            Debug.Log($"{name}의 점수 수학:{math} 국어:{korean} 영어:{english}");
        }

    }

    [CustomEditor(typeof(TestData))]
    public class DataEditor : Editor
    {
        TestData data;

        void OnEnable()
        {
            data = (TestData)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label("Read Data Examples");

            if (GUILayout.Button("Pull Data Method One"))
            {
                UpdateStats(UpdateMethodOne);
            }
        }

        void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
        {
            SpreadsheetManager.Read(new GSTU_Search(data.associatedSheet, data.associatedWorksheet), callback, mergedCells);
        }

        void UpdateMethodOne(GstuSpreadSheet ss)
        {
            //data.UpdateStats(ss.rows["Jim"]);
            foreach (string dataName in data.Names)
                data.UpdateStats(ss.rows[dataName], dataName);
            EditorUtility.SetDirty(target);
        }

    }
}