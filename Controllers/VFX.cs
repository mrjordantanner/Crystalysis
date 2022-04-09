using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Scripts
{
    public class VFX : MonoBehaviour
    {
        #region Singleton
        public static VFX Instance;
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

        [HideInInspector]
        public GameObject VFXContainer;

        private void Start()
        {
            VFXContainer = new GameObject("VFXContainer");
        }

        public void Explode(Rigidbody rb, float force, float radius, float upwards)
        {

            StartCoroutine(AddExplosionForce(rb, force, radius, upwards));
        }

        IEnumerator AddExplosionForce(Rigidbody rb, float force, float radius, float upwards)
        {
            rb.gameObject.transform.SetParent(VFXContainer.transform);
            yield return new WaitForSeconds(0f);
            rb.AddExplosionForce(force, rb.transform.position, radius, upwards, ForceMode.Impulse);
        }

    }
}
