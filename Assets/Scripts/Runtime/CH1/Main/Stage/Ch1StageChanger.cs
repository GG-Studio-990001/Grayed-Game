using Cinemachine;
using Runtime.InGameSystem;
using Runtime.Interface;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.CH1.Main.Stage
{
    public class Ch1StageChanger : IStageChanger
    {
        private readonly Transform _player;
        private readonly Stage[] _stages;
        private readonly FadeController _fadeController;
        private Stage _currentStage;
        private CinemachineConfiner2D _confiner2D;
        
        public Stage CurrentStage => _currentStage;
        public Action OnStageStart { get; set; }
        public Action OnStageEnd { get; set; }
        
        public Ch1StageChanger(Transform player, Stage[] stages, FadeController fadeController, CinemachineConfiner2D confiner2D)
        {
            _player = player;
            _stages = stages;
            _fadeController = fadeController;
            _confiner2D = confiner2D;
        }
        
        public async Task SwitchStage(int moveStageNumber, Vector2 spawnPosition)
        {
            OnStageStart?.Invoke();

            if (_fadeController is not null)
                _fadeController.StartFadeOut();

            await Task.Delay(1000); // temp
            
            _currentStage?.Disable();
            
            _currentStage = _stages[moveStageNumber - 1];
            
            _player.transform.position = spawnPosition;
            
            _currentStage.Enable();
            _confiner2D.m_BoundingShape2D = _currentStage.GetStageCollider();
            
            await Task.Delay(1000);
            
            if (_fadeController is not null)
                _fadeController.StartFadeIn();

            OnStageEnd?.Invoke();
            
            return;
        }
    }
}