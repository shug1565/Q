using System;
using System.Collections.Generic;
using System.Text;
using ManagedXLL;
using System.IO;
using System.Linq;

namespace Q.XLS
{
  public static class XlsUtils
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
  }
}
