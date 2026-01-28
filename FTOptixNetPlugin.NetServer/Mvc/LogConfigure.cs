using NLog.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Mvc
{
    public class LogConfigure
    {


        List<LogRule> rules = new List<LogRule>();

        public void AddConsole(LogLevel minLevel,LogLevel maxLevel)
        {
            var target = new NLog.Targets.ConsoleTarget($"logconsole_{System.DateTime.Now.Ticks.ToString()}" );


            rules.Add(new LogRule()
            {
                Minlv = GetLevel(minLevel),
                Maxlv = GetLevel(maxLevel),
                Target = target

            });
        }


        public void AddColordConsole(LogLevel minLevel, LogLevel maxLevel)
        {
            var target = new NLog.Targets.ColoredConsoleTarget($"logcolorconsole_{System.DateTime.Now.Ticks.ToString()}");


            rules.Add(new LogRule()
            {
                Minlv = GetLevel(minLevel),
                Maxlv = GetLevel(maxLevel),
                Target = target

            });
        }


        public void AddFile(LogLevel minLevel, LogLevel maxLevel,string fileName)
        {
            var target = new NLog.Targets.FileTarget($"logfile_{System.DateTime.Now.Ticks.ToString()}") { FileName = fileName,Encoding =Encoding.UTF8};


            rules.Add(new LogRule()
            {
                Minlv = GetLevel(minLevel),
                Maxlv = GetLevel(maxLevel),
                Target = target

            });
        }



        private NLog.LogLevel GetLevel(LogLevel lv)
        {
            switch (lv)
            {
                case LogLevel.Trace:
                    return NLog.LogLevel.Trace;
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;

                case LogLevel.Info:
                    return NLog.LogLevel.Info;

                case LogLevel.Warn:
                    return NLog.LogLevel.Warn;

                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Fatal:
                    return NLog.LogLevel.Fatal;
                default:
                    return NLog.LogLevel.Off;
            }
        }



        public LoggingConfiguration Build()
        {
            var config = new NLog.Config.LoggingConfiguration();
            foreach (var rule in rules)
            {
                config.AddRule(rule.Minlv, rule.Maxlv, rule.Target);
            }
            NLog.LogManager.Configuration = config;
            return config;
        }

        }

    public enum LogLevel
    {
        Trace=0,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    
    }

    

    internal class LogRule
    {
        internal NLog.LogLevel Minlv { get; set; }
        internal NLog.LogLevel Maxlv { get; set; }
        internal NLog.Targets.Target Target { get; set; }
    }



}
