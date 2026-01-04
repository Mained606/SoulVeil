using SoulVeil.Gameplay.Cards.Data;
using SoulVeil.Gameplay.Skins.Data;

namespace SoulVeil.Infrastructure.StaticData
{
    /// <summary>
    /// 정적 데이터 접근 계약
    /// - 카드/스킨 등 "정의 데이터" 조회를 제공한다
    /// </summary>
    public interface IStaticDataService
    {
        bool IsLoaded { get; }

        CardDefinition GetCard(string cardId);
        SkinDefinition GetSkin(string skinId);
    }
}