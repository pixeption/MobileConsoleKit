using System;
using UnityEditor;
using UnityEngine;

namespace MobileConsole.Editor
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Enum targetEnum = (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue);
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(position, label, property);

			Enum enumNew = EnumMaskField(position, label, targetEnum);

            if (!property.hasMultipleDifferentValues || EditorGUI.EndChangeCheck())
                property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());

            EditorGUI.EndProperty();
        }

		public static Enum EnumMaskField(Rect position, GUIContent label, Enum value)
		{
#if UNITY_2017_3_OR_NEWER
			return EditorGUI.EnumFlagsField(position, label, value);
#else
			return OrderToEnumMask(EditorGUI.EnumMaskField(position, label, EnumToOrderMask(value, false)));
#endif
		}

		public static Enum EnumToOrderMask(Enum enumMaskEnum, bool allowCompound)
		{
			var enumType = enumMaskEnum.GetType();
			var enumValues = Enum.GetValues(enumType);
			var enumMask = Convert.ToInt64(enumMaskEnum);

			var orderMask = 0L;

			for (int i = 0; i < enumValues.Length; i++)
			{
				var enumValue = enumValues.GetValue(i);
				var enumValueMask = Convert.ToInt64(enumValue);

				// Skip none values, because they'd always be included
				if (enumValueMask == 0)
				{
					continue;
				}

				// Skip compound values, which can be useful for properly
				// separating items in the inspector and avoiding a deadlock
				if (!allowCompound && !Mathf.IsPowerOfTwo((int)enumValueMask))
				{
					continue;
				}

				if ((enumMask & enumValueMask) == enumValueMask)
				{
					var indexMask = 1L << i;
					orderMask |= indexMask;
				}
			}

			return (Enum)Enum.ToObject(enumType, orderMask);
		}

		public static Enum OrderToEnumMask(Enum orderMaskEnum)
		{
			var enumType = orderMaskEnum.GetType();
			var enumValues = Enum.GetValues(enumType);
			var orderMask = Convert.ToInt64(orderMaskEnum);

			var enumMask = 0L;

			for (int i = 0; i < enumValues.Length; i++)
			{
				var indexMask = 1L << i;

				if ((orderMask & indexMask) == indexMask)
				{
					var enumValue = enumValues.GetValue(i);
					var enumValueMask = Convert.ToInt64(enumValue);

					enumMask |= enumValueMask;
				}
			}

			return (Enum)Enum.ToObject(enumType, enumMask);
		}
    }
}