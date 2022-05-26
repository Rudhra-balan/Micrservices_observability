using System;
using System.Diagnostics;
using System.Text;
using Web.Application.Services.Interface;
using Web.DomainCore.Model;

namespace Web.Infrastructure.ShellExecutor.Bridge
{
    public sealed class ShellConfigurator
    {
        public ShellConfigurator(IBridgeSystem bridgeSystem)
        {
            BridgeSystem = bridgeSystem ?? throw new ArgumentException(nameof(bridgeSystem));
            if (!Platform.Platform.IsWin()) Execute("chmod +x cmd.sh");
        }

        private static IBridgeSystem BridgeSystem { get; set; }
        
        public Response Execute(string command, Output? output = Output.Hidden, string dir = "")
        {
            var result = new Response();
            var stderr = new StringBuilder();
            var stdout = new StringBuilder();

            var cmd = BridgeSystem.CommandConstructor(command, output, dir);

            var startInfo = new ProcessStartInfo
            {
                FileName = BridgeSystem.GetFileName()
            };
           
            foreach (var item in cmd)
                startInfo.ArgumentList.Add(item);

            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = output != Output.External;
            startInfo.RedirectStandardError = output != Output.External;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = output != Output.External;
            if (!string.IsNullOrEmpty(dir) && output != Output.External) startInfo.WorkingDirectory = dir;

            using var process = Process.Start(startInfo);
            switch (output)
            {
                case Output.Internal:
                    
                    while (process != null && !process.StandardOutput.EndOfStream)
                    {
                        var line = process.StandardOutput.ReadLine();
                        stdout.AppendLine(line);
                    }

                    while (process != null && !process.StandardError.EndOfStream)
                    {
                        var line = process.StandardError.ReadLine();
                        stderr.AppendLine(line);
                    }

                    break;
                case Output.Hidden:
                    stdout.AppendLine(process?.StandardOutput.ReadToEnd());
                    stderr.AppendLine(process?.StandardError.ReadToEnd());
                    break;
            }

            process?.WaitForExit();
            result.Stdout = stdout.ToString();
            result.Stderr = stderr.ToString();
            if (process != null) result.Code = process.ExitCode;

            return result;
        }

    }
}