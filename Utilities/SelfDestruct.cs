using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class SelfDestruct : MonoBehaviour
    {
        public float lifeSpan = 3f;

        [Range(0,1)]
        public float randomization = 0.5f;

        private void Start()
        {
            var randomTime = Random.Range(lifeSpan * randomization, lifeSpan * (1 + randomization));
            Destroy(gameObject, randomTime);
        }
    }
}
