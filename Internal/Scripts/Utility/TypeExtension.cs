using System;
using System.Reflection;

namespace MobileConsole
{
	public static class TypeExtension
    {
        public static bool IsNumericType(this FieldInfo fieldInfo)
        {
            return IsNumericType(fieldInfo.FieldType);
        }

        public static bool IsNumericType(this Type type)
        {
            // Enum is considered as Int32
            if (type.IsEnum)
                return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static T GetCustomAttribute<T>(this Type type, bool inherit = false) where T : Attribute
        {
            var attrs = type.GetCustomAttributes(typeof(T), inherit);
            if (attrs.Length > 0) return (T)attrs[0];
            else return default(T);
        }

		public static bool HasAttribute<T>(this FieldInfo fieldInfo, bool inherit = false) where T : Attribute
		{
			return GetCustomAttribute<T>(fieldInfo, inherit) != null;
		}

		public static bool HasAttribute(this FieldInfo fieldInfo, Type attributeType, bool inherit = false)
		{
			return GetCustomAttribute(fieldInfo, attributeType, inherit) != null;
		}

		public static bool HasAttribute<T>(this FieldInfo fieldInfo, out T outAttribute, bool inherit = false) where T : Attribute
		{
			outAttribute = GetCustomAttribute<T>(fieldInfo, inherit);
			return outAttribute != default(T);
		}

		public static T GetCustomAttribute<T>(this FieldInfo fieldInfo, bool inherit = false) where T : Attribute
		{
			return (T)GetCustomAttribute(fieldInfo, typeof(T), inherit);
		}

		public static object GetCustomAttribute(this FieldInfo fieldInfo, Type attributeType, bool inherit = false)
		{
			var attrs = fieldInfo.GetCustomAttributes(attributeType, inherit);
			if (attrs.Length > 0) return attrs[0];
			else return null;
		}


		public static bool HasAttribute<T>(this MethodInfo methodInfo, bool inherit = false) where T : Attribute
		{
			return GetCustomAttribute<T>(methodInfo, inherit) != null;
		}

		public static bool HasAttribute(this MethodInfo methodInfo, Type attributeType, bool inherit = false)
		{
			return GetCustomAttribute(methodInfo, attributeType, inherit) != null;
		}

		public static bool HasAttribute<T>(this MethodInfo methodInfo, out T outAttribute, bool inherit = false) where T : Attribute
		{
			outAttribute = GetCustomAttribute<T>(methodInfo, inherit);
			return outAttribute != default(T);
		}

		public static T GetCustomAttribute<T>(this MethodInfo methodInfo, bool inherit = false) where T : Attribute
		{
			return (T)GetCustomAttribute(methodInfo, typeof(T), inherit);
		}

		public static object GetCustomAttribute(this MethodInfo methodInfo, Type attributeType, bool inherit = false)
		{
			var attrs = methodInfo.GetCustomAttributes(attributeType, inherit);
			if (attrs.Length > 0) return attrs[0];
			else return null;
		}
    }
}

