using Cinemachine;
using Runtime.InGameSystem;
using UnityEngine;

namespace Runtime.CH1.Main.Stage
{
    public class Ch1StageController : MonoBehaviour
    {
        [field:SerializeField] public CinemachineConfiner2D Confiner2D { get; private set; }
        public Ch1StageChanger StageChanger { get; private set; }
        
        [SerializeField] private Stage[] stages;
        private FadeController _fadeController;
        private Transform _playerTransform;

        [SerializeField] private Vector3[] _stageDefaultPosition;

        public Stage CurrentStage => StageChanger.CurrentStage;
        
        public void Init(FadeController fadeController, Transform playerTransform)
        {
            _fadeController = fadeController;
            _playerTransform = playerTransform;
            
            StageChangerInit();
            StageSettings();
        }

        private void StageChangerInit()
        {
            StageChanger = new Ch1StageChanger(_playerTransform, stages, _fadeController, Confiner2D);

            StageChanger.OnStageStart += () => Managers.Data.InGameKeyBinder.PlayerInputDisable();
            StageChanger.OnStageEnd += () => Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }

        private void StageSettings()
        {
            foreach (var stage in stages)
            {
                stage.StageSettings(StageChanger);
            }

            _ = StageChanger.SetStage(Managers.Data.CH1.Stage, _stageDefaultPosition[Managers.Data.CH1.Stage - 1]);

            #region
            //#if UNITY_EDITOR
            //            // 아래는 개발용
            //            // 유니티 에디터라면 하이어라키에서 활성화된 스테이지의 지정 위치에서 시작

            //            int firstStage = 0;
            //            foreach (var stage in stages)
            //            {
            //                // 가장 처음 활성화된 스테이지 선택
            //                if (stage.IsActivate())
            //                {
            //                    firstStage = stage.StageNumber;
            //                    break;
            //                }
            //            }

            //            // 나머지 스테이지 끄기
            //            foreach (var stage in stages)
            //            {
            //                if (stage.StageNumber != firstStage)
            //                    stage.Disable();
            //            }

            //            // 아무것도 안켜져있다면 스테이지1
            //            if (firstStage == 0)
            //                firstStage++;

            //            await StageChanger.SetStage(firstStage, _stageDefaultPosition[firstStage - 1]);
            //            return;
            //#else
            // 빌드라면 저장된 스테이지의 지정 위치에서 시작
            // SetStage(Managers.Data.Stage, _stageDefaultPosition[Managers.Data.Stage - 1]);
            //#endif
            #endregion
        }

        //public void SetStage(int moveStageNumber, Vector2 spawnPosition)
        //{
        //    Debug.Log("SetStage" + moveStageNumber);

        //    foreach (var stage in stages)
        //    {
        //        stage.Disable();
        //    }

        //    stages[moveStageNumber - 1].Enable();
        //    _playerTransform.position = spawnPosition;
        //    Confiner2D.m_BoundingShape2D = stages[moveStageNumber - 1].GetStageCollider();

        //    Managers.Data.Stage = moveStageNumber;
        //    // Managers.Data.SaveGame();
        //}
    }
}
