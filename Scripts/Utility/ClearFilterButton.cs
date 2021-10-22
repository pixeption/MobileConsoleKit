using TMPro;
using UnityEngine;

namespace MobileConsole
{
	public class ClearFilterButton : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField]
		TMP_InputField _inputField;
#pragma warning restore 0649

		void Awake()
		{
			_inputField.onValueChanged.AddListener(OnValueChanged);
			_inputField.onSelect.AddListener(OnSelect);
			_inputField.onDeselect.AddListener(OnDeselect);
			gameObject.SetActive(false);
		}

		void OnValueChanged(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				gameObject.SetActive(false);
			}
		}

		void OnSelect(string value)
		{
			gameObject.SetActive(false);
		}

		void OnDeselect(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				gameObject.SetActive(true);
			}
		}
	}
}