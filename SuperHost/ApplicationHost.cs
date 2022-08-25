using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SuperFramework.SuperHost
{
    [Serializable]
    public abstract class ApplicationHost : SafeObject, IApplicationHost
    {
        public ApplicationHost()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            AppDomain.CurrentDomain.DomainUnload += HostExited;
        }

        public event EventHandler OnExited;
        public abstract bool Init(DynamicEventHandler handler);

        public AppDomain AppDomain => AppDomain.CurrentDomain;

        public IEnumerable<Type> GetType(Func<Type, bool> predicate)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (predicate(type))
                        yield return type;
                }
            }
        }

        public static void Exit(ApplicationHost host)
        {
            host.Dispose();
            AppDomain.Unload(host.AppDomain);
        }

        protected virtual Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var domain = sender as AppDomain;
            Assembly assembly = null;
            var filePath = string.Empty;
            var match = Regex.Match(args.Name, "([^,]+), Version=([^,]+)");
            FileVersionInfo file = null;
            if (match.Success && match.Groups.Count == 3)
            {
                file = Directory.GetFiles(domain.BaseDirectory, $"{match.Groups[1].Value}.exe", SearchOption.AllDirectories).
                    Select(item => FileVersionInfo.GetVersionInfo(item)).
                    Where(item => item.FileVersion.Equals(match.Groups[2].Value)).FirstOrDefault();

                if (file == null)
                    file = Directory.GetFiles(domain.BaseDirectory, $"{match.Groups[1].Value}.dll", SearchOption.AllDirectories).
                        Select(item => FileVersionInfo.GetVersionInfo(item)).
                        Where(item => item.FileVersion.Equals(match.Groups[2].Value)).FirstOrDefault();
            }
            if (file != null)
                assembly = Assembly.LoadFrom(file.FileName);
            return assembly;
        }
        private void HostExited(object sender, EventArgs e) => OnExited?.Invoke(sender, e);
    }
}
