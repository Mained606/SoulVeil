using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace SoulVeil.Core.Input
{
    [CreateAssetMenu(fileName = "GameInputReader", menuName = "SoulVeil/Input/GameInputReader")]
    public class GameInputReader : ScriptableObject, IInputReader, Controls.IPlayerActions
    {
        // ======================================================================================
        // 변수 & 이벤트
        // ======================================================================================
        
        private Controls controls;
        
        // IInputReader Events (외부에서 구독할 이벤트들)
        public event Action<Vector2> OnMoveEvent;
        public event Action<Vector2> OnLookEvent;
        public event Action OnAttackEvent;
        public event Action OnDashEvent;
        public event Action OnInteractEvent;
        public event Action OnMenuEvent;
        
        // ======================================================================================
        // 초기화 및 해제
        // ======================================================================================
        
        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new Controls();
                controls.Player.SetCallbacks(this); // Player 맵 사용
            }
            
            EnableInput();
        }

        private void OnDisable()
        {
            DisableInput();
        }
        
        // ======================================================================================
        // Public Methods (외부 제어용)
        // ======================================================================================
        
        public void EnableInput()
        {
            controls.Player.Enable();
        }

        public void DisableInput()
        {
            controls.Player.Disable();
        }
        
        // ======================================================================================
        // Interface Callbacks (내부 로직)
        // ======================================================================================
        
        public void OnMove(InputAction.CallbackContext context)
        {
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            OnLookEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed) OnAttackEvent?.Invoke();
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed) OnDashEvent?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed) OnInteractEvent?.Invoke();
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
            if (context.performed) OnMenuEvent?.Invoke();
        }
    }
}