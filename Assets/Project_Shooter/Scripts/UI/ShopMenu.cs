using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Shooter.Gameplay;
using Shooter.ScriptableObjects;

namespace Shooter.UI
{
    public class ShopMenu : MonoBehaviour
    {
        public static ShopMenu m_Main;

        [Header("UI Элементы")]
        public GameObject m_ShopPanel;
        public Text m_GemCountText;
        public Image m_CloseButton;
        private GameObject m_BackgroundOverlay; // Затемняющий фон

        [Header("Оружие")]
        public Text m_PistolText; // Текстовый элемент для покупки пистолета (кликабельный)
        public Text m_ShotgunText; // Текстовый элемент для покупки дробовика (кликабельный)
        public Text m_MachinegunText; // Текстовый элемент для покупки пулемета (кликабельный)
        public Text m_RPGText; // Текстовый элемент для покупки РПГ (кликабельный)
        public Text m_PistolPriceText;
        public Text m_ShotgunPriceText;
        public Text m_MachinegunPriceText;
        public Text m_RPGPriceText;
        public Text m_PistolStatusText;
        public Text m_ShotgunStatusText;
        public Text m_MachinegunStatusText;
        public Text m_RPGStatusText;

        [Header("Улучшения")]
        public Button m_BuyWeaponPowerButton;
        public Text m_WeaponPowerPriceText;
        public Text m_WeaponPowerStatusText;

        [Header("Баффы")]
        public Text m_HealthBuffText; // Текстовый элемент для покупки баффа здоровья (кликабельный)
        public Text m_DamageBuffText; // Текстовый элемент для покупки баффа урона (кликабельный)
        public Text m_HealthBuffPriceText;
        public Text m_DamageBuffPriceText;
        public Text m_HealthBuffStatusText;
        public Text m_DamageBuffStatusText;

        [Header("Цены")]
        public int m_PistolPrice = 0; // Бесплатно по умолчанию
        public int m_ShotgunPrice = 125;
        public int m_MachinegunPrice = 150;
        public int m_RPGPrice = 200;
        public int m_WeaponPowerPrice = 100;
        public int m_HealthBuffPrice = 200;
        public int m_DamageBuffPrice = 200;

        private SaveData m_SaveData;
        public bool m_IsShopOpen = false;

        void Awake()
        {
            if (m_Main != null && m_Main != this)
            {
                Destroy(this);
                return;
            }
            m_Main = this;
        }

        void OnDestroy()
        {
            if (m_Main == this)
            {
                m_Main = null;
            }
        }

        void Start()
        {
            CreateBackgroundOverlay();

            if (m_ShopPanel != null)
            {
                m_ShopPanel.SetActive(false);
            }

            if (GameControl.m_Current != null && GameControl.m_Current.m_MainSaveData != null)
            {
                m_SaveData = GameControl.m_Current.m_MainSaveData;
            }

            if (m_CloseButton != null)
            {
                SetupCloseButton();
            }

            SetupClickableText(m_PistolText, BuyPistol);
            SetupClickableText(m_ShotgunText, BuyShotgun);
            SetupClickableText(m_MachinegunText, BuyMachinegun);
            SetupClickableText(m_RPGText, BuyRPG);
            SetupClickableText(m_HealthBuffText, BuyHealthBuff);
            SetupClickableText(m_DamageBuffText, BuyDamageBuff);

            if (m_BuyWeaponPowerButton != null)
            {
                m_BuyWeaponPowerButton.onClick.AddListener(BuyWeaponPower);
            }
        }

        private void SetupClickableText(Text textElement, UnityEngine.Events.UnityAction onClickAction)
        {
            if (textElement == null || onClickAction == null)
                return;

            Button button = textElement.GetComponent<Button>();
            if (button == null)
            {
                button = textElement.gameObject.AddComponent<Button>();
            }

            button.targetGraphic = textElement;
            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            button.colors = colors;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClickAction);
        }

        private void SetupCloseButton()
        {
            if (m_CloseButton == null)
                return;

            m_CloseButton.raycastTarget = true;

            EventTrigger trigger = m_CloseButton.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = m_CloseButton.gameObject.AddComponent<EventTrigger>();
            }

