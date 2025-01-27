using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CH2.Dialogue
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] private List<Character> _characters = new();
        [SerializeField] private float _leftX; // 왼쪽일 때 x값
        [SerializeField] private float _rightX; // 오른쪽일 때 x값

        public void SetCharacterPos(string standing, string pos)
        {
            // Debug.Log($"SetCharacterPos called with standing: {standing}, pos: {pos}"); // 디버깅용 로그

            foreach (var character in _characters)
            {
                // Debug.Log($"Checking character: {character.CharacterName}"); // 추가

                if (character.CharacterName == standing)
                {
                    character.gameObject.SetActive(true);

                    float xPos = pos.Equals("A") ? _leftX : _rightX;
                    character.transform.localPosition = new Vector3(xPos, character.transform.localPosition.y, character.transform.localPosition.z);

                    return;
                }
            }

            Debug.LogError("캐릭터 못찾음");
        }


        public void HideCharacter(string standing)
        {
            foreach (var character in _characters)
            {
                if (character.CharacterName == standing)
                {
                    character.gameObject.SetActive(false);
                    return;
                }
            }

            Debug.LogError("캐릭터 못찾음");
        }

        public void SetCharacterExpression(string standing, int Index)
        {
            foreach (var character in _characters)
            {
                if (character.CharacterName == standing)
                {
                    character.SetExpression(Index);
                    return;
                }
            }

            Debug.LogError("캐릭터 못찾음");
        }

        public void HighlightSpeakingCharacter(string speaker)
        {
            foreach (var character in _characters)
            {
                if (!character.gameObject.activeInHierarchy) continue;

                if (character.CharacterName == speaker)
                    character.SetHighlight(true);
                else
                    character.SetHighlight(false);
            }
        }
    }
}