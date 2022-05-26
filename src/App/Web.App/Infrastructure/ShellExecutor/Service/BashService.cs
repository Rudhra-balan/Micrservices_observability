﻿

using Microsoft.Extensions.Logging;
using System;
using Web.Application.Services.Interface;
using Web.DomainCore.Model;
using Web.Infrastructure.ShellExecutor.Bridge;
using OperatingSystem = Web.Infrastructure.ShellExecutor.Platform.OperatingSystem;

namespace Web.Infrastructure.ShellExecutor.Service
{
    public class BashService: IBashService
    {
        private static IBridgeSystem BridgeSystem { get; set; }
        private ShellConfigurator Shell { get; set; }
        private ILogger<BashService> _logger { get; set; }
     
        public BashService(ILogger<BashService> logger)
        {
            
            BridgeSystem = Platform.Platform.GetCurrent() switch
            {
                OperatingSystem.Windows => Bridge.BridgeSystem.Bat,
                OperatingSystem.Linux => Bridge.BridgeSystem.Bash,
                OperatingSystem.Mac => Bridge.BridgeSystem.Bash,
                _ => BridgeSystem
            };

            Shell = new ShellConfigurator(BridgeSystem);

            _logger = logger;
        }


        public Response Run(string command, Output output = Output.Internal)
        {
            var result = new Response();
            try
            {
                result = Shell.Execute(command, output);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bash Excutor error for Request {@command}", command);
                result.Exception = ex;
            }

            return result;

        }

    }
}
