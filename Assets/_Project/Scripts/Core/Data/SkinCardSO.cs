using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoulVeil.Core.Data
{
    [CreateAssetMenu(fileName = "NewSkinCard", menuName = "SoulVeil/Cards/Skin Card")]
    public class SkinCardSO : BaseCardSO
    {
        [Header("Visual Resources")]
        // AssetReference를 사용하여 씬에 바로 로드하지 않고 주소값만 들고 있음
        // 필요할 때 비동기(Async)로 로드하여 메모리 절약
        public AssetReferenceGameObject modelPrefab; 
        
        // 애니메이터 컨트롤러도 교체해야 한다면 참조 (예: 4족 보행 몬스터 등)
        public AssetReference runtimeAnimatorController;
    }
}