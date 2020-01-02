using GraphQL.Types;
using GraphQL.Language.AST;
using GraphQL;

namespace GraphQLPlugin.ModelType
{
    public class UIntGraphType : ScalarGraphType
    {
        public override object ParseLiteral(IValue value)
        {
            switch (value)
            {
                case UIntValue uintValue:
                    return uintValue.Value;

                case IntValue intValue:
                    if (intValue.Value >= 0)
                        return (uint)intValue.Value;
                    return null;

                case LongValue longValue:
                    if (uint.MinValue <= longValue.Value && longValue.Value <= uint.MaxValue)
                        return (uint)longValue.Value;
                    return null;

                default:
                    return null;
            }
        }

        public override object ParseValue(object value) => ValueConverter.ConvertTo(value, typeof(uint));

        public override object Serialize(object value) => ParseValue(value);
    }

    public class ULongGraphType : ScalarGraphType
    {
        public override object ParseLiteral(IValue value)
        {
            switch (value)
            {
                case ULongValue ulongValue:
                    return ulongValue.Value;

                case IntValue intValue:
                    if (intValue.Value >= 0)
                        return (ulong)intValue.Value;
                    return null;

                case LongValue longValue:
                    if (longValue.Value >= 0)
                        return (ulong)longValue.Value;
                    return null;

                default:
                    return null;
            }
        }

        public override object ParseValue(object value) => ValueConverter.ConvertTo(value, typeof(ulong));

        public override object Serialize(object value) => ParseValue(value);
    }

    public class ByteGraphType : ScalarGraphType
    {
        public override object ParseLiteral(IValue value)
        {
            switch (value)
            {
                case ByteValue byteValue:
                    return byteValue.Value;

                case IntValue intValue:
                    if (byte.MinValue <= intValue.Value && intValue.Value <= byte.MaxValue)
                        return (byte)intValue.Value;
                    return null;

                default:
                    return null;
            }
        }

        public override object ParseValue(object value) => ValueConverter.ConvertTo(value, typeof(byte));

        public override object Serialize(object value) => ParseValue(value);
    }
    public class ByteValue : ValueNode<byte>
    {
        public ByteValue(byte value) => Value = value;

        protected override bool Equals(ValueNode<byte> other) => Value == other.Value;
    }

}
