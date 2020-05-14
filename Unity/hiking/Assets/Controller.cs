//
// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// サンプル
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Networking;
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

        private int newCntStep;
        private int previousCntStep;
        private bool isMoving = false;
        private int cntKeepNotMoving = 0;
        private bool isAccessingServer = false;

        void Start ()
        {
            anim = GetComponent<Animator> ();
            StartCoroutine(GetText());
        }

        void FixedUpdate ()
        {
            float h = Input.GetAxis ("Horizontal");
            float v = Input.GetAxis ("Vertical");
            
            /* Get v value from step counter via HTTP */
            /* keep moving for few frames after count stops to make it smooth */
            if (newCntStep - previousCntStep > 0) {
                isMoving = true;
                cntKeepNotMoving = 0;
            } else {
                cntKeepNotMoving++;
                if (cntKeepNotMoving > 50) {
                    isMoving = false;
                }
            }
            if (isMoving) {
                v += 1;
            }
            previousCntStep = newCntStep;
            // Debug.Log(newCntStep);
            if (!isAccessingServer) StartCoroutine(GetText());

            /* Get h from VR headset. change rotation only when moving */
            if (v > 0) {
                Quaternion vrRotation = InputTracking.GetLocalRotation(XRNode.CenterEye);
                h += -vrRotation.y;
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

        IEnumerator GetText() {
            isAccessingServer = true;
            UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:5000/get_count");
            yield return request.Send();
            // if (request.isError) {
            //     Debug.Log(request.error);
            // } else {
                if (request.responseCode == 200) {
                    string text = request.downloadHandler.text;
                    newCntStep = int.Parse(text);
                    // Debug.Log(newCntStep);
                }
                isAccessingServer = false;
            // }
        }
    }
}