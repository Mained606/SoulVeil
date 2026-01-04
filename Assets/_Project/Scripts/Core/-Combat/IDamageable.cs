namespace SoulVeil.Core.Combat
{
    // 나중에 몬스터, 플레이어, 나무상자 등 모든 '맞는 존재'들이 상속받을 인터페이스
    public interface IDamageable
    {
        void TakeDamage(int damage);
        bool IsDead { get; }
        // 필요하다면 Heal, MaxHealth 등 추가
    }
}