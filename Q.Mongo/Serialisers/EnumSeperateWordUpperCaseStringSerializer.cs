using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Q.Mongo.Conventions;
using System;

namespace Q.Mongo.Serialisers
{
  public class EnumSeperateWordUpperCaseStringSerializer<T> : SerializerBase<T?> where T : struct, IConvertible
  {
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T? value)
    {
      if (value == null) context.Writer.WriteString("null");
      var str = SeperateWordsNamingConvention.SeperateWordRegex.Replace(value.ToString(), "_");
      context.Writer.WriteString(str.ToUpper());
    }
    public override T? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
      var str = context.Reader.ReadString();
      if (str == null || str == "" || str == "null") return null;
      return (T)Enum.Parse(typeof(T), str.Replace("_", ""), true);
    }
  }
}