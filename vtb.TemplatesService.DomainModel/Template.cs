using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace vtb.TemplatesService.DomainModel
{
    [DebuggerDisplay("{Label}")]
    public class Template
    {
        [BsonId]
        public Guid TemplateId { get; set; }

        public Guid TenantId { get; set; }

        public string Label { get; set; }
        public string TemplateKindKey { get; set; }
        public bool IsDefault { get; set; } = false;
        public List<TemplateVersion> Versions { get; set; } = new List<TemplateVersion>();

        [BsonIgnore]
        public TemplateVersion ActiveVersion => Versions?.SingleOrDefault(v => v.IsActive);
    }
}