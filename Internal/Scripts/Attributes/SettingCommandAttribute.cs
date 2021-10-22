using System;

namespace MobileConsole
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingCommandAttribute : Attribute
    {
        public string name;
        public string description;
        public int order;
    }
}