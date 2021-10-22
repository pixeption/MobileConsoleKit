using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole.UI
{
	public class CheckboxCell : BaseCheckboxCell 
	{
#pragma warning disable 0649
		[SerializeField]
		TextMeshProUGUI _label;

		[SerializeField]
		Image _backgroundImage;

		[SerializeField]
		Toggle _toggle;
#pragma warning restore 0649

		bool _canDispatch;

		void Awake()
		{
			_canDispatch = true;
			_toggle.onValueChanged.AddListener(OnToggleValueChanged);
		}
		
		public override void SetText(string name)
		{
			_label.text = name;
		}

		public override void SetToggle(bool value)
		{
			_canDispatch = false;
			_toggle.isOn = value;
			_canDispatch = true;
		}

		public override void SetHeaderOffset(float offset)
		{
			_label.margin = new Vector4(offset, 0, 0, 0);
		}

		void OnToggleValueChanged(bool value)
		{
			if (_canDispatch)
			{
				NotifyOnValueChanged(value);
			}
		}
	}
}