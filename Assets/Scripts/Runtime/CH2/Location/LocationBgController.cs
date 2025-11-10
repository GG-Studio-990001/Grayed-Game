using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH2.Location
{
    public class LocationBgController : MonoBehaviour
    {
        [SerializeField] private Image _bgImg;
        [SerializeField] private TextMeshProUGUI _locationTxt;
        [SerializeField] private Sprite[] _bgSprites;
        [SerializeField] private Sprite[] _extraBgSprites;
        public Dictionary<string, string> LocationTexts = new Dictionary<string, string>
        {
            { "Entrance", "마을 입구" },
            { "Square", "광장" },
            { "Temple", "신전" },
            { "Statue", "달러 동상" },
            { "Cave", "동굴" },
            { "Base", "기지" },
            { "Storage", "창고 앞" },
            { "InStorage", "창고 내부" },
            { "InTemple", "신전 내부" },
            { "TempleRoom", "신전 방" },
            { "Backstreet", "골목길" },
            { "DollarHouse", "달러의 집" },
            { "Passage", "통로" },
            { "InStatue", "달러 동상 안" },
            { "StatueCrack", "달러 동상 틈" }
        };

        private Dictionary<string, int> _locationSprites = new Dictionary<string, int>
        {
            { "Entrance", 0 },        // 마을 입구
            { "Square", 1 },          // 광장
            { "Temple", 2 },          // 신전
            { "Statue", 3 },          // 달러 동상 => 창고 앞으로 대체
            { "Cave", 4 },            // 동굴
            { "Base", 5 },            // 기지
            { "Storage", 6 },         // 창고
            { "InStorage", 7 },      // 창고 내부
            { "InTemple", 8 },       // 신전 내부 => 신전 방만 사용
            { "TempleRoom", 9 },     // 신전 방
            { "Backstreet", 10 },      // 골목길
            { "DollarHouse", 11 },    // 달러의 집 => 창고 앞으로 대체
            { "Passage", 12 },        // 통로 => 수정된 이미지로 대체
            { "InStatue", 13 },      // 달러 동상 내부 => 창고 안으로 대체
            { "StatueCrack", 14 }     // 달러 동상 틈새 => 수정된 이미지로 대체
        };

        public void TempleRoomCarpetOpen()
        {
            _bgSprites[9] = _extraBgSprites[0];
            _bgImg.sprite = _bgSprites[9];
        }

        public void TempleClean()
        {
            _bgSprites[9] = _extraBgSprites[1];
            _bgSprites[2] = _extraBgSprites[2];
        }

        public void SetLocationUI()
        {
            string loc = Managers.Data.CH2.Location;

            if (LocationTexts.TryGetValue(loc, out string locationText))
            {
                _locationTxt.text = locationText;
            }
        }

        public void SetBG()
        {
            string loc = Managers.Data.CH2.Location;

            if (_locationSprites.TryGetValue(loc, out int spriteIndex) && spriteIndex < _bgSprites.Length)
            {
                _bgImg.sprite = _bgSprites[spriteIndex];
            }
        }
    }
}