using System.Text.Json.Nodes;

namespace NeoOrder.OneGate.Controls.Views;

public sealed record BridgeInvocation(JsonObject Request, BridgeRequestContext Context);
