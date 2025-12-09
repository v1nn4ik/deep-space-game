using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class PWeapon_RPG : Weapon_Base
    {
        [Header("Цветовая заливка")]
        public Color m_WeaponColor = new Color(0.6f, 0.2f, 1f, 1f); // Фиолетовый цвет

        private Renderer[] m_Renderers;

        void Start()
        {
            m_Renderers = GetComponentsInChildren<Renderer>();
            ApplyColor();
        }

        private void ApplyColor()
        {
            if (m_Renderers != null)
            {
                foreach (Renderer renderer in m_Renderers)
                {
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = m_WeaponColor;
                    }
                }
            }
        }

        protected override void Update()
        {
            if (!WeaponEnable)
                return;
            
            FireDelay = 1.5f;

            FireDelayTimer -= Time.deltaTime;
            if (FireDelayTimer <= 0)
                FireDelayTimer = 0;

            RecoilTimer -= 10 * Time.deltaTime;
            if (RecoilTimer <= 0)
                RecoilTimer = 0;

            if (Input_FireHold)
            {
                if (FireDelayTimer == 0)
                {
                    FireWeapon();
                    FireDelayTimer = FireDelay;
                    RecoilTimer = 1f;
                }
            }
        }

        public override void FireWeapon()
        {
            if (BulletPrefab == null || m_FirePoint == null)
                return;
            
            GameObject obj = Instantiate(BulletPrefab);
            obj.transform.position = m_FirePoint.position;
            obj.transform.forward = m_FirePoint.forward;
            Projectile_Base proj = obj.GetComponent<Projectile_Base>();
            if (proj != null)
            {
                proj.Creator = m_Owner;
                proj.Speed = ProjectileSpeed;
                proj.Damage = Damage * 3f;
                proj.m_Range = Range;
            }
            Destroy(obj, 5);

            obj = Instantiate(EffectPrefab);
            obj.transform.SetParent(m_ParticlePoint);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.forward = m_ParticlePoint.forward;
            Destroy(obj, 3);
        }
    }
}
