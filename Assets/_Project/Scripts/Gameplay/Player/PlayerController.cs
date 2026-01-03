using UnityEngine;
using SoulVeil.Core.Input;

namespace SoulVeil.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        public enum PlayerState { Idle, Move, Attack, Dash, Dead }

        [Header("Dependencies")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private PlayerMover mover;

        [Header("State (Read Only)")]
        [SerializeField] private PlayerState currentState;

        // 입력값 캐싱
        private Vector2 inputMove;
        private Vector2 inputLook;

        private void OnEnable()
        {
            if (inputReader == null) return;

            inputReader.OnMoveEvent += OnMoveInput;
            inputReader.OnLookEvent += OnLookInput;
            inputReader.OnAttackEvent += OnAttackInput;
            inputReader.OnDashEvent += OnDashInput;
        }

        private void OnDisable()
        {
            if (inputReader == null) return;

            inputReader.OnMoveEvent -= OnMoveInput;
            inputReader.OnLookEvent -= OnLookInput;
            inputReader.OnAttackEvent -= OnAttackInput;
            inputReader.OnDashEvent -= OnDashInput;
        }

        private void Update()
        {
            switch (currentState)
            {
                case PlayerState.Idle:
                case PlayerState.Move:
                    HandleMovement();
                    break;
                
                case PlayerState.Attack:
                    break;
                
                case PlayerState.Dash:
                    break;
            }
        }

        private void HandleMovement()
        {
            mover.MoveFrame(inputMove);
            mover.LookFrame(inputLook);

            if (inputMove.sqrMagnitude > 0.01f)
                currentState = PlayerState.Move;
            else
                currentState = PlayerState.Idle;
        }

        // --- Callbacks ---
        private void OnMoveInput(Vector2 input)
        {
            Debug.Log($"입력 들어옴: {input}");
            inputMove = input;
        }
        private void OnLookInput(Vector2 input) => inputLook = input;

        private void OnAttackInput()
        {
            if (currentState == PlayerState.Idle || currentState == PlayerState.Move)
            {
                currentState = PlayerState.Attack;
                Debug.Log("Attack!");
                Invoke(nameof(ReturnToIdle), 0.5f);
            }
        }

        private void OnDashInput()
        {
            if (currentState == PlayerState.Move || currentState == PlayerState.Idle)
            {
                currentState = PlayerState.Dash;
                Debug.Log("Dash!");
                Invoke(nameof(ReturnToIdle), 0.2f);
            }
        }

        private void ReturnToIdle()
        {
            currentState = PlayerState.Idle;
        }
    }
}