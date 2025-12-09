using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shooter.Gameplay;

namespace Shooter.UI
{
    public class UI_HUD : MonoBehaviour
    {
        public Image m_DamageOverlay;
        public Text[] m_PlayerTexts_1;
        public Text m_GemCountText;
        public Text m_GunNameText;
        public Image m_WeaponIconImage; // Иконка текущего оружия
        public Image m_AimTargetImage;
        public RectTransform m_MainCanvas;

        public Image m_WeaponPowerTime;
        public Image m_PlayerHealth;

        [Space]
        public Image m_BossHealthBase;
        public Image m_BossHealth;

        [Space]
        public Image m_PowerBase;
        public Image m_PowerBar;
        public Text m_PowerNameText;
        public Text m_PowerAmountText;

        public string[] m_WeaponNames = new string[4] { "ПИСТОЛЕТ", "ДРОБОВИК", "ПУЛЕМЕТ", "РПГ" };
        public Sprite[] m_WeaponIcons = new Sprite[4]; // Массив иконок оружий

        public static UI_HUD m_Main;

        void Awake()
        {
            m_Main = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            //Cursor.visible = false;
            m_BossHealthBase.gameObject.SetActive(false);
        }

        void Update()
        {
            if (PlayerControl.MainPlayerController != null && m_GemCountText != null)
            {
                m_GemCountText.text = PlayerControl.MainPlayerController.m_GemCount.ToString();
            }

            if (PlayerChar.m_Current != null && PlayerChar.m_Current.m_TempTarget != null && 
                CameraControl.m_Current != null && CameraControl.m_Current.m_MyCamera != null && 
                m_AimTargetImage != null && m_MainCanvas != null)
            {
                m_AimTargetImage.gameObject.SetActive(true);
                Vector3 v = CameraControl.m_Current.m_MyCamera.WorldToScreenPoint(PlayerChar.m_Current.m_TempTarget.m_TargetCenter.position);
                v.x = v.x / (float)Screen.width;
                v.y = v.y / (float)Screen.height;

                v.x = m_MainCanvas.sizeDelta.x * v.x;
                v.y = m_MainCanvas.sizeDelta.y * v.y;

                m_AimTargetImage.rectTransform.anchoredPosition = Helper.ToVector2(v);



            }
            else
            {
                m_AimTargetImage.gameObject.SetActive(false);
            }


            if (PlayerChar.m_Current == null)
                return;

            if (m_WeaponPowerTime != null)
            {
                m_WeaponPowerTime.fillAmount = PlayerChar.m_Current.m_WpnPowerTime / 16f;
            }

            DamageControl damage = PlayerChar.m_Current.GetComponent<DamageControl>();
            if (damage != null && m_PlayerHealth != null)
            {
                m_PlayerHealth.fillAmount = damage.Damage / damage.MaxDamage;
            }

            if (m_GunNameText != null && PlayerChar.m_Current.m_WeaponNum < m_WeaponNames.Length)
            {
                m_GunNameText.text = m_WeaponNames[PlayerChar.m_Current.m_WeaponNum];
            }

            // Обновляем иконку оружия
            if (m_WeaponIconImage != null && m_WeaponIcons != null && 
                PlayerChar.m_Current.m_WeaponNum >= 0 && PlayerChar.m_Current.m_WeaponNum < m_WeaponIcons.Length)
            {
                if (m_WeaponIcons[PlayerChar.m_Current.m_WeaponNum] != null)
                {
                    m_WeaponIconImage.sprite = m_WeaponIcons[PlayerChar.m_Current.m_WeaponNum];
                    m_WeaponIconImage.enabled = true;
                }
                else
                {
                    m_WeaponIconImage.enabled = false;
                }
            }

            if (GameControl.m_Current != null && GameControl.m_Current.m_LevelBoss != null)
            {
                damage = GameControl.m_Current.m_LevelBoss.GetComponent<DamageControl>();
                if (damage != null && m_BossHealth != null)
                {
                    m_BossHealth.fillAmount = damage.Damage / damage.MaxDamage;
                }
            }

            PlayerPowers p = PlayerChar.m_Current.GetComponent<PlayerPowers>();
            if (p != null && p.m_HavePower)
            {
                if (m_PowerBase != null)
                {
                    m_PowerBase.gameObject.SetActive(true);
                }
                if (m_PowerNameText != null && m_PowerAmountText != null)
                {
                    switch (p.m_PowerNum)
                    {
                        case 0:
                            m_PowerNameText.text = "Граната";
                            m_PowerAmountText.gameObject.SetActive(true);
                            m_PowerAmountText.text = p.m_AmmoCount.ToString();
                            if (m_PowerBar != null)
                            {
                                m_PowerBar.gameObject.SetActive(false);
                            }
                            break;
                        case 1:
                            m_PowerNameText.text = "Бомба";
                            m_PowerAmountText.gameObject.SetActive(true);
                            m_PowerAmountText.text = p.m_AmmoCount.ToString();
                            if (m_PowerBar != null)
                            {
                                m_PowerBar.gameObject.SetActive(false);
                            }
                            break;
                    }
                }
            }
            else
            {
                if (m_PowerBase != null)
                {
                    m_PowerBase.gameObject.SetActive(false);
                }
            }

        }

        public void ShowBossHealth()
        {
            m_BossHealthBase.gameObject.SetActive(true);
        }
        public void HideBossHealth()
        {
            m_BossHealthBase.gameObject.SetActive(false);
        }

        public void UpdateWeaponIcon()
        {
            if (PlayerChar.m_Current == null)
                return;

            // Обновляем иконку оружия
            if (m_WeaponIconImage != null && m_WeaponIcons != null && 
                PlayerChar.m_Current.m_WeaponNum >= 0 && PlayerChar.m_Current.m_WeaponNum < m_WeaponIcons.Length)
            {
                if (m_WeaponIcons[PlayerChar.m_Current.m_WeaponNum] != null)
                {
                    m_WeaponIconImage.sprite = m_WeaponIcons[PlayerChar.m_Current.m_WeaponNum];
                    m_WeaponIconImage.enabled = true;
                }
                else
                {
                    m_WeaponIconImage.enabled = false;
                }
            }
        }

    }
}
