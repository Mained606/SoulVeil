using System;
using UnityEngine;

namespace SoulVeil.Core.Diagnostics
{
    /// <summary>
    /// UnityEngine.Debug 기반 로깅 구현체.
    /// - 에디터/개발 빌드에서는 Info 로그를 포함해 충분한 로그를 남긴다.
    /// - 릴리즈 빌드에서는 Info 로그를 컴파일 단계에서 제거해 성능/노이즈를 줄인다.
    /// </summary>
    public sealed class UnityLogService : ILogService
    {
        private const string DefaultChannel = "SoulVeil";

        private readonly string channel;

        public UnityLogService(string channel)
        {
            this.channel = string.IsNullOrWhiteSpace(channel) ? DefaultChannel : channel;
        }

        public void Info(string message)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log(Format("INFO", Sanitize(message)));
#endif
        }

        public void Warning(string message)
        {
            Debug.LogWarning(Format("WARN", Sanitize(message)));
        }

        public void Error(string message)
        {
            Debug.LogError(Format("ERROR", Sanitize(message)));
        }

        public void Exception(Exception exception, string contextMessage = null)
        {
            if (exception == null)
            {
                Error("Exception 호출에 null 예외가 전달되었습니다.");
                if (!string.IsNullOrWhiteSpace(contextMessage))
                    Error($"예외 문맥: {Sanitize(contextMessage)}");
                return;
            }

            if (!string.IsNullOrWhiteSpace(contextMessage))
                Debug.LogError(Format("EXCEPTION", Sanitize(contextMessage)));

            Debug.LogException(exception);
        }

        private string Sanitize(string message)
        {
            return string.IsNullOrWhiteSpace(message) ? "(empty)" : message;
        }

        private string Format(string level, string message)
        {
            return $"[{channel}][{level}] {message}";
        }
    }
}