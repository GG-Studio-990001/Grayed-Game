using Runtime.ETC;
using System.Collections;
using UnityEngine;
using Sound = Runtime.ETC.Sound;

namespace Runtime.CH1.Pacmom
{
    public class PMController : MonoBehaviour
    {
        #region 선언
        private PMSprite _spriteController;
        private PMData _dataController;
        [SerializeField]
        private InGameDialogue _dialogue;
        [SerializeField]
        private Timer _timer;
        [SerializeField]
        private Door _door;

        [Header("=Character=")]
        [SerializeField]
        private Rapley _rapley;
        [SerializeField]
        private Pacmom _pacmom;
        [SerializeField]
        private Dust[] _dusts = new Dust[GlobalConst.DustCnt];
        private readonly DustRoom[] _dustRooms = new DustRoom[GlobalConst.DustCnt];

        public bool IsGameOver { get; private set; } = false;
        public bool IsVacuumMode { get; private set; } = false;
        private readonly float _vacuumDuration = 10f;
        private readonly float _vacuumEndDuration = 3f;
        private bool _isMoving;
        #endregion

        #region Awake
        private void Awake()
        {
            Managers.Data.LoadGame();
            Debug.Log("IsPacmomPlayed: " + Managers.Data.IsPacmomPlayed);
            Debug.Log("IsPacmomCleared: " + Managers.Data.IsPacmomCleared);

            AssignComponent();
            AssignController();
        }

        private void AssignComponent()
        {
            _spriteController = GetComponent<PMSprite>();
            _dataController = GetComponent<PMData>();

            for (int i = 0; i < GlobalConst.DustCnt; i++)
            {
                _dustRooms[i] = _dusts[i].GetComponent<DustRoom>();
            }
        }

        private void AssignController()
        {
            _dialogue.GameController = this;
            _timer.GameController = this;
            _pacmom.GameController = this;

            for (int i = 0; i < GlobalConst.DustCnt; i++)
            {
                _dusts[i].GameController = this;
                _dustRooms[i].GameController = this;
            }
        }
        #endregion

        #region Start
        private void Start()
        {
            SetCharacterMove(false);
        }

        public void StartGame()
        {
            ResetStates();
            SetCharacterMove(true);

            _timer.SetTimer(true);
        }
        #endregion

        #region End
        public void GameOver()
        {
            Managers.Sound.StopSFX();
            _timer.SetTimer(false);
            IsGameOver = true;

            SetCharacterMove(false);

            _dialogue.GameOverDialogue();
            _dataController.ChooseAWinner();
        }
        #endregion

        #region Vacuum Mode
        public void UseVacuum()
        {
            _dialogue.VacuumDialogue(IsVacuumMode);

            if (!IsVacuumMode)
            {
                Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_BGM_02");
            }
            else
            {
                StopCoroutine(nameof(VacuumTime));

                Managers.Sound.StopSFX();
                Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_BGM_01");
            }

            StartCoroutine(nameof(VacuumTime));
        }

        private IEnumerator VacuumTime()
        {
            StopCoroutine(nameof(DustExitRoomSoon));
            VacuumModeOn();

            yield return new WaitForSeconds(_vacuumDuration - _vacuumEndDuration);
            if (IsGameOver) yield break;

            _spriteController.SetPacmomBlinkSprite();

            yield return new WaitForSeconds(_vacuumEndDuration);
            if (IsGameOver) yield break;

            VacuumModeOff();
        }

        private void VacuumModeOn()
        {
            _spriteController.SetVacuumModeSprites();
            SetVacuumSpeed();
            SetVacuumMode(true);
        }

        private void VacuumModeOff()
        {
            _spriteController.SetNormalSprites();
            SetNormalSpeed();
            SetVacuumMode(false);

            DustExitRoom();
        }
        #endregion

        #region Common
        private void ResetStates()
        {
            _spriteController.SetNormalSprites();

            _rapley.Movement.ResetState();
            _pacmom.Movement.ResetState();

            for (int i = 0; i < GlobalConst.DustCnt; i++)
            {
                _dusts[i].Movement.ResetState();
                _dustRooms[i].SetInRoom(true);
            }

            DustExitRoom();
        }

        private void SetCharacterMove(bool move)
        {
            _rapley.Movement.SetCanMove(move);
            _pacmom.Movement.SetCanMove(move);
            for (int i = 0; i < GlobalConst.DustCnt; i++)
                _dusts[i].Movement.SetCanMove(move);

            _isMoving = move;
        }

        private void SetVacuumMode(bool isVacuumMode)
        {
            IsVacuumMode = isVacuumMode;
            _pacmom.VacuumMode(isVacuumMode);
            for (int i = 0; i < GlobalConst.DustCnt; i++)
            {
                _dusts[i].Movement.SetEyeNormal(!isVacuumMode);
            }

            _door.ActiveDoor(isVacuumMode);
        }

        private void SetVacuumSpeed()
        {
            _pacmom.Movement.SetSpeedMultiplier(1.2f);
            _rapley.Movement.SetSpeedMultiplier(0.7f);
            for (int i = 0; i < GlobalConst.DustCnt; i++)
                _dusts[i].Movement.SetSpeedMultiplier(0.7f);
        }

        private void SetNormalSpeed()
        {
            _pacmom.Movement.SetSpeedMultiplier(1f);
            _rapley.Movement.SetSpeedMultiplier(1f);
            for (int i = 0; i < GlobalConst.DustCnt; i++)
                _dusts[i].Movement.SetSpeedMultiplier(1f);
        }

        private void DustExitRoom()
        {
            int dustInRoom = (_dustRooms[0].IsInRoom ? 1 : 0) + (_dustRooms[1].IsInRoom ? 1 : 0);

            for (int i = 0; i < GlobalConst.DustCnt; i++)
            {
                if (_dustRooms[i].IsInRoom)
                {
                    _dustRooms[i].ExitRoom(GlobalConst.DustCnt - dustInRoom);
                    dustInRoom--;
                }
            }
        }
        #endregion

        #region Eaten
        public void RapleyEaten()
        {
            Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_SFX_06");

            _dataController.TakeHalfCoins();
            _rapley.Movement.ResetState();
        }

        public void DustEaten(Dust dust)
        {
            Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_SFX_06");

            dust.Movement.SetCanMove(false);
            dust.Movement.ResetState();
            dust.GetComponent<DustRoom>().SetInRoom(true);

            _dialogue.BeCaughtDialogue(dust.DustID);

            // TODO: 청소모드가 시작되면 이 코루틴은 취소
            if (!IsVacuumMode)
            {
                StartCoroutine(nameof(DustExitRoomSoon), dust);
            }
        }

        IEnumerator DustExitRoomSoon(Dust dust)
        {
            yield return new WaitForSeconds(2f);
            dust.GetComponent<DustRoom>().ExitRoom(dust.DustID);
        }

        public void DialogueStop()
        {
            _dialogue.StopDialogue();
        }

        public Vector3 GetPacmomPos()
        {
            return _pacmom.Movement.Rigid.position;
        }

        public void CoinEatenByRapley()
        {
            if (!_isMoving)
                return;

            Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_SFX_10");

            _dataController.RapleyScore1Up();

            if (!_dataController.HasRemainingCoins())
            {
                GameOver();
            }
        }

        public void CoinEatenByPacmom()
        {
            if (!_isMoving)
                return;

            Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_SFX_12");

            _dataController.PacmomScore1Up();

            if (!_dataController.HasRemainingCoins())
            {
                GameOver();
            }
        }
        #endregion
    }
}