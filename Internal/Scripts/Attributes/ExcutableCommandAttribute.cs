using System;

namespace MobileConsole
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExecutableCommandAttribute : Attribute
    {
        public string name;
        public string description;
        public int order = 0;
    }
}