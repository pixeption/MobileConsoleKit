using System;
using System.Reflection;
using UnityEngine;


namespace MobileConsole.UI
{
	public class FieldNodeView : NodeView
	{
		const string ID_SLIDER_CELL = "SliderCell";
		const string ID_INPUT_CELL = "InputCell";
		const string ID_DROPDOWN_CELL = "DropdownCell";
		const string ID_CHECKBOX_CELL = "CheckboxCell";

		Command _command;
		VariableInfo _variableInfo;
		BaseCellControl _cellControl;
		string _cellIdentifier;

		public FieldNodeView(Command command, VariableInfo variableInfo)
		{
			_command = command;
			_variableInfo = variableInfo;
			ParseCellIdentifier();
			CreateCellControl();
		}
		
		public override ScrollViewCell CreateCell(RecycleScrollView scrollView, AssetConfig config, int cellIndex)
		{
			BaseCell cell = (BaseCell)scrollView.CreateCell(_cellIdentifier);
			cell.SetText(DisplayText);
			cell.SetHeaderOffset(LogConsoleSettings.GetTreeViewOffsetByLevel(level));
			cell.SetBackgroundColor(LogConsoleSettings.GetCellColor(cellIndex));
			_cellControl.UpdateData(cell);
			return cell;
		}

		protected void CreateCellControl()
		{
			switch (_cellIdentifier)
			{
				case ID_SLIDER_CELL:
					_cellControl = new SliderCellControl();
					break;
				case ID_INPUT_CELL:
					_cellControl = new InputCellControl();
					break;
				case ID_DROPDOWN_CELL:
					_cellControl = new DropdownCellControl();
					break;
				case ID_CHECKBOX_CELL:
					_cellControl = new CheckboxCellControl();
					break;
				default:
					break;
			}

			_cellControl.command = _command;
			_cellControl.variableInfo = _variableInfo;
		}

		void ParseCellIdentifier()
		{
			if (_variableInfo.fieldInfo.IsNumericType())
			{
				if (_variableInfo.fieldInfo.HasAttribute<RangeAttribute>())
					_cellIdentifier = ID_SLIDER_CELL;
				else if (_variableInfo.fieldInfo.HasAttribute<DropdownAttribute>())
					_cellIdentifier = ID_DROPDOWN_CELL;
				else
					_cellIdentifier = ID_INPUT_CELL;
			}
			else if (_variableInfo.fieldInfo.FieldType == typeof(string))
			{
				if (_variableInfo.fieldInfo.HasAttribute<DropdownAttribute>())
					_cellIdentifier = ID_DROPDOWN_CELL;
				else
					_cellIdentifier = ID_INPUT_CELL;
			}
			else if (_variableInfo.fieldInfo.FieldType.IsEnum)
			{
				_cellIdentifier = ID_DROPDOWN_CELL;
			}
			else if (_variableInfo.fieldInfo.FieldType == typeof(bool))
			{
				_cellIdentifier = ID_CHECKBOX_CELL;
			}

			if (string.IsNullOrEmpty(_cellIdentifier))
				throw new System.Exception("Couldn't parse cell identifier, something must wrong");
		}
	}


	public abstract class BaseCellControl
	{
		public Command command;
		public VariableInfo variableInfo;

		public abstract void UpdateData(ScrollViewCell cell);

