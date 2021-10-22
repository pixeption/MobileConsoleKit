using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole.UI
{
	[RequireComponent(typeof(CanvasScaler))]
	public class CanvasScalerCorrection : MonoBehaviour
	{
		[SerializeField]
		float _landscapeWidth = 1920;

		[SerializeField]
		float _landscapeHeight = 1080;

		void Awake()
		{
			CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
			if (Screen.orientation == ScreenOrientation.Landscape)
			{
				canvasScaler.referenceResolution = new Vector2(_landscapeWidth, _landscapeHeight);
			}
			else
			{
				canvasScaler.referenceResolution = new Vector2(_landscapeHeight, _landscapeWidth);
			}
		}
	}
}
