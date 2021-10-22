using System;
using System.Reflection;
using UnityEngine;

namespace MobileConsole
{
	public class VariableInfo
	{
		public FieldInfo fieldInfo;
		public MethodInfo callbackMethodInfo;

		public void TryNotifyOnValueChanged()
		{
			if (callbackMethodInfo != null)
			{
				try
				{
					callbackMethodInfo.Invoke(this, null);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}
	}
}