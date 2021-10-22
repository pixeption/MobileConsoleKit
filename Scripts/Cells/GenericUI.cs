using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole.UI
{
	public class GenericUI : UIBridge
	{
#pragma warning disable 0649
		[SerializeField]
		Toggle _toggle;

		[SerializeField]
		TextMeshProUGUI _label;

		[SerializeField]
		TMP_InputField _input;
#pragma warning restore 0649

		public override bool toggle
		{
			get
			{
				if (_toggle != null) return _toggle.isOn;
				else return base.toggle;
			}
			set
			{
				if (_toggle != null) _toggle.isOn = value;
			}
		}

		public override string text 
		{ 
			get
			{
				if (_label != null) return _label.text;
				else return base.text;
			}
			set
			{
				if (_label != null) _label.text = value;
			}
		}

		public override string input
		{
			get
			{
				if (_input != null) return _input.text;
				else return base.text;
			}
			set
			{
				if (_input != null) _input.text = value;
			}
		}
	}
}