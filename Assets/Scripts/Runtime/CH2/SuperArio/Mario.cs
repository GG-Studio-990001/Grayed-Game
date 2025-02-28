using Runtime.ETC;
using System.Collections;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class Mario : MonoBehaviour
    {
        [SerializeField] private Sprite sitSprite;
        private float _jumpHeight = 1;
        private float _jumpSpeed = 7;
        private bool _isJump;
        private bool _isTop;

        private Vector2 _startPos;
        private Animator _ani;
        private SpriteRenderer _spr;
        private Sprite initSprite;
        private static readonly int Jump1 = Animator.StringToHash("Jump");

        private void Start()
        {
            _ani = GetComponent<Animator>();
            _spr = GetComponent<SpriteRenderer>();
            initSprite = _spr.sprite;
            _startPos = transform.position;
            ArioManager.instance.OnPlay += InitData;
        }
        
        private void OnDestroy()
        {
            ArioManager.instance.OnPlay -= InitData;
        }

        private void Update()
        {
            if (_isJump)
            {
                if (transform.position.y <= _jumpHeight - 0.1f && !_isTop)
                {
                    transform.position = Vector2.Lerp(transform.position,
                        new Vector2(transform.position.x, _jumpHeight), _jumpSpeed * Time.deltaTime);
                }
                else
                {
                    _isTop = true;
                }

                if (transform.position.y > _startPos.y && _isTop)
                {
                    transform.position = Vector2.MoveTowards(transform.position, _startPos, _jumpSpeed * Time.deltaTime);
                }
            }
        }

        private IEnumerator Jump()
        {
            if (_isJump)
                yield break;
            
            _isJump = true;
            Debug.Log("Jump");
            _ani.SetTrigger("Jump");
            yield return new WaitForSeconds(0.65f);
            _isJump = false;
            _isTop = false;
            transform.position = _startPos;
        }

        private IEnumerator Sit()
        {
            _ani.enabled = false;
            _spr.sprite = sitSprite;
            yield return new WaitForSeconds(0.65f);
            _ani.enabled = true;
            _spr.sprite = initSprite;
        }

        private void InitData(bool isPlay)
        {
            transform.position = _startPos;
            _spr.sprite = initSprite;
            _isJump = false;
            _isTop = false;
            
            if (isPlay)
            {
                _ani.enabled = true;
            }
            else
            {
                _ani.enabled = false;
                StopAllCoroutines();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GlobalConst.ObstacleStr) && ArioManager.instance.IsPlay)
            {
                var isSit = other.GetComponent<ObstacleBase>().isSitObstacle;
                StartCoroutine(!isSit ? Jump() : Sit());
            }
        }
    }
}