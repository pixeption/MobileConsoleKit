using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MobileConsole.UI
{
    public class DropdownCell : BaseDropdownCell
    {
#pragma warning disable 0649
		[SerializeField]
		TextMeshProUGUI _fieldName;
		
		[SerializeField]
		TMP_Dropdown _dropdown;

		[SerializeField]
		Image _backgroundImage;
#pragma warning restore 0649

		bool _canDispatch;

		void Awake()
		{
			_canDispatch = true;
			_dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
		}		

		public override void SetText(string text)
		{
			_fieldName.text = text;
		}

		public override void SetIndex(int index)
		{
			_canDispatch = false;
			_dropdown.value = index;
			_canDispatch = true;
			_dropdown.RefreshShownValue();
		}

		public override void SetOptions(string[] options)
		{
			_canDispatch = false;

			_dropdown.options.Clear();
			for (int i = 0; i < options.Length; i++)
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(options[i]);
				_dropdown.options.Add(optionData);
			}

			_canDispatch = true;
		}

		public override void SetHeaderOffset(float offset)
		{
			_fieldName.margin = new Vector4(offset, 0, 0, 0);
		}

		void OnDropdownValueChanged(int index)
		{
			if (_canDispatch)
			{
				base.OnValueChanged(this, index);
			}
		}
	}
}