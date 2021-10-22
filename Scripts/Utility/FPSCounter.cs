using System.Collections;
using UnityEngine;
using TMPro;

namespace MobileConsole.UI
{
	public class FPSCounter : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField]
		float _frequency = 0.5f;

		[SerializeField]
		TextMeshProUGUI _text;
#pragma warning restore 0649

		int _lastFrameCount;
		float _lastTime;

		void OnEnable()
		{
			StartCoroutine(FPSCount());
		}

		IEnumerator FPSCount()
		{
			while (true)
			{
				_lastFrameCount = Time.frameCount;
				_lastTime = Time.realtimeSinceStartup;

				yield return new WaitForSecondsRealtime(_frequency);

				float timeSpan = Time.realtimeSinceStartup - _lastTime;
				int frameCount = Time.frameCount - _lastFrameCount;
				int fps = Mathf.RoundToInt(frameCount / timeSpan);
				
				_text.text = fps.ToString();
				_text.color = GetTextColor(fps);
			}
		}

		Color GetTextColor(int fps)
		{
			if (fps >= 50)
			{
				return Color.green; 
			}
			else if (fps >= 30)
			{
				return Color.yellow;
			}
			else
			{
				return Color.red;
			}
		}
	}
}