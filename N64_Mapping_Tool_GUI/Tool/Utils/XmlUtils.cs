using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace N64Application.Tool.Utils
{
    static class XmlUtils
    {
        #region Constants

        private const string XmlSettings = "settings.xml";

        #endregion

        #region Methods
        /// <summary>
        /// Get the settings of the application or recreate them if they don't exist
        /// </summary>
        /// <returns></returns>
        public static Settings GetSettings()
        {
            Settings settings = null;
            if (System.IO.File.Exists(XmlSettings))
            {
                // If settings.xml exist, we load it
                settings = DeserializeSettings();
            }
            else
            {
                // If it doesn't exist, we create an empty one
                settings = new Settings();
                settings.SaveSettings();
            }
            return settings;
        }


        /// <summary>
        /// Read and parse settings.xml
        /// </summary>
        /// <returns>Returns the parsed settings</returns>
        public static Settings DeserializeSettings()
        {
            Settings settings = null;
            try
            {
                using (var reader = new System.IO.StreamReader(XmlSettings))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    settings = (Settings)serializer.Deserialize(reader);
                }
            }
            catch { }
            return settings;
        }

        /// <summary>
        /// Read and parse the given string of settings.xml
        /// </summary>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static Settings DeserializeSettings(string xmlText)
        {
            Settings settings = null;
            try
            {
                using (var reader = new System.IO.StringReader(xmlText))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    settings = (Settings)serializer.Deserialize(reader);
                }
            }
            catch { }
            return settings;
        }


        /// <summary>
        /// Save the given settings in settings.xml
        /// </summary>
        /// <param name="settings"></param>
        public static bool SerializeSettings(Settings settings)
        {
            if (settings != null)
            {
                var xmlWritterSettings = new System.Xml.XmlWriterSettings() { Indent = true };
                try
                {
                    using (var writer = new System.IO.StreamWriter(XmlSettings))
                    using (var xmlWriter = System.Xml.XmlWriter.Create(writer, xmlWritterSettings))
                    {
                        var serializer = new XmlSerializer(typeof(Settings));
                        serializer.Serialize(xmlWriter, settings);
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        #endregion

    }
}
