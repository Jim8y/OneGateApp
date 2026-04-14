using NeoOrder.OneGate.Properties;

namespace NeoOrder.OneGate.Controls.Views.Validation;

public partial class StringLength : Validator
{
    public int Min { get; set; }
    public required int Max { get; set; }

    public override bool Check(object? value)
    {
        if (value is not string s) return false;
        return s.Length >= Min && s.Length <= Max;
    }

    public override string GetErrorMessage(string? title)
    {
        string format;
        if (ErrorMessage != null)
            format = ErrorMessage;
        else if (Min == 0)
            format = Strings.DefaultStringLengthErrorMessageMax;
        else if (Min == Max)
            format = Strings.DefaultStringLengthErrorMessageLength;
        else
            format = Strings.DefaultStringLengthErrorMessageMinMax;
        return string.Format(format, title ?? Strings.Field, Min, Max);
    }
}
