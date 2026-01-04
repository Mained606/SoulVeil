namespace SoulVeil.Infrastructure.Save
{
    /// <summary>
    /// 세이브 데이터 버전
    /// - 세이브 구조가 바뀔 때마다 증가시킨다
    /// - 로드시 버전에 따라 마이그레이션을 수행한다
    /// </summary>
    public static class SaveVersion
    {
        public const int Current = 1;
    }
}