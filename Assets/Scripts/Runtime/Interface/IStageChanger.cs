using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.Interface
{
    public interface IStageChanger
    {
        public Task SwitchStage(int moveStageNumber, Vector2 spawnPosition);
        public Action OnStageStart { get; set; }
        public Action OnStageEnd { get; set; }
    }
}