using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using SoulVeil.Core.Diagnostics;
using SoulVeil.Gameplay.Cards.Data;
using SoulVeil.Gameplay.Skins.Data;

namespace SoulVeil.Infrastructure.StaticData
{
    /// <summary>
    /// Addressables 기반 정적 데이터 서비스
    /// - Boot 단계에서 Catalog를 로드하고 인덱싱해 둔다
    /// </summary>
    public sealed class AddressablesStaticDataService : IStaticDataService
    {
        private readonly ILogService logService;

        private AsyncOperationHandle<CardCatalog> cardCatalogHandle;
        private AsyncOperationHandle<SkinCatalog> skinCatalogHandle;

        private CardCatalog cardCatalog;
        private SkinCatalog skinCatalog;

        public bool IsLoaded { get; private set; }

        public AddressablesStaticDataService(ILogService logService)
        {
            this.logService = logService;
        }

        /// <summary>
        /// Boot에서 코루틴으로 호출하는 로드 루틴
        /// </summary>
        public IEnumerator LoadAll()
        {
            IsLoaded = false;

            cardCatalogHandle = Addressables.LoadAssetAsync<CardCatalog>(StaticDataKeys.CardCatalogAddress);
            yield return cardCatalogHandle;

            if (cardCatalogHandle.Status != AsyncOperationStatus.Succeeded || cardCatalogHandle.Result == null)
            {
                logService.Error($"CardCatalog 로드 실패: {StaticDataKeys.CardCatalogAddress}");
                yield break;
            }

            cardCatalog = cardCatalogHandle.Result;
            cardCatalog.BuildIndex();

            skinCatalogHandle = Addressables.LoadAssetAsync<SkinCatalog>(StaticDataKeys.SkinCatalogAddress);
            yield return skinCatalogHandle;

            if (skinCatalogHandle.Status != AsyncOperationStatus.Succeeded || skinCatalogHandle.Result == null)
            {
                logService.Error($"SkinCatalog 로드 실패: {StaticDataKeys.SkinCatalogAddress}");
                yield break;
            }

            skinCatalog = skinCatalogHandle.Result;
            skinCatalog.BuildIndex();

            IsLoaded = true;
            logService.Info("StaticData loaded. CardCatalog/SkinCatalog ready.");
        }

        public CardDefinition GetCard(string cardId)
        {
            if (!IsLoaded || cardCatalog == null)
                throw new System.InvalidOperationException("StaticData가 로드되지 않았습니다.");

            if (cardCatalog.TryGet(cardId, out CardDefinition definition))
                return definition;

            throw new System.Collections.Generic.KeyNotFoundException($"존재하지 않는 cardId: {cardId}");
        }

        public SkinDefinition GetSkin(string skinId)
        {
            if (!IsLoaded || skinCatalog == null)
                throw new System.InvalidOperationException("StaticData가 로드되지 않았습니다.");

            if (skinCatalog.TryGet(skinId, out SkinDefinition definition))
                return definition;

            throw new System.Collections.Generic.KeyNotFoundException($"존재하지 않는 skinId: {skinId}");
        }

        public void Release()
        {
            // 추후 App 종료/리셋 정책에서 호출할 수 있게 둔다
            if (cardCatalogHandle.IsValid()) Addressables.Release(cardCatalogHandle);
            if (skinCatalogHandle.IsValid()) Addressables.Release(skinCatalogHandle);

            cardCatalog = null;
            skinCatalog = null;
            IsLoaded = false;
        }
    }
}
