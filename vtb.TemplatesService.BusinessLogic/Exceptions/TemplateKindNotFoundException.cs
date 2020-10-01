using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    [Serializable]
    public class TemplateKindNotFoundException : Exception
    {
        public string TemplateKindKey { get; }

        public TemplateKindNotFoundException(string templateKindKey) : base($"TemplateKind with TemplateKindKey: {templateKindKey} does not exists")
        {
            TemplateKindKey = templateKindKey;
        }

        protected TemplateKindNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateKindKey = info.GetString($"{nameof(TemplateKindNotFoundException)}.{nameof(TemplateKindKey)}");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateKindNotFoundException)}.{nameof(TemplateKindKey)}", TemplateKindKey);
        }
    }
}