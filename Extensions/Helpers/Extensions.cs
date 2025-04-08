using System.Reflection;

namespace Notes_Back_CS.Extensions.Helpers
{
    public static class Extensions
    {
        public static string GetMethodName(this MethodBase method)
        {
            string _methodName = method.DeclaringType.FullName;

            if (_methodName.Contains(">") || _methodName.Contains("<"))
            {
                _methodName = _methodName.Split('<', '>')[1];
            }
            else
            {
                _methodName = method.Name;
            }

            return _methodName;
        }

        public static string GetClassName(this MethodBase method)
        {
            string className = method.DeclaringType.FullName;

            if (className.Contains(">") || className.Contains("<"))
            {
                className = className.Split('+')[0];
            }
            return className;
        }

        public static string GetFullExceptionMessage(this Exception ex, bool isEmail = false)
        {
            var msgs = new List<string>();
            while (ex != null)
            {
                msgs.Add(isEmail ? ex.Message : ex.Message.Replace("<br><br>", " ").Replace("<br>", ", "));
                ex = ex.InnerException;
            }
            return string.Join(" :: ", msgs);
        }
    }
}