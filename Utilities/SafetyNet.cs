using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class SafetyNet : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //Debug.Log("Net detected player");
                //other.gameObject.transform.parent.position = GameManager.Instance.playerSpawnPoint.position;

                other.gameObject.GetComponent<PlayerCharacter>().parentObject.transform.
                    DOMove(GameManager.Instance.playerSpawnPoint.position, 0f);

            }

            if (other.CompareTag("HealingShard") || other.CompareTag("CrystalChunk"))
            {
                Destroy(other.gameObject);
            }
        }

    }
}
