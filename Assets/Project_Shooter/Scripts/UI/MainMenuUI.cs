using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Shooter.ScriptableObjects;
namespace Shooter
{
    public class MainMenuUI : MonoBehaviour
    {
        public SaveData m_SaveData;
        // Start is called before the first frame update
        void Start()
        {
            m_SaveData.Load();
        }

        public void BtnStart()
        {
            // Сбрасываем прогресс перед запуском игры, чтобы начать с начала
            if (m_SaveData != null)
            {
                m_SaveData.ResetProgress();
                Debug.Log("Прогресс сброшен. Игра начнется с начала.");
            }
            SceneManager.LoadScene("Level_1");
        }

        public void BtnResetProgress()
        {
            // Отдельный метод для сброса прогресса (можно использовать из кнопки в меню)
            if (m_SaveData != null)
            {
                m_SaveData.ResetProgress();
                Debug.Log("Прогресс сброшен.");
            }
        }
    }
}