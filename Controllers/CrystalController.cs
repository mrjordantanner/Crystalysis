using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace Assets.Scripts
{
    public class CrystalController : MonoBehaviour
    {
        #region Singleton
        public static CrystalController Instance;
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

        public bool debugMode, dealDamage;

        bool spawning;
        public CrystalNode[] nodes;
        public float spawnTimer;
        public Vector2 initialCrystalRange = new Vector2(5, 15);

        public float effectTick = 1f;

        [HideInInspector]
        public GameObject
            crystalPrefabLevel1,
            crystalPrefabLevel2,
            crystalPrefabLevel3,
            crystalModelLevel1,
            crystalModelLevel2,
            crystalModelLevel3,
            healingShardPrefab,
            crystalChunkPrefab;

        public float spawnTimeDelta = 0.05f;
        public float levelUpTimeDelta = 0.1f;

        public float
            baseSpawnTime = 5f,
            spawnTimeLimit = 2.5f,
            currentSpawnTime;

        public float
            baseLevelUpTime = 10f,
            levelUpTimeLimit = 5f,
            currentLevelUpTime;

        public Dictionary<Crystal.Level, float> damage;
        public Dictionary<Crystal.Level, Vector2> dropRange;
        public Dictionary<Crystal.Level, float> maxHP;
        public Dictionary<Crystal.Level, Vector2> explosionSize;

        float
            damageLevel1 = 2f,
            damageLevel2 = 4f,
            damageLevel3 = 6f;

        float
            maxHP_level1 = 60f,
            maxHP_level2 = 100f,
            maxHP_level3 = 150f;

        Vector2
            healingShardDropRange_level1 = new Vector2(1, 3),
            healingShardDropRange_level2 = new Vector2(3, 6),
            healingShardDropRange_level3 = new Vector2(6, 12);

        Vector2
            explosionSize_level1 = new Vector2(50, 75),
            explosionSize_level2 = new Vector2(75, 150),
            explosionSize_level3 = new Vector2(150, 300);

        public float healingShardDropRate = 0.33f;
        public float shardExplosionForce = 2f;
        public float shardExplosionRadius = 2f;
        public float shardUpwardsModifier = 2f;

        [HideInInspector]
        public int crystalsInPlay;

        public float spawnDelay = 3f;
        float spawnDelayTimer;

        public float difficultyInterval = 5f;
        float difficultyTimer;

        public void Init()
        {
            DestroyAllCrystals();

            nodes = FindObjectsOfType<CrystalNode>();

            LoadResources();
            BuildDictionaries();

            crystalsInPlay = 0;

            spawnDelayTimer = spawnDelay;
            spawning = false;

            currentSpawnTime = baseSpawnTime;
            spawnTimer = baseSpawnTime;

            currentLevelUpTime = baseLevelUpTime;

            difficultyTimer = difficultyInterval;

            if (!debugMode)
            {
                Invoke("EnableSpawning", spawnDelay);
            }
        }

        void LoadResources()
        {
            crystalPrefabLevel1 = Resources.Load<GameObject>("Crystals/Crystal-Lv1");
            crystalPrefabLevel2 = Resources.Load<GameObject>("Crystals/Crystal-Lv2");
            crystalPrefabLevel3 = Resources.Load<GameObject>("Crystals/Crystal-Lv3");

            crystalModelLevel1 = Resources.Load<GameObject>("Crystals/Models/Crystal-Lv1-Model");
            crystalModelLevel2 = Resources.Load<GameObject>("Crystals/Models/Crystal-Lv2-Model");
            crystalModelLevel3 = Resources.Load<GameObject>("Crystals/Models/Crystal-Lv3-Model");

            healingShardPrefab = Resources.Load<GameObject>("Collectibles/Healing Shard");
            crystalChunkPrefab = Resources.Load<GameObject>("Crystals/Crystal Chunk");
        }

        void BuildDictionaries()
        {
            damage = new Dictionary<Crystal.Level, float>();
            dropRange = new Dictionary<Crystal.Level, Vector2>();
            maxHP = new Dictionary<Crystal.Level, float>();
            explosionSize = new Dictionary<Crystal.Level, Vector2>();

            damage.Add(Crystal.Level.One, damageLevel1);
            damage.Add(Crystal.Level.Two, damageLevel2);
            damage.Add(Crystal.Level.Three, damageLevel3);

            maxHP.Add(Crystal.Level.One, maxHP_level1);
            maxHP.Add(Crystal.Level.Two, maxHP_level2);
            maxHP.Add(Crystal.Level.Three, maxHP_level3);

            dropRange.Add(Crystal.Level.One, healingShardDropRange_level1);
            dropRange.Add(Crystal.Level.Two, healingShardDropRange_level2);
            dropRange.Add(Crystal.Level.Three, healingShardDropRange_level3);

            explosionSize.Add(Crystal.Level.One, explosionSize_level1);
            explosionSize.Add(Crystal.Level.Two, explosionSize_level2);
            explosionSize.Add(Crystal.Level.Three, explosionSize_level3);
        }

        private void Update()
        {
            if (!debugMode)
            {
                HandleSpawnTimer();
            }

            HandleDifficultyTimer();
        }

        void HandleDifficultyTimer()
        {
            difficultyTimer -= Time.deltaTime;

            if (difficultyTimer <= 0)
            {
                IncreaseSpawnRate();
                IncreaseLevelUpRate();
                difficultyTimer = difficultyInterval;
            }
        }


        void EnableSpawning()
        {
            spawning = true;
            SpawnInitialCrystals();
        }

        void HandleSpawnTimer()
        {
            if (!spawning) return;

            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                var node = GetRandomNode();

                if (node != null)
                {
                    SpawnNewCrystal(node);
                }

                spawnTimer = currentSpawnTime;
            }
        }

        void IncreaseSpawnRate()
        {
            if (currentSpawnTime <= spawnTimeLimit) return;

            currentSpawnTime -= spawnTimeDelta;
            spawnTimer = currentSpawnTime;
        }

        void IncreaseLevelUpRate()
        {
            if (currentLevelUpTime <= levelUpTimeLimit) return;

            currentLevelUpTime -= levelUpTimeDelta;
        }

        List<CrystalNode> GetAvailableNodes()
        {
            var availableNodes = new List<CrystalNode>();

            foreach (var node in nodes)
            {
                if (node.crystal == null)
                {
                    availableNodes.Add(node);
                }
            }

            return availableNodes;
        }

        CrystalNode GetRandomNode()
        {
            var availableNodes = GetAvailableNodes();

            if (availableNodes.Count > 0)
            {
                var randomIndex = Random.Range(0, availableNodes.Count);
                var node = availableNodes[randomIndex];

                if (node.crystal == null)
                {
                    return node;
                }
            }

            return null;
        }


        void SpawnNewCrystal(CrystalNode node)
        {
            if (node != null)
            {
                var offset = new Vector3(0f, 4f, 0f);
                var newCrystal = Instantiate(crystalPrefabLevel1, node.transform.position, Quaternion.identity);
                newCrystal.transform.DOMove(node.transform.position + offset, 1f);
                var crystal = newCrystal.GetComponent<Crystal>();
                node.crystal = crystal;
                crystal.node = node;
                crystal.dealDamage = dealDamage;

                crystalsInPlay++;
                HUD.Instance.UpdateCrystalCount(crystalsInPlay);
            }
        }

        public void DestroyAllCrystals()
        {
            var crystals = FindObjectsOfType<Crystal>();

            foreach (var crystal in crystals)
            {
                DestroyImmediate(crystal.gameObject);
            }
        }

        void SpawnInitialCrystals()
        {
            var availableNodes = GetAvailableNodes();
            var amount = Random.Range(initialCrystalRange.x, initialCrystalRange.y);

            for (int i = 0; i < amount; i++)
            {
                var index = Random.Range(0, availableNodes.Count);
                var randomNode = availableNodes[index];

                SpawnNewCrystal(randomNode);
            }
        }
    }
}
