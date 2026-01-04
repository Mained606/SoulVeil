using UnityEngine;

namespace SoulVeil.Gameplay.Player
{
    public class PlayerMover: MonoBehaviour
    {
        [Header("Movement Settings")] 
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float gravity = -9.81f;
        
        [Header("Ground Detection")]
        [SerializeField] private LayerMask groundLayer; // 터레인이 속한 레이어 지정
        
        // 내부 컴포넌트
        private CharacterController controller;
        private Camera mainCamera;
        private Vector3 velocity;   // 수직 속도(중력용)

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            mainCamera = Camera.main;
        }

        public void MoveFrame(Vector2 inputDirection)
        {
            // 1. 입력 벡터 변환
            Vector3 targetDir = new Vector3(inputDirection.x, 0, inputDirection.y);
            
            // 2. 바닥 체크 및 중력 초기화
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // 바닥 밀착용 미세 힘
            }

            // 3. 중력 적용
            velocity.y += gravity * Time.deltaTime;

            // 4. 최종 이동 (수평 + 수직)
            Vector3 finalMove = (targetDir * moveSpeed) + velocity;
            
            controller.Move(finalMove * Time.deltaTime);
        }
        
        public void LookFrame(Vector2 mouseScreenPos)
        {
            // 화면 -> 월드 레이 발사
            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);
            
            // 터레인(Ground)과 충돌 체크
            // (Mathf.Infinity는 사거리를 무제한으로)
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundLayer))
            {
                Vector3 hitPoint = hitInfo.point;
                hitPoint.y = transform.position.y; // 캐릭터 높이로 보정 (고개 숙임 방지)

                Vector3 dir = (hitPoint - transform.position).normalized;

                // 회전 적용
                if (dir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }    
}

