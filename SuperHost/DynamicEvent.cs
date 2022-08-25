using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SuperFramework.SuperHost
{
    public delegate object DynamicEventHandler(object sender, string eventName, object[] args);
    public class DynamicEvent : SafeObject
    {
        private readonly IntPtr _handle = IntPtr.Zero;
        private readonly Delegate _method;
        private DynamicEventHandler _handler;
        private static readonly List<DynamicEvent> _bindEvents = new List<DynamicEvent>();

        public DynamicEvent(object target, EventInfo e)
        {
            _handle = GCHandle.ToIntPtr(GCHandle.Alloc(this));
            Target = target;
            EveInfo = e;
            var types = EveInfo.EventHandlerType.GetMethod("Invoke").GetParameters().Select(q => q.ParameterType).ToArray();
            var method = new DynamicMethod(string.Empty, null, types, typeof(DynamicEvent).Module);
            var gen = method.GetILGenerator();
            if (IntPtr.Size == 8)
                gen.Emit(OpCodes.Ldc_I8, _handle.ToInt64());
            else
                gen.Emit(OpCodes.Ldc_I4, _handle.ToInt32());
            //gen.Emit(OpCodes.Ldstr, e.Name);
            gen.Emit(OpCodes.Ldc_I4_S, types.Length);
            gen.Emit(OpCodes.Newarr, typeof(object));
            for (int i = 0; i < types.Length; i++)
            {
                gen.Emit(OpCodes.Dup);
                gen.Emit(OpCodes.Ldc_I4, i);
                gen.Emit(OpCodes.Ldarg, i);
                if (types[i].IsValueType)
                    gen.Emit(OpCodes.Box, types[i]);
                gen.Emit(OpCodes.Stelem_Ref);
            }
            gen.Emit(OpCodes.Call, typeof(DynamicEvent).GetMethod("OnEventExecute", BindingFlags.NonPublic | BindingFlags.Static));
            gen.Emit(OpCodes.Pop);
            gen.Emit(OpCodes.Ret);
            _method = method.CreateDelegate(EveInfo.EventHandlerType);
        }

        public object Target { get; }
        public EventInfo EveInfo { get; }

        public void Bind(DynamicEventHandler eventHandler)
        {
            if (_handler == null)
            {
                lock (this)
                {
                    if (_handler == null)
                    {
                        EveInfo.AddEventHandler(Target, _method);
                        _handler = eventHandler;
                        _bindEvents.Add(this);
                        return;
                    }
                }
            }
            throw new NotSupportedException("事件已绑定, 请解绑后再尝试!");
        }

        public void UnBind()
        {
            if (_handler != null)
            {
                lock (this)
                {
                    if (_handler != null)
                    {
                        EveInfo.RemoveEventHandler(Target, _method);
                        _handler = null;
                        _bindEvents.Remove(this);
                    }
                }
            }
        }

        internal static object OnEventExecute(IntPtr self, object[] args)
        {
#pragma warning disable CS1690 // 访问引用封送类的字段上的成员可能导致运行时异常
            var context = _bindEvents.FirstOrDefault(q => q._handle.Equals(self));
#pragma warning restore CS1690 // 访问引用封送类的字段上的成员可能导致运行时异常
            if (context != null)
            {
                if (args.Length > 0 && args[0] == context.Target)
                {
                    var arr = new object[args.Length - 1];
                    Array.Copy(args, 1, arr, 0, arr.Length);
                    args = arr;
                }
                return context._handler?.Invoke(context.Target, context.EveInfo.Name, args);
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                UnBind();
            }
            catch
            { }
            try
            {
                GCHandle.FromIntPtr(_handle).Free();
            }
            catch
            { }
            base.Dispose(disposing);
        }
    }
}

