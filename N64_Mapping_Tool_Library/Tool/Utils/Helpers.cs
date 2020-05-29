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
    public class Helper
    {
        internal static readonly string newLine = Environment.NewLine;
        internal const char slash = '/';
        internal const char comma = ',';
        internal const char space = ' ';
        internal const string str_space = " ";
        //public static readonly char[] space = { ' ' };
        internal static readonly string[] comma_space = { ", " };
        internal static readonly string[] empty_array = { };
        internal static readonly Regex bmpSplit = new Regex("([A-Za-z0-9]*_c.bmp)");
        internal static readonly Regex quoteSplit = new Regex("\"(.*)\"");
        internal static readonly Regex keyValueSplit = new Regex("\\s(.*)");



        /// <summary>
        /// Convert a string to int, return null if error
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>int or null</returns>
        public static int? StringToInt(string str)
        {
            try { return int.Parse(str); }
            catch { return null; }
        }

        /// <summary>
        /// Convert a string to double, return null if error
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double? StringToDouble(string str)
        {
            try { return double.Parse(str); }
            catch { return null; }
        }

        /// <summary>
        /// Return true if the given string is a double, false otherwise
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDoubleValue(string str)
        {
            return StringToDouble(str) != null;
        }

        /// <summary>
        /// Replace any , with . in the string for a correct string conversion
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StrDoubleComma(string str)
        {
            return str.Replace(',', '.');  // . required for a correct conversion
        }

        /// <summary>
        /// Delete any , at the end of the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimEndComma(string str)
        {
            return str.TrimEnd(comma);
        }

        /// <summary>
        /// Split the string at every space
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitBySpace(string str)
        {
            return str.Split(space);
        }

        /// <summary>
        /// Split the string at every slash
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitBySlash(string str)
        {
            return str.Split(slash);
        }

        /// <summary>
        /// Split the string at every , followed by a space
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitByCommaSpace(string str)
        {
            return str.Split(comma_space, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 1 -> 001 | 10 -> 010
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string GetIntWithDigits(int i)
        {
            if (i >= 0 && i < 10)
                return string.Format("00{0}", i);
            else if (i >= 10 && i < 100)
                return string.Format("0{0}", i);
            return i.ToString();
        }

        /// <summary>
        /// Merge a given index and string separated by a _ (i.e. "200_text")
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MergeIndexStr(int idx, string str)
        {
            return string.Format("{0}_{1}", GetIntWithDigits(idx), str);
        }


        /// <summary>
        /// Read the given file and return a list
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string[] ReadFile(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                return FileHelper.ReadAllLines(filename);
            }
            return null;
        }

        /// <summary>
        /// Read the given file, return a list and insert an empty line at the end
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string[] ReadFileAppend(string filename)
        {
            string[] lines = ReadFile(filename);
            if (lines != null)
            {
                Array.Resize(ref lines, lines.Length + 1);
                lines[lines.Length - 1] = "";
            }
            return lines;
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
            string[] materialSplit = bmpSplit.Split(material);
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
            string[] lineSplit = bmpSplit.Split(line);
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
            return keyValueSplit.Split(line); // \s = whitespaces
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
    }

    class AngleHelper
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


    /// <summary>
    /// Class used to store a game name and a list of blacklisted textures
    /// </summary>
    public class GameTexturesXml
    {
        public string Name { get; set; }
        public HashSet<string> Textures { get; set; }
        // HashSet<string> textureList = new HashSet<string>();

        public GameTexturesXml()
        {
            Name = "";
            Textures = new HashSet<string>();
        }
    }

    /// <summary>
    /// Class with functions to interact with the xml format used by the tool
    /// </summary>
    public class XmlHelper
    {

        /// <summary>
        /// Parse the provided xml format and return a list
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static List<GameTexturesXml> ParseTextureXml(string filename)
        {
            XmlDocument doc = new XmlDocument();

            List<GameTexturesXml> gamesList = new List<GameTexturesXml>();
            try { doc.Load(filename); }
            catch { return gamesList; }

            XmlNode games = doc.DocumentElement.SelectSingleNode("/games");
            if (games != null)
            {
                foreach (XmlNode game in games.SelectNodes("game"))
                {
                    GameTexturesXml gameTextures = new GameTexturesXml();

                    XmlNode name = game.SelectSingleNode("name");
                    if (name == null)
                        continue;
                    else
                        gameTextures.Name = name.InnerText;

                    XmlNode textures = game.SelectSingleNode("textures");
                    if (textures == null)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (XmlNode texture in textures.SelectNodes("texture"))
                        {
                            gameTextures.Textures.Add(texture.InnerText);
                        }
                    }
                    gamesList.Add(gameTextures);
                }
            }
            return gamesList;
        }

        /// <summary>
        /// Iterate through the given list and save as xml to the format provided
        /// </summary>
        /// <param name="gamesList"></param>
        /// <param name="filename"></param>
        public static void SaveTextureXml(string filename, List<GameTexturesXml> gamesList)
        {
            // TODO: Do a propper XML implementation with custom classes ?

            XmlDocument doc = new XmlDocument();
            
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlNode games = doc.CreateNode(XmlNodeType.Element, "games","");

            foreach (GameTexturesXml gameTextures in gamesList)
            {
                XmlNode game = doc.CreateNode(XmlNodeType.Element, "game", "");

                XmlNode name = doc.CreateNode(XmlNodeType.Element, "name", "");
                name.InnerText = gameTextures.Name;
                game.AppendChild(name);

                XmlNode textures = doc.CreateNode(XmlNodeType.Element, "textures", "");
                foreach (string text in gameTextures.Textures)
                {
                    XmlNode texture = doc.CreateNode(XmlNodeType.Element, "texture", "");
                    texture.InnerText = text;
                    textures.AppendChild(texture);
                }
                game.AppendChild(textures);
                games.AppendChild(game);
            }
            
            doc.AppendChild(games);
            doc.Save(filename);
        }
    }

    /// <summary>
    /// Count the time spent
    /// </summary>
    public class TestTime
    {
        public long StartTime { get; set; }
        public long EndTime { get; set; }

        public TestTime()
        {
            StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
        
        /// <summary>
        /// Update the StartTime variable with the current time
        /// </summary>
        public void UpdateStartTime()
        {
            StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Update the EndTime variable with the current time
        /// </summary>
        public void UpdateEndTime()
        {
            EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
        
        private long GetTimeDiff()
        {
            return EndTime - StartTime;
        }

        private string GetFormattedDate()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(GetTimeDiff());
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
        }

        /// <summary>
        /// Display the time difference
        /// </summary>
        /// <param name="message"></param>
        public void Display(string message = "Time passed: ")
        {
            Console.WriteLine(string.Format("{0}{1}", message, GetFormattedDate()));
        }
        
        public override string ToString()
        {
            return GetFormattedDate();
        }
    }
}
