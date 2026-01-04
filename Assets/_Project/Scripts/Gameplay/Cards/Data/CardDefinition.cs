using System;
using UnityEngine;

namespace SoulVeil.Gameplay.Cards.Data
{
    /// <summary>
    /// 카드의 정적 정의 데이터
    /// - 런타임 소유/강화/장착 상태는 SaveData(런타임 데이터)에 존재해야 한다
    /// </summary>
    [CreateAssetMenu(menuName = "SoulVeil/Cards/Card Definition")]
    public sealed class CardDefinition : ScriptableObject
    {
        [SerializeField] private string cardId;
        [SerializeField] private string displayName;

        public string CardId => cardId;
        public string DisplayName => displayName;

        private void OnValidate()
        {
            // 카드 ID는 공백이면 안 된다(상용화에서 데이터 문제 조기 발견)
            if (!string.IsNullOrWhiteSpace(cardId)) return;

            // OnValidate에서 Debug.LogWarning은 에디터 편의 기능이다
            Debug.LogWarning($"{name}: cardId가 비어있습니다.");
        }
    }
}