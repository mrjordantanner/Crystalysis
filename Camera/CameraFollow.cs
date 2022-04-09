using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public class CameraFollow : MonoBehaviour
	{
		[HideInInspector]
		public Transform target;
		public float smoothing = 5f;
		[HideInInspector]
		public Vector3 offset;

		public Transform cameraStartPosition;

        void Start()
        {
			//cameraStartPosition = transform.position;
		}

		public void ResetPosition()
        {
			transform.position = cameraStartPosition.position;
		}

        void LateUpdate()
		{
			if (target == null) return;
			Vector3 targetCamPos = target.position + offset;
			transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
		}
	}
}