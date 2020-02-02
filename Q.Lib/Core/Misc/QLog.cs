using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Q.Lib.Core.Misc
{
  public static class QLog
  {
    private static Logger logger = null;
    private static ManualResetEvent waitHandle = new ManualResetEvent(false);
    static QLog()
    {
      //#if DEBUG
      //      // Setup the logging view for Sentinel - http://sentinel.codeplex.com
      //      var sentinalTarget = new NLogViewerTarget()
      //      {
      //        Name = "sentinal",
      //        Address = "udp://127.0.0.1:9999",
      //        IncludeNLogData = false
      //      };
      //      var sentinalRule = new LoggingRule("*", LogLevel.Trace, sentinalTarget);
      //      LogManager.Configuration.AddTarget("sentinal", sentinalTarget);
      //      LogManager.Configuration.LoggingRules.Add(sentinalRule);

      //      // Setup the logging view for Harvester - http://harvester.codeplex.com
      //      var harvesterTarget = new OutputDebugStringTarget()
      //      {
      //        Name = "harvester",
      //        Layout = "${log4jxmlevent:includeNLogData=false}"
      //      };
      //      var harvesterRule = new LoggingRule("*", LogLevel.Trace, harvesterTarget);
      //      LogManager.Configuration.AddTarget("harvester", harvesterTarget);
      //      LogManager.Configuration.LoggingRules.Add(harvesterRule);
      //#endif

      var config = new LoggingConfiguration();

      var consoleTarget = new ColoredConsoleTarget("target1")
      {
        Layout = @"${date:format=HH\:mm\:ss} ${level} ${message} ${exception}"
      };

      var fileTarget = new FileTarget("target2")
      {
        FileName = AppDomain.CurrentDomain.BaseDirectory + "run.log",
        Layout = "${longdate} ${level} ${message}  ${exception}"
      };

      config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);  // add to file
      config.AddRuleForAllLevels(consoleTarget); // all to console

      NLog.LogManager.Configuration = config;

      Task.Run(() => { 
        logger = LogManager.GetCurrentClassLogger();
        waitHandle.Set();
      });
    }
    public static Task Info(string msg)
    {
      waitHandle.WaitOne();
      return Task.Run(() => logger.Info(msg));
    }
    public static Task Error(string msg)
    {
      waitHandle.WaitOne();
      return Task.Run(() => logger.Error(msg));
    }
    public static Task Error(Exception e, string msg)
    {
      waitHandle.WaitOne();
      return Task.Run(() => logger.Error(e, msg));
    }
  }
}
