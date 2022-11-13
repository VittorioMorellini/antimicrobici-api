using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Utils
{
    public class General
    {
        public static readonly DateTime BaseDateTime = new DateTime(1899, 12, 30);

        public static readonly DateTime NullDateTime = DateTime.MinValue;

        public static int StringToInt(string v)
        {
            if (string.IsNullOrEmpty(v.Trim()))
            {
                return 0;
            }

            return int.Parse(v);
        }

        public static double StringToDouble(string v)
        {
            if (string.IsNullOrEmpty(v.Trim()))
            {
                return 0.0;
            }

            return double.Parse(v);
        }

        public static bool IsNumeric(string v)
        {
            double result;
            return double.TryParse(v, out result);
        }

        [Obsolete("Utilizzare TimeSpanToDB")]
        public static DateTime TimeToDBDate(DateTime time)
        {
            return new DateTime(1899, 12, 30, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        [Obsolete("Utilizzare TimeSpanToDB")]
        public static DateTime TimeToDBDate(TimeSpan time)
        {
            return new DateTime(1899, 12, 30, time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
        }

        public static string DBToString(object v)
        {
            if (v == DBNull.Value || v == null)
            {
                return string.Empty;
            }

            if (Type.GetTypeCode(v.GetType()) == TypeCode.DateTime)
            {
                return $"{v:d}";
            }

            return v.ToString().Trim();
        }

        public static object StringToDB(string v)
        {
            if (string.IsNullOrEmpty(v))
            {
                return DBNull.Value;
            }

            return v.Trim();
        }

        public static double DBToDouble(object v)
        {
            if (v == DBNull.Value)
            {
                return 0.0;
            }

            return Convert.ToDouble(v);
        }

        public static decimal DBToDecimal(object v)
        {
            if (v == DBNull.Value)
            {
                return 0m;
            }

            return Convert.ToDecimal(v);
        }

        public static int DBToInt(object v)
        {
            if (v == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(v);
        }

        public static Int64 DBToBigint(object v)
        {
            if (v == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt64(v);
        }
        
        public static object IntToDB(long n)
        {
            if (n == 0)
            {
                return DBNull.Value;
            }

            return n;
        }

        public static TimeSpan DBToTimeSpan(object dateTime)
        {
            if (dateTime == DBNull.Value)
            {
                return new TimeSpan(0L);
            }

            return ((DateTime)dateTime).Subtract(BaseDateTime);
        }

        public static object TimeSpanToDB(TimeSpan timeSpan)
        {
            if (timeSpan.Ticks == 0)
            {
                return DBNull.Value;
            }

            return BaseDateTime.Add(timeSpan);
        }

        public static DateTime DBToDateTime(object dateTime)
        {
            if (dateTime == DBNull.Value)
            {
                return NullDateTime;
            }

            return (DateTime)dateTime;
        }

        public static object DateTimeToDB(DateTime dateTime)
        {
            if (dateTime == NullDateTime)
            {
                return DBNull.Value;
            }

            return dateTime;
        }

        public static int BoolToDB(bool v)
        {
            if (!v)
            {
                return 0;
            }

            return -1;
        }

        public static bool DBToBool(object v)
        {
            if (v == DBNull.Value || !Convert.ToBoolean(v))
            {
                return false;
            }

            return true;
        }

        public static string DBToNullableString(object v)
        {
            if (v == DBNull.Value || v == null)
            {
                return null;
            }

            return DBToString(v);
        }

        public static object NullableStringToDB(string v)
        {
            return StringToDB(v);
        }

        public static double? DBToNullableDouble(object v)
        {
            if (v == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDouble(v);
        }

        public static int? DBToNullableInt(object v)
        {
            if (v == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(v);
        }

        public static object NullableIntToDB(long? n)
        {
            if (n.HasValue)
            {
                return n.Value;
            }

            return DBNull.Value;
        }

        public static object NullableDoubleToDB(double? n)
        {
            if (n.HasValue)
            {
                return n.Value;
            }

            return DBNull.Value;
        }

        public static DateTime? DBToNullableDateTime(object dateTime)
        {
            if (dateTime == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDateTime(dateTime);
        }

        public static object NullableDateTimeToDB(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value;
            }

            return DBNull.Value;
        }

        public static object NullableBoolToDB(bool? v)
        {
            if (v.HasValue)
            {
                return v.Value ? (-1) : 0;
            }

            return DBNull.Value;
        }

        public static bool? DBToNullableBool(object v)
        {
            if (v == DBNull.Value)
            {
                return null;
            }

            return Convert.ToBoolean(v);
        }

        public static TimeSpan? DBToNullableTimeSpan(object v)
        {
            if (v == DBNull.Value)
            {
                return null;
            }

            return ((DateTime)v).Subtract(BaseDateTime);
        }

        public static object NullableTimeSpanToDB(TimeSpan? v)
        {
            if (v.HasValue)
            {
                return BaseDateTime.Add(v.Value);
            }

            return DBNull.Value;
        }

        public static decimal? DBToNullableDecimal(object v)
        {
            if (v == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDecimal(v);
        }

        public static object NullableDecimalToDB(decimal? n)
        {
            if (!n.HasValue)
            {
                return DBNull.Value;
            }

            return n;
        }

        public static string RaddoppiaVirgolette(string s)
        {
            for (int num = s.IndexOf("'"); num >= 0; num = s.IndexOf("'", num + 2))
            {
                s = s.Substring(0, num + 1) + "'" + s.Substring(num + 1);
            }

            return s;
        }

        public static string RaddoppiaDoppieVirgolette(string s)
        {
            for (int num = s.IndexOf("\""); num >= 0; num = s.IndexOf("\"", num + 2))
            {
                s = s.Substring(0, num + 1) + "\"" + s.Substring(num + 1);
            }

            return s;
        }

        [Obsolete("Please use the method Split() in the System.String class")]
        public static string[] ScanParm(string parametersString, string separator)
        {
            return parametersString.Split(separator.ToCharArray());
        }

        public static string SuperTrim(string s)
        {
            s = s.Trim();
            while (s.IndexOf("  ") != -1)
            {
                s = s.Replace("  ", " ");
            }

            return s;
        }

        public static string PrimaLetteraMaiuscola(string s)
        {
            if (s.Length > 0)
            {
                s = s.Substring(0, 1).ToUpper() + s.Substring(1);
            }

            return s;
        }

        public static string EliminaCarattereDaEstremi(string s, string carattere)
        {
            if (s.StartsWith(carattere))
            {
                s = s.Substring(1);
            }

            if (s.EndsWith(carattere))
            {
                s = s.Substring(0, s.Length - 1);
            }

            return s.Trim();
        }

        public static string StripNomeTabella(string nomeCompletoTabella)
        {
            int num = nomeCompletoTabella.Length - 1;
            if (num == 0)
            {
                return string.Empty;
            }

            while ((num > 0) & (nomeCompletoTabella.Substring(num, 1) != "."))
            {
                num--;
            }

            if (nomeCompletoTabella.Substring(num, 1) == ".")
            {
                return nomeCompletoTabella.Substring(num + 1);
            }

            return nomeCompletoTabella;
        }

        public static double RoundDouble(double number, int digits)
        {
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            int num4 = 0;
            double num5 = Math.Pow(10.0, digits);
            num2 = Math.Floor(number * num5);
            num3 = number * num5 - num2;
            num4 = (int)Math.Floor(num3 * 10.0);
            if (num4 >= 5)
            {
                num2 += 1.0;
            }

            return num2 / num5;
        }

        public static string StripNomeFileNoExt(string fullPath)
        {
            return Path.GetDirectoryName(fullPath) + Path.GetFileNameWithoutExtension(fullPath);
        }

        [Obsolete("Please use System.IO.Path.GetFileName instead")]
        public static string StripNomeFileNoPath(string fullPath)
        {
            int num = fullPath.Length;
            if (num == 0)
            {
                return string.Empty;
            }

            while ((num > 1) & (fullPath.Substring(num - 1, 1) != "\\"))
            {
                num--;
            }

            if (fullPath.Substring(num - 1, 1) == "\\")
            {
                return fullPath.Substring(num);
            }

            return fullPath;
        }

        [Obsolete("Please use System.IO.Path.GetFileNameWithoutExtension instead")]
        public static string StripNomeFileNoPathNoExt(string fullPath)
        {
            return StripNomeFileNoPath(StripNomeFileNoExt(fullPath));
        }

        [Obsolete("Please use System.IO.Path.GetDirectoryName instead")]
        public static string StripPath(string fullPath)
        {
            int num = fullPath.Length;
            if (num == 0)
            {
                return string.Empty;
            }

            while ((num > 1) & (fullPath.Substring(num - 1, 1) != "\\"))
            {
                num--;
            }

            if (fullPath.Substring(num - 1, 1) == "\\")
            {
                return fullPath.Substring(0, num);
            }

            return string.Empty;
        }
    }
}
