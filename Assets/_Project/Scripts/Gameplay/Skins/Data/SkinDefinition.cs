using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoulVeil.Gameplay.Skins.Data
{
    /// <summary>
    /// 스킨의 정적 정의 데이터
    /// - 외형 리소스는 Addressables AssetReference로 참조한다
    /// </summary>
    [CreateAssetMenu(menuName = "SoulVeil/Skins/Skin Definition")]
    public sealed class SkinDefinition : ScriptableObject
    {
        [SerializeField] private string skinId;
        [SerializeField] private string displayName;

        // 예: 캐릭터 외형 프리팹(혹은 Mesh/Material로 쪼개도 됨)
        [SerializeField] private AssetReferenceGameObject skinPrefab;

        public string SkinId => skinId;
        public string DisplayName => displayName;
        public AssetReferenceGameObject SkinPrefab => skinPrefab;
    }
}