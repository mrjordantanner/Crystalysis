using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Weapon : MonoBehaviour
    {
        public float damage = 25f;

        [HideInInspector]
        public GameObject CurrentTarget, PreviousTarget;

        SphereCollider collider;

        public GameObject[] ImpactEffects;

        void Update()
        {
            PreviousTarget = null;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Crystal"))
            {
                CurrentTarget = other.gameObject;

                var crystal = CurrentTarget.GetComponent<Crystal>();
                if (!crystal.isDestroyed)
                {
                    Combat.Instance.CrystalHit(crystal, this);
                }

                PreviousTarget = CurrentTarget;

            }

            if (other.gameObject.CompareTag("Destructible"))
            {
                CurrentTarget = other.gameObject;
                Combat.Instance.DestructibleHit(CurrentTarget);
            }

        }

    }



}