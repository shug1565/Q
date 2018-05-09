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
    [WorksheetFunction("QDir")]
    public static XlOper[] Dir(string dn, string sel = "*")
    {
      DirectoryInfo d = new DirectoryInfo(dn);
      FileInfo[] Files = d.GetFiles(sel);
      var ret = Files.Select(x => x.Name.ToXlOper()).ToArray();
      return ret;
    }
  }
}
