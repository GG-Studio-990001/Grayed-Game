using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.CH2.SuperArio
{
    public class StoreWall : MonoBehaviour
    {
        [field: SerializeField] public bool IsLeft { get; private set; }
        [SerializeField] private bool canOpen = false;
        private Animator _animator;
        private Animator _childAnimator;
        private BoxCollider2D _col;
        private BoxCollider2D _childCol;
        private void Start()
        {
            if (!canOpen)
                return;
            _animator = GetComponent<Animator>();
            _col = GetComponent<BoxCollider2D>();

            if (transform.childCount > 0)
            {
                _childAnimator = transform.GetChild(0).GetComponent<Animator>();
                _childCol = transform.GetChild(0).GetComponent<BoxCollider2D>();
            }
        }

        public void OpenWall()
        {
            StartCoroutine(OpenWallCoroutine());
        }

        private IEnumerator OpenWallCoroutine()
        {
            _animator.enabled = true;
            yield return new WaitForSeconds(0.5f);
            _childAnimator.enabled = true;
            _col.enabled = false;
            _childCol.enabled = false;
        }
        public void CloseWall()
        {
            _animator.Rebind();
            _childAnimator.Rebind();
            _animator.enabled = false;
            _childAnimator.enabled = false;
            _col.enabled = true;
            _childCol.enabled = true;
        }
    }
}
