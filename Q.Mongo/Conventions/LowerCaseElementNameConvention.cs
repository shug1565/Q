using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Q.Mongo.Conventions
{
  public class LowerCaseElementNameConvention : IMemberMapConvention
  {
    public string Name => nameof(LowerCaseElementNameConvention);
    public void Apply(BsonMemberMap memberMap) => memberMap.SetElementName(memberMap.ElementName.ToLower());
  }
}
