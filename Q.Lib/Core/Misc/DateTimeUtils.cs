using System;

namespace Q.Lib.Core.Misc
{
    public static class DateTimeUtils
    {
      public static DateTime Posix = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc); 
      public static DateTime FromPosix(this long unixTimeStampMillis) => Posix.AddMilliseconds(unixTimeStampMillis);
    }
}