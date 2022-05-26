
using System.Reflection;
using Webhook.Application.Services.Interface;
using Webhook.DomainCore.Model;

namespace Webhook.Infrastructure.ShellExecutor.Bridge
{
    public static partial class BridgeSystem
    {
        public static IBridgeSystem Bash => new BridgeSystemBash();
    }

    public sealed class BridgeSystemBash : IBridgeSystem
    {
        public string GetFileName()
        {
            return "/bin/bash";
        }

      
        public string[] CommandConstructor(string command, Output? output = Output.Hidden, string dir = "")
        {
            var list = new List<string>();

            if (output == Output.External)
            {
                var pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cmd.sh");
                var assembly = typeof(BridgeSystemBat).GetTypeInfo().Assembly;
                var resourceName = assembly.GetManifestResourceNames().First(s =>
                    s.EndsWith("cmd.sh", StringComparison.CurrentCultureIgnoreCase));
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
                list.Add("-c");
                list.Add($"{command}");
            }

            return list.ToArray();
        }

     
    }
}