using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulVeil.Infrastructure.Save
{
    /// <summary>
    /// 플레이어 세이브 데이터(최소 스켈레톤)
    /// - 카드/스킨/스탯 구조는 이후 단계에서 확장한다
    /// - JsonUtility 제약(딕셔너리 미지원)을 고려해 List 기반으로 시작한다
    /// </summary>
    [Serializable]
    public sealed class PlayerSaveData
    {
        [SerializeField] private int version;
        [SerializeField] private string playerId;
        [SerializeField] private string lastSavedUtc;

        [SerializeField] private List<string> ownedCardIds = new List<string>();
        [SerializeField] private List<string> equippedCardIds = new List<string>();

        public int Version => version;
        public string PlayerId => playerId;
        public string LastSavedUtc => lastSavedUtc;

        public IReadOnlyList<string> OwnedCardIds => ownedCardIds;
        public IReadOnlyList<string> EquippedCardIds => equippedCardIds;

        public static PlayerSaveData CreateDefault(string playerId)
        {
            PlayerSaveData data = new PlayerSaveData();
            data.version = SaveVersion.Current;
            data.playerId = string.IsNullOrWhiteSpace(playerId) ? "LocalPlayer" : playerId;
            data.lastSavedUtc = DateTime.UtcNow.ToString("O");
            return data;
        }

        public void TouchSavedTime()
        {
            lastSavedUtc = DateTime.UtcNow.ToString("O");
        }

        public void SetVersionToCurrent()
        {
            version = SaveVersion.Current;
        }
    }
}