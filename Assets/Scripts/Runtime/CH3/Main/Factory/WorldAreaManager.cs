using UnityEngine;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    public class WorldAreaManager : MonoBehaviour
    {
        [System.Serializable]
        public class AreaDefinition
        {
            public string areaId;
            public Vector2Int start;
            public Vector2Int end;
            public List<SpawnRule> spawnRules;
        }

        [SerializeField] private List<AreaDefinition> areaDefinitions;
        [SerializeField] private bool initializeOnStart = true;

        private void Start()
        {
            if (initializeOnStart)
            {
                InitializeAreas();
            }
        }

        public void InitializeAreas()
        {
            foreach (var areaDef in areaDefinitions)
            {
                GridArea area = new GridArea(areaDef.start, areaDef.end, areaDef.areaId);
                
                if (GridAreaSpawner.Instance.HasArea(areaDef.areaId))
                {
                    GridAreaSpawner.Instance.ClearArea(areaDef.areaId);
                }

                GridAreaSpawner.Instance.RegisterArea(area);
                GridAreaSpawner.Instance.PopulateArea(areaDef.areaId, areaDef.spawnRules);
            }
        }

        public void ResetArea(string areaId)
        {
            var areaDef = areaDefinitions.Find(a => a.areaId == areaId);
            if (areaDef != null)
            {
                GridAreaSpawner.Instance.ClearArea(areaId);
                GridAreaSpawner.Instance.PopulateArea(areaId, areaDef.spawnRules);
            }
        }

        public void ResetAllAreas()
        {
            foreach (var areaDef in areaDefinitions)
            {
                ResetArea(areaDef.areaId);
            }
        }
    }
}