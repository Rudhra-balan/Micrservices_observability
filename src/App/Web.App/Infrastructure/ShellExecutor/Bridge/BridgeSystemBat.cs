
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Web.Application.Services.Interface;
using Web.DomainCore.Model;

namespace Web.Infrastructure.ShellExecutor.Bridge
{
    public static partial class BridgeSystem
    {
        public static IBridgeSystem Bat => new BridgeSystemBat();
    }

    public sealed class BridgeSystemBat : IBridgeSystem
    {
        public string GetFileName()
        {
            return "cmd.exe";
        }

        public string[] CommandConstructor(string command, Output? output = Output.Hidden, string dir = "")
        {
           
            var list = new List<string>();

            if (output == Output.External)
            {
                var pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cmd.bat");
                var assembly = typeof(BridgeSystemBat).GetTypeInfo().Assembly;
                    var resourceName = assembly.GetManifestResourceNames().First(s =>
                        s.EndsWith("cmd.bat", StringComparison.CurrentCultureIgnoreCase));
                    using var stream = assembly.GetManifestResourceStream(resourceName);
                    using var fileStream = File.Create(pathToFile);
                    if (stream != null)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(fileStream);
                    }
                list.Add(pathToFile);
                list.Add($"{command}");
                if (!string.IsNullOrEmpty(dir)) list.Add($"{dir}");
            }
            else
            {
                list.Add("/c");
                list.Add($"{command}");
            }

            return list.ToArray();
        }

    }
}