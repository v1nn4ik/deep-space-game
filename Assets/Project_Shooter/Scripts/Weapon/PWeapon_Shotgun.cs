using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class PWeapon_Shotgun : Weapon_Base
    {
        [Header("Цветовая заливка")]
        public Color m_WeaponColor = new Color(1f, 0.2f, 0.2f, 1f); // Красный цвет

        private Renderer[] m_Renderers;
        
        
        //audio
        private AudioClip fireClip;
        
        private AudioSource fireAudioSource;

        // Start is called before the first frame update
        void Start()
        {
            // Находим все Renderer компоненты в дочерних объектах
            m_Renderers = GetComponentsInChildren<Renderer>();
            ApplyColor();

            fireClip = Resources.Load<AudioClip>("Audio/light_blast_5");
            fireAudioSource = gameObject.AddComponent<AudioSource>();
            fireAudioSource.clip = fireClip;
            fireAudioSource.playOnAwake = false;
            fireAudioSource.volume = 0.1f;
            
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

        // Update is called once per frame
        void Update()
        {
            if (m_PowerLevel == 0)
            {
                FireDelay = .3f;
            }
            else if (m_PowerLevel == 1)
            {
                FireDelay = .25f;
            }


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

            //Input_FireHold = false;
        }

        public override void FireWeapon()
        {

            GameObject obj;

            if (m_PowerLevel == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    obj = Instantiate(BulletPrefab);
                    obj.transform.position = m_FirePoint.position;
                    obj.transform.forward = Quaternion.Euler(0,-6+ i * 6, 0) * m_FirePoint.forward;
                    Projectile_Base proj = obj.GetComponent<Projectile_Base>();
                    proj.Creator = m_Owner;
                    proj.Speed = ProjectileSpeed;
                    proj.Damage = Damage;
                    proj.m_Range = Range;
                    Destroy(obj, 5);
                }

            }
            else if (m_PowerLevel == 1)
            {
                for (int i = 0; i < 6; i++)
                {
                    obj = Instantiate(BulletPrefab);
                    obj.transform.position = m_FirePoint.position;
                    obj.transform.forward = Quaternion.Euler(0, -30+i * 10, 0) * m_FirePoint.forward;
                    Projectile_Base proj = obj.GetComponent<Projectile_Base>();
                    proj.Creator = m_Owner;
                    proj.Speed = ProjectileSpeed;
                    proj.Damage = Damage;
                    proj.m_Range = Range;
                    Destroy(obj, 5);
                }
            }

            obj = Instantiate(EffectPrefab);
            obj.transform.SetParent(m_ParticlePoint);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.forward = m_ParticlePoint.forward;
            Destroy(obj, 3);
            
            if (fireClip != null && fireAudioSource != null)
            {
                fireAudioSource.PlayOneShot(fireClip);
            }
        }
    }
}