using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateKindKeyAlreadyTakenException : Exception
    {
        public string TemplateKindKey { get; }

        public TemplateKindKeyAlreadyTakenException(string templateKindKey) : base($"TemplateKind with TemplateKindKey: {templateKindKey} already exists")
        {
            TemplateKindKey = templateKindKey;
        }

        protected TemplateKindKeyAlreadyTakenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateKindKey = info.GetString($"{nameof(TemplateKindKeyAlreadyTakenException)}.{nameof(TemplateKindKey)}");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateKindKeyAlreadyTakenException)}.{nameof(TemplateKindKey)}", TemplateKindKey);
        }
    }
}