using UnityEngine;
using System.Collections;
 
public class FPSDisplay : MonoBehaviour
{
	[Header("Fullscreen Debugging")]
    public bool enable = false;
	private float deltaTime = 0.0f;
	private PlayerController playerController;
	private PlayerForce playerForce;

	private void Start()
	{
		playerController = gameObject.GetComponentInParent<PlayerController>();
		playerForce = gameObject.GetComponentInParent<PlayerForce>();
	}
 
	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}
 
	void OnGUI()
	{
		if(enable)
        {
		    int w = Screen.width, h = Screen.height;

			// Base rectangle
			Rect baseRect = new Rect(0, 0, w, h);
			baseRect.x = 5;
 
			// Style of debugger
    		GUIStyle style = new GUIStyle();
 		    style.alignment = TextAnchor.UpperLeft;
		    style.fontSize = h / 40;
		    style.normal.textColor = Color.red;

			// FPS counter
		    float msec = deltaTime * 1000.0f;
		    float fps = 1.0f / deltaTime;
		    string FPStext = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		    GUI.Label(baseRect, FPStext, style);

			// Velocity display
			Rect rect1 = baseRect;
			rect1.y = 20;
			string playerVel = "Velocity: " + playerController.velocity;
			GUI.Label(rect1, playerVel, style);
			
			Rect rect2 = baseRect;
			rect2.y = 40;
			string forceVel = "Force Velocity: " + playerForce.velocity;
			GUI.Label(rect2, forceVel, style);
        }
	}
}