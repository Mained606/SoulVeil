using UnityEngine;

namespace SoulVeil.Core.Data
{
    // 속성 타입 정의 (이 파일 내에 함께 두거나, 별도 Enums.cs로 빼도 됩니다)
    public enum ElementType 
    { 
        None, 
        Fire, 
        Water, 
        Earth, 
        Wind,
        Light,
        Dark,
        Holy,
        Poison
    }

    [CreateAssetMenu(fileName = "NewStatCard", menuName = "SoulVeil/Cards/Stat Card")]
    public class StatCardSO : BaseCardSO
    {
        [Header("Combat Stats")]
        public int strength;      // 근력 (물리 공격력)
        public int agility;       // 민첩 (이동 속도, 회피율)
        public int intelligence;  // 지능 (마법 공격력, 쿨타임)
        public int vitality;      // 체력 (최대 HP)

        [Header("Elemental")]
        public ElementType attackElement; // 공격 속성
    }
}