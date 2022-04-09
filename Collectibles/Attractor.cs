using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts
{
    public class Attractor : MonoBehaviour
    {
        public float pickupRadius = 15f;
        SphereCollider sphereCollider;
        public float attractionForce = 10f;
        public float turningSensitivity = 500f;

        private void Start()
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = pickupRadius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("HealingShard"))
            {
                var attractedObject = other.gameObject.GetComponent<AttractedObject>();
                attractedObject.attractionForce = attractionForce;
                attractedObject.turningSensitivity = turningSensitivity;

            }

        }

    }
}
