using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace Shooter.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SaveData", menuName = "CustomObjects/SaveData", order = 1)]
    public class SaveData : ScriptableObject
    {
        public int m_CheckpointNumber = 0;
        public int m_LastUnlockedLevel = 0;
        public int m_GemCount = 0;
        public int m_CashAmount = 0;
        
        // Сохранение купленных оружий и улучшений
        public bool m_HasPistol = true; // Пистолет доступен по умолчанию
        public bool m_HasShotgun = false;
        public bool m_HasMachinegun = false;
        public bool m_HasRPG = false;
        public int m_WeaponPowerLevel = 0; // Уровень улучшения оружия
        public int m_HealthBuffLevel = 0; // Уровень баффа здоровья (0-4)
        public int m_DamageBuffLevel = 0; // Уровень баффа урона (0-4)

        public void Save()
        {
            PlayerPrefs.SetInt("m_CheckpointNumber", m_CheckpointNumber);
            PlayerPrefs.SetInt("m_GemCount", m_GemCount);
            PlayerPrefs.SetInt("m_CashAmount", m_CashAmount);
            PlayerPrefs.SetInt("m_HasPistol", m_HasPistol ? 1 : 0);
            PlayerPrefs.SetInt("m_HasShotgun", m_HasShotgun ? 1 : 0);
            PlayerPrefs.SetInt("m_HasMachinegun", m_HasMachinegun ? 1 : 0);
            PlayerPrefs.SetInt("m_HasRPG", m_HasRPG ? 1 : 0);
            PlayerPrefs.SetInt("m_WeaponPowerLevel", m_WeaponPowerLevel);
            PlayerPrefs.SetInt("m_HealthBuffLevel", m_HealthBuffLevel);
            PlayerPrefs.SetInt("m_DamageBuffLevel", m_DamageBuffLevel);
            PlayerPrefs.Save();

            // Сохранение m_GemCount в облачное хранилище через PluginYG
            YandexGame.savesData.m_GemCount = m_GemCount;
            YandexGame.SaveProgress();

            // Запись в лидерборд
            YandexGame.NewLeaderboardScores("gemslb", m_GemCount); // Запись в лидерборд gemslb
        }

        public void Load()
        {
            m_CheckpointNumber = PlayerPrefs.GetInt("m_CheckpointNumber", 0);
            m_GemCount = PlayerPrefs.GetInt("m_GemCount", 0);
            m_CashAmount = PlayerPrefs.GetInt("m_CashAmount", 0);
            m_HasPistol = PlayerPrefs.GetInt("m_HasPistol", 1) == 1;
            m_HasShotgun = PlayerPrefs.GetInt("m_HasShotgun", 0) == 1;
            m_HasMachinegun = PlayerPrefs.GetInt("m_HasMachinegun", 0) == 1;
            m_HasRPG = PlayerPrefs.GetInt("m_HasRPG", 0) == 1;
            m_WeaponPowerLevel = PlayerPrefs.GetInt("m_WeaponPowerLevel", 0);
            m_HealthBuffLevel = PlayerPrefs.GetInt("m_HealthBuffLevel", 0);
            m_DamageBuffLevel = PlayerPrefs.GetInt("m_DamageBuffLevel", 0);
        }

        public void ResetProgress()
        {
            // Сбрасываем все значения прогресса
            m_CheckpointNumber = 0;
            m_LastUnlockedLevel = 0;
            m_GemCount = 0;
            m_CashAmount = 0;
            m_HasPistol = true;
            m_HasShotgun = false;
            m_HasMachinegun = false;
            m_HasRPG = false;
            m_WeaponPowerLevel = 0;
            m_HealthBuffLevel = 0;
            m_DamageBuffLevel = 0;

            // Удаляем ключи из PlayerPrefs
            PlayerPrefs.DeleteKey("m_CheckpointNumber");
            PlayerPrefs.DeleteKey("m_GemCount");
            PlayerPrefs.DeleteKey("m_CashAmount");
            PlayerPrefs.DeleteKey("m_HasPistol");
            PlayerPrefs.DeleteKey("m_HasShotgun");
            PlayerPrefs.DeleteKey("m_HasMachinegun");
            PlayerPrefs.DeleteKey("m_HasRPG");
            PlayerPrefs.DeleteKey("m_WeaponPowerLevel");
            PlayerPrefs.DeleteKey("m_HealthBuffLevel");
            PlayerPrefs.DeleteKey("m_DamageBuffLevel");
            PlayerPrefs.Save();

            // Сбрасываем данные в YandexGame
            if (YandexGame.SDKEnabled)
            {
                YandexGame.savesData.m_GemCount = 0;
                YandexGame.SaveProgress();
            }
        }
    }
}
