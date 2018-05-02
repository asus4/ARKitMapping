using UnityEngine;

namespace AppKit
{
    public class CameraOffset : MonoBehaviour
    {
        public Camera _camera;
        public Transform cameraContainer;
        public KeyCode triggerKey = KeyCode.Space;

        void Start()
        {
            if (cameraContainer == null)
            {
                var go = new GameObject("CameraTransform");
                cameraContainer = go.transform;
                cameraContainer.position = _camera.transform.position;
                _camera.transform.parent = cameraContainer;
            }
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if (Input.GetKeyDown(triggerKey))
            {
                SetOffset();
            }
        }

        void OnDrawGizmos()
        {
            if (_camera == null) { return; }

            Gizmos.color = Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawFrustum(
                Vector3.zero, _camera.fieldOfView,
                _camera.farClipPlane, _camera.nearClipPlane, _camera.aspect
            );
        }

        void SetOffset()
        {
            cameraContainer.position = transform.position;
            cameraContainer.rotation = transform.rotation;
            // var camera = container.GetComponentInChildren<Camera>();
        }
    }
}
