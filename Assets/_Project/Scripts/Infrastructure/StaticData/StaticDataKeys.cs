namespace SoulVeil.Infrastructure.StaticData
{
    /// <summary>
    /// Addressables로 로드할 정적 데이터 주소 키
    /// - 문자열 산재를 방지하기 위해 한 곳에서 관리한다
    /// </summary>
    public static class StaticDataKeys
    {
        public const string CardCatalogAddress = "StaticData/CardCatalog";
        public const string SkinCatalogAddress = "StaticData/SkinCatalog";
    }
}