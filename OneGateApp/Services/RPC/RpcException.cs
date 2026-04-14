namespace NeoOrder.OneGate.Services.RPC;

class RpcException(int code, string message, object? data = null) : Exception(message)
{
    public int Code => code;
    public new object? Data => data;
}
