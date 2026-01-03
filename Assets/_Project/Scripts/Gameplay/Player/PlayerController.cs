using UnityEngine;
using SoulVeil.Core.Data;
using SoulVeil.Core.Input;
using SoulVeil.Core.Combat;
using SoulVeil.Gameplay.Player.Data;
using SoulVeil.Gameplay.Player.Visual;

namespace SoulVeil.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        // 추후 FSM으로 대체 필요 임시 Enum 방식 적용
        public enum PlayerState { Idle, Move, Attack, Dash, Dead }
        
        
        [Header("Data & Stats")]
        [SerializeField] private StatCardSO defaultPlayerStats; // 기본(맨몸) 스탯
        // [SerializeField] private StatCardSO startingCard;       // 초기 장착 카드
        
        [Header("Visuals")]
        [SerializeField] private PlayerVisual visual;     // 비주얼 담당
        [SerializeField] private SkinCardSO defaultSkin;  // 기본 스킨 (맨몸)
    
        // 인스펙터 노출을 위해 SerializeField 사용
        [SerializeField] private PlayerStats stats = new PlayerStats();
        
        [Header("Core Components")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private PlayerMover mover;
        [SerializeField] private Animator currentAnimator;   // 애니메이터 캐싱 (비주얼에서 받아옴)
        
        // [추후 구현] 헬스 시스템 (지금은 주석 처리)
        // private HealthSystem healthSystem;

        [Header("State (Read Only)")]
        [SerializeField] private PlayerState currentState;

        // 입력값 캐싱
        private Vector2 moveInput;
        private Vector2 lookInput;

        private void Awake()
        {
            // 의존성 null 체크
            if (inputReader == null) Debug.LogError($"{nameof(inputReader)} is null");
            if (mover == null) Debug.LogError($"{nameof(mover)} is null");
            if (visual == null) Debug.LogError($"{nameof(visual)} is null");
            
            // 스탯 초기화
            if (defaultPlayerStats != null)
            {
                stats.SetBaseStats(defaultPlayerStats);
            }
            else
            {
                Debug.LogError($"{nameof(defaultPlayerStats)} is null");
            }
            
            // 헬스 시스템 초기화 (추후 구현)
            // healthSystem = GetComponent<HealthSystem>();
            // if (healthSystem) healthSystem.Initialize(stats.MaxHealth);
        }

        private void OnEnable()
        {
            if (inputReader == null) return;

            inputReader.OnMoveEvent += HandleMoveInput;
            inputReader.OnLookEvent += HandleLookInput;
            inputReader.OnAttackEvent += HandleAttackInput;
            inputReader.OnDashEvent += HandleDashInput;

            if (visual != null)
                visual.OnAnimatorChanged += HandleAnimatorChanged;
        }

        private void OnDisable()
        {
            if (inputReader == null) return;

            inputReader.OnMoveEvent -= HandleMoveInput;
            inputReader.OnLookEvent -= HandleLookInput;
            inputReader.OnAttackEvent -= HandleAttackInput;
            inputReader.OnDashEvent -= HandleDashInput;
            
            if (visual != null)
                visual.OnAnimatorChanged -= HandleAnimatorChanged;
        }

        private void Start()
        {
            // 기본 스킨 장착
            if (defaultSkin != null)
            {
                EquipSkinCard(defaultSkin);
            }
            
            // // 시작 스탯 카드가 있다면 장착
            // if (startingCard != null)
            // {
            //     stats.EquipCard(startingCard);
            // }
        }

        private void Update()
        {
            if (currentState == PlayerState.Dead) return;

            ProcessMovement();
        }
        
        // 로직 처리
        private void ProcessMovement()
        {
            if (currentState == PlayerState.Attack) return; // 공격 중 이동 불가 등

            // Mover에게 이동 위임 (Stats의 민첩성을 이동속도에 반영 가능)
            // 예: float speedModifier = 1.0f + (stats.TotalAgility * 0.05f);
            mover.MoveFrame(moveInput); // speedModifier 인자 추가 가능
            mover.LookFrame(lookInput);

            // 상태 갱신
            if (moveInput.sqrMagnitude > 0.01f)
                currentState = PlayerState.Move;
            else
                currentState = PlayerState.Idle;
        }
        
        // 외부(UI/인벤토리)에서 호출하는 카드 교체 메서드
        public void EquipStatCard(StatCardSO newCard)
        {
            stats.EquipCard(newCard);
            
            // [추후 구현] 스탯 변경에 따른 체력통 업데이트 로직
            // if (healthSystem) healthSystem.SetMaxHealth(stats.MaxHealth);
        }
        
        // 외부 호출용 스킨 교체 메서드
        public void EquipSkinCard(SkinCardSO newSkin)
        {
            if (visual != null)
            {
                visual.UpdateVisual(newSkin);
                Debug.Log($"[PlayerController] 스킨 교체 요청: {newSkin.displayName}");
            }
        }

        // --- Callbacks ---
        private void HandleAnimatorChanged(Animator newAnimator)
        {
            this.currentAnimator = newAnimator;
            // 여기서 애니메이션 파라미터 초기화 등을 수행할 수 있음
            // 예: currentAnimator.SetBool("IsGrounded", true);
            Debug.Log("[PlayerController] 애니메이터 갱신 완료!");
        }
        
        private void HandleMoveInput(Vector2 input) => moveInput = input;
        private void HandleLookInput(Vector2 input) => lookInput = input;

        private void HandleAttackInput()
        {
            if (currentState != PlayerState.Idle && currentState != PlayerState.Move) return;

            currentState = PlayerState.Attack;
            
            // 공격 로직 테스트
            Debug.Log($"[Action] 공격! 속성: {stats.AttackElement}, 데미지: {stats.TotalStrength}");
            
            // 애니메이션 종료 시점 등을 가정 (0.5초 후 복귀)
            Invoke(nameof(ReturnToIdle), 0.5f);
        }

        private void HandleDashInput()
        {
            if (currentState != PlayerState.Idle && currentState != PlayerState.Move) return;
             
            currentState = PlayerState.Dash;
            Debug.Log($"[Action] 대쉬! 민첩함: {stats.TotalAgility}");
            Invoke(nameof(ReturnToIdle), 0.2f);
        }

        private void ReturnToIdle() => currentState = PlayerState.Idle;
    }
}