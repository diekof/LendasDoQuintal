using UnityEngine;

namespace LendasDoQuintal.Camera
{
    public class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private Vector2 minBounds = new Vector2(-20f, -5f);
        [SerializeField] private Vector2 maxBounds = new Vector2(40f, 10f);
        [SerializeField] private float referenceOrthographicSize = 5.4f;
        [SerializeField] private float targetAspect = 16f / 9f;

        private Vector3 velocity;
        private UnityEngine.Camera followCamera;

        private void LateUpdate()
        {
            ApplyFixedView();

            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                target = player != null ? player.transform : null;
            }

            if (target == null)
            {
                return;
            }

            Vector3 desired = target.position + offset;
            desired.x = Mathf.Clamp(desired.x, minBounds.x, maxBounds.x);
            desired.y = Mathf.Clamp(desired.y, minBounds.y, maxBounds.y);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }

        public void Configure(Transform newTarget)
        {
            target = newTarget;
        }

        public void Configure(Transform newTarget, Vector2 newMinBounds, Vector2 newMaxBounds, Vector3 newOffset)
        {
            target = newTarget;
            minBounds = newMinBounds;
            maxBounds = newMaxBounds;
            offset = newOffset;
        }

        public void Configure(Transform newTarget, Vector2 newMinBounds, Vector2 newMaxBounds, Vector3 newOffset, float newOrthographicSize, float newTargetAspect)
        {
            Configure(newTarget, newMinBounds, newMaxBounds, newOffset);
            referenceOrthographicSize = newOrthographicSize;
            targetAspect = newTargetAspect;
            ApplyFixedView();
        }

        private void ApplyFixedView()
        {
            if (followCamera == null)
            {
                followCamera = GetComponent<UnityEngine.Camera>();
            }

            if (followCamera == null || !followCamera.orthographic)
            {
                return;
            }

            float currentAspect = Mathf.Max(0.01f, followCamera.aspect);
            float targetWidth = referenceOrthographicSize * 2f * targetAspect;
            followCamera.orthographicSize = Mathf.Max(referenceOrthographicSize, targetWidth / (2f * currentAspect));
        }
    }
}
