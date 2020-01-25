using System;
using UnityEngine;
using UnityEngine.UI;

namespace DebugTools
{
	public class DebugDisplay : MonoBehaviour
	{
		public float updateInterval = 0.5F;

		private float accum = 0; // FPS accumulated over the interval
		private int frames = 0; // Frames drawn over the interval
		private float timeleft; // Left time for current interval
		private Text m_MyText;
		private GUIStyle style;
		private int w = Screen.width;
		private int h = Screen.height;
		private Rect rect;
		private string format;
		private float msec;

		void Start()
		{
			style = new GUIStyle();
			rect = new Rect(0, 0, w, h * 2 / 100);
			style.alignment = TextAnchor.UpperLeft;

			timeleft = updateInterval;
		}

		void Update()
		{
			msec = Time.deltaTime * 1000.0f;
			timeleft -= Time.deltaTime;
			accum += Time.timeScale / Time.deltaTime;
			frames++;

			// Interval ended - update GUI text and start new interval
			if (timeleft <= 0.0)
			{
				// display two fractional digits (f2 format)
				float fps = accum / frames;
				
				if (fps < 30)
					style.normal.textColor = Color.yellow;
				else if (fps < 10)
					style.normal.textColor = Color.red;
				else
					style.normal.textColor = Color.green;

				format = "FPS: " + (int)fps + " " + string.Format("{0:0.0} ms", msec);

				timeleft = updateInterval;
				accum = 0.0F;
				frames = 0;
			}
		}
		
		void OnGUI()
		{
			GUI.Label(rect, format, style);
		}
	}
}