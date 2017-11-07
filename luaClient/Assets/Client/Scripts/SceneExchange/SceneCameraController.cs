using System;
using UnityEngine;


public class SceneCameraController:MonoBehaviour
	{
	public Transform mainCameraTrans;
		enum slideVector { nullVector, left, right };  

		private Vector2 lastPos;//上一个位置  

		private Vector2 currentPos;//下一个位置  

		private Vector2 startPos;//下一个位置  
	private Vector3 startCameraEuler;

		private slideVector currentVector = slideVector.nullVector;//当前滑动方向  
	private slideVector lastVector = slideVector.nullVector;//当前滑动方向  

		private float timer;//时间计数器  

		public float offsetTime = 0.01f;//判断的时间间隔 
	public float delta_slider = 0;

	public float moveDelta = 10;

	public AnimationCurve animationCurve  = new AnimationCurve();
	public float yieldTimer = 0;

	public Vector2 spacePos;

	void LateUpdate(){
		if (currentVector != slideVector.nullVector) {
			Vector3 tmp = mainCameraTrans.transform.localEulerAngles;
			spacePos = currentPos - startPos;
			float y = startCameraEuler.y + spacePos.x/2;
			mainCameraTrans.transform.localEulerAngles = new Vector3 (startCameraEuler.x, y, startCameraEuler.z);
			yieldTimer = 0;
		} else {
			if (animationCurve.keys.Length > 0) {
				float maxTimer = animationCurve.keys [animationCurve.keys.Length - 1].time;

				if (yieldTimer <= maxTimer) {
					yieldTimer += Time.deltaTime;
					float yieldVal = animationCurve.Evaluate (yieldTimer);

					Vector3 tmp = mainCameraTrans.transform.localEulerAngles;
					float y = Mathf.Lerp (tmp.y, tmp.y + delta_slider * yieldVal * moveDelta, Time.deltaTime);
					mainCameraTrans.transform.localEulerAngles = new Vector3 (tmp.x, y, tmp.z);
				}
			}
		}
	}
	

		void Update(){
			if (Input.GetMouseButtonDown (0)) {  
				lastPos = Input.mousePosition;  
				currentPos = Input.mousePosition;  
				startPos = Input.mousePosition; 
			startCameraEuler = mainCameraTrans.transform.localEulerAngles;
				timer = 0; 
				delta_slider = 0;
			}  
			if (Input.GetMouseButton (0)) {  
				currentPos = Input.mousePosition;  
				timer += Time.deltaTime;  
				if (timer > offsetTime) {  
					if (currentPos.x < lastPos.x) {  
						if (currentVector == slideVector.left) {  
							return;  
						}  
						//TODO trun Left event  
						lastVector = slideVector.left;  
						currentVector = slideVector.left;  
//						if (player != null) {  
//							player.AttackLeft ();  
//						}  
						//Debug.Log ("Turn left");  
					}   
					if (currentPos.x > lastPos.x) {  
						if (currentVector == slideVector.right) {  
							return;  
						}  
						//TODO trun right event  
						lastVector = slideVector.right;  
						currentVector = slideVector.right;  
//						if (player != null) {  
//							player.AttackRight ();  
//						}  
						//Debug.Log ("Turn right");  
					}  
				delta_slider = delta_slider + (currentPos.x - lastPos.x)/2;
					lastPos = currentPos;  
					timer = 0;  
				}  
			}  
			if (Input.GetMouseButtonUp (0)) {  
				if (lastVector != slideVector.nullVector && currentVector == slideVector.nullVector) {  
					if (lastVector == slideVector.left) {  
						//TODO trun Left event  
						//Debug.Log("click attack left++++++++++++");  
//						player.AttackLeft();  

					} else {  
						//Debug.Log("click attack right++++++++++++");  
//						player.AttackRight();  
					}  
				}  
			currentVector = slideVector.nullVector;  
			}  
		}
	}


