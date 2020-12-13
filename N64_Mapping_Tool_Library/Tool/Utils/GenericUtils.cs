using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml;

namespace N64Library.Tool.Utils
{
    /// <summary>
    /// Class with static functions used for different purposes
    /// </summary>
    internal static class GenericUtils
    {

        #region Constants


        private static readonly string Credits =
           "# -----------------" + Environment.NewLine +
           "# N64 Mapping Tool" + Environment.NewLine +
           "#    By Moltard" + Environment.NewLine +
           "# -----------------";
        private static readonly string[] CommaSpace = { ", " };
        private static readonly Regex BmpSplit = new Regex("([A-Za-z0-9]*_c.bmp)");
        private static readonly Regex QuoteSplit = new Regex("\"(.*)\"");
        private static readonly Regex KeyValueSplit = new Regex("\\s(.*)");

        #endregion

        #region Methods


        public static string GetCreditsFile() { return Credits; }

        /// <summary>
        /// Delete any , at the end of the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimEndComma(string str)
        {
            return str.TrimEnd(',');
        }

        /// <summary>
        /// Split the string at every space
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitBySpace(string str)
        {
            return str.Split(' ');
        }

        /// <summary>
        /// Split the string at every slash
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitBySlash(string str)
        {
            return str.Split('/');
        }

        /// <summary>
        /// Split the string at every , followed by a space
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitByCommaSpace(string str)
        {
            return str.Split(CommaSpace, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 1 -> 001 | 10 -> 010
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string GetIntWithDigits(int i)
        {
            if (i >= 0 && i < 10)
            {
                return "00" + i; // 001
            }
            else if (i >= 10 && i < 100)
            {
                return "0" + i; // 010
            }
                
            return i.ToString();
        }

        /// <summary>
        /// Merge a given index and string separated by a _ (i.e. "001_Str")
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MergeIndexStr(int idx, string str)
        {
            return GetIntWithDigits(idx) + "_" + str; // 001_Str
        }

        /// <summary>
        /// Remove every whitespace from every line
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string[] RemoveBlankSpaces(string[] lines)
        {
            if (lines != null)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = lines[i].Trim(); // Remove Blank spaces
                }
            }
            return lines;
        }

        /// <summary>
        /// Extract the XXXXXXXX_c.bmp texture name from the string
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string ExtractTextureNameFromMaterial(string material)
        {
            // e.g. '58_71F0CAE7_c.bmp'
            //string splitReg = "([A-Za-z0-9]*_c.bmp)"; // ([A-Z0-9]*_c.bmp)
            //string[] lineSplit = System.Text.RegularExpressions.Regex.Split(line, bmpSplit);
            string[] materialSplit = BmpSplit.Split(material);
            if (materialSplit.Length >= 2)
                return materialSplit[1];
            return null;
        }


        /// <summary>
        /// Extract the String between 2 quotes
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string SplitQuoteValue(string line)
        {
            // e.g. "3DCCECBB_c.bmp"
            //string splitReg = "\"(.*)\""; // "(.*)"

            //string[] lineSplit = System.Text.RegularExpressions.Regex.Split(line, quoteSplit);
            string[] lineSplit = BmpSplit.Split(line);
            if (lineSplit.Length >= 2)
                return lineSplit[1];
            return null;
        }

        /// <summary>
        /// Split the key and value of a line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string[] SplitKeyValue(string line)
        {
            // e.g. g 0_C542287_c.bmp
            // "\\s(.*)"
            //return System.Text.RegularExpressions.Regex.Split(line, keyValueSplit); // \s = whitespaces
            return KeyValueSplit.Split(line); // \s = whitespaces
        }


        /// <summary>
        /// Get the Key Value of the line as a Tuple
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetKeyValueFromSplit(string line)
        {
            return GetKeyValueFromSplit((SplitKeyValue(line)));
        }

        /// <summary>
        /// Get the Key Value of the line split as a Tuple
        /// </summary>
        /// <param name="lineSplit"></param>
        /// <returns></returns>
        public static Tuple<string,string> GetKeyValueFromSplit(string[] lineSplit)
        {
            if (lineSplit.Length >= 2)
                return new Tuple<string, string>(lineSplit[0], lineSplit[1]);
            return null;

        }

        /// <summary>
        /// Get the value from a "key" "value" string
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string GetValueFromSplit(string line)
        {
            return GetValueFromSplit(SplitKeyValue(line));
        }

        private static string GetValueFromSplit(string[] lineSplit)
        {
            if (lineSplit.Length >= 2)
                return lineSplit[1];
            return null;
        }

        #endregion

    }

    class AngleUtils
    {

        /// <summary>
        /// Convert a degree angle to radian
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double DegreeToRadian(double d)
        {
            return (d * (Math.PI)) / 180.0;
        }

        /// <summary>
        /// Return the rounded cosinus of the given angle
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Cos(double d)
        {
            return Math.Round(Math.Cos(d), 6);
        }

        /// <summary>
        /// Return the rounded sinus of the given angle
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Sin(double d)
        {
            return Math.Round(Math.Sin(d), 6);
        }

    }

}
