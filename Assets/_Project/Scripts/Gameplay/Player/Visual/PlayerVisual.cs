using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using SoulVeil.Core.Data; // SkinCardSO 참조

namespace SoulVeil.Gameplay.Player.Visual
{
    /// <summary>
    /// 플레이어의 외형(모델)을 비동기로 로드하고 교체하는 클래스.
    /// Addressables의 수명 주기를 관리합니다.
    /// </summary>
    public class PlayerVisual : MonoBehaviour
    {
        // ==========================================
        // 상태 및 캐시
        // ==========================================
        private GameObject currentModelInstance; // 현재 생성된 모델 (해제용)
        private bool isLoading = false;          // 로딩 중복 방지 락(Lock)
        
        // 애니메이터가 교체될 때 컨트롤러에게 알려주기 위한 이벤트
        public event Action<Animator> OnAnimatorChanged;
        
        // 현재 애니메이터 (외부에서 접근 가능)
        public Animator CurrentAnimator { get; private set; }
        
        private void OnDestroy()
        {
            // 게임 종료/씬 이동 시 메모리 누수 방지를 위해 반드시 해제
            UnloadCurrentModel();
        }

        // ==========================================
        // 공개 메서드
        // ==========================================

        /// <summary>
        /// 스킨 카드를 받아 모델을 비동기로 교체합니다. (Fire-and-Forget)
        /// </summary>
        public async void UpdateVisual(SkinCardSO skinCard)
        {
            if (skinCard == null)
            {
                Debug.LogWarning("[PlayerVisual] 스킨 카드가 null입니다.");
                return;
            }
            
            // 로딩 중이거나, 프리팹 주소가 비어있으면 패스
            if (isLoading || skinCard.modelPrefab.RuntimeKeyIsValid() == false) return;

            // 로딩 시작 (Lock)
            isLoading = true;

            try
            {
                // 기존 모델 정리 (메모리 해제)
                UnloadCurrentModel();

                // 비동기 생성 (InstantiateAsync)
                // transform을 부모로 지정하여 플레이어 아래에 붙임
                var handle = skinCard.modelPrefab.InstantiateAsync(transform);
                
                // await: 로딩이 끝날 때까지 여기서 비동기 대기 (프레임 드랍 없음)
                currentModelInstance = await handle.Task;

                // 트랜스폼 초기화 (혹시 모를 오프셋 방지)
                if (currentModelInstance != null)
                {
                    currentModelInstance.transform.localPosition = Vector3.zero;
                    currentModelInstance.transform.localRotation = Quaternion.identity;
                
                    // 애니메이터 갱신 및 전파
                    CurrentAnimator = currentModelInstance.GetComponentInChildren<Animator>();
                    if (CurrentAnimator != null)
                    {
                        OnAnimatorChanged?.Invoke(CurrentAnimator);
                    }
                    else
                    {
                        Debug.LogError($"[PlayerVisual] 모델({skinCard.name})에 Animator가 없습니다!");
                    }
                }
            }
            catch (Exception e)
            {
                // 로딩 실패 시 예외 처리
                Debug.LogError($"[PlayerVisual] 스킨 로딩 실패: {e.Message}");
            }
            finally
            {
                // 로딩 종료 (Unlock) - 성공하든 실패하든 무조건 실행
                isLoading = false;
            }
        }

        // ==========================================
        // 내부 로직
        // ==========================================
        
        private void UnloadCurrentModel()
        {
            if (currentModelInstance != null)
            {
                // Destroy 대신 Addressables.ReleaseInstance를 사용해야 함
                // 그래야 내부 레퍼런스 카운트가 감소하고 메모리가 확보됨
                Addressables.ReleaseInstance(currentModelInstance);
                currentModelInstance = null;
                CurrentAnimator = null;
            }
        }
    }
}