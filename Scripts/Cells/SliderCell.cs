using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MobileConsole.UI
{
    public class SliderCell : BaseSliderCell
    {
#pragma warning disable 0649
		[SerializeField]
		TextMeshProUGUI _fieldName;

		[SerializeField]
		Image _backgroundImage;

		[SerializeField]
		TextMeshProUGUI _textValue;

		[SerializeField]
		protected RelativeSlider _slider;
#pragma warning restore 0649

		protected bool _canDispatch;

		void Awake()
		{
			_canDispatch = true;
			_slider.onValueChanged.AddListener(OnSliderValueChanged);
		}


		public override void SetText(string text)
		{
			_fieldName.text = text;
		}

		public override void SetHeaderOffset(float offset)
		{
			_fieldName.margin = new Vector4(offset, 0, 0, 0);
		}

		public override void SetValue(float value)
		{
			_canDispatch = false;
			_slider.value = value;
			_textValue.text = GetDisplayValue(value);
			_canDispatch = true;
		}

		public override void SetConfig(float min, float max, bool wholeNumbers)
		{
			_canDispatch = false;
			_slider.wholeNumbers = wholeNumbers;
			_slider.minValue = min;
			_slider.maxValue = max;
			_canDispatch = true;
		}

		public override void UseRelativeSlider(bool useRelative)
		{
			_slider.useRelative = useRelative;
		}

		void OnSliderValueChanged(float value)
		{
            _textValue.text = GetDisplayValue(value);
			NotifyOnValueChanged(value);
		}

		string GetDisplayValue(float value)
		{
			return _slider.wholeNumbers ? value.ToString() : value.ToString("N2");
		}
    }
}