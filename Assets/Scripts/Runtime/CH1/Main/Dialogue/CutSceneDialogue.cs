using Runtime.CH1.Main.Player;
using UnityEngine;
using DG.Tweening;
using Runtime.ETC;
using Runtime.CH1.Main.Npc;
using Yarn.Unity;
using System.Collections;
using SLGDefines;

namespace Runtime.CH1.Main.Dialogue
{
    public class CutSceneDialogue : MonoBehaviour
    {
        [Header("=Player=")]
        public TopDownPlayer Player;
        [SerializeField] private Vector3[] _location = new Vector3[2];
        [Header("=Npc=")]
        public NpcPosition NpcPos;
        [SerializeField] private NpcBody[] _npc = new NpcBody[3];
        [SerializeField] private NpcBody _mamago;
        [SerializeField] private Vector3[] _mamagoLocation;
        [SerializeField] private NpcBody _michael;
        [Header("=Else=")]
        [SerializeField] private GameObject _lucky;
        [SerializeField] private GameObject _stage2;
        [SerializeField] private Bridge _bridge;
        [SerializeField] private DialogueRunner _luckyDialogue;
        [SerializeField] private GameObject _visualNovel;
        [SerializeField] private GameObject _mamagoBubble;
        [SerializeField] private DialogueRunner _luckyRunner;
        private Sequence _shakeTween;

        public void NpcsMove()
        {
            _npc[1].Anim.SetAnimation(GlobalConst.MoveStr, Vector2.right);
            _npc[1].transform.DOMove(NpcPos.GetSingeNpcPos(1, 9), 3f).SetEase(Ease.Linear);

            _npc[2].Anim.SetAnimation(GlobalConst.MoveStr, Vector2.right);
            _npc[2].transform.DOMove(NpcPos.GetSingeNpcPos(2, 9), 3f).SetEase(Ease.Linear);
        }

        public void Scene4End()
        {
            CharactersStop(3);
            _npc[0].transform.localPosition = NpcPos.GetSingeNpcPos(0, 9);
            _npc[0].gameObject.SetActive(false);
            _michael.gameObject.SetActive(false);

            SLGActionComponent slgAction = FindObjectOfType<SLGActionComponent>();
            if (slgAction != null)
            {
                slgAction.MoveOnNextProgress();
            }
        }

        public void MichaelAction(int i)
        {
            switch (i)
            {
                case 0:
                    ShowMichael();
                    ShowSubObjectDropAnim();
                    break;
                case 1:
                    MichaelStop();
                    break;
                case 2:
                    ShowPackDropAnim();
                    MichaelRun();
                    break;
            }
        }

        private void ShowMichael()
        {
            _michael.gameObject.SetActive(true);
        }

        private void ShowSubObjectDropAnim()
        {
            StartCoroutine(nameof(SubObjectDrop));
        }

        private void ShowPackDropAnim()
        {
            SLGActionComponent slgAction = FindObjectOfType<SLGActionComponent>();
            if (slgAction != null)
            {
                GameObject _packObject = slgAction.SLGTriggerObject;
                _packObject.SetActive(true);
                Vector3 _cachedPos = _packObject.transform.position;
                Vector3 _startPos = _michael.gameObject.transform.position;
                _startPos.y += 1;
                _packObject.transform.position = _startPos;
                _packObject.transform.DOMove(_cachedPos, 0.5f).SetEase(Ease.Linear);
            }
        }

        IEnumerator SubObjectDrop()
        {
            SubObjectDropAnim(0);
            yield return new WaitForSeconds(1.0f);

            SubObjectDropAnim(1);
        }

        private void SubObjectDropAnim(int InObjectIndex)
        {
            SLGActionComponent slgAction = FindObjectOfType<SLGActionComponent>();
            if (slgAction != null)
            {
                if (slgAction.SLGSubObjects.Length > InObjectIndex)
                {
                    GameObject _subObject = slgAction.SLGSubObjects[InObjectIndex];
                    _subObject.SetActive(true);
                    Vector3 cachedPos = _subObject.transform.position;
                    Vector3 _startPos = _michael.gameObject.transform.position;
                    _startPos.y += 0.5f;
                    _subObject.transform.position = _startPos;
                    _subObject.transform.DOMove(cachedPos, 0.5f).SetEase(Ease.Linear);
                }
            }
        }

        private void MichaelStop()
        {
            _michael.GetComponent<Animator>().SetTrigger("Stop");
        }

        public void R2MonRun()
        {
            _npc[2].Anim.SetAnimation(GlobalConst.MoveStr, Vector2.right);
            _npc[2].transform.DOMove(new Vector3(101.08f, -15f, 0), 4f).SetEase(Ease.Linear);

            // 비켜주기
            //float posX = Player.transform.localPosition.x;
            //if (posX >= 90.9f)
            //{
            //    StartCoroutine(nameof(RapleyGetOutR2Mon), (Player.transform.localPosition.y > -15.4f));
            //}
        }

