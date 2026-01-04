using System.Collections.Generic;
using UnityEngine;

namespace SoulVeil.Gameplay.Cards.Data
{
    /// <summary>
    /// 카드 정의 데이터 집합(Catalog)
    /// - Addressables로 로드되는 "정적 데이터 엔트리" 역할
    /// - 직렬화는 List로, 런타임 조회는 Dictionary로 인덱싱한다
    /// </summary>
    [CreateAssetMenu(menuName = "SoulVeil/Cards/Card Catalog")]
    public sealed class CardCatalog : ScriptableObject
    {
        [SerializeField] private List<CardDefinition> cards = new List<CardDefinition>();

        private Dictionary<string, CardDefinition> lookup;

        public void BuildIndex()
        {
            lookup = new Dictionary<string, CardDefinition>(cards.Count);

            for (int i = 0; i < cards.Count; i++)
            {
                CardDefinition definition = cards[i];
                if (definition == null) continue;

                string id = definition.CardId;
                if (string.IsNullOrWhiteSpace(id)) continue;

                // 중복 ID는 데이터 오류이므로 마지막 것이 덮어씌우지 않게 한다
                if (lookup.ContainsKey(id)) continue;

                lookup.Add(id, definition);
            }
        }

        public bool TryGet(string cardId, out CardDefinition definition)
        {
            if (lookup == null) BuildIndex();
            return lookup.TryGetValue(cardId, out definition);
        }
    }
}