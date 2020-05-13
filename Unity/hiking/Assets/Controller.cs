//
// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// サンプル
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using UnityEngine.XR;
using System.Collections;

namespace UnityChan
{
    [RequireComponent(typeof(Animator))]

    public class Controller : MonoBehaviour
    {
        public float animSpeed = 1.5f;
        public float forwardSpeed = 7.0f;
        public float backwardSpeed = 2.0f;
        public float rotateSpeed = 2.0f;
        private Animator anim;

        // 初期化
        void Start ()
        {
            anim = GetComponent<Animator> ();
        }

        void FixedUpdate ()
        {
            float h = Input.GetAxis ("Horizontal");
            float v = Input.GetAxis ("Vertical");

            if (v > 0) {
                Quaternion vrRotation = InputTracking.GetLocalRotation(XRNode.CenterEye);
                h += vrRotation.y;
            }

            Vector3 velocity = new Vector3 (0, 0, v);
            velocity = transform.TransformDirection (velocity);
            if (v > 0.1) {
                velocity *= forwardSpeed;
            } else if (v < -0.1) {
                velocity *= backwardSpeed;
            }

            transform.localPosition += velocity * Time.fixedDeltaTime;
            transform.Rotate (0, h * rotateSpeed, 0);	

            anim.SetFloat ("Speed", v);
            anim.SetFloat ("Direction", h);
            anim.speed = animSpeed;
         }
    }
}