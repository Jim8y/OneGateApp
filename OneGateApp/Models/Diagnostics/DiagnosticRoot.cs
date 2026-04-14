namespace NeoOrder.OneGate.Models.Diagnostics;

public class DiagnosticRoot : DiagnosticNode, ICanCall
{
    public required DiagnosticNode[] Calls { get; init; }
}
