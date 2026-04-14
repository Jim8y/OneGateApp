namespace NeoOrder.OneGate.Models.Diagnostics;

public class Diagnostic
{
    public required DiagnosticRoot Traces { get; init; }

    public IReadOnlyCollection<Invocation> FindAllInvocations()
    {
        var invocations = new List<Invocation>();
        FindAllInvocations(Traces, invocations);
        return invocations;
    }

    static void FindAllInvocations(ICanCall node, List<Invocation> invocations)
    {
        if (node is Invocation invocation)
            invocations.Add(invocation);
        foreach (var call in node.Calls.OfType<ICanCall>())
            FindAllInvocations(call, invocations);
    }
}
