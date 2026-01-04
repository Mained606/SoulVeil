using System;
using UnityEngine;
using SoulVeil.Core.Data;     // StatCardSO
using SoulVeil.Core.EventBus; // GameEventBus

namespace SoulVeil.Gameplay.Player.Data
{
    [Serializable]
    public class PlayerStats
    {
        // ==========================================
        // 데이터 소스 (Data Sources)
        // ==========================================
        [Header("Sources")]
        [SerializeField] private StatCardSO baseStats;       // 기본 스탯 (맨몸)
        
        // 장착된 카드는 읽기 전용으로 공개 (UI 등에서 참조)
        public StatCardSO EquippedStatCard { get; private set; }

        // ==========================================
        // 계산된 프로퍼티 (Derived Stats)
        // ==========================================
        // 호출될 때마다 베이스 + 장비 카드를 합산하여 반환
        public int TotalStrength => GetStatValue(s => s.strength);
        public int TotalAgility => GetStatValue(s => s.agility);
        public int TotalIntelligence => GetStatValue(s => s.intelligence);
        public int TotalVitality => GetStatValue(s => s.vitality);
        public int MaxHealth => GetStatValue(s => s.vitality) * 10; // 체력 공식 예시: Vitality * 10

        public ElementType AttackElement => EquippedStatCard != null ? EquippedStatCard.attackElement : ElementType.None;

        // ==========================================
        // 로직 및 메서드
        // ==========================================
        
        // 반복되는 스탯 합산 로직을 줄이기 위한 헬퍼 함수
        private int GetStatValue(Func<StatCardSO, int> selector)
        {
            int baseVal = baseStats != null ? selector(baseStats) : 0;
            int equipVal = EquippedStatCard != null ? selector(EquippedStatCard) : 0;
            return baseVal + equipVal;
        }

        /// <summary>
        /// 초기 베이스 스탯 설정 (게임 시작 시 Controller가 호출)
        /// </summary>
        public void SetBaseStats(StatCardSO defaultStats)
        {
            baseStats = defaultStats;
        }

        /// <summary>
        /// 카드 장착
        /// </summary>
        public void EquipCard(StatCardSO newCard)
        {
            EquippedStatCard = newCard;
            
            // 스탯이 변경되었음을 시스템 전파 (UI 갱신, 체력통 업데이트 등)
            GameEventBus.Publish(new PlayerStatsChangedEvent());
            
            Debug.Log($"[PlayerStats] === 장비 교체: {newCard.displayName} ===");
            Debug.Log($" - 힘(STR): {TotalStrength}");
            Debug.Log($" - 민첩(AGI): {TotalAgility}");
            Debug.Log($" - 지능(INT): {TotalIntelligence}");
            Debug.Log($" - 체력(VIT): {TotalVitality} -> MaxHP: {MaxHealth}");
        }
    }
}