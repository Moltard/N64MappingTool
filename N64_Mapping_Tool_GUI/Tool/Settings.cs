using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace N64Application.Tool
{
    #region Class - Settings

    /// <summary>
    /// Store all settings of the user
    /// </summary>
    [Serializable]
    [XmlRoot("AppSettings")]
    public class Settings
    {

        #region Attributes

        [XmlArray(ElementName = "BlackList", Order = 1)]
        [XmlArrayItem(ElementName = "Config")]
        public ObservableCollection<BlackListConfig> BlackList { get; set; }

        #endregion

        #region Constructor

        public Settings()
        {
        }

        #endregion

        #region Methods - Init Attributes

        /// <summary>
        /// Init the attributes that werent init by the xml (cause of missing parameters).
        /// </summary>
        public void InitAllAttributes()
        {
            if (BlackList == null)
            {
                InitBlackListConfigs();
            }
        }

        private void InitBlackListConfigs()
        {
            BlackList = new ObservableCollection<BlackListConfig>();
        }

        #endregion
        

        #region Method - Save Settings

        /// <summary>
        /// Save the current setting to settings.xml
        /// </summary>
        public bool SaveSettings()
        {
            return Utils.XmlUtils.SerializeSettings(this);
        }

        #endregion

    }

    #endregion

    #region Class - BlackListConfig

    /// <summary>
    /// Store a blacklist config name and its blacklisted textures
    /// </summary>
    [Serializable]
    public class BlackListConfig : INotifyPropertyChanged
    {

        #region Attributes

        private string name;

        [XmlArray(ElementName = "Textures", Order = 2)]
        [XmlArrayItem(ElementName = "Texture")]
        public ObservableCollection<string> Textures { get; set; }

        /// <summary>
        /// Name of the game
        /// </summary>
        [XmlElement(ElementName = "Name", Order = 1)]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        #endregion

        #region Constructor

        public BlackListConfig()
        {
        }

        /// <summary>
        /// Create a default BlackListConfig
        /// </summary>
        /// <returns></returns>
        public static BlackListConfig GetNewConfig()
        {
            return new BlackListConfig
            {
                Name = "New Blacklist Config",
                Textures = new ObservableCollection<string>()
            };
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

    }

    #endregion


}
