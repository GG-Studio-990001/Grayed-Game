using System;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;

namespace Runtime.CH2.SuperArio
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class NPC : MonoBehaviour
    {
        [SerializeField] private DialogueRunner _dialogueRunner;
        [SerializeField] private Sprite[] _npcSprites;
        [SerializeField] private Sprite[] _itemSprites;
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer _itemSpriteRenderer;
        private char _parts;
        
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _itemSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

            _parts = Managers.Data.CH2.ArioStage[0];
            switch (_parts)
            {
                case '1':
                    _spriteRenderer.sprite = _npcSprites[0];
                    _itemSpriteRenderer.sprite = _itemSprites[0];
                    break;
                case '2':
                    _spriteRenderer.sprite = _npcSprites[1];
                    _itemSpriteRenderer.sprite = _itemSprites[1];
                    break;
                case '3':
                    _spriteRenderer.sprite = _npcSprites[2];
                    _itemSpriteRenderer.sprite = _itemSprites[2];
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out ArioReward ario))
            {
                switch (_parts)
                {
                    case '1':
                        _dialogueRunner.StartDialogue("NPC1");
                        break;
                    case '2':
                        _dialogueRunner.StartDialogue("NPC2");
                        break;
                    case '3':
                        _dialogueRunner.StartDialogue("NPC3");
                        break;
                }
            }
        }
    }
}