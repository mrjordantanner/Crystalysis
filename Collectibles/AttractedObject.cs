using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class AttractedObject : MonoBehaviour
    {
        [HideInInspector]
        public bool isAttracted;

        GameObject attractionTarget;
        Rigidbody rb;
        BoxCollider collider;

        [HideInInspector]
        public float turningSensitivity, attractionForce;


        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (isAttracted && attractionTarget != null)
            {
                Vector3 relativePos = attractionTarget.transform.position - transform.position;
                Quaternion rot = Quaternion.LookRotation(relativePos);
                //transform.rotation = Quaternion.Slerp(transform.rotation, rot, turningSensitivity);
                transform.rotation = rot;
                rb.AddForce(transform.forward.normalized * attractionForce * Time.deltaTime, ForceMode.Acceleration);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Attractor"))
            {
                isAttracted = true;
                attractionTarget = other.gameObject;
                //collider.enabled = false;
            }
        }

    }
}
