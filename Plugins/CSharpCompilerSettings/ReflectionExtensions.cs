using System.Text;
using System;
using System.Linq;
using System.Reflection;

namespace Coffee.CSharpCompilerSettings
{
    internal static class ReflectionExtensions
    {
        const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;
        static readonly StringBuilder _sb = new StringBuilder(1024);

        private static object Inst(this object self)
        {
            return (self is Type) ? null : self;
        }

        private static Type Type(this object self)
        {
            return (self as Type) ?? self.GetType();
        }

        public static object New(this Type self, params object[] args)
        {
            var types = args.Select(x => x.GetType()).ToArray();
            return self.Type().GetConstructor(types)
                .Invoke(args);
        }

        public static object Call(this object self, string methodName, params object[] args)
        {
            var types = args.Select(x => x.GetType()).ToArray();
            return self.Type().GetMethods(FLAGS)
                .Where(x => x.Name == methodName)
                .First(x => x.GetParameters().Select(y => y.ParameterType).SequenceEqual(types))
                .Invoke(self.Inst(), args);
        }

        public static object Call(this object self, Type[] genericTypes, string methodName, params object[] args)
        {
            return self.Type().GetMethods(FLAGS)
                .First(x => x.IsGenericMethodDefinition && x.Name == methodName)
                .MakeGenericMethod(genericTypes)
                .Invoke(self.Inst(), args);
        }

        public static object Get(this object self, string memberName, MemberInfo mi = null)
        {
            mi = mi ?? self.Type().GetField(memberName, FLAGS) ?? self.Type().GetProperty(memberName, FLAGS) as MemberInfo;
            if (mi is PropertyInfo)
                return (mi as PropertyInfo).GetValue(self.Inst(), new object[0]);
            else if (mi is FieldInfo)
                return (mi as FieldInfo).GetValue(self.Inst());
            else
                throw new Exception(string.Format("Reflection not found: {0} in {1}", memberName, self.Type()));
        }

        public static void Set(this object self, string memberName, object value, MemberInfo mi = null)
        {
            mi = mi ?? self.Type().GetField(memberName, FLAGS) ?? self.Type().GetProperty(memberName, FLAGS) as MemberInfo ?? self.Type().GetField(memberName, FLAGS);
            if (mi is PropertyInfo)
                (mi as PropertyInfo).SetValue(self.Inst(), value, new object[0]);
            else
                (mi as FieldInfo).SetValue(self.Inst(), value);
        }

        public static void AddEvent(this object self, string memberName, Action callback)
        {
            var ev = self.Get(memberName) as Action;
            ev += callback;
            self.Set(memberName, ev);
        }

        public static void AddEvent<T>(this object self, string memberName, Action<T> callback)
        {
            var ev = self.Get(memberName) as Action<T>;
            ev += callback;
            self.Set(memberName, ev);
        }
        public static void RemoveEvent(this object self, string memberName, Action callback)
        {
            var ev = self.Get(memberName) as Action;
            ev -= callback;
            self.Set(memberName, ev);
        }

        public static void RemoveEvent<T>(this object self, string memberName, Action<T> callback)
        {
            var ev = self.Get(memberName) as Action<T>;
            ev -= callback;
            self.Set(memberName, ev);
        }

        public static string DumpEvent(this object self, string memberName)
        {
            _sb.Length = 0;
            var fi = self.Type().GetField(memberName, FLAGS);
            var handler = fi.GetValue(self.Inst()) as MulticastDelegate;
            var invocationList = handler != null ? handler.GetInvocationList() : new Delegate[0];
            _sb.AppendFormat("Dump {0}.{1} event ({2} callbacks):\n", fi.DeclaringType, fi.Name, invocationList.Length);
            for (var i = 0; i < invocationList.Length; i++)
            {
                var m = invocationList[i].Method;
                _sb.AppendFormat("  -> [{0}({1})] {2}\n", m.DeclaringType, m.DeclaringType.Assembly.GetName().Name, m);
            }
            return _sb.ToString();
        }
    }
}
