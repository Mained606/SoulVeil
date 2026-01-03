using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulVeil.Core.EventBus
{
    public static class GameEventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> subscribers = new();

        public static void Subscribe<T>(Action<T> callback)
        {
            Type type = typeof(T);
            if (!subscribers.ContainsKey(type))
            {
                subscribers[type] = new List<Delegate>();
            }
            subscribers[type].Add(callback);
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            Type type = typeof(T);
            if (subscribers.ContainsKey(type))
            {
                subscribers[type].Remove(callback);
            }
        }

        public static void Publish<T>(T eventMessage)
        {
            Type type = typeof(T);
            if (subscribers.ContainsKey(type))
            {
                // 리스트 복사본 생성 (순회 중 수정 오류 방지)
                var callbacks = new List<Delegate>(subscribers[type]);
                foreach (var callback in callbacks)
                {
                    (callback as Action<T>)?.Invoke(eventMessage);
                }
            }
        }
    }
    
    // 이벤트 정의용 구조체 (나중에 사용)
    public struct PlayerStatsChangedEvent { }
}
