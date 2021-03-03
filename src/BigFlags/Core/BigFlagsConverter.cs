using System;
using System.ComponentModel;
using System.Globalization;

namespace Core
{
    /// <summary>
    /// Converts objects to and from BigFlags instances.
    /// </summary>
    public class BigFlagsConverter : TypeConverter
    {
        /// <summary>
        /// Can convert to string only.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

        /// <summary>
        /// Can convert from any object.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => true;

        /// <summary>
        /// Converts BigFlags to a string.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is BigFlags && CanConvertTo(destinationType)) return value.ToString();
            return null;
        }

        /// <summary>
        /// Attempts to parse <paramref name="value"/> and create and
        /// return a new BigFlags instance.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = Convert.ToString(value);
            BigFlags.TryParse(s, out var result);
            return result;
        }
    }



    public class BigFlagsConverter2 : TypeConverter
    {
        /// <summary>
        /// Can convert to string only.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

        /// <summary>
        /// Can convert from any object.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => true;

        /// <summary>
        /// Converts BigFlags to a string.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is BigFlags && CanConvertTo(destinationType)) return value.ToString();
            return null;
        }

        /// <summary>
        /// Attempts to parse <paramref name="value"/> and create and
        /// return a new BigFlags instance.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = Convert.ToString(value);
            BigFlags.TryParse(s, out var result);
            return result;
        }
    }
}