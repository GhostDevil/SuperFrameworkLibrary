﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace SuperFramework
{
    public delegate object DynamicEventHandler(object sender, EventInfo @event, object[] args);
    public class DynamicEvent : SafeObject
    {
        private bool _binding = false;
        private IntPtr _handle = IntPtr.Zero;
        private object Target { get; }
        private EventInfo EveInfo { get; }
        private Delegate _method;
        private static List<DynamicEvent> _bindEvents = new List<DynamicEvent>();

        public DynamicEvent(object target, EventInfo e)
        {
            _handle = GCHandle.ToIntPtr(GCHandle.Alloc(this));
            GC.KeepAlive(this);
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

        public event DynamicEventHandler OnExecute;

        public void Bind()
        {
            if (!_binding)
            {
                lock (this)
                {
                    if (!_binding)
                    {
                        EveInfo.AddEventHandler(Target, _method);
                        _bindEvents.Add(this);
                        return;
                    }
                }
            }
            throw new NotSupportedException("事件已绑定, 请解绑后再尝试!");
        }

        public void UnBind()
        {
            if (!_binding)
            {
                lock (this)
                {
                    if (!_binding)
                    {
                        EveInfo.RemoveEventHandler(Target, _method);
                        _bindEvents.Remove(this);
                    }
                }
            }
        }

        private object Invoke(object[] args)
        {
            try
            {
                if (args.Length > 0 && args[0] == Target)
                {
                    var arr = new object[args.Length - 1];
                    Array.Copy(args, 1, arr, 0, arr.Length);
                    args = arr;
                }
                return OnExecute?.Invoke(Target, EveInfo, args);
            }
            catch (Exception e)
            {
                Console.WriteLine($"动态事件执行失败! 当前对象: 【{JsonConvert.SerializeObject(Target)}】, 当前事件: 【{EveInfo.Name}】, 通知数据: 【{JsonConvert.SerializeObject(args)}】, 错误信息: 【{e.Message}】");
            }
            return null;
        }

        internal static object OnEventExecute(IntPtr self, object[] args)
        {
            return _bindEvents.FirstOrDefault(q => q._handle.Equals(self))?.Invoke(args);
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