namespace NeoOrder.OneGate.Services.RPC;

[AttributeUsage(AttributeTargets.Method)]
class RpcMethodAttribute(string? name = null) : Attribute
{
    public string? Name => name;
}
