using System;
using System.IO;
using UnityEngine;
using SoulVeil.Core.Diagnostics;

namespace SoulVeil.Infrastructure.Save
{
    /// <summary>
    /// 로컬 JSON 파일 기반 세이브 서비스
    /// - 파일 손상 대비: temp에 먼저 쓰고 -> 메인 교체, 기존 메인은 백업 유지
    /// - JsonUtility 사용: Unity 기본 직렬화 기반(안정성 우선)
    /// </summary>
    public sealed class LocalJsonSaveService : ISaveService
    {
        private readonly ILogService logService;

        private readonly string saveFilePath;
        private readonly string backupFilePath;
        private readonly string tempFilePath;

        private PlayerSaveData current;
        private bool isLoaded;

        public PlayerSaveData Current => current;
        public bool IsLoaded => isLoaded;

        public LocalJsonSaveService(ILogService logService, string fileName = "playerSave.json")
        {
            this.logService = logService;

            string basePath = Application.persistentDataPath;
            saveFilePath = Path.Combine(basePath, fileName);
            backupFilePath = saveFilePath + ".bak";
            tempFilePath = saveFilePath + ".tmp";
        }

        public void Load()
        {
            if (TryLoadFrom(saveFilePath, out PlayerSaveData loaded))
            {
                current = EnsureUpToDate(loaded);
                isLoaded = true;
                logService.Info($"Save loaded: {saveFilePath}");
                return;
            }

            if (TryLoadFrom(backupFilePath, out PlayerSaveData backupLoaded))
            {
                current = EnsureUpToDate(backupLoaded);
                isLoaded = true;
                logService.Warning($"Main save corrupted. Loaded from backup: {backupFilePath}");

                // 백업 기반으로 메인 세이브를 복구 저장
                Save();
                return;
            }

            current = PlayerSaveData.CreateDefault("LocalPlayer");
            isLoaded = true;
            logService.Warning("No valid save found. Created default save data.");

            Save();
        }

        public void Save()
        {
            if (!isLoaded || current == null)
            {
                logService.Error("Save called before Load or Current is null.");
                return;
            }

            try
            {
                current.SetVersionToCurrent();
                current.TouchSavedTime();

                string json = JsonUtility.ToJson(current, true);

                EnsureDirectoryExists(saveFilePath);

                // 1) temp 파일에 먼저 기록
                File.WriteAllText(tempFilePath, json);

                // 2) 기존 메인 파일이 있으면 백업 갱신
                if (File.Exists(saveFilePath))
                    File.Copy(saveFilePath, backupFilePath, true);

                // 3) temp -> main 교체
                ReplaceFile(tempFilePath, saveFilePath);

                logService.Info($"Save written: {saveFilePath}");
            }
            catch (Exception exception)
            {
                logService.Exception(exception, "Save failed.");
            }
            finally
            {
                TryDeleteTemp();
            }
        }

        public void ResetToDefault()
        {
            current = PlayerSaveData.CreateDefault("LocalPlayer");
            isLoaded = true;

            logService.Warning("Save reset to default.");
            Save();
        }

        private PlayerSaveData EnsureUpToDate(PlayerSaveData loaded)
        {
            if (loaded == null)
                return PlayerSaveData.CreateDefault("LocalPlayer");

            if (loaded.Version != SaveVersion.Current)
            {
                logService.Warning($"Save version mismatch. Loaded={loaded.Version}, Current={SaveVersion.Current}. Migration needed.");
                // TODO: 여기서 버전별 마이그레이션 수행
                // loaded = SaveMigrator.Migrate(loaded);
                loaded.SetVersionToCurrent();
            }

            return loaded;
        }

        private bool TryLoadFrom(string path, out PlayerSaveData data)
        {
            data = null;

            try
            {
                if (!File.Exists(path))
                    return false;

                string json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                    return false;

                data = JsonUtility.FromJson<PlayerSaveData>(json);
                return data != null;
            }
            catch (Exception exception)
            {
                logService.Exception(exception, $"Load failed: {path}");
                return false;
            }
        }

        private void EnsureDirectoryExists(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (string.IsNullOrWhiteSpace(directory))
                return;

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        private void ReplaceFile(string sourcePath, string targetPath)
        {
            if (File.Exists(targetPath))
                File.Delete(targetPath);

            File.Move(sourcePath, targetPath);
        }

        private void TryDeleteTemp()
        {
            try
            {
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
            catch
            {
                // 실패해도 치명적이지 않다(다음 저장 시 덮어쓰기 됨)
            }
        }
    }
}
