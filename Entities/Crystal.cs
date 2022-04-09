using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts
{
    /// <summary>
    /// Sits on Crystal GameObjects and dictates Crystal level, health, collisions, lifespan, and effect amount.
    /// </summary>
    public class Crystal : MonoBehaviour
    {
        public float currentHP;
        public float maxHP = 100f;
        public float damage;
        public bool dealDamage;

        public enum Level { One, Two, Three };
        public Level level;

        float levelUpTimer;
        public float effectTimer;
        public bool isDestroyed;
        
        public GameObject model;

        public Animator animator;
        PlayerCharacter player;
        MeshRenderer renderer;
        BoxCollider collider;

        [HideInInspector]
        public CrystalNode node;
        public HealthBar healthBar;

        public GameObject explosionPrefab;
        public string explosionSoundEffect;

        bool playerContact;
        float playerContactTimer;



        private void Start()
        {
            player = FindObjectOfType<PlayerCharacter>();
            currentHP = maxHP;
            healthBar.Set(maxHP, currentHP);
            renderer = GetComponent<MeshRenderer>();
            collider = GetComponent<BoxCollider>();

            ResetTimers();

            gameObject.name = "Crystal-Lv1";
        }


        private void Update()
        {
            if (playerContact) HandleContactDamageTimer();

            if (level != Level.Three)
            {
                HandleLevelUpTimer();
            }

            HandleEffectTimer();

        }

        void HandleContactDamageTimer()
        {
            playerContactTimer -= Time.deltaTime;
            if (playerContactTimer <= 0) playerContact = false;

        }

        void ResetTimers()
        {
            effectTimer = CrystalController.Instance.effectTick;
            levelUpTimer = CrystalController.Instance.currentLevelUpTime;
        }


        void HandleLevelUpTimer()
        {
            levelUpTimer -= Time.deltaTime;

            if (levelUpTimer <= 0)
            {
                LevelUp();
                levelUpTimer = CrystalController.Instance.currentLevelUpTime;
            }
        }


        void HandleEffectTimer()
        {
            effectTimer -= Time.deltaTime;

            if (effectTimer <= 0)
            {
                InvokeEffect();
                effectTimer = CrystalController.Instance.effectTick;
            }

        }

        void InvokeEffect()
        {
            if (player != null)
            {
                if (dealDamage)
                {
                    player.TakeDamage(CrystalController.Instance.damage[level], false);
                }

            }

        }

        void LevelUp()
        {
            var offset = new Vector3(0f, 4f, 0f);
            GameObject newModel;

            switch (level)
            {
                case Level.One:
                    level = Level.Two;
                    newModel = Instantiate(CrystalController.Instance.crystalModelLevel2, node.transform.position, Quaternion.identity, gameObject.transform);
                    newModel.transform.DOMove(node.transform.position + offset, 1f);
                    StartCoroutine(AssignNewStats(newModel));
                    gameObject.name = "Crystal-Lv2";
                    break;

                case Level.Two:
                    level = Level.Three;
                    newModel = Instantiate(CrystalController.Instance.crystalModelLevel3, node.transform.position, Quaternion.identity, gameObject.transform);
                    newModel.transform.DOMove(node.transform.position + offset, 1f);
                    StartCoroutine(AssignNewStats(newModel));
                    gameObject.name = "Crystal-Lv3";
                    break;
            }

        }

        IEnumerator AssignNewStats(GameObject newModel)
        {
            var ratio = currentHP / maxHP;

            damage = CrystalController.Instance.damage[level];
            maxHP = CrystalController.Instance.maxHP[level];

            var newCurrentHP = maxHP * ratio;
            currentHP = newCurrentHP;
            healthBar.Set(maxHP, currentHP);

            Destroy(model, 1f);

            yield return new WaitForSeconds(1.1f);

            model = newModel; 
        }

        public void TakeDamage(float amount, bool shouldHitFlash)
        {
            if (currentHP <= 0 || isDestroyed)
            {
                currentHP = 0;
                return;
            }

            currentHP -= (int)(amount);

            healthBar.TakeDamage(amount, currentHP);

            if (currentHP <= 0 && !isDestroyed)
            {
                Explode();
            }
        }

        Tween explosionShake;

        public void Explode()
        {
            var randomRange = CrystalController.Instance.explosionSize[level];
            var crystalChunks = Random.Range(randomRange.x, randomRange.y);

            if (renderer != null)
            {
                renderer.enabled = false;
            }

            if (collider != null)
            {
                collider.enabled = false;
            }

            for (int i = 0; i < crystalChunks; i++)
            {
                var chunk = Instantiate(CrystalController.Instance.crystalChunkPrefab, transform.position, Quaternion.identity);
                var rb = chunk.GetComponent<Rigidbody>();
                VFX.Instance.Explode(
                    rb,
                    CrystalController.Instance.shardExplosionForce,
                    CrystalController.Instance.shardExplosionRadius,
                    CrystalController.Instance.shardUpwardsModifier);
            }

            RollForShardDrop();

            CrystalController.Instance.crystalsInPlay--;
            HUD.Instance.UpdateCrystalCount(CrystalController.Instance.crystalsInPlay);

            explosionShake = Camera.main.DOShakePosition(
                Combat.Instance.explodeShakeDuration,
                Combat.Instance.explodeShakeStrength,
                Combat.Instance.explodeShakeVibrato,
                90,
                true);

            AudioManager.Instance.Play("Shatter-1");

            Destroy(gameObject);

        }

        public void AddExplosionForce(Rigidbody rb)
        {
            rb.AddExplosionForce(
                CrystalController.Instance.shardExplosionForce,
                transform.position,
                CrystalController.Instance.shardExplosionRadius,
                CrystalController.Instance.shardUpwardsModifier,
                ForceMode.Impulse);
        }

        void RollForShardDrop()
        {
            Vector2 range;
            var drop = Random.Range(0, 1f);

            if (drop <= CrystalController.Instance.healingShardDropRate)
            {
                range = CrystalController.Instance.dropRange[level];

                var amount = Random.Range(range.x, range.y);

                DropShards((int)amount);
            }
        }

        void DropShards(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var shard = Instantiate(CrystalController.Instance.healingShardPrefab, transform.position, Quaternion.identity);
                var rb = shard.GetComponent<Rigidbody>();
                AddExplosionForce(rb);

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (playerContact || Combat.Instance.hitEffectsActive) return;

                playerContact = true;
                Combat.Instance.PlayerHit(this);

            }

        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (playerContact || Combat.Instance.hitEffectsActive) return;

                playerContact = true;
                Combat.Instance.PlayerHit(this);

            }
        }
    }
}
