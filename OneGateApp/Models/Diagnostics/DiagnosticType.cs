namespace NeoOrder.OneGate.Models.Diagnostics;

public enum DiagnosticType : byte
{
    Root,
    Dynamic,
    Call,
    Invocation,
    Syscall
}
