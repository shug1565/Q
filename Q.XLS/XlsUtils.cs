using System;
using System.Collections.Generic;
using System.Text;
using ManagedXLL;
using ManagedXLL.Extensibility;
using ManagedXLL.Interop;
using ManagedXLL.RunTime;
using ManagedXLL.Configuration;
using System.IO;
using System.Linq;
using Q.Lib.Core.Misc;
using System.Reflection;

namespace Q.XLS
{
  public abstract class XlsUtils
  {
    [WorksheetFunction("QListFiles")]
    public static XlOper[] ListFiles(string dn, string sel = "*")
    {
      DirectoryInfo d = new DirectoryInfo(dn);
      FileInfo[] Files = d.GetFiles(sel);
      var ret = Files.Select(x => x.Name.ToXlOper()).ToArray();
      return ret;
    }
    [WorksheetFunction("QListDir")]
    public static XlOper[] ListDir(string dir)
    {
      var folders = Directory.GetDirectories(dir);
      var ret = folders.Select(x => x.ToXlOper()).ToArray();
      return ret;
    }
    [WorksheetFunction("QFromPosixMilliSec")]
    public static DateTime FromPosixMilliSec(long ms) => ms.FromPosix();
    [WorksheetFunction("Q", MacroClass.AsyncWait)]
    public static Array Expand(Array a)
    {
      ExcelThread xlt = ExcelThread.Current;
      var app = Excel.Application;
      //app.GetType().InvokeMember("Run", BindingFlags.InvokeMethod, null, app, new object[] { "QQ" });
      object workbook = app.GetType().InvokeMember("ActiveWorkbook", BindingFlags.GetProperty, null, app, null);
      //object properties = app.GetType().InvokeMember("BuiltinDocumentProperties", BindingFlags.GetProperty, null, workbook.GetType(), null);
      //object property = app.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, properties, new object[] { "Author" });
      //object value = app.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, property, null);
      //xlt.CallPrivileged(Excel.Commands.DefineName, "hehe2", "$A$2");
      Excel.CallUDF("QQ");
      XlOper caller = xlt.GetCaller();
      var des = xlt.Call(Excel.Functions.Offset, caller, 1, 0, a.GetLength(0), a.GetLength(1));
      Excel.AsyncRangeUpdate(xlt.RefText(des, false), a);
      return a;
    }
    [ExcelCommand("QQ")]
    public static void Macro()
    {
      ExcelThread xlt = ExcelThread.Current;
      xlt.Call(Excel.Commands.DefineName, "hehe", "$A$1");
      xlt.Set(new ExcelRangeCollection(20, 20), 10);
    }
  }
}
