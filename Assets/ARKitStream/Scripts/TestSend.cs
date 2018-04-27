using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

namespace AppKit
{
    [RequireComponent(typeof(MultiCastOSC))]
    public class TestSend : MonoBehaviour
    {
        public bool isSend;
        Transform _t;

        MultiCastOSC _osc;

        void Start()
        {
            _t = transform;
            _osc = GetComponent<MultiCastOSC>();
            _osc.OnMessage += OnOscMessage;
        }

        void OnDestroy()
        {
            _osc.OnMessage -= OnOscMessage;
        }

        void Update()
        {
            if (!isSend)
            {
                return;
            }

            var pos = _t.localPosition;
            var rot = _t.localRotation;

            var msg = new OSCMessage("/position");

            msg.Append<float>(pos.x);
            msg.Append<float>(pos.y);
            msg.Append<float>(pos.z);

            msg.Append<float>(rot.x);
            msg.Append<float>(rot.y);
            msg.Append<float>(rot.z);
            msg.Append<float>(rot.w);

            _osc.Send(msg);
        }

        void OnOscMessage(OSCMessage msg)
        {
            string dataStr = "";
            foreach (var d in msg.Data)
            {
                dataStr += d.ToString();
                dataStr += ',';
            }
            Debug.LogFormat("[OSC]:{0} {1}", msg.Address, dataStr);
        }
    }
}
