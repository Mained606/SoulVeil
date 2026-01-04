using System;
using System.Collections.Generic;

namespace SoulVeil.Core.Services
{
    /// <summary>
    /// 서비스 인스턴스를 등록/조회하는 레지스트리.
    /// - 전역 접근은 허용하되(조회), 등록은 부트스트랩에서만 수행하는 것을 권장한다.
    /// - 구현체는 인터페이스로 등록하는 방식을 기본으로 한다.
    /// </summary>
    public sealed class GameServicesRegistry
    {
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>(64);

        public void Register<TService>(TService instance) where TService : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance), "서비스 인스턴스가 null입니다.");

            Type key = typeof(TService);

            if (services.ContainsKey(key))
                throw new InvalidOperationException($"이미 등록된 서비스입니다: {key.Name}");

            services.Add(key, instance);
        }

        public bool TryGet<TService>(out TService service) where TService : class
        {
            if (services.TryGetValue(typeof(TService), out object raw) && raw is TService typed)
            {
                service = typed;
                return true;
            }

            service = null;
            return false;
        }

        public TService Get<TService>() where TService : class
        {
            if (TryGet(out TService service))
                return service;

            throw new KeyNotFoundException($"등록되지 않은 서비스입니다: {typeof(TService).Name}");
        }

        public void Clear()
        {
            services.Clear();
        }
    }
}