        private IEnumerator RapleyGetOutR2Mon(bool isUp)
        {
            float posX = Player.transform.localPosition.x;
            Vector2 direction = isUp ? Vector2.up : Vector2.down;
            float posY = isUp ? -14.24f : -16.37f;

            Player.Animation.SetAnimation(GlobalConst.MoveStr, direction);
            Player.transform.DOMove(new Vector3(posX, posY, 0), 0.3f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.3f);

            Player.SetLastInput(Vector2.right); // 오른쪽을 보게끔
            Player.Animation.SetAnimation(GlobalConst.IdleStr, Vector2.right);
        }

        private void MichaelRun()
        {
            // TODO: 매직넘버 리팩터링...
            _michael.Anim.SetAnimation(GlobalConst.MoveStr, Vector2.right);
            _michael.transform.DOMove(new Vector3(89.5400009f, -16.0f, 0), 3.5f).SetEase(Ease.Linear);
        }

        public void DollarRun()
        {
            _npc[0].Anim.SetAnimation(GlobalConst.MoveStr, Vector2.right);
            _npc[0].transform.DOMove(new Vector3(89.5400009f, -16.0f, 0), 6f).SetEase(Ease.Linear);
        }

        public void RebuildBridge()
        {
            SLGActionComponent slgAction = FindObjectOfType<SLGActionComponent>();
            if (slgAction != null)
            {
                slgAction.MoveOnNextBuildingState(SLGBuildingType.Bridge, SLGBuildingProgress.EndConstruct);
            }
        }

        public void BuildCompany()
        {
            SLGActionComponent slgAction = FindObjectOfType<SLGActionComponent>();
            if (slgAction != null)
            {
                slgAction.MoveOnNextBuildingState(SLGBuildingType.MamagoCompany, SLGBuildingProgress.PlayCutScene);
            }
        }
        
        public void EndMamagoCutScene()
        {
            SLGActionComponent slgAction = FindObjectOfType<SLGActionComponent>();
            if (slgAction != null)
            {
                slgAction.MoveOnNextBuildingState(SLGBuildingType.MamagoCompany, SLGBuildingProgress.EndConstruct);
            }
        }

        public void EndCh1()
        {
            SLGActionComponent slgAction = FindObjectOfType<SLGActionComponent>();
            if (slgAction != null)
            {
                slgAction.MoveOnNextProgress();
            }

            Managers.Data.Chapter = 2;
            Managers.Data.SaveGame();
        }

        #region Mamago
        public void ActiveMamagoBubble()
        {
            _mamagoBubble.SetActive(true);
        }

        public void ConstructSFX()
        {
            Managers.Sound.Play(Sound.SFX, "CH1/Mamago_Construction_SFX");
        }

        public void CompleteSFX()
        {
            Managers.Sound.Play(Sound.SFX, "CH1/Mamago_Complete_SFX");
        }

        public void MamagoJump()
        {
            Vector3 nowPos = _mamago.transform.position;
            _mamago.transform.DOJump(nowPos, 0.3f, 1, 0.4f).SetEase(Ease.Linear);
        }

        public void MamagoMove1()
        {
            _mamago.Anim.SetAnimation(GlobalConst.MoveStr, Vector2.left);
            _mamago.transform.DOMove(_mamagoLocation[0], 2f).SetEase(Ease.Linear);

            StartCoroutine(nameof(RapleyGetOutMamago));
        }

        private IEnumerator RapleyGetOutMamago()
        {
            float posX = Player.transform.localPosition.x;
            float posY = Player.transform.localPosition.y;

            Debug.Log(Player.transform.localPosition);
            if (posX >= 90.2f && posX <= 94.5f && posY >= -15.5f && posY <= -14.6f)
            {
                Debug.Log("비켜!");
                Player.Animation.SetAnimation(GlobalConst.MoveStr, Vector2.down);
                Player.transform.DOMove(new Vector3(posX, -15.6f, 0), 0.3f).SetEase(Ease.Linear);

                yield return new WaitForSeconds(0.3f);

                Player.SetLastInput(Vector2.down);
                Player.Animation.SetAnimation(GlobalConst.IdleStr, Vector2.down);
            }
        }

        public void MamagoMove2()
        {
            _mamago.Anim.SetAnimation(GlobalConst.MoveStr, Vector2.up);
            _mamago.transform.DOMove(_mamagoLocation[1], 0.8f).SetEase(Ease.Linear);
        }

        public void MamagoEnter()
        {
            _mamago.gameObject.SetActive(false);
        }
        #endregion

        #region 3Match
        public void SetNpcPosition(int i)
        {
            switch(i)
            {
                case 0:
                    // 3매치 깬 후 동굴 앞
                    NpcPos.SetNpcPosition(6);
                    break;
                case 1:
                    // 씬4에서 맵3 처음 같이 갔을 때
                    NpcPos.SetNpcPosition(8);
                    break;
                default:
                    // Debug.Log("SLG 후 알투몬 위치 변경");
                    NpcPos.SetNpcPosition(i);
                    break;
            }
        }

