using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MobileConsole.UI
{
    public class InputCell : BaseInputCell
    {
#pragma warning disable 0649
		[SerializeField]
		TextMeshProUGUI _fieldName;

		[SerializeField]
		TMP_InputField _inputField;

		[SerializeField]
		Image _backgroundImage;
#pragma warning restore 0649

		bool _isNumeric;
		bool _canDispatch;

		void Awake()
		{
			_canDispatch = true;
			_inputField.onEndEdit.AddListener(OnInputValueChanged);
		}

		public override void SetText(string text)
		{
			_fieldName.text = text;
		}

		public override void SetInput(string text)
		{
			_canDispatch = false;
			_inputField.text = text;
			_canDispatch = true;
		}

		public override void SetHeaderOffset(float offset)
		{
			_fieldName.margin = new Vector4(offset, 0, 0, 0);
		}

		public override void SetIsNumeric(bool isNumeric)
		{
			_isNumeric = isNumeric;
			_inputField.keyboardType = _isNumeric ? TouchScreenKeyboardType.NumberPad : TouchScreenKeyboardType.Default;
			_inputField.contentType = _isNumeric ? TMP_InputField.ContentType.DecimalNumber : TMP_InputField.ContentType.Standard;
		}

		public override void SetReadonly(bool isReadonly)
		{
			_inputField.readOnly = isReadonly;
		}

		void OnInputValueChanged(string value)
		{
			if (_canDispatch)
			{
				NotifyOnValueChanged(value);
			}
		}
    }
}
