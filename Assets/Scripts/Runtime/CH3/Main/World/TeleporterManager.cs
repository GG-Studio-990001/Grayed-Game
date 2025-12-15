using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 모든 텔레포트 타워를 관리하고 활성화 상태를 추적하는 싱글톤 매니저
    /// </summary>
    public class TeleporterManager : MonoBehaviour
    {
        public static TeleporterManager Instance { get; private set; }

        private Dictionary<TeleportRegion, Teleporter> teleporters = new Dictionary<TeleportRegion, Teleporter>();
        private HashSet<TeleportRegion> activatedRegions = new HashSet<TeleportRegion>();
        private static readonly Dictionary<TeleportRegion, int> RegionPriorities = new Dictionary<TeleportRegion, int>
        {
            { TeleportRegion.BaseCamp, 0 },
            { TeleportRegion.Michael, 1 },
            { TeleportRegion.Farmer, 2 },
            { TeleportRegion.Dollar, 3 }
        };

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            activatedRegions.Add(TeleportRegion.BaseCamp);
        }

        public void RegisterTeleporter(Teleporter teleporter)
        {
            if (teleporter == null) return;

            teleporters[teleporter.Region] = teleporter;
        }

        public void UnregisterTeleporter(Teleporter teleporter)
        {
            if (teleporter == null) return;

            teleporters.Remove(teleporter.Region);
        }

        public void ActivateTeleporter(TeleportRegion region)
        {
            activatedRegions.Add(region);
        }

        public bool IsTeleporterActivated(TeleportRegion region)
        {
            return activatedRegions.Contains(region);
        }

        public List<TeleportRegion> GetAvailableRegions(TeleportRegion currentRegion)
        {
            var availableRegions = activatedRegions
                .Where(r => r != currentRegion)
                .OrderBy(r => GetRegionPriority(r))
                .ToList();

            return availableRegions;
        }

        private int GetRegionPriority(TeleportRegion region)
        {
            return RegionPriorities.TryGetValue(region, out int priority) ? priority : 999;
        }

        public Teleporter GetTeleporter(TeleportRegion region)
        {
            if (teleporters.TryGetValue(region, out Teleporter teleporter))
            {
                return teleporter;
            }

            Teleporter foundTeleporter = FindTeleporterInScene(region);
            if (foundTeleporter != null)
            {
                RegisterTeleporter(foundTeleporter);
            }

            return foundTeleporter;
        }

        private Teleporter FindTeleporterInScene(TeleportRegion region)
        {
            Teleporter[] allTeleporters = FindObjectsOfType<Teleporter>();
            foreach (var tp in allTeleporters)
            {
                if (tp.Region == region)
                {
                    return tp;
                }
            }
            return null;
        }

        public bool HasOtherActivatedTeleporters(TeleportRegion excludeRegion)
        {
            return activatedRegions.Any(r => r != excludeRegion);
        }
    }
}

