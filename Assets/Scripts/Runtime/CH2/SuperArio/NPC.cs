using System;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH2.SuperArio
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class NPC : MonoBehaviour
    {
        [SerializeField] private DialogueRunner _dialogueRunner;
        [SerializeField] private Sprite[] _sprites;
        private SpriteRenderer _spriteRenderer;
        
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            var parts = Managers.Data.CH2.ArioStage[0];
            switch (parts)
            {
                case '1':
                    _spriteRenderer.sprite = _sprites[0];
                    break;
                case '2':
                    _spriteRenderer.sprite = _sprites[1];
                    break;
                case '3':
                    _spriteRenderer.sprite = _sprites[2];
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out ArioReward ario))
            {
                _dialogueRunner.StartDialogue("NPC1");
            }
        }
    }
}