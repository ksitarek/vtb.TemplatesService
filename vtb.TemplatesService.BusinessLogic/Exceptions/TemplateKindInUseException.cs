using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    [Serializable]
    public class TemplateKindInUseException : Exception
    {
        public string TemplateKindKey { get; }

        public TemplateKindInUseException(string templateKindKey) : base($"TemplateKind with TemplateKindKey: {templateKindKey} is still in use")
        {
            TemplateKindKey = templateKindKey;
        }

        protected TemplateKindInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TemplateKindKey = info.GetString($"{nameof(TemplateKindInUseException)}.{nameof(TemplateKindKey)}");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateKindInUseException)}.{nameof(TemplateKindKey)}", TemplateKindKey);
        }
    }
}