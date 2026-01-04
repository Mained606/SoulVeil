using UnityEngine;
using SoulVeil.Core.Services;
using SoulVeil.Core.Diagnostics;
using SoulVeil.Infrastructure.Save;
using SoulVeil.Infrastructure.StaticData;

namespace SoulVeil.Core.Bootstrap
{
    /// <summary>
    /// 앱의 단일 진입점.
    /// - Boot 씬에서 1회 생성된다.
    /// - DontDestroyOnLoad로 앱 수명 동안 유지된다.
    /// - 이후 단계에서 StartupFlow와 서비스 등록을 실행한다.
    /// </summary>
    public sealed class AppBootstrapper : MonoBehaviour
    {
        private static AppBootstrapper instance;
        private bool hasBooted;

        private void Awake()
        {
            // 중복 생성 방지
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Boot가 실행중인지 확인
            if (hasBooted) return;
            hasBooted = true;

            RunBootstrap();
        }

        private void RunBootstrap()
        {
            //서비스 레지스트리 생성 및 전역 파사드 초기화
            GameServicesRegistry registry = new GameServicesRegistry();
            GameServices.Initialize(registry);
            
            // 서비스 레지스트리 등록
            // 로그 서비스
            ILogService logService = new UnityLogService("SoulVeil");
            registry.Register<ILogService>(logService);
            
            // 세이브 서비스
            ISaveService saveService = new LocalJsonSaveService(logService);
            registry.Register<ISaveService>(saveService);
            
            saveService.Load();
            
            IStaticDataService staticDataService = new AddressablesStaticDataService(logService);
            registry.Register<IStaticDataService>(staticDataService);
            
            StartCoroutine(((AddressablesStaticDataService)staticDataService).LoadAll());
            
            logService.Info("Bootstrap started. SaveService loaded.");
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }
    }
}