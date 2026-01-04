using UnityEngine;
using UnityEngine.InputSystem; // 뉴 인풋 시스템 필수
using SoulVeil.Core.Data;      // SO 참조
using SoulVeil.Gameplay.Player; // PlayerController 참조

namespace SoulVeil.Core.Utils
{
    /// <summary>
    /// 개발용 디버그 스크립트.
    /// 숫자키 1, 2번을 눌러 장비 세트(스탯+스킨)를 즉시 교체 테스트합니다.
    /// 릴리즈 빌드에서는 이 컴포넌트를 제거하거나 비활성화해야 합니다.
    /// </summary>
    public class PlayerDebugTester : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private PlayerController targetPlayer;

        [Header("Test Set 1 (Key: 1)")]
        [SerializeField] private StatCardSO statSet1;
        [SerializeField] private SkinCardSO skinSet1;

        [Header("Test Set 2 (Key: 2)")]
        [SerializeField] private StatCardSO statSet2;
        [SerializeField] private SkinCardSO skinSet2;

        private void Update()
        {
            // 키보드가 연결되어 있는지 확인 (안전장치)
            if (Keyboard.current == null) return;
            if (targetPlayer == null) return;

            // 숫자키 1번 입력
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ApplySet(1, statSet1, skinSet1);
            }

            // 숫자키 2번 입력
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ApplySet(2, statSet2, skinSet2);
            }
        }

        private void ApplySet(int setNumber, StatCardSO stat, SkinCardSO skin)
        {
            Debug.Log($"<color=yellow>[Debug] 테스트 세트 {setNumber} 적용 시작...</color>");

            // 1. 스탯 카드 교체
            if (stat != null)
            {
                targetPlayer.EquipStatCard(stat);
            }
            else
            {
                Debug.LogWarning($"[Debug] 세트 {setNumber}의 스탯 카드가 비어있습니다.");
            }

            // 2. 스킨 카드 교체
            if (skin != null)
            {
                targetPlayer.EquipSkinCard(skin);
            }
            else
            {
                Debug.LogWarning($"[Debug] 세트 {setNumber}의 스킨 카드가 비어있습니다.");
            }
        }
    }
}