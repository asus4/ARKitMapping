using UnityEngine;
using UnityEngine.UI;

namespace AppKit.XR
{
    [RequireComponent(typeof(Text))]
    public class ARKitStreamInfo : MonoBehaviour
    {
        [SerializeField]
        ARKitStream stream;

        Text label;
        void Start()
        {
            label = GetComponent<Text>();
        }

        void Update()
        {
            label.text = stream.ToInfoString();
        }
    }
}
