namespace NeoOrder.OneGate.Models.Diagnostics;

interface ICanCall
{
    public DiagnosticNode[] Calls { get; }
}
