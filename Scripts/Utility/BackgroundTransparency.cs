using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole.UI
{
	[RequireComponent(typeof(Image))]
	public class BackgroundTransparency : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField]
		Image _image;
#pragma warning restore 0649

		float _defaultAlpha;

		void Awake()
		{
			_defaultAlpha = _image.color.a;
			EventBridge.OnBackgroundTransparencyChanged += OnBackgroundTransparencyChanged;

			OnBackgroundTransparencyChanged();
		}

		void OnBackgroundTransparencyChanged()
		{
			float finalAlpha = _defaultAlpha * LogConsoleSettings.Instance.backgroundTransparency;
			Color color = _image.color;
			color.a = finalAlpha;
			_image.color = color;
		}
	}
}