using System;

namespace Q.Lib.Finance.Currency {
  public class GBP : Currency<GBP.Unit>{
    public long Pence { get => value; set => this.value = value; }
    public decimal Pound { get => Pence / 100m; set => Pence = Convert.ToInt64(value * 100); }
    public decimal Mil { get => Pence / 1e8m; set => Pence = Convert.ToInt64(value * 1e8m); }
    public override string ISO => "GBP";
    public override string Sign => "£";
    public GBP(long pence) => Pence = pence;
    public GBP(decimal pound) => Pound = pound;
    public GBP(double pound) => Pound = Convert.ToDecimal(pound);
    public override string ToString() => ToString(DefaultUnit);
    public string ToString(Unit u, string format = null) => u == Unit.Pence ? Pence.ToString(format) + "p" : "£" + Pound.ToString(format);
    public enum Unit { Pence, Pound }
  }
}
