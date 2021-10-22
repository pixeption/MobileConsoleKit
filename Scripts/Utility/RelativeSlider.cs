using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MobileConsole.UI
{
	public class RelativeSlider : Slider
	{
		public bool useRelative = false;
		float _lastNormalizedValue;
		float _fullDistance;

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (useRelative)
			{
				if (!MayDrag(eventData))
					return;
				
				if (Camera.main == null)
				{
					Debug.LogWarning("Can not found main camera!");
					return;
				}

				_lastNormalizedValue = normalizedValue;
				_fullDistance = Mathf.Min(Camera.main.pixelWidth, Camera.main.pixelHeight) / 2;
			}
			else
			{
				base.OnPointerDown(eventData);
			}
		}

		public override void OnDrag(PointerEventData eventData)
		{
			if (useRelative)
			{
				if (!MayDrag(eventData))
					return;

				float dragAmount = eventData.position.x - eventData.pressPosition.x;
				float normalizedAmount = dragAmount / _fullDistance;
				normalizedValue = normalizedAmount + _lastNormalizedValue;
			}
			else
			{
				base.OnDrag(eventData);
			}
		}

		bool MayDrag(PointerEventData eventData)
		{
			return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}
	}	
}