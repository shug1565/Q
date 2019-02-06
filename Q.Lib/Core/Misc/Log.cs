﻿using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Q.Lib.Core.Misc
{
  public static class Log
  {
    public static Logger NLogger { get; private set; }
    static Log()
    {
#if DEBUG
      // Setup the logging view for Sentinel - http://sentinel.codeplex.com
      var sentinalTarget = new NLogViewerTarget()
      {
        Name = "sentinal",
        Address = "udp://127.0.0.1:9999",
        IncludeNLogData = false
      };
      var sentinalRule = new LoggingRule("*", LogLevel.Trace, sentinalTarget);
      LogManager.Configuration.AddTarget("sentinal", sentinalTarget);
      LogManager.Configuration.LoggingRules.Add(sentinalRule);

      // Setup the logging view for Harvester - http://harvester.codeplex.com
      var harvesterTarget = new OutputDebugStringTarget()
      {
        Name = "harvester",
        Layout = "${log4jxmlevent:includeNLogData=false}"
      };
      var harvesterRule = new LoggingRule("*", LogLevel.Trace, harvesterTarget);
      LogManager.Configuration.AddTarget("harvester", harvesterTarget);
      LogManager.Configuration.LoggingRules.Add(harvesterRule);
#endif

      LogManager.ReconfigExistingLoggers();

      NLogger = LogManager.GetCurrentClassLogger();
    }
  }
}
