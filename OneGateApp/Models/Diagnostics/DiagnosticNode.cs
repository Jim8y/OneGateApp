using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models.Diagnostics;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(DiagnosticRoot), "root")]
[JsonDerivedType(typeof(DynamicScript), "dynamic")]
[JsonDerivedType(typeof(InternalCall), "call")]
[JsonDerivedType(typeof(Invocation), "invocation")]
[JsonDerivedType(typeof(Syscall), "syscall")]
public abstract class DiagnosticNode
{
}