		protected void OnValueChanged()
		{
			// In case of game logic causes any exception, add try catch here to prevent log console corruption
			try
			{
				command.SaveVariabledInfo(variableInfo);
                command.OnValueChanged(variableInfo.fieldInfo.Name);

                if (variableInfo.callbackMethodInfo != null)
				{
					variableInfo.callbackMethodInfo.Invoke(command, null);
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
	}

	public class CheckboxCellControl : BaseCellControl
	{
		BaseCheckboxCell _checkboxCell;

		public override void UpdateData(ScrollViewCell cell)
		{
			_checkboxCell = (BaseCheckboxCell)cell;
			_checkboxCell.OnValueChanged = OnValueChanged;
			_checkboxCell.SetToggle((bool)variableInfo.fieldInfo.GetValue(command));
		}

		void OnValueChanged(BaseCheckboxCell cell, bool enabled)
		{
			variableInfo.fieldInfo.SetValue(command, enabled);
			base.OnValueChanged();
		}
	}

	public class InputCellControl : BaseCellControl
	{
		BaseInputCell _inputCell;
		bool _isNumeric;
		
		public override void UpdateData(ScrollViewCell cell)
		{
			_isNumeric = variableInfo.fieldInfo.FieldType.IsNumericType();
			_inputCell = (BaseInputCell)cell;
			_inputCell.OnValueChanged = OnValueChanged;
			_inputCell.SetInput(GetFieldValueAsString());
			_inputCell.SetIsNumeric(_isNumeric);
		}

		string GetFieldValueAsString()
		{
			object fieldValue = variableInfo.fieldInfo.GetValue(command) ?? "";
			return fieldValue.ToString();
		}

		void OnValueChanged(BaseInputCell cell, string value)
		{
			if (string.IsNullOrEmpty(value) && _isNumeric)
			{
				_inputCell.SetInput(GetFieldValueAsString());
				return;
			}
			else
			{
				try
				{
					variableInfo.fieldInfo.SetValue(command, Convert.ChangeType(value, variableInfo.fieldInfo.FieldType));
					base.OnValueChanged();
				}
				catch
				{
					_inputCell.SetInput(GetFieldValueAsString());
				}
			}			
		}
	}

	public class DropdownCellControl : BaseCellControl
	{
		static IDropdownField[] _presetDropdownFields = new IDropdownField[]
		{
			new EnumDropdownField(),
			new StringDropdownField(),
			new NumericDropdownField()
		};

		BaseDropdownCell _dropdownCell;
		IDropdownField _dropdownField;
		string[] _options;

		public override void UpdateData(ScrollViewCell cell)
		{
			_dropdownCell = (BaseDropdownCell)cell;
			_dropdownCell.OnValueChanged = OnValueChanged;

			_dropdownField = null;
			foreach (var dropdownField in _presetDropdownFields)
			{
				if (dropdownField.TryParse(command, variableInfo, out _options))
				{
					_dropdownField = dropdownField;
					break;
				}
			}

			if (_dropdownField == null || _options == null || _options.Length == 0)
			{
				throw new System.Exception("There is something wrong!");
			}

			// If the field doesn't have value yet, set it to the first element (0) in options
			int dropdownIndex = _dropdownField.GetDropdownIndex(command, variableInfo, _options);
			if (dropdownIndex == -1)
			{
                _dropdownField.OnValueChanged(command, variableInfo, _options, 0);
			}

			_dropdownCell.SetOptions(_options);
			_dropdownCell.SetIndex(dropdownIndex);
		}

		void OnValueChanged(BaseDropdownCell cell, int index)
		{
			_dropdownField.OnValueChanged(command, variableInfo, _options, index);
			base.OnValueChanged();
		}


		#region Dropdown Field
		abstract class IDropdownField
		{
			public abstract bool TryParse(Command command, VariableInfo variableInfo, out string[] options);
			public abstract int GetDropdownIndex(Command command, VariableInfo variableInfo, string[] options);
			public abstract void OnValueChanged(Command command, VariableInfo variableInfo, string[] options, int index);
		}

		class EnumDropdownField : IDropdownField
		{
			public override bool TryParse(Command command, VariableInfo variableInfo, out string[] options)
			{
				if (!variableInfo.fieldInfo.FieldType.IsEnum)
				{
					options = null;
					return false;
				}

				options = Enum.GetNames(variableInfo.fieldInfo.FieldType);

				return true;
			}

			public override int GetDropdownIndex(Command command, VariableInfo variableInfo, string[] options)
			{
				return (int)variableInfo.fieldInfo.GetValue(command);
			}

			public override void OnValueChanged(Command command, VariableInfo variableInfo, string[] options, int index)
			{
				variableInfo.fieldInfo.SetValue(command, index);
			}
		}

		class StringDropdownField : IDropdownField
		{
			public override bool TryParse(Command command, VariableInfo variableInfo, out string[] options)
			{
				if (variableInfo.fieldInfo.FieldType != typeof(string))
				{
					options = null;
					return false;
				}

				DropdownAttribute dropdownAttr;
				if (!variableInfo.fieldInfo.HasAttribute<DropdownAttribute>(out dropdownAttr))
				{
					options = null;
					return false;
				}

				MethodInfo methodInfo = command.GetType().GetMethod(dropdownAttr.methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (methodInfo == null)
				{
					throw new Exception("Could not found method name: " + dropdownAttr.methodName);
				}

				try
				{
					options = (string[])methodInfo.Invoke(command, null);
				}
				catch
				{
					throw new Exception("Could not retrieve options from method name: " + dropdownAttr.methodName);
				}

				return true;
			}

			public override int GetDropdownIndex(Command command, VariableInfo variableInfo, string[] options)
			{
				string strValue = (string)variableInfo.fieldInfo.GetValue(command);
				return Array.IndexOf(options, strValue);
			}

			public override void OnValueChanged(Command command, VariableInfo variableInfo, string[] options, int index)
			{
				variableInfo.fieldInfo.SetValue(command, options[index]);
			}
		}

		class NumericDropdownField : IDropdownField
		{
			public override bool TryParse(Command command, VariableInfo variableInfo, out string[] options)
			{
				if (!variableInfo.fieldInfo.FieldType.IsNumericType())
				{
					options = null;
					return false;
				}

				DropdownAttribute dropdownAttr;
				if (!variableInfo.fieldInfo.HasAttribute<DropdownAttribute>(out dropdownAttr))
				{
					options = null;
					return false;
				}

				MethodInfo methodInfo = command.GetType().GetMethod(dropdownAttr.methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (methodInfo == null)
				{
					throw new Exception("Could not found method name: " + dropdownAttr.methodName);
				}

				Array fieldOptions;
				try
				{
					fieldOptions = (Array)methodInfo.Invoke(command, null);
				}
				catch
				{
					throw new Exception("Could not retrieve options from method name: " + dropdownAttr.methodName);
				}

				options = new string[fieldOptions.Length];
				for (int i = 0; i < fieldOptions.Length; i++)
				{
					options[i] = fieldOptions.GetValue(i).ToString();
				}

				return true;
			}

			public override int GetDropdownIndex(Command command, VariableInfo variableInfo, string[] options)
			{
				return Array.IndexOf(options, variableInfo.fieldInfo.GetValue(command).ToString());
			}

			public override void OnValueChanged(Command command, VariableInfo variableInfo, string[] options, int index)
			{
				variableInfo.fieldInfo.SetValue(command, Convert.ChangeType(options[index], variableInfo.fieldInfo.FieldType));
			}
		}
		#endregion
	}

	public class SliderCellControl : BaseCellControl
	{
		BaseSliderCell _sliderCell;

		public override void UpdateData(ScrollViewCell cell)
		{
			_sliderCell = (BaseSliderCell)cell;
			_sliderCell.OnValueChanged = OnValueChanged;

			var rangeAttr = variableInfo.fieldInfo.GetCustomAttribute<RangeAttribute>(false);
			var relativeAttr = variableInfo.fieldInfo.GetCustomAttribute<RelativeSliderAttribute>(false);
			bool isWholeNumbers = variableInfo.fieldInfo.FieldType != typeof(float) && variableInfo.fieldInfo.FieldType != typeof(double);
			float value = (float)Convert.ChangeType(variableInfo.fieldInfo.GetValue(command), typeof(float));
			_sliderCell.UseRelativeSlider(relativeAttr != null);
			_sliderCell.SetConfig(rangeAttr.min, rangeAttr.max, isWholeNumbers);
			_sliderCell.SetValue(Mathf.Clamp(value, rangeAttr.min, rangeAttr.max));
		}

		void OnValueChanged(BaseSliderCell cell, float value)
		{
			variableInfo.fieldInfo.SetValue(command, Convert.ChangeType(value, variableInfo.fieldInfo.FieldType));
			base.OnValueChanged();
		}
	}
}