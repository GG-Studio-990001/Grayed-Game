using Runtime.ETC;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Runtime.CH2.SuperArio
{
    public class Mario_Opening : MonoBehaviour
    {
        [SerializeField] private Transform[] targets;
        [SerializeField] private Sprite jumpSprite;
        [SerializeField] private Ario_Opening arioOpening;
        [SerializeField] private GameObject _camera;
        private float _jumpHeight = 1;
        private float _jumpSpeed = 7;
        private bool _isJump;
        private bool _isTop;

        private Vector2 _startPos;
        private Animator _ani;
        private SpriteRenderer _spr;
        private Sprite initSprite;

        private void Start()
        {
            _ani = GetComponent<Animator>();
            _spr = GetComponent<SpriteRenderer>();
            initSprite = _spr.sprite;
            _startPos = transform.position;

            Move(targets[0].position);
        }

        public void PauseAnimation()
        {
            if (_ani != null)
            {
                _ani.ResetTrigger("Jump");
                _ani.SetBool("Run", false);
                _ani.speed = 0;
            }
        }

        public void PlayAnimation()
        {
            if (_ani != null)
            {
                _ani.enabled = true;
                _ani.speed = 1;
            }
        }

        private void StopAnimation()
        {
            if (_ani != null)
            {
                _ani.speed = 0;
                _ani.enabled = false;
            }
        }

        private void Move(Vector2 point)
        {
            _ani.SetBool("Run", true);
            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(transform.DOMove(point, 1f).SetEase(Ease.Linear))
                .AppendCallback(() =>
                {
                    _ani.SetBool("Run", false);
                })
                .AppendInterval(1f)
                .OnComplete(() =>
                {
                    _spr.sprite = jumpSprite;
                    Jump(targets[1].position);
                    StopAnimation();
                });
        }

        private void Jump(Vector2 point)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMoveY(point.y - 0.55f, 0.5f).SetEase(Ease.Linear));
            sequence.Join(transform.DOMoveX(point.x - 0.55f, 0.5f).SetEase(Ease.Linear))
                .OnComplete(() => arioOpening.Up());
        }
        
        public void DropAndMove(Vector2 dropPoint, Vector2 movePoint)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMoveY(dropPoint.y, 0.25f).SetEase(Ease.Linear));
            sequence.Join(transform.DOMoveX(dropPoint.x-1.5f, 0.25f).SetEase(Ease.Linear))
                .AppendCallback(() =>
                {
                    _spr.sprite = initSprite;
                    PlayAnimation();
                    _ani.SetBool("Run", false);
                    _ani.ResetTrigger("Jump");
                })
                .AppendInterval(1f)
                .AppendCallback(() => _ani.SetBool("Run", true))
                .Append(transform.DOMoveX(movePoint.x, 0.5f).SetEase(Ease.Linear))
                .AppendInterval(1f)
                .OnComplete(() =>
                {
                    // 카메라 끄기
                    _camera.SetActive(false);
                    ArioManager.Instance.WaitStart();
                });
        }
    }
}