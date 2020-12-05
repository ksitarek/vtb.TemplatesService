using MongoDB.Bson.Serialization.Attributes;

namespace vtb.TemplatesService.DomainModel
{
    public class TemplateKind
    {
        [BsonId]
        public string TemplateKindKey { get; set; }
    }
}