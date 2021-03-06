﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

namespace AppKit.XR
{
    [RequireComponent(typeof(MultiCastOSC))]
    public class ARKitReceiver : MonoBehaviour
    {
        [SerializeField] Camera _camera;

        MultiCastOSC _osc;
        Transform _t;

        void Start()
        {
            _osc = GetComponent<MultiCastOSC>();
            _osc.OnMessage += OnOscMessage;

            _t = _camera.transform;
        }

        void OnDestroy()
        {
            _osc.OnMessage -= OnOscMessage;
        }

        void OnOscMessage(OSCMessage msg)
        {
            switch (msg.Address)
            {
                case "/position":
                    var data = msg.Data;
                    Debug.AssertFormat(data.Count == 7, "Data has 7 floats - actally : {0}", data.Count);

                    Vector3 pos;
                    pos.x = (System.Single)data[0];
                    pos.y = (System.Single)data[1];
                    pos.z = (System.Single)data[2];
                    Quaternion rot;
                    rot.x = (System.Single)data[3];
                    rot.y = (System.Single)data[4];
                    rot.z = (System.Single)data[5];
                    rot.w = (System.Single)data[6];
                    // _t.SetPositionAndRotation(pos, rot);
                    _t.localPosition = pos;
                    _t.localRotation = rot;
                    break;
                default:
                    Debug.Log($"[OSC] {msg.Address}");
                    break;
            }
        }
    }

}
