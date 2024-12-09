using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class ObstacleBase : MonoBehaviour
    {
        public bool isSitObstacle;
        [SerializeField] private bool _isChild;
        [SerializeField] private Vector2 _startPos;
        [SerializeField] private float _endPos;

        private List<ArioCoin> _coins;

        private void Awake()
        {
            _coins = GetComponentsInChildren<ArioCoin>(true).ToList();
        }

        private void OnEnable()
        {
            _coins.ForEach(x => x.RandomCoin());
            if (!_isChild)
                transform.position = _startPos;
        }

        private void OnDisable()
        {
            _coins.ForEach(x =>
            {
                if(x.gameObject.activeSelf)
                    x.gameObject.SetActive(false);
            });
        }

        private void Update()
        {
            if (!_isChild)
            {
                if (ArioManager.instance.IsPlay)
                    transform.Translate(Vector2.left * Time.deltaTime * ArioManager.instance.GameSpeed);

                if (transform.position.x < _endPos)
                    gameObject.SetActive(false);
            }
        }
    }
}