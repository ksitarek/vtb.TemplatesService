using System;
using System.Runtime.Serialization;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateLabelAlreadyTakenException : Exception
    {
        public string Label { get; }

        public TemplateLabelAlreadyTakenException(string label)
                    : base($"Unable to create new Template. The label {label} is already taken.")
        {
            Label = label;
        }

        protected TemplateLabelAlreadyTakenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Label = info.GetString($"{nameof(TemplateLabelAlreadyTakenException)}.{nameof(Label)}");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(TemplateLabelAlreadyTakenException)}.{nameof(Label)}", Label);
        }
    }
}