using System.Collections;
using UnityEngine;

namespace Runtime.CH4
{
    public class CH4Stage2GameController : MonoBehaviour
    {
        [field: SerializeField]
        public int NowLevel { get; private set; }
        [SerializeField] private GameObject Player;
        [SerializeField] private GameObject Level1Obj;
        [SerializeField] private GameObject Level2Obj;
        [SerializeField] private SwitchLocation switchLocation;
        [SerializeField] private SwitchLocation2 switchLocation2;
        [SerializeField] private GameObject[] BGs;

        private void Start()
        {
            if (NowLevel == 1)
                StartLevel1();
            else
                StartLevel2();
        }

        public void StartLevel1()
        {
            Player.SetActive(false);
            NowLevel = 1;
            Level1Obj.SetActive(true);
            StartCoroutine(nameof(Level1));
        }

        public void StartLevel2()
        {
            Player.SetActive(false);
            Level1Obj.SetActive(false);
            NowLevel = 2;
            Level2Obj.SetActive(true);
            StartCoroutine(nameof(Level2));
        }

        private IEnumerator Level1()
        {
            // 맵 로드 프레임 동안 대기
            yield return null;

            foreach (var bg in BGs)
                bg.SetActive(false);

            switchLocation.StartLevel();

            yield return null;

            Player.SetActive(true);
        }

        private IEnumerator Level2()
        {
            // 맵 로드 프레임 동안 대기
            yield return null;

            foreach (var bg in BGs)
                bg.SetActive(false);

            switchLocation2.StartLevel();

            yield return null;

            Player.transform.localPosition = new(-37.98f, -10.97f, 0);
            Player.SetActive(true);
        }
    }
}