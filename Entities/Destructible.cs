using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Destructible : MonoBehaviour
    {
        public GameObject explosionPrefab;
        public string soundEffect;

        public void Explode()
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation, VFX.Instance.VFXContainer.transform);
        }
    }
}
