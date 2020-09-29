using System;

namespace vtb.TemplatesService.BusinessLogic.Exceptions
{
    public class TemplateLabelAlreadyTakenException : Exception
    {
        public TemplateLabelAlreadyTakenException(string label)
            : base($"Unable to create new Template. The label {label} is already taken.")
        {
            Label = label;
        }

        public string Label { get; }
    }
}