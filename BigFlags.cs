using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace BigThings
{
    /// <summary>
    /// The BigFlags struct behaves like a Flags enumerated type.
    /// <para>
    /// Note that if this struct will be stored in some type of data
    /// store, it should be stored as a string type. There are two
    /// reasons for this:
    /// </para>
    /// <para>
    /// 1. Presumably, this pattern is being used because the number
    /// of values will exceed 64 (max positions in a long flags enum).
    /// Since this is so, there is in any case no numeric type which
    /// can store all the possible combinations of flags.
    /// </para>
    /// <para>
    /// 2. The "enum" values are assigned based on the order that the
    /// static public fields are defined. It is much safer to store
    /// these fields by name in case the fields are rearranged. This
    /// is particularly important if this represents a permission set!
    /// </para>
    /// </summary>
    [TypeConverter(typeof(BigFlagsConverter))]
    [SuppressMessage("ReSharper", "UnassignedReadonlyField", Justification = "This thing populate it self.")]
    public struct BigFlags : IComparable, IConvertible, IEquatable<BigFlags>, IComparable<BigFlags>
    {
        #region State

        private static readonly IList<FieldInfo> Fields = typeof(BigFlags).GetFields(BindingFlags.Public | BindingFlags.Static).ToList();
        private static readonly IList<BigFlags> FieldValues = GetFieldValues();

        // ?
        private static readonly bool ZeroInit = true;

        private BigInteger _value;
        public BigInteger Value => _value;

        /// <summary>
        /// Creates a value taking ZeroInit into consideration.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static BigInteger CreateValue(int index)
        {
            if (ZeroInit && index == 0) return 0;
            
            var idx = ZeroInit ? index - 1 : index;

            return new BigInteger(1) << idx;
        }

        #endregion State

        #region Construction

        /// <summary>
        /// Static constructor. Sets the static public fields.
        /// </summary>
        private static IList<BigFlags> GetFieldValues()
        {
            var fieldValues = new List<BigFlags>();
            for (var i = 0; i < Fields.Count; i++)
            {
                var field = Fields[i];
                var fieldVal = new BigFlags { _value = CreateValue(i) };
                field.SetValue(null, fieldVal);
                fieldValues.Add(fieldVal);
            }
            return fieldValues;
        }

        #endregion Construction

        #region Operators

        /// <summary>
        /// OR operator. Or together BigFlags instances.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static BigFlags operator | (BigFlags lhs, BigFlags rhs) => new BigFlags { _value = lhs._value | rhs._value };

        /// <summary>
        /// AND operator. And together BigFlags instances.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static BigFlags operator &(BigFlags lhs, BigFlags rhs) => new BigFlags {_value = lhs._value & rhs._value};

        /// <summary>
        /// XOR operator. Xor together BigFlags instances.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static BigFlags operator ^(BigFlags lhs, BigFlags rhs) => new BigFlags {_value = lhs._value ^ rhs._value};

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(BigFlags lhs, BigFlags rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(BigFlags lhs, BigFlags rhs) => !(lhs == rhs);

        /// <summary>
        /// Greater than operator.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator >(BigFlags lhs, BigFlags rhs) => lhs > rhs;

        /// <summary>
        /// Lower than operator.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator <(BigFlags lhs, BigFlags rhs) => lhs < rhs;


        /// <summary>
        /// Greater than or equal operator.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator >=(BigFlags lhs, BigFlags rhs) => lhs > rhs;

        /// <summary>
        /// Lower than or equal operator.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator <=(BigFlags lhs, BigFlags rhs) => lhs < rhs;

        #endregion Operators

        #region System.Object Overrides

        /// <summary>
        /// Overridden. Returns a comma-separated string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (ZeroInit && _value == 0) return Fields[0].Name;
            var names = new List<string>();
            for (var i = 0; i < Fields.Count; i++)
            {
                if (ZeroInit && i == 0) continue;
                var bi = CreateValue(i);
                if ((_value & bi) == bi) names.Add(Fields[i].Name);
            }
            return string.Join(", ", names);
        }

        /// <summary>
        /// Overridden. Gets the hash code of the internal BitArray.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Overridden. Compares equality with another object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is BigFlags flags && Equals(flags);

        #endregion System.Object Overrides


        #region IEquatable<BigFlags> Members

        /// <summary>
        /// Strongly-typed equality method.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BigFlags other) => _value == other._value;

        #endregion IEquatable<BigFlags> Members



        #region IComparable<BigFlags> Members

        /// <summary>
        /// Compares based on highest bit set. Instance with higher
        /// bit set is bigger.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(BigFlags other) => _value.CompareTo(other._value);
        

        #endregion IComparable<BigFlags> Members

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if (obj is BigFlags flags) return CompareTo(flags);
            return -1;
        }
        #endregion IComparable Members

        #region IConvertible Members

        /// <summary>
        /// Returns TypeCode.Object.
        /// </summary>
        /// <returns></returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider) => throw new NotSupportedException();
        
        byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(_value);

        char IConvertible.ToChar(IFormatProvider provider) => throw new NotSupportedException();

        DateTime IConvertible.ToDateTime(IFormatProvider provider) => throw new NotSupportedException();

        decimal IConvertible.ToDecimal(IFormatProvider provider) => Convert.ToDecimal(_value);
        
        double IConvertible.ToDouble(IFormatProvider provider) => Convert.ToDouble(_value);

        short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(_value);

        int IConvertible.ToInt32(IFormatProvider provider) => Convert.ToInt32(_value);

        long IConvertible.ToInt64(IFormatProvider provider) => Convert.ToInt64(_value);

        sbyte IConvertible.ToSByte(IFormatProvider provider) => Convert.ToSByte(_value);

        float IConvertible.ToSingle(IFormatProvider provider) => Convert.ToSingle(_value);
        
        string IConvertible.ToString(IFormatProvider provider) => ToString();

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            var tc = TypeDescriptor.GetConverter(this);

            return tc.ConvertTo(this, conversionType) ?? throw new InvalidOperationException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(_value);

        uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(_value);

        ulong IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64(_value);


        #endregion IConvertible Members

        #region Public Interface

        /// <summary>
        /// Checks <paramref name="flags"/> to see if all the bits set in
        /// that flags are also set in this flags.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool HasFlag(BigFlags flags) => (this & flags) == flags;
        
        /// <summary>
        /// Gets the names of this BigFlags enumerated type.
        /// </summary>
        /// <returns></returns>
        public static string[] GetNames() => Fields.Select(x => x.Name).ToArray();

        /// <summary>
        /// Gets all the values of this BigFlags enumerated type.
        /// </summary>
        /// <returns></returns>
        public static BigFlags[] GetValues() => FieldValues.ToArray();
        
        /// <summary>
        /// Standard TryParse pattern. Parses a BigFlags result from a string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out BigFlags result)
        {
            result = new BigFlags();

            if (string.IsNullOrEmpty(s)) return true;

            var fieldNames = s.Split(',');
            foreach (var f in fieldNames)
            {
                var field = Fields.FirstOrDefault(x => string.Equals(x.Name, f.Trim(), StringComparison.OrdinalIgnoreCase));
                if (null == field)
                {
                    result = new BigFlags();
                    return false;
                }
                var i = Fields.IndexOf(field);
                result._value |= CreateValue(i);
            }
            return true;
        }

        public static readonly BigFlags Value0;
        public static readonly BigFlags Value1;
        public static readonly BigFlags Value2;
        public static readonly BigFlags Value3;
        public static readonly BigFlags Value4;
        public static readonly BigFlags Value5;
        public static readonly BigFlags Value6;
        public static readonly BigFlags Value7;
        public static readonly BigFlags Value8;
        public static readonly BigFlags Value9;
        public static readonly BigFlags Value10;
        public static readonly BigFlags Value11;
        public static readonly BigFlags Value12;
        public static readonly BigFlags Value13;
        public static readonly BigFlags Value14;
        public static readonly BigFlags Value15;
        public static readonly BigFlags Value16;
        public static readonly BigFlags Value17;
        public static readonly BigFlags Value18;
        public static readonly BigFlags Value19;
        public static readonly BigFlags Value20;
        public static readonly BigFlags Value21;
        public static readonly BigFlags Value22;
        public static readonly BigFlags Value23;
        public static readonly BigFlags Value24;
        public static readonly BigFlags Value25;
        public static readonly BigFlags Value26;
        public static readonly BigFlags Value27;
        public static readonly BigFlags Value28;
        public static readonly BigFlags Value29;
        public static readonly BigFlags Value30;
        public static readonly BigFlags Value31;
        public static readonly BigFlags Value32;
        public static readonly BigFlags Value33;
        public static readonly BigFlags Value34;
        public static readonly BigFlags Value35;
        public static readonly BigFlags Value36;
        public static readonly BigFlags Value37;
        public static readonly BigFlags Value38;
        public static readonly BigFlags Value39;
        public static readonly BigFlags Value40;
        public static readonly BigFlags Value41;
        public static readonly BigFlags Value42;
        public static readonly BigFlags Value43;
        public static readonly BigFlags Value44;
        public static readonly BigFlags Value45;
        public static readonly BigFlags Value46;
        public static readonly BigFlags Value47;
        public static readonly BigFlags Value48;
        public static readonly BigFlags Value49;
        public static readonly BigFlags Value50;
        public static readonly BigFlags Value51;
        public static readonly BigFlags Value52;
        public static readonly BigFlags Value53;
        public static readonly BigFlags Value54;
        public static readonly BigFlags Value55;
        public static readonly BigFlags Value56;
        public static readonly BigFlags Value57;
        public static readonly BigFlags Value58;
        public static readonly BigFlags Value59;
        public static readonly BigFlags Value60;
        public static readonly BigFlags Value61;
        public static readonly BigFlags Value62;
        public static readonly BigFlags Value63;
        public static readonly BigFlags Value64;
        public static readonly BigFlags Value65;
        public static readonly BigFlags Value66;
        public static readonly BigFlags Value67;
        public static readonly BigFlags Value68;
        public static readonly BigFlags Value69;
        public static readonly BigFlags Value70;
        public static readonly BigFlags Value71;
        public static readonly BigFlags Value72;
        public static readonly BigFlags Value73;
        public static readonly BigFlags Value74;
        public static readonly BigFlags Value75;
        public static readonly BigFlags Value76;
        public static readonly BigFlags Value77;
        public static readonly BigFlags Value78;
        public static readonly BigFlags Value79;
        public static readonly BigFlags Value80;
        public static readonly BigFlags Value81;
        public static readonly BigFlags Value82;
        public static readonly BigFlags Value83;
        public static readonly BigFlags Value84;
        public static readonly BigFlags Value85;
        public static readonly BigFlags Value86;
        public static readonly BigFlags Value87;
        public static readonly BigFlags Value88;
        public static readonly BigFlags Value89;
        public static readonly BigFlags Value90;
        public static readonly BigFlags Value91;
        public static readonly BigFlags Value92;
        public static readonly BigFlags Value93;
        public static readonly BigFlags Value94;
        public static readonly BigFlags Value95;
        public static readonly BigFlags Value96;
        public static readonly BigFlags Value97;
        public static readonly BigFlags Value98;
        public static readonly BigFlags Value99;
        public static readonly BigFlags Value100;
        public static readonly BigFlags Value101;
        public static readonly BigFlags Value102;
        public static readonly BigFlags Value103;
        public static readonly BigFlags Value104;
        public static readonly BigFlags Value105;
        public static readonly BigFlags Value106;
        public static readonly BigFlags Value107;
        public static readonly BigFlags Value108;
        public static readonly BigFlags Value109;
        public static readonly BigFlags Value110;
        public static readonly BigFlags Value111;
        public static readonly BigFlags Value112;
        public static readonly BigFlags Value113;
        public static readonly BigFlags Value114;
        public static readonly BigFlags Value115;
        public static readonly BigFlags Value116;
        public static readonly BigFlags Value117;
        public static readonly BigFlags Value118;
        public static readonly BigFlags Value119;
        public static readonly BigFlags Value120;
        public static BigFlags Value121 => Value1 | Value2 | Value118;  
        public static readonly BigFlags Value122;
        public static readonly BigFlags Value123;
        public static readonly BigFlags Value124;
        public static readonly BigFlags Value125;
        public static readonly BigFlags Value126;
        public static readonly BigFlags Value127;
        public static readonly BigFlags Value128;
        public static readonly BigFlags Value129;
        public static BigFlags Value130 => Value1 | Value2 | Value3 | Value4 | Value5 | Value6 | Value7 | Value8 | Value9 | Value10 | Value11 | Value12 | Value13 | Value14 | Value15 | Value16 | Value17 | Value18 | Value19 | Value20 | Value21 | Value22 | Value23 | Value24 | Value25 | Value26 | Value27 | Value28 | Value29 | Value30 | Value31 | Value32 | Value33 | Value34 | Value35 | Value36 | Value37 | Value38 | Value39 | Value40 | Value41 | Value42 | Value43 | Value44 | Value45 | Value46 | Value47 | Value48 | Value49 | Value50 | Value51 | Value52 | Value53 | Value54 | Value55 | Value56 | Value57 | Value58 | Value59 | Value60 | Value61 | Value62 | Value63 | Value64;

        #endregion Public Interface
    }

}
