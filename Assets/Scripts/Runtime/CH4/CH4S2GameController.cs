using System.Collections;
using UnityEngine;

namespace Runtime.CH4
{
    public class CH4S2GameController : MonoBehaviour
    {
        [field: SerializeField]
        public int NowLevel { get; private set; }
        [SerializeField] private GameObject Player;
        [SerializeField] private GameObject[] LevelObjs;
        [SerializeField] private SwitchLocation[] switchLocation;
        [SerializeField] private GameObject[] BGs;
        [SerializeField] private Vector3 PlayerInitPos;

        private void Start()
        {
            StartLevel(NowLevel);
        }

        public void StartLevel(int level)
        {
            Player.SetActive(false);

            // 모든 레벨 오브젝트 초기화
            foreach (var obj in LevelObjs)
                obj.SetActive(false);

            NowLevel = level;

            LevelObjs[level - 1].SetActive(true);

            StartCoroutine(LevelRoutine(level));
        }

        private IEnumerator LevelRoutine(int level)
        {
            // 맵 로드 프레임 동안 대기
            yield return null;

            foreach (var bg in BGs)
                bg.SetActive(false);

            switchLocation[level - 1].StartLevel();

            yield return null;
            
            Player.transform.localPosition = PlayerInitPos;
            Player.SetActive(true);
        }
    }
}