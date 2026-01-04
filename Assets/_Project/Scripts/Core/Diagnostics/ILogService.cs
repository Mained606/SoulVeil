using System;

namespace SoulVeil.Core.Diagnostics
{
    /// <summary>
    /// 로깅 서비스 인터페이스.
    /// - 게임 전반에서 동일한 채널로 로그를 남기기 위한 진입점이다.
    /// - 구현체를 바꾸기 쉽도록(테스트/릴리즈 정책) 인터페이스로 분리한다.
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// 정보성 로그. 릴리즈 빌드에서는 구현체 정책에 따라 출력되지 않을 수 있다.
        /// </summary>
        void Info(string message);
        void Warning(string message);
        void Error(string message);

        void Exception(Exception exception, string contextMessage = null);
    }
}