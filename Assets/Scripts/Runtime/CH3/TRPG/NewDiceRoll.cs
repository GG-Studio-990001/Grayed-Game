using UnityEngine;
using DG.Tweening; // DOTween 필요

namespace Runtime.CH3.TRPG
{
    public class NewDiceRoll : MonoBehaviour
    {
        private Vector3 _initPos;


        // 원하는 값 (예: 1~10)
        public int targetValue = 1;

        // 각 눈마다 대응되는 회전값(위로 오는 회전)
        [SerializeField] private Vector3[] faceRotations;

        // 착지 반경 관련
        [SerializeField] private Transform center;
        [SerializeField] private float radius;

        // 굴림 지속 시간
        [SerializeField] private float rollDuration = 0.5f;

        private void Awake()
        {
            _initPos = transform.position;
        }

        public void RollDice()
        {
            // 초기화
            transform.position = _initPos;
            transform.rotation = Random.rotation;

            // 목표 위치 계산
            Vector2 circle = Random.insideUnitCircle * radius;
            Vector3 targetPos = center.position + new Vector3(circle.x, 0, circle.y);

            // 목표 회전
            Quaternion targetRot = Quaternion.Euler(faceRotations[targetValue - 1]);

            // DOTween으로 자연스럽게 이동/회전
            transform.DOMove(targetPos, rollDuration).SetEase(Ease.Linear);
            transform.DORotateQuaternion(targetRot, rollDuration).SetEase(Ease.Linear);
        }
    }
}
