using Runtime.ETC;
using UnityEngine;
using DG.Tweening;

namespace Runtime.CH2.SuperArio
{
    public class Ario_Opening : MonoBehaviour
    {
        [SerializeField] private Mario_Opening _mario;
        [SerializeField] private Transform[] _targets;
        [SerializeField] private GameObject _txtBallon;
        
        private Animator _ani;
        private SpriteRenderer _spr;
        
        private void Start()
        {
            if (ArioManager.Instance != null && ArioManager.Instance.SkipOpening)
                return;
            _spr = GetComponent<SpriteRenderer>();
            _ani = GetComponent<Animator>();
            _ani.enabled = false;
        }
        
        private void PlayAnimation()
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

        public void Up()
        {
            Sequence sequence = DOTween.Sequence();
            
            sequence.Append(transform.DOMoveY(transform.position.y + 0.725f, 1f).SetEase(Ease.Linear))
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    Managers.Sound.Play(Sound.SFX, "SuperArio/Opening/CH2_SUB_SFX_06");
                    // 말풍선
                    _txtBallon.SetActive(true);
                })
                .AppendInterval(1f)
                .OnComplete(() =>
                {
                    // 말풍선
                    _txtBallon.SetActive(false);
                    _ani.enabled = true;;
                    Drop(_targets[0].position + Vector3.down * 0.05f);
                });
        }
        
        private void Drop(Vector3 point)
        {
            Managers.Sound.Play(Sound.SFX, "SuperArio/Opening/CH2_SUB_SFX_08_1");
            //_ani.SetBool("Run", true);
            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(transform.DOJump(point, 2f,1,0.5f).SetEase(Ease.Linear))
                .AppendCallback(() =>
                {
                    Managers.Sound.Play(Sound.SFX, "SuperArio/Opening/CH2_SUB_SFX_08_2");
                })
                .AppendInterval(1f)
                .OnComplete(() =>
                {
                    Move(_targets[1].position);
                });
        }

        private void Move(Vector2 point)
        {
            Managers.Sound.Play(Sound.SFX, "SuperArio/Opening/CH2_SUB_SFX_09");
            Sequence sequence = DOTween.Sequence();
            sequence.Join(transform.DOMoveX(point.x, 0.75f).SetEase(Ease.Linear))
                .AppendInterval(1f)
                .OnComplete(() =>
                {
                    _mario.DropAndMove(_targets[0].position, _targets[1].position);
                });
        }
    }
}