using UnityEngine;

namespace SoulVeil.Core.Data
{
    public abstract class BaseCardSO : ScriptableObject
    {
        [Header("Common Info")] 
        public string id;           // 데이터 식별용 ID (예: "card_slime_01")
        public string displayName;  // 인게임 표시 이름 (예: "Slime Soul")
        
        [TextArea]
        public string description;  // 툴팁 등에서 보여줄 설명
        
        public Sprite icon;         // UI 아이콘
    }
}