            trigger.triggers.Clear();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { CloseShop(); });
            trigger.triggers.Add(entry);
        }

        private void CreateBackgroundOverlay()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
                return;

            if (m_BackgroundOverlay == null)
            {
                m_BackgroundOverlay = new GameObject("ShopBackgroundOverlay");
                m_BackgroundOverlay.transform.SetParent(canvas.transform, false);

                RectTransform rectTransform = m_BackgroundOverlay.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;

                Image overlayImage = m_BackgroundOverlay.AddComponent<Image>();
                overlayImage.color = new Color(0, 0, 0, 0.7f);
                overlayImage.raycastTarget = true;

                Button overlayButton = m_BackgroundOverlay.AddComponent<Button>();
                overlayButton.onClick.AddListener(CloseShop);
                overlayButton.transition = Selectable.Transition.None;

                m_BackgroundOverlay.transform.SetAsFirstSibling();

                m_BackgroundOverlay.SetActive(false);
            }
        }

        void Update()
        {
            if (m_IsShopOpen)
            {
                UpdateUI();

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseShop();
                }
            }
        }

        public void OpenShop()
        {
            if (m_SaveData == null && GameControl.m_Current != null && GameControl.m_Current.m_MainSaveData != null)
            {
                m_SaveData = GameControl.m_Current.m_MainSaveData;
            }

            if (m_BackgroundOverlay == null)
            {
                CreateBackgroundOverlay();
            }

            if (m_BackgroundOverlay != null)
            {
                m_BackgroundOverlay.SetActive(true);
                m_BackgroundOverlay.transform.SetAsFirstSibling();
            }

            if (m_ShopPanel == null)
                return;

            m_ShopPanel.SetActive(true);
            m_IsShopOpen = true;
            Time.timeScale = 0;

            if (m_SaveData != null)
            {
                UpdateUI();
            }
        }

        public void CloseShop()
        {
            if (m_BackgroundOverlay != null)
            {
                m_BackgroundOverlay.SetActive(false);
            }

            if (m_ShopPanel != null)
            {
                m_ShopPanel.SetActive(false);
                m_IsShopOpen = false;
                Time.timeScale = 1;

                if (m_SaveData != null && PlayerControl.MainPlayerController != null)
                {
                    m_SaveData.m_GemCount = PlayerControl.MainPlayerController.m_GemCount;
                    m_SaveData.Save();
                }
            }
        }

        private void UpdateUI()
        {
            if (PlayerControl.MainPlayerController == null || m_SaveData == null)
                return;

            if (m_GemCountText != null)
            {
                m_GemCountText.text = PlayerControl.MainPlayerController.m_GemCount.ToString();
            }

            if (m_PistolStatusText != null)
            {
                if (m_SaveData.m_HasPistol)
                {
                    m_PistolStatusText.text = "КУПЛЕНО";
                    if (m_PistolText != null)
                    {
                        Button pistolButton = m_PistolText.GetComponent<Button>();
                        if (pistolButton != null)
                        {
                            pistolButton.interactable = false;
                        }
                    }
                }
                else
                {
                    m_PistolStatusText.text = "НЕ КУПЛЕНО";
                    if (m_PistolText != null)
                    {
                        Button pistolButton = m_PistolText.GetComponent<Button>();
                        if (pistolButton != null)
                        {
                            pistolButton.interactable = PlayerControl.MainPlayerController.m_GemCount >= m_PistolPrice;
                        }
                    }
                }
            }

            if (m_PistolText != null && m_SaveData != null)
            {
                if (m_SaveData.m_HasPistol)
                {
                    m_PistolText.text = "ПИСТОЛЕТ (КУПЛЕНО)";
                }
                else
                {
                    m_PistolText.text = "ПИСТОЛЕТ";
                }
            }

            if (m_PistolPriceText != null)
            {
                m_PistolPriceText.text = m_PistolPrice.ToString();
            }

            if (m_ShotgunStatusText != null)
            {
                if (m_SaveData.m_HasShotgun)
                {
                    m_ShotgunStatusText.text = "КУПЛЕНО";
                    if (m_ShotgunText != null)
                    {
                        Button shotgunButton = m_ShotgunText.GetComponent<Button>();
                        if (shotgunButton != null)
                        {
                            shotgunButton.interactable = false;
                        }
                    }
                }
                else
                {
                    m_ShotgunStatusText.text = "НЕ КУПЛЕНО";
                    if (m_ShotgunText != null)
                    {
                        Button shotgunButton = m_ShotgunText.GetComponent<Button>();
                        if (shotgunButton != null)
                        {
                            shotgunButton.interactable = PlayerControl.MainPlayerController.m_GemCount >= m_ShotgunPrice;
                        }
                    }
                }
            }

            if (m_ShotgunText != null && m_SaveData != null)
            {
                if (m_SaveData.m_HasShotgun)
                {
                    m_ShotgunText.text = "ДРОБОВИК (КУПЛЕНО)";
                }
                else
                {
                    m_ShotgunText.text = "ДРОБОВИК";
                }
            }

            if (m_ShotgunPriceText != null)
            {
                m_ShotgunPriceText.text = m_ShotgunPrice.ToString();
            }

            if (m_WeaponPowerStatusText != null)
            {
                if (m_SaveData.m_WeaponPowerLevel >= 1)
                {
                    m_WeaponPowerStatusText.text = "КУПЛЕНО";
                    if (m_BuyWeaponPowerButton != null)
                    {
                        m_BuyWeaponPowerButton.interactable = false;
                    }
                }
                else
                {
                    m_WeaponPowerStatusText.text = "НЕ КУПЛЕНО";
                    if (m_BuyWeaponPowerButton != null)
                    {
                        m_BuyWeaponPowerButton.interactable = PlayerControl.MainPlayerController.m_GemCount >= m_WeaponPowerPrice;
                    }
                }
            }

            if (m_WeaponPowerPriceText != null)
            {
                m_WeaponPowerPriceText.text = m_WeaponPowerPrice.ToString();
            }

            if (m_MachinegunStatusText != null)
            {
                if (m_SaveData.m_HasMachinegun)
                {
                    m_MachinegunStatusText.text = "КУПЛЕНО";
                    if (m_MachinegunText != null)
                    {
                        Button machinegunButton = m_MachinegunText.GetComponent<Button>();
                        if (machinegunButton != null)
                        {
                            machinegunButton.interactable = false;
                        }
                    }
                }
                else
                {
                    m_MachinegunStatusText.text = "НЕ КУПЛЕНО";
                    if (m_MachinegunText != null)
                    {
                        Button machinegunButton = m_MachinegunText.GetComponent<Button>();
                        if (machinegunButton != null)
                        {
                            machinegunButton.interactable = PlayerControl.MainPlayerController.m_GemCount >= m_MachinegunPrice;
                        }
                    }
                }
            }

            if (m_MachinegunPriceText != null)
            {
                m_MachinegunPriceText.text = m_MachinegunPrice.ToString();
            }

            if (m_MachinegunText != null && m_SaveData != null)
            {
                if (m_SaveData.m_HasMachinegun)
                {
                    m_MachinegunText.text = "ПУЛЕМЕТ (КУПЛЕНО)";
                }
                else
                {
                    m_MachinegunText.text = "ПУЛЕМЕТ";
                }
            }

            if (m_RPGStatusText != null)
            {
                if (m_SaveData.m_HasRPG)
                {
                    m_RPGStatusText.text = "КУПЛЕНО";
                    if (m_RPGText != null)
                    {
                        Button rpgButton = m_RPGText.GetComponent<Button>();
                        if (rpgButton != null)
                        {
                            rpgButton.interactable = false;
                        }
                    }
                }
                else
                {
                    m_RPGStatusText.text = "НЕ КУПЛЕНО";
                    if (m_RPGText != null)
                    {
                        Button rpgButton = m_RPGText.GetComponent<Button>();
                        if (rpgButton != null)
                        {
                            rpgButton.interactable = PlayerControl.MainPlayerController.m_GemCount >= m_RPGPrice;
                        }
                    }
                }
            }

            if (m_RPGPriceText != null)
            {
                m_RPGPriceText.text = m_RPGPrice.ToString();
            }

            if (m_RPGText != null && m_SaveData != null)
            {
                if (m_SaveData.m_HasRPG)
                {
                    m_RPGText.text = "РПГ (КУПЛЕНО)";
                }
                else
                {
                    m_RPGText.text = "РПГ";
                }
            }

            if (m_HealthBuffStatusText != null)
            {
                int currentLevel = m_SaveData.m_HealthBuffLevel;
                if (currentLevel >= 4)
                {
                    m_HealthBuffStatusText.text = $"УРОВЕНЬ {currentLevel}/4 (МАКС)";
                    if (m_HealthBuffText != null)
                    {
                        Button healthBuffButton = m_HealthBuffText.GetComponent<Button>();
                        if (healthBuffButton != null)
                        {
                            healthBuffButton.interactable = false;
                        }
                    }
                }
                else
                {
                    m_HealthBuffStatusText.text = $"УРОВЕНЬ {currentLevel}/4";
                    if (m_HealthBuffText != null)
                    {
                        Button healthBuffButton = m_HealthBuffText.GetComponent<Button>();
                        if (healthBuffButton != null)
                        {
                            healthBuffButton.interactable = PlayerControl.MainPlayerController.m_GemCount >= m_HealthBuffPrice;
                        }
                    }
                }
            }

            if (m_HealthBuffPriceText != null)
            {
                m_HealthBuffPriceText.text = m_HealthBuffPrice.ToString();
            }

            if (m_HealthBuffText != null && m_SaveData != null)
            {
                int currentLevel = m_SaveData.m_HealthBuffLevel;
                if (currentLevel >= 4)
                {
                    m_HealthBuffText.text = "БАФФ ЗДОРОВЬЯ (МАКС)";
                }
                else
                {
                    m_HealthBuffText.text = $"БАФФ ЗДОРОВЬЯ (УР. {currentLevel})";
                }
            }

            if (m_DamageBuffStatusText != null)
            {
                int currentLevel = m_SaveData.m_DamageBuffLevel;
                if (currentLevel >= 4)
                {
                    m_DamageBuffStatusText.text = $"УРОВЕНЬ {currentLevel}/4 (МАКС)";
                    if (m_DamageBuffText != null)
                    {
                        Button damageBuffButton = m_DamageBuffText.GetComponent<Button>();
                        if (damageBuffButton != null)
                        {
                            damageBuffButton.interactable = false;
                        }
                    }
                }
                else
                {
                    m_DamageBuffStatusText.text = $"УРОВЕНЬ {currentLevel}/4";
                    if (m_DamageBuffText != null)
                    {
                        Button damageBuffButton = m_DamageBuffText.GetComponent<Button>();
                        if (damageBuffButton != null)
                        {
                            damageBuffButton.interactable = PlayerControl.MainPlayerController.m_GemCount >= m_DamageBuffPrice;
                        }
                    }
                }
            }

            if (m_DamageBuffPriceText != null)
            {
                m_DamageBuffPriceText.text = m_DamageBuffPrice.ToString();
            }

            if (m_DamageBuffText != null && m_SaveData != null)
            {
                int currentLevel = m_SaveData.m_DamageBuffLevel;
                if (currentLevel >= 4)
                {
                    m_DamageBuffText.text = "БАФФ УРОНА (МАКС)";
                }
                else
                {
                    m_DamageBuffText.text = $"БАФФ УРОНА (УР. {currentLevel})";
                }
            }
        }

        private void BuyPistol()
        {
            if (m_SaveData == null || PlayerControl.MainPlayerController == null)
                return;

            if (m_SaveData.m_HasPistol)
                return;

            if (PlayerControl.MainPlayerController.m_GemCount >= m_PistolPrice)
            {
                PlayerControl.MainPlayerController.m_GemCount -= m_PistolPrice;
                m_SaveData.m_HasPistol = true;
                m_SaveData.Save();

                if (PlayerChar.m_Current != null && PlayerChar.m_Current.m_Weapons != null && PlayerChar.m_Current.m_Weapons.Length > 0)
                {
                    if (PlayerChar.m_Current.m_Weapons[0] != null)
                    {
                        PlayerChar.m_Current.m_Weapons[0].WeaponEnable = true;
                        if (m_SaveData.m_WeaponPowerLevel >= 1)
                        {
                            PlayerChar.m_Current.m_Weapons[0].m_PowerLevel = 1;
                        }
                    }
                }

                UpdateUI();
            }
        }

        private void BuyShotgun()
        {
            if (m_SaveData == null || PlayerControl.MainPlayerController == null)
                return;

            if (m_SaveData.m_HasShotgun)
                return;

            if (PlayerControl.MainPlayerController.m_GemCount >= m_ShotgunPrice)
            {
                PlayerControl.MainPlayerController.m_GemCount -= m_ShotgunPrice;
                m_SaveData.m_HasShotgun = true;
                m_SaveData.Save();

                if (PlayerChar.m_Current != null && PlayerChar.m_Current.m_Weapons != null && PlayerChar.m_Current.m_Weapons.Length > 1)
                {
                    if (PlayerChar.m_Current.m_Weapons[1] != null)
                    {
                        PlayerChar.m_Current.m_Weapons[1].WeaponEnable = true;
                        if (m_SaveData.m_WeaponPowerLevel >= 1)
                        {
                            PlayerChar.m_Current.m_Weapons[1].m_PowerLevel = 1;
                        }
                        PlayerChar.m_Current.SetWeapon(1);
                    }
                }

                UpdateUI();
                if (UI_HUD.m_Main != null)
                {
                    UI_HUD.m_Main.UpdateWeaponIcon();
                }
            }
        }

        private void BuyWeaponPower()
        {
            if (m_SaveData == null || PlayerControl.MainPlayerController == null)
                return;

            if (m_SaveData.m_WeaponPowerLevel >= 1)
                return;

            if (PlayerControl.MainPlayerController.m_GemCount >= m_WeaponPowerPrice)
            {
                PlayerControl.MainPlayerController.m_GemCount -= m_WeaponPowerPrice;
                m_SaveData.m_WeaponPowerLevel = 1;
                m_SaveData.Save();

                if (PlayerChar.m_Current != null)
                {
                    PlayerChar.m_Current.SetWeaponPowerLevel(1);
                }

                UpdateUI();
            }
        }

        private void BuyMachinegun()
        {
            if (m_SaveData == null || PlayerControl.MainPlayerController == null)
                return;

            if (m_SaveData.m_HasMachinegun)
                return;

            if (PlayerControl.MainPlayerController.m_GemCount >= m_MachinegunPrice)
            {
                PlayerControl.MainPlayerController.m_GemCount -= m_MachinegunPrice;
                m_SaveData.m_HasMachinegun = true;
                m_SaveData.Save();

                if (PlayerChar.m_Current != null && PlayerChar.m_Current.m_Weapons != null && PlayerChar.m_Current.m_Weapons.Length > 2)
                {
                    if (PlayerChar.m_Current.m_Weapons[2] != null)
                    {
                        PlayerChar.m_Current.m_Weapons[2].WeaponEnable = true;
                        if (m_SaveData.m_WeaponPowerLevel >= 1)
                        {
                            PlayerChar.m_Current.m_Weapons[2].m_PowerLevel = 1;
                        }
                        PlayerChar.m_Current.SetWeapon(2);
                    }
                }

                UpdateUI();
                if (UI_HUD.m_Main != null)
                {
                    UI_HUD.m_Main.UpdateWeaponIcon();
                }
            }
        }

        private void BuyRPG()
        {
            if (m_SaveData == null || PlayerControl.MainPlayerController == null)
                return;

            if (m_SaveData.m_HasRPG)
                return;

            if (PlayerControl.MainPlayerController.m_GemCount >= m_RPGPrice)
            {
                PlayerControl.MainPlayerController.m_GemCount -= m_RPGPrice;
                m_SaveData.m_HasRPG = true;
                m_SaveData.Save();

                if (PlayerChar.m_Current != null && PlayerChar.m_Current.m_Weapons != null && PlayerChar.m_Current.m_Weapons.Length > 3)
                {
                    if (PlayerChar.m_Current.m_Weapons[3] != null)
                    {
                        PlayerChar.m_Current.m_Weapons[3].WeaponEnable = true;
                        if (m_SaveData.m_WeaponPowerLevel >= 1)
                        {
                            PlayerChar.m_Current.m_Weapons[3].m_PowerLevel = 1;
                        }
                        PlayerChar.m_Current.SetWeapon(3);
                    }
                }

                UpdateUI();
                if (UI_HUD.m_Main != null)
                {
                    UI_HUD.m_Main.UpdateWeaponIcon();
                }
            }
        }

        private void BuyHealthBuff()
        {
            if (m_SaveData == null || PlayerControl.MainPlayerController == null)
                return;

            if (m_SaveData.m_HealthBuffLevel >= 4)
                return;

            if (PlayerControl.MainPlayerController.m_GemCount >= m_HealthBuffPrice)
            {
                PlayerControl.MainPlayerController.m_GemCount -= m_HealthBuffPrice;
                m_SaveData.m_HealthBuffLevel++;
                m_SaveData.Save();
                UpdateUI();
            }
        }

        private void BuyDamageBuff()
        {
            if (m_SaveData == null || PlayerControl.MainPlayerController == null)
                return;

            if (m_SaveData.m_DamageBuffLevel >= 4)
                return;

            if (PlayerControl.MainPlayerController.m_GemCount >= m_DamageBuffPrice)
            {
                PlayerControl.MainPlayerController.m_GemCount -= m_DamageBuffPrice;
                m_SaveData.m_DamageBuffLevel++;
                m_SaveData.Save();
                UpdateUI();
            }
        }
    }
}

