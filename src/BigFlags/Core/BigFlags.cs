using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Core
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
        public static readonly BigFlags Value131;
        public static readonly BigFlags Value132;
        public static readonly BigFlags Value133;
        public static readonly BigFlags Value134;
        public static readonly BigFlags Value135;
        public static readonly BigFlags Value136;
        public static readonly BigFlags Value137;
        public static readonly BigFlags Value138;
        public static readonly BigFlags Value139;
        public static readonly BigFlags Value140;
        public static readonly BigFlags Value141;
        public static readonly BigFlags Value142;
        public static readonly BigFlags Value143;
        public static readonly BigFlags Value144;
        public static readonly BigFlags Value145;
        public static readonly BigFlags Value146;
        public static readonly BigFlags Value147;
        public static readonly BigFlags Value148;
        public static readonly BigFlags Value149;
        public static readonly BigFlags Value150;
        public static readonly BigFlags Value151;
        public static readonly BigFlags Value152;
        public static readonly BigFlags Value153;
        public static readonly BigFlags Value154;
        public static readonly BigFlags Value155;
        public static readonly BigFlags Value156;
        public static readonly BigFlags Value157;
        public static readonly BigFlags Value158;
        public static readonly BigFlags Value159;
        public static readonly BigFlags Value160;
        public static readonly BigFlags Value161;
        public static readonly BigFlags Value162;
        public static readonly BigFlags Value163;
        public static readonly BigFlags Value164;
        public static readonly BigFlags Value165;
        public static readonly BigFlags Value166;
        public static readonly BigFlags Value167;
        public static readonly BigFlags Value168;
        public static readonly BigFlags Value169;
        public static readonly BigFlags Value170;
        public static readonly BigFlags Value171;
        public static readonly BigFlags Value172;
        public static readonly BigFlags Value173;
        public static readonly BigFlags Value174;
        public static readonly BigFlags Value175;
        public static readonly BigFlags Value176;
        public static readonly BigFlags Value177;
        public static readonly BigFlags Value178;
        public static readonly BigFlags Value179;
        public static readonly BigFlags Value180;
        public static readonly BigFlags Value181;
        public static readonly BigFlags Value182;
        public static readonly BigFlags Value183;
        public static readonly BigFlags Value184;
        public static readonly BigFlags Value185;
        public static readonly BigFlags Value186;
        public static readonly BigFlags Value187;
        public static readonly BigFlags Value188;
        public static readonly BigFlags Value189;
        public static readonly BigFlags Value190;
        public static readonly BigFlags Value191;
        public static readonly BigFlags Value192;
        public static readonly BigFlags Value193;
        public static readonly BigFlags Value194;
        public static readonly BigFlags Value195;
        public static readonly BigFlags Value196;
        public static readonly BigFlags Value197;
        public static readonly BigFlags Value198;
        public static readonly BigFlags Value199;
        public static readonly BigFlags Value200;
        public static readonly BigFlags Value201;
        public static readonly BigFlags Value202;
        public static readonly BigFlags Value203;
        public static readonly BigFlags Value204;
        public static readonly BigFlags Value205;
        public static readonly BigFlags Value206;
        public static readonly BigFlags Value207;
        public static readonly BigFlags Value208;
        public static readonly BigFlags Value209;
        public static readonly BigFlags Value210;
        public static readonly BigFlags Value211;
        public static readonly BigFlags Value212;
        public static readonly BigFlags Value213;
        public static readonly BigFlags Value214;
        public static readonly BigFlags Value215;
        public static readonly BigFlags Value216;
        public static readonly BigFlags Value217;
        public static readonly BigFlags Value218;
        public static readonly BigFlags Value219;
        public static readonly BigFlags Value220;
        public static readonly BigFlags Value221;
        public static readonly BigFlags Value222;
        public static readonly BigFlags Value223;
        public static readonly BigFlags Value224;
        public static readonly BigFlags Value225;
        public static readonly BigFlags Value226;
        public static readonly BigFlags Value227;
        public static readonly BigFlags Value228;
        public static readonly BigFlags Value229;
        public static readonly BigFlags Value230;
        public static readonly BigFlags Value231;
        public static readonly BigFlags Value232;
        public static readonly BigFlags Value233;
        public static readonly BigFlags Value234;
        public static readonly BigFlags Value235;
        public static readonly BigFlags Value236;
        public static readonly BigFlags Value237;
        public static readonly BigFlags Value238;
        public static readonly BigFlags Value239;
        public static readonly BigFlags Value240;
        public static readonly BigFlags Value241;
        public static readonly BigFlags Value242;
        public static readonly BigFlags Value243;
        public static readonly BigFlags Value244;
        public static readonly BigFlags Value245;
        public static readonly BigFlags Value246;
        public static readonly BigFlags Value247;
        public static readonly BigFlags Value248;
        public static readonly BigFlags Value249;
        public static readonly BigFlags Value250;
        public static readonly BigFlags Value251;
        public static readonly BigFlags Value252;
        public static readonly BigFlags Value253;
        public static readonly BigFlags Value254;
        public static readonly BigFlags Value255;
        public static readonly BigFlags Value256;
        public static readonly BigFlags Value257;
        public static readonly BigFlags Value258;
        public static readonly BigFlags Value259;
        public static readonly BigFlags Value260;
        public static readonly BigFlags Value261;
        public static readonly BigFlags Value262;
        public static readonly BigFlags Value263;
        public static readonly BigFlags Value264;
        public static readonly BigFlags Value265;
        public static readonly BigFlags Value266;
        public static readonly BigFlags Value267;
        public static readonly BigFlags Value268;
        public static readonly BigFlags Value269;
        public static readonly BigFlags Value270;
        public static readonly BigFlags Value271;
        public static readonly BigFlags Value272;
        public static readonly BigFlags Value273;
        public static readonly BigFlags Value274;
        public static readonly BigFlags Value275;
        public static readonly BigFlags Value276;
        public static readonly BigFlags Value277;
        public static readonly BigFlags Value278;
        public static readonly BigFlags Value279;
        public static readonly BigFlags Value280;
        public static readonly BigFlags Value281;
        public static readonly BigFlags Value282;
        public static readonly BigFlags Value283;
        public static readonly BigFlags Value284;
        public static readonly BigFlags Value285;
        public static readonly BigFlags Value286;
        public static readonly BigFlags Value287;
        public static readonly BigFlags Value288;
        public static readonly BigFlags Value289;
        public static readonly BigFlags Value290;
        public static readonly BigFlags Value291;
        public static readonly BigFlags Value292;
        public static readonly BigFlags Value293;
        public static readonly BigFlags Value294;
        public static readonly BigFlags Value295;
        public static readonly BigFlags Value296;
        public static readonly BigFlags Value297;
        public static readonly BigFlags Value298;
        public static readonly BigFlags Value299;
        public static readonly BigFlags Value300;
        public static readonly BigFlags Value301;
        public static readonly BigFlags Value302;
        public static readonly BigFlags Value303;
        public static readonly BigFlags Value304;
        public static readonly BigFlags Value305;
        public static readonly BigFlags Value306;
        public static readonly BigFlags Value307;
        public static readonly BigFlags Value308;
        public static readonly BigFlags Value309;
        public static readonly BigFlags Value310;
        public static readonly BigFlags Value311;
        public static readonly BigFlags Value312;
        public static readonly BigFlags Value313;
        public static readonly BigFlags Value314;
        public static readonly BigFlags Value315;
        public static readonly BigFlags Value316;
        public static readonly BigFlags Value317;
        public static readonly BigFlags Value318;
        public static readonly BigFlags Value319;
        public static readonly BigFlags Value320;
        public static readonly BigFlags Value321;
        public static readonly BigFlags Value322;
        public static readonly BigFlags Value323;
        public static readonly BigFlags Value324;
        public static readonly BigFlags Value325;
        public static readonly BigFlags Value326;
        public static readonly BigFlags Value327;
        public static readonly BigFlags Value328;
        public static readonly BigFlags Value329;
        public static readonly BigFlags Value330;
        public static readonly BigFlags Value331;
        public static readonly BigFlags Value332;
        public static readonly BigFlags Value333;
        public static readonly BigFlags Value334;
        public static readonly BigFlags Value335;
        public static readonly BigFlags Value336;
        public static readonly BigFlags Value337;
        public static readonly BigFlags Value338;
        public static readonly BigFlags Value339;
        public static readonly BigFlags Value340;
        public static readonly BigFlags Value341;
        public static readonly BigFlags Value342;
        public static readonly BigFlags Value343;
        public static readonly BigFlags Value344;
        public static readonly BigFlags Value345;
        public static readonly BigFlags Value346;
        public static readonly BigFlags Value347;
        public static readonly BigFlags Value348;
        public static readonly BigFlags Value349;
        public static readonly BigFlags Value350;
        public static readonly BigFlags Value351;
        public static readonly BigFlags Value352;
        public static readonly BigFlags Value353;
        public static readonly BigFlags Value354;
        public static readonly BigFlags Value355;
        public static readonly BigFlags Value356;
        public static readonly BigFlags Value357;
        public static readonly BigFlags Value358;
        public static readonly BigFlags Value359;
        public static readonly BigFlags Value360;
        public static readonly BigFlags Value361;
        public static readonly BigFlags Value362;
        public static readonly BigFlags Value363;
        public static readonly BigFlags Value364;
        public static readonly BigFlags Value365;
        public static readonly BigFlags Value366;
        public static readonly BigFlags Value367;
        public static readonly BigFlags Value368;
        public static readonly BigFlags Value369;
        public static readonly BigFlags Value370;
        public static readonly BigFlags Value371;
        public static readonly BigFlags Value372;
        public static readonly BigFlags Value373;
        public static readonly BigFlags Value374;
        public static readonly BigFlags Value375;
        public static readonly BigFlags Value376;
        public static readonly BigFlags Value377;
        public static readonly BigFlags Value378;
        public static readonly BigFlags Value379;
        public static readonly BigFlags Value380;
        public static readonly BigFlags Value381;
        public static readonly BigFlags Value382;
        public static readonly BigFlags Value383;
        public static readonly BigFlags Value384;
        public static readonly BigFlags Value385;
        public static readonly BigFlags Value386;
        public static readonly BigFlags Value387;
        public static readonly BigFlags Value388;
        public static readonly BigFlags Value389;
        public static readonly BigFlags Value390;
        public static readonly BigFlags Value391;
        public static readonly BigFlags Value392;
        public static readonly BigFlags Value393;
        public static readonly BigFlags Value394;
        public static readonly BigFlags Value395;
        public static readonly BigFlags Value396;
        public static readonly BigFlags Value397;
        public static readonly BigFlags Value398;
        public static readonly BigFlags Value399;
        public static readonly BigFlags Value400;
        public static readonly BigFlags Value401;
        public static readonly BigFlags Value402;
        public static readonly BigFlags Value403;
        public static readonly BigFlags Value404;
        public static readonly BigFlags Value405;
        public static readonly BigFlags Value406;
        public static readonly BigFlags Value407;
        public static readonly BigFlags Value408;
        public static readonly BigFlags Value409;
        public static readonly BigFlags Value410;
        public static readonly BigFlags Value411;
        public static readonly BigFlags Value412;
        public static readonly BigFlags Value413;
        public static readonly BigFlags Value414;
        public static readonly BigFlags Value415;
        public static readonly BigFlags Value416;
        public static readonly BigFlags Value417;
        public static readonly BigFlags Value418;
        public static readonly BigFlags Value419;
        public static readonly BigFlags Value420;
        public static readonly BigFlags Value421;
        public static readonly BigFlags Value422;
        public static readonly BigFlags Value423;
        public static readonly BigFlags Value424;
        public static readonly BigFlags Value425;
        public static readonly BigFlags Value426;
        public static readonly BigFlags Value427;
        public static readonly BigFlags Value428;
        public static readonly BigFlags Value429;
        public static readonly BigFlags Value430;
        public static readonly BigFlags Value431;
        public static readonly BigFlags Value432;
        public static readonly BigFlags Value433;
        public static readonly BigFlags Value434;
        public static readonly BigFlags Value435;
        public static readonly BigFlags Value436;
        public static readonly BigFlags Value437;
        public static readonly BigFlags Value438;
        public static readonly BigFlags Value439;
        public static readonly BigFlags Value440;
        public static readonly BigFlags Value441;
        public static readonly BigFlags Value442;
        public static readonly BigFlags Value443;
        public static readonly BigFlags Value444;
        public static readonly BigFlags Value445;
        public static readonly BigFlags Value446;
        public static readonly BigFlags Value447;
        public static readonly BigFlags Value448;
        public static readonly BigFlags Value449;
        public static readonly BigFlags Value450;
        public static readonly BigFlags Value451;
        public static readonly BigFlags Value452;
        public static readonly BigFlags Value453;
        public static readonly BigFlags Value454;
        public static readonly BigFlags Value455;
        public static readonly BigFlags Value456;
        public static readonly BigFlags Value457;
        public static readonly BigFlags Value458;
        public static readonly BigFlags Value459;
        public static readonly BigFlags Value460;
        public static readonly BigFlags Value461;
        public static readonly BigFlags Value462;
        public static readonly BigFlags Value463;
        public static readonly BigFlags Value464;
        public static readonly BigFlags Value465;
        public static readonly BigFlags Value466;
        public static readonly BigFlags Value467;
        public static readonly BigFlags Value468;
        public static readonly BigFlags Value469;
        public static readonly BigFlags Value470;
        public static readonly BigFlags Value471;
        public static readonly BigFlags Value472;
        public static readonly BigFlags Value473;
        public static readonly BigFlags Value474;
        public static readonly BigFlags Value475;
        public static readonly BigFlags Value476;
        public static readonly BigFlags Value477;
        public static readonly BigFlags Value478;
        public static readonly BigFlags Value479;
        public static readonly BigFlags Value480;
        public static readonly BigFlags Value481;
        public static readonly BigFlags Value482;
        public static readonly BigFlags Value483;
        public static readonly BigFlags Value484;
        public static readonly BigFlags Value485;
        public static readonly BigFlags Value486;
        public static readonly BigFlags Value487;
        public static readonly BigFlags Value488;
        public static readonly BigFlags Value489;
        public static readonly BigFlags Value490;
        public static readonly BigFlags Value491;
        public static readonly BigFlags Value492;
        public static readonly BigFlags Value493;
        public static readonly BigFlags Value494;
        public static readonly BigFlags Value495;
        public static readonly BigFlags Value496;
        public static readonly BigFlags Value497;
        public static readonly BigFlags Value498;
        public static readonly BigFlags Value499;
        public static readonly BigFlags Value500;
        public static readonly BigFlags Value501;
        public static readonly BigFlags Value502;
        public static readonly BigFlags Value503;
        public static readonly BigFlags Value504;
        public static readonly BigFlags Value505;
        public static readonly BigFlags Value506;
        public static readonly BigFlags Value507;
        public static readonly BigFlags Value508;
        public static readonly BigFlags Value509;
        public static readonly BigFlags Value510;
        public static readonly BigFlags Value511;
        public static readonly BigFlags Value512;
        public static readonly BigFlags Value513;
        public static readonly BigFlags Value514;
        public static readonly BigFlags Value515;
        public static readonly BigFlags Value516;
        public static readonly BigFlags Value517;
        public static readonly BigFlags Value518;
        public static readonly BigFlags Value519;
        public static readonly BigFlags Value520;
        public static readonly BigFlags Value521;
        public static readonly BigFlags Value522;
        public static readonly BigFlags Value523;
        public static readonly BigFlags Value524;
        public static readonly BigFlags Value525;
        public static readonly BigFlags Value526;
        public static readonly BigFlags Value527;
        public static readonly BigFlags Value528;
        public static readonly BigFlags Value529;
        public static readonly BigFlags Value530;
        public static readonly BigFlags Value531;
        public static readonly BigFlags Value532;
        public static readonly BigFlags Value533;
        public static readonly BigFlags Value534;
        public static readonly BigFlags Value535;
        public static readonly BigFlags Value536;
        public static readonly BigFlags Value537;
        public static readonly BigFlags Value538;
        public static readonly BigFlags Value539;
        public static readonly BigFlags Value540;
        public static readonly BigFlags Value541;
        public static readonly BigFlags Value542;
        public static readonly BigFlags Value543;
        public static readonly BigFlags Value544;
        public static readonly BigFlags Value545;
        public static readonly BigFlags Value546;
        public static readonly BigFlags Value547;
        public static readonly BigFlags Value548;
        public static readonly BigFlags Value549;
        public static readonly BigFlags Value550;
        public static readonly BigFlags Value551;
        public static readonly BigFlags Value552;
        public static readonly BigFlags Value553;
        public static readonly BigFlags Value554;
        public static readonly BigFlags Value555;
        public static readonly BigFlags Value556;
        public static readonly BigFlags Value557;
        public static readonly BigFlags Value558;
        public static readonly BigFlags Value559;
        public static readonly BigFlags Value560;
        public static readonly BigFlags Value561;
        public static readonly BigFlags Value562;
        public static readonly BigFlags Value563;
        public static readonly BigFlags Value564;
        public static readonly BigFlags Value565;
        public static readonly BigFlags Value566;
        public static readonly BigFlags Value567;
        public static readonly BigFlags Value568;
        public static readonly BigFlags Value569;
        public static readonly BigFlags Value570;
        public static readonly BigFlags Value571;
        public static readonly BigFlags Value572;
        public static readonly BigFlags Value573;
        public static readonly BigFlags Value574;
        public static readonly BigFlags Value575;
        public static readonly BigFlags Value576;
        public static readonly BigFlags Value577;
        public static readonly BigFlags Value578;
        public static readonly BigFlags Value579;
        public static readonly BigFlags Value580;
        public static readonly BigFlags Value581;
        public static readonly BigFlags Value582;
        public static readonly BigFlags Value583;
        public static readonly BigFlags Value584;
        public static readonly BigFlags Value585;
        public static readonly BigFlags Value586;
        public static readonly BigFlags Value587;
        public static readonly BigFlags Value588;
        public static readonly BigFlags Value589;
        public static readonly BigFlags Value590;
        public static readonly BigFlags Value591;
        public static readonly BigFlags Value592;
        public static readonly BigFlags Value593;
        public static readonly BigFlags Value594;
        public static readonly BigFlags Value595;
        public static readonly BigFlags Value596;
        public static readonly BigFlags Value597;
        public static readonly BigFlags Value598;
        public static readonly BigFlags Value599;
        public static readonly BigFlags Value600;
        public static readonly BigFlags Value601;
        public static readonly BigFlags Value602;
        public static readonly BigFlags Value603;
        public static readonly BigFlags Value604;
        public static readonly BigFlags Value605;
        public static readonly BigFlags Value606;
        public static readonly BigFlags Value607;
        public static readonly BigFlags Value608;
        public static readonly BigFlags Value609;
        public static readonly BigFlags Value610;
        public static readonly BigFlags Value611;
        public static readonly BigFlags Value612;
        public static readonly BigFlags Value613;
        public static readonly BigFlags Value614;
        public static readonly BigFlags Value615;
        public static readonly BigFlags Value616;
        public static readonly BigFlags Value617;
        public static readonly BigFlags Value618;
        public static readonly BigFlags Value619;
        public static readonly BigFlags Value620;
        public static readonly BigFlags Value621;
        public static readonly BigFlags Value622;
        public static readonly BigFlags Value623;
        public static readonly BigFlags Value624;
        public static readonly BigFlags Value625;
        public static readonly BigFlags Value626;
        public static readonly BigFlags Value627;
        public static readonly BigFlags Value628;
        public static readonly BigFlags Value629;
        public static readonly BigFlags Value630;
        public static readonly BigFlags Value631;
        public static readonly BigFlags Value632;
        public static readonly BigFlags Value633;
        public static readonly BigFlags Value634;
        public static readonly BigFlags Value635;
        public static readonly BigFlags Value636;
        public static readonly BigFlags Value637;
        public static readonly BigFlags Value638;
        public static readonly BigFlags Value639;
        public static readonly BigFlags Value640;
        public static readonly BigFlags Value641;
        public static readonly BigFlags Value642;
        public static readonly BigFlags Value643;
        public static readonly BigFlags Value644;
        public static readonly BigFlags Value645;
        public static readonly BigFlags Value646;
        public static readonly BigFlags Value647;
        public static readonly BigFlags Value648;
        public static readonly BigFlags Value649;
        public static readonly BigFlags Value650;
        public static readonly BigFlags Value651;
        public static readonly BigFlags Value652;
        public static readonly BigFlags Value653;
        public static readonly BigFlags Value654;
        public static readonly BigFlags Value655;
        public static readonly BigFlags Value656;
        public static readonly BigFlags Value657;
        public static readonly BigFlags Value658;
        public static readonly BigFlags Value659;
        public static readonly BigFlags Value660;
        public static readonly BigFlags Value661;
        public static readonly BigFlags Value662;
        public static readonly BigFlags Value663;
        public static readonly BigFlags Value664;
        public static readonly BigFlags Value665;
        public static readonly BigFlags Value666;
        public static readonly BigFlags Value667;
        public static readonly BigFlags Value668;
        public static readonly BigFlags Value669;
        public static readonly BigFlags Value670;
        public static readonly BigFlags Value671;
        public static readonly BigFlags Value672;
        public static readonly BigFlags Value673;
        public static readonly BigFlags Value674;
        public static readonly BigFlags Value675;
        public static readonly BigFlags Value676;
        public static readonly BigFlags Value677;
        public static readonly BigFlags Value678;
        public static readonly BigFlags Value679;
        public static readonly BigFlags Value680;
        public static readonly BigFlags Value681;
        public static readonly BigFlags Value682;
        public static readonly BigFlags Value683;
        public static readonly BigFlags Value684;
        public static readonly BigFlags Value685;
        public static readonly BigFlags Value686;
        public static readonly BigFlags Value687;
        public static readonly BigFlags Value688;
        public static readonly BigFlags Value689;
        public static readonly BigFlags Value690;
        public static readonly BigFlags Value691;
        public static readonly BigFlags Value692;
        public static readonly BigFlags Value693;
        public static readonly BigFlags Value694;
        public static readonly BigFlags Value695;
        public static readonly BigFlags Value696;
        public static readonly BigFlags Value697;
        public static readonly BigFlags Value698;
        public static readonly BigFlags Value699;
        public static readonly BigFlags Value700;
        public static readonly BigFlags Value701;
        public static readonly BigFlags Value702;
        public static readonly BigFlags Value703;
        public static readonly BigFlags Value704;
        public static readonly BigFlags Value705;
        public static readonly BigFlags Value706;
        public static readonly BigFlags Value707;
        public static readonly BigFlags Value708;
        public static readonly BigFlags Value709;
        public static readonly BigFlags Value710;
        public static readonly BigFlags Value711;
        public static readonly BigFlags Value712;
        public static readonly BigFlags Value713;
        public static readonly BigFlags Value714;
        public static readonly BigFlags Value715;
        public static readonly BigFlags Value716;
        public static readonly BigFlags Value717;
        public static readonly BigFlags Value718;
        public static readonly BigFlags Value719;
        public static readonly BigFlags Value720;
        public static readonly BigFlags Value721;
        public static readonly BigFlags Value722;
        public static readonly BigFlags Value723;
        public static readonly BigFlags Value724;
        public static readonly BigFlags Value725;
        public static readonly BigFlags Value726;
        public static readonly BigFlags Value727;
        public static readonly BigFlags Value728;
        public static readonly BigFlags Value729;
        public static readonly BigFlags Value730;
        public static readonly BigFlags Value731;
        public static readonly BigFlags Value732;
        public static readonly BigFlags Value733;
        public static readonly BigFlags Value734;
        public static readonly BigFlags Value735;
        public static readonly BigFlags Value736;
        public static readonly BigFlags Value737;
        public static readonly BigFlags Value738;
        public static readonly BigFlags Value739;
        public static readonly BigFlags Value740;
        public static readonly BigFlags Value741;
        public static readonly BigFlags Value742;
        public static readonly BigFlags Value743;
        public static readonly BigFlags Value744;
        public static readonly BigFlags Value745;
        public static readonly BigFlags Value746;
        public static readonly BigFlags Value747;
        public static readonly BigFlags Value748;
        public static readonly BigFlags Value749;
        public static readonly BigFlags Value750;
        public static readonly BigFlags Value751;
        public static readonly BigFlags Value752;
        public static readonly BigFlags Value753;
        public static readonly BigFlags Value754;
        public static readonly BigFlags Value755;
        public static readonly BigFlags Value756;
        public static readonly BigFlags Value757;
        public static readonly BigFlags Value758;
        public static readonly BigFlags Value759;
        public static readonly BigFlags Value760;
        public static readonly BigFlags Value761;
        public static readonly BigFlags Value762;
        public static readonly BigFlags Value763;
        public static readonly BigFlags Value764;
        public static readonly BigFlags Value765;
        public static readonly BigFlags Value766;
        public static readonly BigFlags Value767;
        public static readonly BigFlags Value768;
        public static readonly BigFlags Value769;
        public static readonly BigFlags Value770;
        public static readonly BigFlags Value771;
        public static readonly BigFlags Value772;
        public static readonly BigFlags Value773;
        public static readonly BigFlags Value774;
        public static readonly BigFlags Value775;
        public static readonly BigFlags Value776;
        public static readonly BigFlags Value777;
        public static readonly BigFlags Value778;
        public static readonly BigFlags Value779;
        public static readonly BigFlags Value780;
        public static readonly BigFlags Value781;
        public static readonly BigFlags Value782;
        public static readonly BigFlags Value783;
        public static readonly BigFlags Value784;
        public static readonly BigFlags Value785;
        public static readonly BigFlags Value786;
        public static readonly BigFlags Value787;
        public static readonly BigFlags Value788;
        public static readonly BigFlags Value789;
        public static readonly BigFlags Value790;
        public static readonly BigFlags Value791;
        public static readonly BigFlags Value792;
        public static readonly BigFlags Value793;
        public static readonly BigFlags Value794;
        public static readonly BigFlags Value795;
        public static readonly BigFlags Value796;
        public static readonly BigFlags Value797;
        public static readonly BigFlags Value798;
        public static readonly BigFlags Value799;
        public static readonly BigFlags Value800;
        public static readonly BigFlags Value801;
        public static readonly BigFlags Value802;
        public static readonly BigFlags Value803;
        public static readonly BigFlags Value804;
        public static readonly BigFlags Value805;
        public static readonly BigFlags Value806;
        public static readonly BigFlags Value807;
        public static readonly BigFlags Value808;
        public static readonly BigFlags Value809;
        public static readonly BigFlags Value810;
        public static readonly BigFlags Value811;
        public static readonly BigFlags Value812;
        public static readonly BigFlags Value813;
        public static readonly BigFlags Value814;
        public static readonly BigFlags Value815;
        public static readonly BigFlags Value816;
        public static readonly BigFlags Value817;
        public static readonly BigFlags Value818;
        public static readonly BigFlags Value819;
        public static readonly BigFlags Value820;
        public static readonly BigFlags Value821;
        public static readonly BigFlags Value822;
        public static readonly BigFlags Value823;
        public static readonly BigFlags Value824;
        public static readonly BigFlags Value825;
        public static readonly BigFlags Value826;
        public static readonly BigFlags Value827;
        public static readonly BigFlags Value828;
        public static readonly BigFlags Value829;
        public static readonly BigFlags Value830;
        public static readonly BigFlags Value831;
        public static readonly BigFlags Value832;
        public static readonly BigFlags Value833;
        public static readonly BigFlags Value834;
        public static readonly BigFlags Value835;
        public static readonly BigFlags Value836;
        public static readonly BigFlags Value837;
        public static readonly BigFlags Value838;
        public static readonly BigFlags Value839;
        public static readonly BigFlags Value840;
        public static readonly BigFlags Value841;
        public static readonly BigFlags Value842;
        public static readonly BigFlags Value843;
        public static readonly BigFlags Value844;
        public static readonly BigFlags Value845;
        public static readonly BigFlags Value846;
        public static readonly BigFlags Value847;
        public static readonly BigFlags Value848;
        public static readonly BigFlags Value849;
        public static readonly BigFlags Value850;
        public static readonly BigFlags Value851;
        public static readonly BigFlags Value852;
        public static readonly BigFlags Value853;
        public static readonly BigFlags Value854;
        public static readonly BigFlags Value855;
        public static readonly BigFlags Value856;
        public static readonly BigFlags Value857;
        public static readonly BigFlags Value858;
        public static readonly BigFlags Value859;
        public static readonly BigFlags Value860;
        public static readonly BigFlags Value861;
        public static readonly BigFlags Value862;
        public static readonly BigFlags Value863;
        public static readonly BigFlags Value864;
        public static readonly BigFlags Value865;
        public static readonly BigFlags Value866;
        public static readonly BigFlags Value867;
        public static readonly BigFlags Value868;
        public static readonly BigFlags Value869;
        public static readonly BigFlags Value870;
        public static readonly BigFlags Value871;
        public static readonly BigFlags Value872;
        public static readonly BigFlags Value873;
        public static readonly BigFlags Value874;
        public static readonly BigFlags Value875;
        public static readonly BigFlags Value876;
        public static readonly BigFlags Value877;
        public static readonly BigFlags Value878;
        public static readonly BigFlags Value879;
        public static readonly BigFlags Value880;
        public static readonly BigFlags Value881;
        public static readonly BigFlags Value882;
        public static readonly BigFlags Value883;
        public static readonly BigFlags Value884;
        public static readonly BigFlags Value885;
        public static readonly BigFlags Value886;
        public static readonly BigFlags Value887;
        public static readonly BigFlags Value888;
        public static readonly BigFlags Value889;
        public static readonly BigFlags Value890;
        public static readonly BigFlags Value891;
        public static readonly BigFlags Value892;
        public static readonly BigFlags Value893;
        public static readonly BigFlags Value894;
        public static readonly BigFlags Value895;
        public static readonly BigFlags Value896;
        public static readonly BigFlags Value897;
        public static readonly BigFlags Value898;
        public static readonly BigFlags Value899;
        public static readonly BigFlags Value900;
        public static readonly BigFlags Value901;
        public static readonly BigFlags Value902;
        public static readonly BigFlags Value903;
        public static readonly BigFlags Value904;
        public static readonly BigFlags Value905;
        public static readonly BigFlags Value906;
        public static readonly BigFlags Value907;
        public static readonly BigFlags Value908;
        public static readonly BigFlags Value909;
        public static readonly BigFlags Value910;
        public static readonly BigFlags Value911;
        public static readonly BigFlags Value912;
        public static readonly BigFlags Value913;
        public static readonly BigFlags Value914;
        public static readonly BigFlags Value915;
        public static readonly BigFlags Value916;
        public static readonly BigFlags Value917;
        public static readonly BigFlags Value918;
        public static readonly BigFlags Value919;
        public static readonly BigFlags Value920;
        public static readonly BigFlags Value921;
        public static readonly BigFlags Value922;
        public static readonly BigFlags Value923;
        public static readonly BigFlags Value924;
        public static readonly BigFlags Value925;
        public static readonly BigFlags Value926;
        public static readonly BigFlags Value927;
        public static readonly BigFlags Value928;
        public static readonly BigFlags Value929;
        public static readonly BigFlags Value930;
        public static readonly BigFlags Value931;
        public static readonly BigFlags Value932;
        public static readonly BigFlags Value933;
        public static readonly BigFlags Value934;
        public static readonly BigFlags Value935;
        public static readonly BigFlags Value936;
        public static readonly BigFlags Value937;
        public static readonly BigFlags Value938;
        public static readonly BigFlags Value939;
        public static readonly BigFlags Value940;
        public static readonly BigFlags Value941;
        public static readonly BigFlags Value942;
        public static readonly BigFlags Value943;
        public static readonly BigFlags Value944;
        public static readonly BigFlags Value945;
        public static readonly BigFlags Value946;
        public static readonly BigFlags Value947;
        public static readonly BigFlags Value948;
        public static readonly BigFlags Value949;
        public static readonly BigFlags Value950;
        public static readonly BigFlags Value951;
        public static readonly BigFlags Value952;
        public static readonly BigFlags Value953;
        public static readonly BigFlags Value954;
        public static readonly BigFlags Value955;
        public static readonly BigFlags Value956;
        public static readonly BigFlags Value957;
        public static readonly BigFlags Value958;
        public static readonly BigFlags Value959;
        public static readonly BigFlags Value960;
        public static readonly BigFlags Value961;
        public static readonly BigFlags Value962;
        public static readonly BigFlags Value963;
        public static readonly BigFlags Value964;
        public static readonly BigFlags Value965;
        public static readonly BigFlags Value966;
        public static readonly BigFlags Value967;
        public static readonly BigFlags Value968;
        public static readonly BigFlags Value969;
        public static readonly BigFlags Value970;
        public static readonly BigFlags Value971;
        public static readonly BigFlags Value972;
        public static readonly BigFlags Value973;
        public static readonly BigFlags Value974;
        public static readonly BigFlags Value975;
        public static readonly BigFlags Value976;
        public static readonly BigFlags Value977;
        public static readonly BigFlags Value978;
        public static readonly BigFlags Value979;
        public static readonly BigFlags Value980;
        public static readonly BigFlags Value981;
        public static readonly BigFlags Value982;
        public static readonly BigFlags Value983;
        public static readonly BigFlags Value984;
        public static readonly BigFlags Value985;
        public static readonly BigFlags Value986;
        public static readonly BigFlags Value987;
        public static readonly BigFlags Value988;
        public static readonly BigFlags Value989;
        public static readonly BigFlags Value990;
        public static readonly BigFlags Value991;
        public static readonly BigFlags Value992;
        public static readonly BigFlags Value993;
        public static readonly BigFlags Value994;
        public static readonly BigFlags Value995;
        public static readonly BigFlags Value996;
        public static readonly BigFlags Value997;
        public static readonly BigFlags Value998;
        public static readonly BigFlags Value999;
        public static readonly BigFlags Value1000;
        public static readonly BigFlags Value1001;
        public static readonly BigFlags Value1002;
        public static readonly BigFlags Value1003;
        public static readonly BigFlags Value1004;
        public static readonly BigFlags Value1005;
        public static readonly BigFlags Value1006;
        public static readonly BigFlags Value1007;
        public static readonly BigFlags Value1008;
        public static readonly BigFlags Value1009;
        public static readonly BigFlags Value1010;
        public static readonly BigFlags Value1011;
        public static readonly BigFlags Value1012;
        public static readonly BigFlags Value1013;
        public static readonly BigFlags Value1014;
        public static readonly BigFlags Value1015;
        public static readonly BigFlags Value1016;
        public static readonly BigFlags Value1017;
        public static readonly BigFlags Value1018;
        public static readonly BigFlags Value1019;
        public static readonly BigFlags Value1020;
        public static readonly BigFlags Value1021;
        public static readonly BigFlags Value1022;
        public static readonly BigFlags Value1023;



        #endregion Public Interface
    }

}