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

        private Vector3 velocity;

        private void LateUpdate()
        {
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
    }
}
