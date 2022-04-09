using UnityEngine;

namespace Assets.Scripts
{
    public class Collector : MonoBehaviour
    {
        public PlayerCharacter player;

        float sfxInterval = 0.1f;
        float timeLastCollected;

        private void Update()
        {
            timeLastCollected += Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (player.isDead) return;

            if (other.CompareTag("HealingShard"))
            {
                // Prevent sfx from stacking in volume
                if (timeLastCollected >= sfxInterval)
                {
                    AudioManager.Instance.Play("Short-Chord-1");
                }
                timeLastCollected = 0;

                var shard = other.gameObject.GetComponent<HealingShard>();
                player.Heal(shard.healValue);
                Destroy(other.gameObject);


            }
        }

    }
}
