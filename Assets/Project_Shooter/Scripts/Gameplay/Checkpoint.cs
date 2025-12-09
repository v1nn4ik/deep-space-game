using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class Checkpoint : MonoBehaviour
    {
        public int m_CheckpointNumber = 0;
        [HideInInspector]
        public bool m_IsActivated = false;
        public Transform m_SpawnPoint;

        public GameObject[] m_Bases;

        public GameObject m_ActivateParticle;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!m_IsActivated)
            {
                if (Vector3.Distance(PlayerChar.m_Current.transform.position,transform.position)<=6)
                {
                    Activate();
                }
            }
        }

        public void Activate()
        {
            if (!m_IsActivated)
            {
                m_IsActivated = true;
                m_Bases[0].SetActive(false);
                m_Bases[1].SetActive(true);
                GameControl.m_Current.HandleCheckpoint(m_CheckpointNumber);

                GameObject obj = Instantiate(m_ActivateParticle);
                obj.transform.position = transform.position + new Vector3(0, .3f, 0);
                Destroy(obj, 3);

                // Открываем магазин при активации чекпоинта
                OpenShopMenu();
            }
        }

        private void OpenShopMenu()
        {
            Shooter.UI.ShopMenu shopMenu = Shooter.UI.ShopMenu.m_Main;
            
            // Если ShopMenu не найден статически, пытаемся найти его в сцене
            if (shopMenu == null)
            {
                shopMenu = FindObjectOfType<Shooter.UI.ShopMenu>();
                if (shopMenu != null)
                {
                    Debug.Log("Checkpoint: ShopMenu найден через FindObjectOfType");
                }
            }

            // Если все еще не найден, создаем его автоматически на объекте GameUI
            if (shopMenu == null)
            {
                GameObject gameUI = GameObject.Find("GameUI");
                if (gameUI != null)
                {
                    shopMenu = gameUI.GetComponent<Shooter.UI.ShopMenu>();
                    if (shopMenu == null)
                    {
                        shopMenu = gameUI.AddComponent<Shooter.UI.ShopMenu>();
                        Debug.Log("Checkpoint: ShopMenu автоматически добавлен на GameUI. НЕОБХОДИМО настроить UI элементы в Inspector!");
                    }
                }
                else
                {
                    Debug.LogError("Checkpoint: GameUI не найден в сцене! Создайте GameObject с компонентом ShopMenu вручную.");
                    return;
                }
            }

            if (shopMenu != null)
            {
                shopMenu.OpenShop();
            }
            else
            {
                Debug.LogError("Checkpoint: Не удалось создать или найти ShopMenu!");
            }
        }
    }
}
