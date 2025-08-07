﻿using FTOptix.HMIProject;
using System.Runtime.InteropServices;

namespace FTOptixNetPlugin.Tools
{
    public static class LogFile
    {
        public static string GetLogFilePath()
        {
            var logFilePath = FindParentFolder(Project.Current.ApplicationDirectory, "FTOptixApplication");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string osRelease = File.ReadAllText("/etc/os-release").ToLower();
                if (!osRelease.Contains("ubuntu"))
                {
                    logFilePath = "/persistent/log/Rockwell_Automation/FactoryTalk_Optix/FTOptixApplication";
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logFilePath ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Rockwell Automation\FactoryTalk Optix\Emulator\Log", Project.Current.BrowseName);
            }
            logFilePath = Path.Combine(logFilePath, "FTOptixRuntime.0.log");
            return logFilePath;


            
        }










        internal static string FindParentFolder(string startPath, string targetFolderName)
        {
            string currentPath = startPath;
            while (currentPath != null)
            {
                var directoryInfo = new DirectoryInfo(currentPath);
                if (directoryInfo.Name.Equals(targetFolderName, StringComparison.OrdinalIgnoreCase))
                {
                    return currentPath;
                }
                currentPath = directoryInfo.Parent?.FullName;
            }
            return null;
        }
    }
}
