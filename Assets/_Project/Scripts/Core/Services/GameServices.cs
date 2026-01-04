using System;
using UnityEngine;

namespace SoulVeil.Core.Services
{
    /// <summary>
    /// 전역 서비스 접근 파사드.
    /// - Initialize는 AppBootstrapper에서 1회 호출한다.
    /// - 이후 시스템들은 GameServices.Get<T>()로 서비스에 접근한다.
    /// </summary>
    public static class GameServices
    {
        private static GameServicesRegistry registry;
        private static bool isInitialized;

        /// <summary>
        /// 에디터 Enter Play Mode Options에서 Domain Reload를 끄는 경우를 포함해,
        /// 플레이 시작마다 정적 상태를 안전하게 초기화한다.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForPlayMode()
        {
            registry = null;
            isInitialized = false;
        }

        public static void Initialize(GameServicesRegistry servicesRegistry)
        {
            if (servicesRegistry == null)
                throw new ArgumentNullException(nameof(servicesRegistry));

            if (isInitialized)
                throw new InvalidOperationException("GameServices는 이미 초기화되었습니다.");

            registry = servicesRegistry;
            isInitialized = true;
        }

        public static TService Get<TService>() where TService : class
        {
            EnsureInitialized();
            return registry.Get<TService>();
        }

        public static bool TryGet<TService>(out TService service) where TService : class
        {
            EnsureInitialized();
            return registry.TryGet(out service);
        }

        private static void EnsureInitialized()
        {
            if (isInitialized && registry != null) return;

            throw new InvalidOperationException(
                "GameServices가 초기화되지 않았습니다. Boot 씬의 AppBootstrapper에서 GameServices.Initialize를 호출해야 합니다.");
        }
    }
}