using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Q.Mongo.Conventions
{
  public class UpperCaseElementNameConvention : IMemberMapConvention
  {
    public string Name => nameof(UpperCaseElementNameConvention);
    public void Apply(BsonMemberMap memberMap) => memberMap.SetElementName(memberMap.ElementName.ToUpper());
  }
}
