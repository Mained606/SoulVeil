using UnityEngine;
using System;

namespace SoulVeil.Core.Input
{
    /// <summary>
    /// 하드웨어 입력(키보드 / 마우스 / 패드 등등)과 게임 로직을 연결하는 중계 인터페이스
    /// </summary>
    public interface IInputReader
    {
        /// <summary>
        /// 입력 수신을 활성화 (씬 로드 직후, 메뉴 닫힘 등)
        /// </summary>
        public void EnableInput();
        
        /// <summary>
        /// 입력 수신을 비활성화(대화, 컷신, 사망 연출 등)
        /// </summary>
        public void DisableInput();
        
        // Value Inputs (값 전달)
        event Action<Vector2> OnMoveEvent;  // 이동 관련
        event Action<Vector2> OnLookEvent;  // 조준, 시선 입력이 발생할 때 호출
        
        // Action Inputs(행동 트리거)
        event Action OnAttackEvent;     // 공격
        event Action OnDashEvent;       // 대쉬
        event Action OnInteractEvent;   // 상호작용
        event Action OnMenuEvent;       // 메뉴, 일시정지
    }
}