        public void BreakBridge()
        {
            Managers.Sound.StopSFX();
            Managers.Sound.Play(Sound.SFX, "CH1/Explosion_SFX");
            _bridge.ActiveBrokenBridge();
        }

        public void ShakeMap(bool shake)
        {
            if (shake)
            {
                _shakeTween = DOTween.Sequence();
                _shakeTween.Append(_stage2.transform.DOShakePosition(5000f, new Vector3(0.1f, 0.1f, 0)));
                Managers.Sound.Play(Sound.SFX, "CH1/BeforeExplosion_SFX");
            }
            else
            {
                _shakeTween.Kill();
            }
        }
        #endregion

        #region Character Anim
        public void NpcJump(int idx)
        {
            Vector3 nowPos = _npc[idx].transform.position;

            _npc[idx].transform.DOJump(nowPos, 0.3f, 1, 0.4f).SetEase(Ease.Linear);
        }

        public void CharactersMove(int num)
        {
            switch (num)
            {
                case 1:
                    CharactersMove1();
                    break;
                case 2:
                    CharactersMove2();
                    break;
                case 3:
                    CharactersMove3();
                    break;
                case 4:
                    CharactersMove4();
                    break;
                default:
                    Debug.LogError("Invalid Move Number");
                    break;
            }
        }

        private void CharactersMove4()
        {
            // 라플리 빼고 맵3으로
            for (int i = 0; i < _npc.Length; i++)
            {
                _npc[i].Anim.SetAnimation(GlobalConst.MoveStr, Vector2.right);
                _npc[i].transform.DOMove(NpcPos.NpcLocations[i].Locations[7], 5f).SetEase(Ease.Linear);
            }
        }

        private void CharactersMove3()
        {
            // 라플리 빼고 다리 건너기
            for (int i = 0; i < _npc.Length; i++)
            {
                _npc[i].Anim.SetAnimation(GlobalConst.MoveStr, Vector2.right);
                _npc[i].transform.DOMove(NpcPos.NpcLocations[i].Locations[5], 5f).SetEase(Ease.Linear);
            }
        }

        private void CharactersMove2()
        {
            // 라플리 빼고 동굴 앞으로 이동
            for (int i = 0; i < _npc.Length; i++)
            {
                _npc[i].Anim.SetAnimation(GlobalConst.MoveStr, Vector2.right);
                _npc[i].transform.DOMove(NpcPos.NpcLocations[i].Locations[4], 5f).SetEase(Ease.Linear);
            }
        }

        private void CharactersMove1() // CH1 초입 변경하면서 0 삭제 후 1 수정
        {
            // 알투몬만 뛰어간다
            _npc[2].Anim.SetAnimation(GlobalConst.MoveStr, Vector2.left);
            _npc[2].transform.DOMove(NpcPos.NpcLocations[2].Locations[1], 2f).SetEase(Ease.Linear);
        }

        public void NpcIdleLeft(int idx)
        {
            _npc[idx].Anim.SetAnimation(GlobalConst.IdleStr, Vector2.left);
        }

        private void NpcIdleRight(int idx)
        {
            _npc[idx].Anim.SetAnimation(GlobalConst.IdleStr, Vector2.right);
        }

        private void NpcIdleUp(int idx)
        {
            _npc[idx].Anim.SetAnimation(GlobalConst.IdleStr, Vector2.up);
        }

        private void NpcIdleDown(int idx)
        {
            _npc[idx].Anim.SetAnimation(GlobalConst.IdleStr, Vector2.down);
        }

        public void CharactersStop(int num)
        {
            // TODO: 리팩터링 필요 너무 막 짬
            switch (num)
            {
                case 0:
                    PlayerIdleRight();
                    break;
                case 1: // 가운데
                    CharIdleCenter();
                    break;
                case 2: // 왼쪽
                    for (int i = 0; i < 3; i++)
                        NpcIdleLeft(i);
                    break;
                case 3: // 아래
                    for (int i = 0; i < 3; i++)
                        NpcIdleDown(i);
                    break;
                case 4: // 오른쪽
                    for (int i = 0; i < 3; i++)
                        NpcIdleRight(i);
                    break;
                default:
                    Debug.LogError("Invalid Move Number");
                    break;
            }
        }

        private void CharIdleCenter()
        {
            Player.Animation.SetAnimation(GlobalConst.IdleStr, Vector2.down);

            NpcIdleRight(0);
            NpcIdleUp(1);
            NpcIdleLeft(2);
        }

        public void PlayerIdleRight()
        {
            Player.Animation.SetAnimation(GlobalConst.IdleStr, Vector2.right);
        }

        public void SetPlayerPosition(int idx)
        {
            Player.transform.position = _location[idx];
        }
        #endregion

        #region Else
        public void ActiveVisualNovel()
        {
            Invoke(nameof(ActivePack), 3.5f);
        }

        private void ActivePack()
        {
            _visualNovel.SetActive(true);
        }

        public void GetVisualNovel()
        {
            _visualNovel.SetActive(false);
        }
        #endregion
    }
}