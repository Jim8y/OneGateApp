using Neo;
using Neo.Cryptography.ECC;
using Neo.Extensions;
using Neo.SmartContract;
using Neo.VM.Types;
using System.Numerics;
using Map = Neo.VM.Types.Map;

namespace NeoOrder.OneGate.Models;

static class Extensions
{
    extension(Map map)
    {
        public string? TryGetString(PrimitiveType key)
        {
            if (map.TryGetValue(key, out var value))
                return value.GetString();
            return null;
        }
    }

    extension(ContractParameter parameter)
    {
        public byte[] GetByteArray()
        {
            return parameter.Type switch
            {
                ContractParameterType.Integer => ((BigInteger)parameter.Value!).ToByteArrayStandard(),
                ContractParameterType.ByteArray => (byte[])parameter.Value!,
                ContractParameterType.String => Utility.StrictUTF8.GetBytes((string)parameter.Value!),
                ContractParameterType.Hash160 => ((UInt160)parameter.Value!).ToArray(),
                ContractParameterType.Hash256 => ((UInt256)parameter.Value!).ToArray(),
                ContractParameterType.PublicKey => ((ECPoint)parameter.Value!).EncodePoint(true),
                ContractParameterType.Signature => (byte[])parameter.Value!,
                _ => throw new InvalidCastException($"Cannot convert parameter of type {parameter.Type} to byte array")
            };
        }

        public BigInteger GetBigInteger()
        {
            return parameter.Type switch
            {
                ContractParameterType.Integer => (BigInteger)parameter.Value!,
                ContractParameterType.ByteArray => new BigInteger((byte[])parameter.Value!),
                _ => throw new InvalidCastException($"Cannot convert parameter of type {parameter.Type} to BigInteger")
            };
        }

        public UInt160 GetHash160()
        {
            return parameter.Type switch
            {
                ContractParameterType.ByteArray => (byte[])parameter.Value!,
                ContractParameterType.Hash160 => (UInt160)parameter.Value!,
                _ => throw new InvalidCastException($"Cannot convert parameter of type {parameter.Type} to UInt160")
            };
        }
    }
}
