using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Assets.Scripts
{
    public class HealthBar : MonoBehaviour
    {
        [HideInInspector] public CanvasGroup canvasGroup;
        public Slider healthBar;
        public Slider deltaHealthBar;
        Canvas canvas;

        float changeDuration = 0.35f;
        float damageTimer = 0f;
        float damageTimerDuration = 0.75f;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }

            ResetDamageTimer();
        }

        private void Update()
        {
            damageTimer -= Time.deltaTime;

            if (damageTimer <= 0)
            {
                ResetDamageTimer();

                if (healthBar.value != deltaHealthBar.value)
                {
                    TweenDeltaHealthBar();
                }
            }
        }

        public void Set(float maxHP, float currentHP)
        {
            deltaHealthBar.maxValue = maxHP;
            healthBar.maxValue = maxHP;
            healthBar.value = currentHP;
        }

        public void TakeDamage(float amount, float currentHP)
        {
            canvasGroup.alpha = 1;
            ResetDamageTimer();
            healthBar.value = currentHP;
            TweenDeltaHealthBar();
        }

        public void TweenDeltaHealthBar()
        {
            var healthTarget = healthBar.value;
            var healthBarTween = DOTween.To(() =>
                deltaHealthBar.value, x => deltaHealthBar.value = x, healthTarget, changeDuration)
                    .SetEase(Ease.OutSine);
        }

        public void RefillHealthBar()
        {
            healthBar.value = 0;
            deltaHealthBar.value = 0;
            var maxValue = healthBar.maxValue;
            var healthBarTween = DOTween.To(() =>
            healthBar.value, x => healthBar.value = x, maxValue, CrystalController.Instance.spawnDelay)
                .SetEase(Ease.OutSine);
        }
        public void ResetDamageTimer()
        {
            damageTimer = damageTimerDuration;
        }

    }

}
