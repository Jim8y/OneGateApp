namespace NeoOrder.OneGate.Models;

class DapiException(int code, string message, InvocationResult? data = null) : Exception(message)
{
    public int Code => code;
    public new InvocationResult? Data => data;
}
