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
    [WorksheetFunction("Q")]
    public static Array Expand(Array a, int rowOffset = 0, int colOffset = 1, string refTxt = null)
    {
      ExcelThread xlt = ExcelThread.Current;
      XlOper o = refTxt == null || refTxt == "" ? xlt.GetCaller() : xlt.Call(Excel.Functions.Indirect, refTxt);
      var des = xlt.Call(Excel.Functions.Offset, o, rowOffset, colOffset, a.GetLength(0), a.GetLength(1));
      Excel.AsyncRangeUpdate(xlt.RefText(des, false), a);
      return a;
      //var app = Excel.Application;
      //app.GetType().InvokeMember("Run", BindingFlags.InvokeMethod, null, app, new object[] { "QDefineRangeName", "hh" });
      //object workbook = app.GetType().InvokeMember("ActiveWorkbook", BindingFlags.GetProperty, null, app, null);
      //object properties = app.GetType().InvokeMember("BuiltinDocumentProperties", BindingFlags.GetProperty, null, workbook.GetType(), null);
      //object property = app.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, properties, new object[] { "Author" });
      //object value = app.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, property, null);
      //xlt.CallPrivileged(Excel.Commands.DefineName, "hehe", "$A$2");
      //xlt.CallUDF("QDefineRangeName", "hh");
    }
    [WorksheetFunction("QAddress", MacroClass.CommandEquivalent)]
    public static string Address([ExcelReference] XlOper o)
    {
      ExcelThread xlt = ExcelThread.Current;
      var ret = xlt.Call(Excel.Functions.GetCell, 1, o);
      return ret.AsString();
    }
    //[ExcelCommand("QDefineRangeName", DetectUserAbort = true)]
    //public static void DefineRangeName(string name)
    //{
    //  ExcelThread xlt = ExcelThread.Current;
    //  XlOper rng = new XlOper(new ExcelRangeCollection(10, 10));
    //  xlt.Call(Excel.Commands.DefineName, name, rng);
    //  //xlt.Set(new ExcelRangeCollection(20, 20, 20, 20), 10);
    //}
  }
}
