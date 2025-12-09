using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Shooter.ScriptableObjects;

namespace Shooter.Gameplay
{
    public class GameControl : MonoBehaviour
    {
        public static GameControl m_Current;

        public bool m_EnteredBossFight = false;
        public GameObject m_LevelBoss;

        public SaveData m_MainSaveData;

        public GameObject m_TextUI_1;


        public GameObject[] m_Tutorials;

        public GameObject m_PauseUI;

        // public GameObject m_DeathEff;

        public bool m_Pausesd = false;
        public bool m_IsShopOpen = false;
        void Awake()
        {
            m_Current = this;
            // Загружаем данные сохранения при старте сцены
            if (m_MainSaveData != null)
            {
                m_MainSaveData.Load();
                Debug.Log($"GameControl: Loaded checkpoint {m_MainSaveData.m_CheckpointNumber}, coins: {m_MainSaveData.m_CashAmount}");
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Co_Start());
            //m_DeathEff.SetActive(false);
        }

        IEnumerator Co_Start()
        {
            if (m_MainSaveData.m_CheckpointNumber == 0)
            {
                m_TextUI_1.SetActive(true);
                FadeControl.m_Current.StartFadeIn();
                yield return new WaitForSeconds(4f);
                FadeControl.m_Current.StartFadeOut();
                yield return new WaitForSeconds(3f);
                m_TextUI_1.SetActive(false);
                FadeControl.m_Current.StartFadeIn();


                yield return new WaitForSeconds(2f);
                m_Tutorials[0].SetActive(true);
                yield return new WaitForSeconds(4f);
                m_Tutorials[0].SetActive(false);
                m_Tutorials[1].SetActive(true);
                yield return new WaitForSeconds(4f);
                m_Tutorials[1].SetActive(false);
            }
            else
            {
                FadeControl.m_Current.StartFadeIn();
                yield return new WaitForSeconds(1f);

            }
        }

        // Update is called once per frame
        void Update()
        {
            // Проверяем, не открыт ли магазин
            if (Shooter.UI.ShopMenu.m_Main != null && Shooter.UI.ShopMenu.m_Main.m_IsShopOpen)
            {
                m_IsShopOpen = true;
            }
            else
            {
                m_IsShopOpen = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Не открываем паузу, если открыт магазин
                if (m_IsShopOpen)
                    return;

                if (!m_Pausesd)
                {
                    m_Pausesd = true;
                    Time.timeScale = 0;
                    m_PauseUI.SetActive(true);
                }
                else
                {
                    m_Pausesd = false;
                    Time.timeScale = 1;
                    m_PauseUI.SetActive(false);
                }
            }
        }

        void FixedUpdate()
        {



        }

        public void HandleCheckpoint(int num)
        {
            if (num > m_MainSaveData.m_CheckpointNumber)
            {
                m_MainSaveData.m_CheckpointNumber = num;
                // Сохраняем количество гемов (монет) при прохождении чекпоинта
                if (PlayerControl.MainPlayerController != null)
                {
                    m_MainSaveData.m_GemCount = PlayerControl.MainPlayerController.m_GemCount;
                    m_MainSaveData.Save();
                }
                m_MainSaveData.Save();
            }
        }

        public void HandlePlayerDeath()
        {
            // Не сохраняем текущие монеты - при смерти откатываемся к последнему чекпоинту
            // Монеты загрузятся из сохранения при старте сцены
            StartCoroutine(Co_HandleGameOver());
        }

        IEnumerator Co_HandleGameOver()
        {
            // m_DeathEff.SetActive(true);
            CameraControl.m_Current.StartShake(.4f, .3f);
            yield return new WaitForSeconds(1);
            FadeControl.m_Current.StartFadeOut();
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


        public void Resume()
        {
            m_Pausesd = false;
            Time.timeScale = 1;
            m_PauseUI.SetActive(false);
        }
        public void Exit()
        {
            m_Pausesd = false;
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
        }

        public void Restart()
        {
            // Пустой метод - функционал рестарта удален, кнопка оставлена
        }
    }
}
