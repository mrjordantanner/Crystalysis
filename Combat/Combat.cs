using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace Assets.Scripts
{
    /// <summary>
    /// Controls interactions between Player and Crystals or other destructible objects
    /// </summary>
    public class Combat : MonoBehaviour
    {
        #region Singleton
        public static Combat Instance;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
        #endregion


        public float hitShakeDuration, hitShakeStrength;
        public int hitShakeVibrato;

        public float explodeShakeDuration, explodeShakeStrength;
        public int explodeShakeVibrato;

        [HideInInspector]
        public PlayerCharacter player;

        [HideInInspector]
        public bool hitEffectsActive;
        float hitEffectsTimer;

        public float damageRandomization = 0.1f;

        private void Update()
        {
            HandleHitEffects();
        }

        void HandleHitEffects()
        {
            if (hitEffectsActive)
            {
                if (hitEffectsTimer < player.hitEffectsDuration)
                {
                    hitEffectsTimer += Time.deltaTime;
                }

                if (hitEffectsTimer >= player.hitEffectsDuration)
                {
                    hitEffectsTimer = 0f;
                    hitEffectsActive = false;
                    RemoveSlowEffect();
                }
            }
        }

        public void PlayerHit(Crystal crystal)
        {
            if (player.isDead || player.invulnerable)
            { 
                return;
            }

            if (player != null)
            {
                var damage = RandomizeDamage(crystal.damage);
                player.TakeDamage(damage, true);
                hitEffectsActive = true;
                ApplySlowEffect();
            }
        }

        Tween hitShake;

        public void CrystalHit(Crystal crystal, Weapon weapon)
        {
            var damage = RandomizeDamage(weapon.damage);
            crystal.TakeDamage(weapon.damage, true);

            hitShake.Kill();
            hitShake = Camera.main.DOShakePosition(
                hitShakeDuration,
                hitShakeStrength,
                hitShakeVibrato,
                90,
                false);

            AudioManager.Instance.Play("Hit-1");
        }

        public void DestructibleHit(GameObject target)
        {
            var destructible = target.GetComponent<Destructible>();
            destructible.Explode();
        }

        float RandomizeDamage(float damage)
        {
            var maxDamage = damage * (1 + damageRandomization);
            var minDamage = damage * damageRandomization;
            return Random.Range(minDamage, maxDamage);
        }

        void ApplySlowEffect()
        {
            player.isSlowed = true;
            player.moveSpeed *= player.slowAmount;

            float duration = player.attackController.attackDuration;
            duration *= player.attackController.attackSlowdown;
            player.attackController.attackDuration = duration;
        }

        void RemoveSlowEffect()
        {
            player.isSlowed = false;
            player.moveSpeed = player.baseMoveSpeed;

            player.attackController.attackDuration = 
                player.attackController.baseAttackDuration;
        }

        public float RandomizeValueWithinRange(float maxValue, float valueRange)
        {
            var value = Random.Range(maxValue * valueRange, maxValue);
            return value;
        }


    }



}