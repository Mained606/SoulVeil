namespace SoulVeil.Infrastructure.Save
{
    /// <summary>
    /// 세이브 서비스 계약
    /// - 로드/저장/리셋을 표준화한다
    /// - 구현체(로컬/클라우드/암호화)를 교체하기 쉽게 인터페이스로 둔다
    /// </summary>
    public interface ISaveService
    {
        PlayerSaveData Current { get; }
        bool IsLoaded { get; }

        void Load();
        void Save();
        void ResetToDefault();
    }
}