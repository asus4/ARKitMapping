using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityOSC;

namespace AppKit.XR
{
    [RequireComponent(typeof(MultiCastOSC))]
    public class ARKitStream : MonoBehaviour
    {
        [SerializeField]
        Camera _camera;

        UnityARSessionNativeInterface _session;
        UnityARCamera _arCamera;


        [Header("AR Config Options")]
        public UnityARAlignment startAlignment = UnityARAlignment.UnityARAlignmentGravity;
        public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.Horizontal;
        public bool getPointCloud = true;
        public bool enableLightEstimation = true;

        MultiCastOSC _osc;

        void Start()
        {
            // Config
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _osc = GetComponent<MultiCastOSC>();
            _session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

            var config = new ARKitWorldTrackingSessionConfiguration();
            config.planeDetection = planeDetection;
            config.alignment = startAlignment;
            config.getPointCloudData = getPointCloud;
            config.enableLightEstimation = enableLightEstimation;

            if (config.IsSupported)
            {
                _session.RunWithConfig(config);
            }

            UnityARSessionNativeInterface.ARFrameUpdatedEvent += OnARFrameUpdate;
        }

        void OnDestroy()
        {
            UnityARSessionNativeInterface.ARFrameUpdatedEvent -= OnARFrameUpdate;
        }


        void Update()
        {
            Vector3 pos; Quaternion rot;
            GetPosRot(out pos, out rot);
            _camera.transform.SetPositionAndRotation(pos, rot);
            _camera.projectionMatrix = _session.GetCameraProjection();

            // Send OSC message
            var msg = new OSCMessage("/position");

            msg.Append<float>(pos.x);
            msg.Append<float>(pos.y);
            msg.Append<float>(pos.z);

            msg.Append<float>(rot.x);
            msg.Append<float>(rot.y);
            msg.Append<float>(rot.z);
            msg.Append<float>(rot.w);

            _osc.Send(msg);

            exmsg = string.Format("sended:{0}", Time.frameCount);
        }

        string exmsg = "";

        void OnARFrameUpdate(UnityARCamera arCamera)
        {
            _arCamera = arCamera;
        }

        System.Text.StringBuilder sb;
        public string ToInfoString()
        {
            if (sb == null)
            {
                sb = new System.Text.StringBuilder();
            }
            else
            {
                sb.Clear();
            }

            string status;
            if (_arCamera.trackingState == ARTrackingState.ARTrackingStateNormal)
            {
                status = "Traking";
            }
            else
            {
                var reasons = new string[]{
                "No reason",
                "Initializing",
                "Excessive Motion",
                "Insufficient Features"
            };
                status = reasons[(int)_arCamera.trackingReason];
            }
            sb.AppendFormat("Status: {0}\n", status);

            Vector3 pos; Quaternion rot;
            GetPosRot(out pos, out rot);
            sb.AppendFormat("Pos: {0}\n", pos);
            sb.AppendFormat("Rot: {0}\n", rot.eulerAngles);
            sb.AppendLine(exmsg);

            return sb.ToString();
        }

        void GetPosRot(out Vector3 pos, out Quaternion rot)
        {
            var matrix = _session.GetCameraPose();
            pos = UnityARMatrixOps.GetPosition(matrix);
            rot = UnityARMatrixOps.GetRotation(matrix);
        }
    }
}
