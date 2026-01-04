using System.Collections.Generic;
using UnityEngine;

namespace SoulVeil.Gameplay.Skins.Data
{
    /// <summary>
    /// 스킨 정의 데이터 집합(Catalog)
    /// </summary>
    [CreateAssetMenu(menuName = "SoulVeil/Skins/Skin Catalog")]
    public sealed class SkinCatalog : ScriptableObject
    {
        [SerializeField] private List<SkinDefinition> skins = new List<SkinDefinition>();

        private Dictionary<string, SkinDefinition> lookup;

        public void BuildIndex()
        {
            lookup = new Dictionary<string, SkinDefinition>(skins.Count);

            for (int i = 0; i < skins.Count; i++)
            {
                SkinDefinition definition = skins[i];
                if (definition == null) continue;

                string id = definition.SkinId;
                if (string.IsNullOrWhiteSpace(id)) continue;

                if (lookup.ContainsKey(id)) continue;

                lookup.Add(id, definition);
            }
        }

        public bool TryGet(string skinId, out SkinDefinition definition)
        {
            if (lookup == null) BuildIndex();
            return lookup.TryGetValue(skinId, out definition);
        }
    }
}