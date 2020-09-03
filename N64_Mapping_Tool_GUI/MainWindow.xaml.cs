using N64Application.Tool.Utils;
using N64Library.Tool.Utils;
using N64Mapping.Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace N64Application
{
    

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal const string TexturesBlacklistXml = "TexturesBlacklist.xml";
        internal const string SameCopyFolder = "//";
        internal ObservableCollection<GameTexturesXml> GamesList;
        internal ObservableCollection<string> GameTextures;
        internal ObservableCollection<string> ObjFileList;


        //--------------------------------------
        //------------- Init GUI ---------------
        //--------------------------------------

        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
            InitCopyTextures();
            InitBlackList();
            InitObjMerge();
        }

        //--------------------------------------
        //---------- Init Obj Merge ------------
        //--------------------------------------

        private void InitCopyTextures()
        {
            CopyTextureOutputBox.Text = SameCopyFolder;
        }

        //--------------------------------------
        //---------- Init Obj Merge ------------
        //--------------------------------------

        private void InitObjMerge()
        {
            ObjFileList = new ObservableCollection<string>();
            // Bind TextBox to the obj list
            MergeObjSpecBox.DataContext = ObjFileList;
        }

        //--------------------------------------
        //---------- Init BlackList ------------
        //--------------------------------------
        
        private void InitBlackList()
        {
            if (!File.Exists(TexturesBlacklistXml))
                SaveTexturesBlacklistXml(); // Create an empty xml

            GamesList = new ObservableCollection<GameTexturesXml>(XmlHelper.ParseTextureXml(TexturesBlacklistXml));
            GameTextures = new ObservableCollection<string>();

            // Set the event handler for when the list gets modified
            GamesList.CollectionChanged += GamesListChanged;

            // Update the states of Button and ComboBox if the list is empty
            GamesListUpdateGUI();

            // Bind ComboBox to the game list
            ObjModifyBlackListSelect.DataContext = GamesList;
            BlacklistTextureGameSelect.DataContext = GamesList;
            BlacklistDeleteGameSelect.DataContext = GamesList;
            BlacklistCopyGameSelect.DataContext = GamesList;

            // Bind TextBox to the texture list
            BlacklistTextureBox.DataContext = GameTextures;
        }


        private void SaveTexturesBlacklistXml()
        {
            if (GamesList != null)
            {
                XmlHelper.SaveTextureXml(TexturesBlacklistXml, new List<GameTexturesXml>(GamesList));
            }
            else
            {
                XmlHelper.SaveTextureXml(TexturesBlacklistXml, new List<GameTexturesXml>());
            }
        }


        private void GamesListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            GamesListUpdateGUI();
        }

        private void GamesListUpdateGUI()
        {
            if (GamesList.Count == 0)
            {
                ObjModifyBlackList.IsChecked = false;
                ObjModifyBlackList.IsEnabled = false;
                BlacklistDeleteGameSelect.IsEnabled = false;
                BlacklistDeleteGameButton.IsEnabled = false;
                BlacklistCopyGameSelect.IsEnabled = false;
                BlacklistTextureGameSelect.IsEnabled = false;

                BlacklistTextureAddButton.IsEnabled = false;
                BlacklistTextureDeleteButton.IsEnabled = false;
                BlacklistTextureSaveButton.IsEnabled = false;
            }
            else
            {
                ObjModifyBlackList.IsEnabled = true;
                BlacklistDeleteGameSelect.IsEnabled = true;
                BlacklistCopyGameSelect.IsEnabled = true;
                BlacklistTextureGameSelect.IsEnabled = true;
            }
        }


        //-------------------------------------
        //------- Events Drag and Drop --------
        //-------------------------------------

        private string[] DragDropGetPaths(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                return e.Data.GetData(DataFormats.FileDrop, true) as string[];
            }
            return null;
        }

        /// <summary>
        /// Extract the path of the first dragged file matching the mode
        /// </summary>
        /// <param name="e">Event data</param>
        /// <param name="mode">Mode</param>
        /// <returns>The path is a matching file found, null otherwise</returns>
        private string DragDropGetPath(DragEventArgs e, DragDropMode mode)
        {
            string filePath = null;
            string[] droppedFilePaths = DragDropGetPaths(e);
            if (droppedFilePaths != null)
            {
                switch (mode)
                {
                    case DragDropMode.Wrl:
                        foreach (var path in droppedFilePaths)
                        {
                            // Get the first WRL dragged if multiple files are dragged
                            if (FileHelper.IsFileExtension(path, GuiHelper.ExtensionWrl))
                            {
                                filePath = path;
                                break;
                            }
                        }
                        break;
                    case DragDropMode.Obj:
                        foreach (var path in droppedFilePaths)
                        {
                            // Get the first OBJ dragged if multiple files are dragged
                            if (FileHelper.IsFileExtension(path, GuiHelper.ExtensionObj))
                            {
                                filePath = path;
                                break;
                            }
                        }
                        break;
                    case DragDropMode.Images:
                        foreach (var path in droppedFilePaths)
                        {
                            // Get the first Image dragged if multiple files are dragged
                            if (FileHelper.IsFileExtension(path, GuiHelper.ExtensionImages))
                            {
                                filePath = path;
                                break;
                            }
                        }
                        break;
                    case DragDropMode.Folder:
                        foreach (var path in droppedFilePaths)
                        {
                            // Get the directory path of the dragged files
                            if (Directory.Exists(path))
                            {
                                filePath = path;
                                break;
                            }
                            else if (File.Exists(path))
                            {
                                filePath = FileHelper.GetDirectoryName(path);
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return filePath;
        }

        /// <summary>
        /// Allow dragging on TextBox
        /// </summary>
        /// <param name="e"></param>
        private void DragOverTextBoxFix(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = DragDropEffects.Move;
        }


        

        

        //--------------------------------------
        //-------- Events Wrl To Obj -----------
        //--------------------------------------

        private void WrlBrowseBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (WrlBrowseBox.Text != string.Empty)
                WrlSaveButton.IsEnabled = true;
            else
                WrlSaveButton.IsEnabled = false;
        }

        private void WrlBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = GuiHelper.OpenFileDialog(FileFilters.Wrl);
            if (fileName != null)
                WrlBrowseBox.Text = fileName;
        }

        private void WrlDragDrop_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Wrl);
            if (filePath != null)
            {
                WrlBrowseBox.Text = filePath;
            }
        }

        private Dictionary<string, string> WrlToObjDictionary(string wrlFilename, string objOutput)
        {
            var dict = new Dictionary<string, string>
            {
                { "WrlFile", wrlFilename },
                { "ObjOutput", objOutput }
            };
            return dict;
        }
        
        private ObjOptions GetWrlToObjOptions()
        {
            ObjOptions options = new ObjOptions();

            // Reverse Vertex Order
            if ((bool)WrlModifyVertex.IsChecked) options |= ObjOptions.ReverseVertex;

            return options;
        }

        private void WrlSaveButton_Click(object sender, RoutedEventArgs e)
        {
            string wrlFileName = WrlBrowseBox.Text;
            if (wrlFileName != string.Empty)
            {
                if (File.Exists(wrlFileName))
                {
                    if (FileHelper.IsFileExtension(wrlFileName,GuiHelper.ExtensionWrl))
                    {
                        string directoryOutput = FileHelper.GetDirectoryName(wrlFileName);
                        ActionN64 action = ActionN64.WrlConversion;
                        string objFileName = GuiHelper.SaveFileDialog(action, directoryOutput);
                        if (objFileName != null) // A filename was picked
                        {
                            bool success = Start.Tool(action, WrlToObjDictionary(wrlFileName, objFileName), GetWrlToObjOptions());
                            MessagesHandler.DisplayMessageSuccessFailed(success);
                        }
                    }
                    else
                    {
                        MessagesHandler.DisplayMessageWarning(MessagesList.WrongExtension, GuiHelper.ExtensionWrl);
                        WrlBrowseBox.Focus();
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.FileNotExists);
                    WrlBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                WrlBrowseBox.Focus();
            }
        }

        // Wrls to Obj
        
        private void WrlsBrowseBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (WrlsBrowseBox.Text != string.Empty)
                WrlsSaveButton.IsEnabled = true;
            else
                WrlsSaveButton.IsEnabled = false;
        }

        private void WrlsBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = GuiHelper.OpenFolderDialog();
            if (folderName != null)
            {
                WrlsBrowseBox.Text = folderName;
            }
        }

        private void WrlsDragDrop_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Folder);
            if (filePath != null)
            {
                WrlsBrowseBox.Text = filePath;
            }
        }

        private Dictionary<string, string> WrlsToObjDictionary(string wrlDirectory)
        {
            var dict = new Dictionary<string, string>
            {
                { "WrlDirectory", wrlDirectory }
            };
            return dict;
        }

        private ObjOptions GetWrlsToObjOptions()
        {
            ObjOptions options = new ObjOptions();

            // Reverse Vertex Order
            if ((bool)WrlsModifyVertex.IsChecked) options |= ObjOptions.ReverseVertex;

            return options;
        }

        private void WrlsSaveButton_Click(object sender, RoutedEventArgs e)
        {
            string wrlFolder = WrlsBrowseBox.Text;
            if (wrlFolder != string.Empty)
            {
                if (Directory.Exists(wrlFolder))
                {
                    string message = string.Format("This will convert every .wrl file in the selected directory and its subdirectories:\n{0}\nDo you confirm ?", wrlFolder);
                    // AskYesNoQuestion

                    bool resultQuestion = MessagesHandler.AskYesNoQuestion(message);
                    if (resultQuestion)
                    {
                        ActionN64 action = ActionN64.WrlsConversion;
                        bool success = Start.Tool(action, WrlsToObjDictionary(wrlFolder), GetWrlsToObjOptions());
                        MessagesHandler.DisplayMessageSuccessFailed(success);
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.DirectoryNotExists);
                    WrlBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                WrlBrowseBox.Focus();
            }
        }

        //--------------------------------------
        //--------- Events Obj Tools -----------
        //--------------------------------------

        private void ObjBrowseBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool notEmpty = (ObjBrowseBox.Text != string.Empty);

            ObjDeleteMaterialsButton.IsEnabled = notEmpty;
            ObjDeleteUnusedMaterialsButton.IsEnabled = notEmpty;
            ObjAddMaterialsButton.IsEnabled = notEmpty;
            ObjModifyButton.IsEnabled = notEmpty;
            ObjToSmdButton.IsEnabled = notEmpty;
            RefModelToSmdButton.IsEnabled = notEmpty;
            CopyTextureUpdateGUI();

        }

        private void ObjBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = GuiHelper.OpenFileDialog(FileFilters.Obj);
            if (fileName != null)
                ObjBrowseBox.Text = fileName;
        }

        private void ObjDragDrop_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Obj);
            if (filePath != null)
            {
                ObjBrowseBox.Text = filePath;
            }
        }

        private void ObjModifyUniformScale_Changed(object sender, RoutedEventArgs e)
        {
            if ((bool)ObjModifyUniformScale.IsChecked) // Checked
            {
                ObjModifyScaleValueXYZ.IsEnabled = true;
                ObjModifyNonUniformScale.IsEnabled = false;
            }
            else // Unchecked
            {
                ObjModifyScaleValueXYZ.IsEnabled = false;
                ObjModifyNonUniformScale.IsEnabled = true;
            }
        }

        private void ObjModifyNonUniformScale_Changed(object sender, RoutedEventArgs e)
        {
            if ((bool)ObjModifyNonUniformScale.IsChecked) // Checked
            {
                ObjModifyScaleValueX.IsEnabled = true;
                ObjModifyScaleValueY.IsEnabled = true;
                ObjModifyScaleValueZ.IsEnabled = true;
                ObjModifyUniformScale.IsEnabled = false;
            }
            else // Unchecked
            {
                ObjModifyScaleValueX.IsEnabled = false;
                ObjModifyScaleValueY.IsEnabled = false;
                ObjModifyScaleValueZ.IsEnabled = false;
                ObjModifyUniformScale.IsEnabled = true;
            }
        }
        
        private void ObjModifyRotate_Changed(object sender, RoutedEventArgs e)
        {
            if ((bool)ObjModifyRotate.IsChecked) // Checked
            {
                ObjModifyRotateValueX.IsEnabled = true;
                ObjModifyRotateValueY.IsEnabled = true;
                ObjModifyRotateValueZ.IsEnabled = true;
            }
            else // Unchecked
            {
                ObjModifyRotateValueX.IsEnabled = false;
                ObjModifyRotateValueY.IsEnabled = false;
                ObjModifyRotateValueZ.IsEnabled = false;
            }
        }

        private void ObjModifyBlackList_Changed(object sender, RoutedEventArgs e)
        {
            if ((bool)ObjModifyBlackList.IsChecked) // Checked
            {
                ObjModifyBlackListSelect.IsEnabled = true;
            }
            else
            {
                ObjModifyBlackListSelect.IsEnabled = false;
            }
        }

        private Dictionary<string, string> ObjToolsDictionary(string objFilename, string objOutput, bool modify)
        {
            var dict = new Dictionary<string, string>
            {
                { "ObjFile", objFilename },
                { "ObjOutput", objOutput }
            };
            if (modify)
            {
                dict.Add("ScaleValue", ObjModifyScaleValueXYZ.Text);
                dict.Add("ScaleValueX", ObjModifyScaleValueX.Text);
                dict.Add("ScaleValueY", ObjModifyScaleValueY.Text);
                dict.Add("ScaleValueZ", ObjModifyScaleValueZ.Text);
                dict.Add("RotateValueX", ObjModifyRotateValueX.Text);
                dict.Add("RotateValueY", ObjModifyRotateValueY.Text);
                dict.Add("RotateValueZ", ObjModifyRotateValueZ.Text);        
            }
            return dict;
        }
        
        private ObjOptions GetModifyObjOptions()
        {
            ObjOptions options = new ObjOptions();

            // Reverse Vertex Order
            if ((bool)ObjModifyVertex.IsChecked) options |= ObjOptions.ReverseVertex;

            // Merge Objects and Materials
            if ((bool)ObjModifyMerge.IsChecked) options |= ObjOptions.Merge;

            // Sort Objects and Materials
            if ((bool)ObjModifySort.IsChecked) options |= ObjOptions.Sort;

            // Uniform Scaling
            if ((bool)ObjModifyUniformScale.IsChecked) options |= ObjOptions.UniformScale;

            // Non Uniform Scaling
            if ((bool)ObjModifyNonUniformScale.IsChecked) options |= ObjOptions.NonUniformScale;

            // Rotate Model
            if ((bool)ObjModifyRotate.IsChecked) options |= ObjOptions.Rotate;

            // Textures Blacklist
            if ((bool)ObjModifyBlackList.IsChecked)
            {
                var selectedGame = ObjModifyBlackListSelect.SelectedValue;
                if (selectedGame != null)
                {
                    if (selectedGame.GetType().Equals(typeof(GameTexturesXml))) // Safe check
                    {
                        Start.texturesList.Clear();
                        Start.texturesList.AddRange(((GameTexturesXml)selectedGame).Textures);
                        options |= ObjOptions.BlackList;
                    }
                }
            }

            return options;
        }

        private void ObjToolsButtons_Click(ActionN64 action)
        {
            string objFileName = ObjBrowseBox.Text;
            if (objFileName != string.Empty)
            {
                if (File.Exists(objFileName))
                {
                    if (FileHelper.IsFileExtension(objFileName, GuiHelper.ExtensionObj))
                    {
                        string directoryOutput = FileHelper.GetDirectoryName(objFileName);

                        string objOutputName = GuiHelper.SaveFileDialog(action, directoryOutput);
                        if (objOutputName != null)
                        {
                            if (action == ActionN64.ModifyObj) // Modify Obj
                            {
                                bool success = Start.Tool(action, ObjToolsDictionary(objFileName, objOutputName, true), GetModifyObjOptions());
                                MessagesHandler.DisplayMessageSuccessFailed(success);
                            }
                            else if(action == ActionN64.AddMaterials || action == ActionN64.DeleteMaterials
                                    || action == ActionN64.DeleteUnusedMaterials) // Delete & Add Materials
                            {
                                bool success = Start.Tool(action, ObjToolsDictionary(objFileName, objOutputName, false), 0);
                                MessagesHandler.DisplayMessageSuccessFailed(success);
                            }
                        }
                    }
                    else
                    {
                        MessagesHandler.DisplayMessageWarning(MessagesList.WrongExtension, GuiHelper.ExtensionObj);
                        ObjBrowseBox.Focus();
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.FileNotExists);
                    ObjBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                ObjBrowseBox.Focus();
            }
        }


        private void ObjDeleteMaterialsButton_Click(object sender, RoutedEventArgs e)
        {
            ObjToolsButtons_Click(ActionN64.DeleteMaterials);
        }
        
        private void ObjDeleteUnusedMaterialsButton_Click(object sender, RoutedEventArgs e)
        {
            ObjToolsButtons_Click(ActionN64.DeleteUnusedMaterials);
        }

        private void ObjAddMaterialsButton_Click(object sender, RoutedEventArgs e)
        {
            ObjToolsButtons_Click(ActionN64.AddMaterials);
        }

        private void ObjModifyButton_Click(object sender, RoutedEventArgs e)
        {
            ObjToolsButtons_Click(ActionN64.ModifyObj);
        }

        //--------------------------------------
        //---------- Events Obj To Smd ---------
        //--------------------------------------

        private Dictionary<string, string> ObjToSmdDictionary(string objFilename, string smdOutput)
        {
            var dict = new Dictionary<string, string>
            {
                { "ObjFile", objFilename },
                { "SmdOutput", smdOutput }
            };
            return dict;
        }

        private ObjOptions GetObjSmdOptions()
        {
            ObjOptions options = new ObjOptions();

            // Use the name of the textures for the smd materials
            if ((bool)ObjToSmdTextureCheckbox.IsChecked)
                options |= ObjOptions.SmdUseTextureName;

            return options;
        }

        private void ObjToSmdButton_Click(object sender, RoutedEventArgs e)
        {
            var action = ActionN64.ObjToSmd;
            string objFileName = ObjBrowseBox.Text;
            if (objFileName != string.Empty)
            {
                if (File.Exists(objFileName))
                {
                    if (FileHelper.IsFileExtension(objFileName, GuiHelper.ExtensionObj))
                    {
                        string directoryOutput = FileHelper.GetDirectoryName(objFileName);

                        string smdOutputName = GuiHelper.SaveFileDialog(action, directoryOutput);
                        if (smdOutputName != null)
                        {
                            bool success = Start.Tool(action, ObjToSmdDictionary(objFileName, smdOutputName), GetObjSmdOptions());
                            MessagesHandler.DisplayMessageSuccessFailed(success);
                        }
                    }
                    else
                    {
                        MessagesHandler.DisplayMessageWarning(MessagesList.WrongExtension, GuiHelper.ExtensionObj);
                        ObjBrowseBox.Focus();
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.FileNotExists);
                    ObjBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                ObjBrowseBox.Focus();
            }
        }

        //--------------------------------------
        //------- Events Reference Model -------
        //--------------------------------------

        private Dictionary<string, string> RefModelDictionary(string objFilename, string smdOutput)
        {
            var dict = new Dictionary<string, string>
            {
                { "ObjFile", objFilename },
                { "SmdOutput", smdOutput },
                { "ScaleValue", RefModelScaleValueXYZ.Text }
            };
            return dict;
        }

        private void RefModelToSmdButton_Click(object sender, RoutedEventArgs e)
        {
            var action = ActionN64.RefModelSmd;
            string objFileName = ObjBrowseBox.Text;
            if (objFileName != string.Empty)
            {
                if (File.Exists(objFileName))
                {
                    if (FileHelper.IsFileExtension(objFileName, GuiHelper.ExtensionObj))
                    {
                        string directoryOutput = FileHelper.GetDirectoryName(objFileName);

                        string smdOutputName = GuiHelper.SaveFileDialog(action, directoryOutput);
                        if (smdOutputName != null)
                        {
                            bool success = Start.Tool(action, RefModelDictionary(objFileName, smdOutputName), 0);
                            MessagesHandler.DisplayMessageSuccessFailed(success);
                        }
                    }
                    else
                    {
                        MessagesHandler.DisplayMessageWarning(MessagesList.WrongExtension, GuiHelper.ExtensionObj);
                        ObjBrowseBox.Focus();
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.FileNotExists);
                    ObjBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                ObjBrowseBox.Focus();
            }
        }

        //--------------------------------------
        //-------- Events Copy Textures --------
        //--------------------------------------

        private void CopyTextureUpdateGUI()
        {
            bool isChecked = (bool)CopyTextureFolderCheckbox.IsChecked;
            CopyTextureFolderBox.IsEnabled = isChecked;
            CopyTextureFolderButton.IsEnabled = isChecked;

            if (ObjBrowseBox.Text != string.Empty) // Obj File
            {
                if (CopyTextureOutputBox.Text != string.Empty) // Output Folder
                {
                    if (isChecked) // Need Texture Folder
                    {
                        if (CopyTextureFolderBox.Text != string.Empty) 
                        {
                            CopyTexturesButton.IsEnabled = true;
                            return;
                        }
                    }
                    else // Don't Need Texture Folder
                    {
                        CopyTexturesButton.IsEnabled = true;
                        return;
                    }
                }
            }
            CopyTexturesButton.IsEnabled = false;
        }
        
        private Dictionary<string, string> CopyTexturesDictionary(string objFilename, string outputFolder)
        {
            return CopyTexturesDictionary(objFilename, outputFolder, string.Empty);
        }

        private Dictionary<string, string> CopyTexturesDictionary(string objFilename, string outputFolder, string textureFolder)
        {
            var dict = new Dictionary<string, string>
            {
                { "ObjFile", objFilename },
                { "DestDir", outputFolder },
                { "TextureDir", textureFolder },
            };

            return dict;
        }


        // Output Folder

        private void CopyTextureOutputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CopyTextureUpdateGUI();
        }
        
        private void CopyTextureOutputButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = GuiHelper.OpenFolderDialog();
            if (folderName != null)
                CopyTextureOutputBox.Text = folderName;
        }
        
        private void CopyTextureOutputDragDrop_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Folder);
            if (filePath != null)
            {
                CopyTextureOutputBox.Text = filePath;
            }
        }

        // Texture Folder

        private void CopyTextureFolderCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            CopyTextureUpdateGUI();
        }

        private void CopyTextureFolderBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CopyTextureUpdateGUI();
        }

        private void CopyTextureFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = GuiHelper.OpenFolderDialog();
            if (folderName != null)
                CopyTextureFolderBox.Text = folderName;
        }

        private void CopyTextureFolderDragDrop_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Folder);
            if (filePath != null)
            {
                CopyTextureFolderBox.Text = filePath;
            }
        }

        // Copy Button
        private void CopyTexturesButton_Click(object sender, RoutedEventArgs e)
        {
            string objFileName = ObjBrowseBox.Text;
            if (objFileName != string.Empty)
            {
                if (File.Exists(objFileName))
                {
                    if (FileHelper.IsFileExtension(objFileName, GuiHelper.ExtensionObj))
                    {
                        string outputFolder = CopyTextureOutputBox.Text;
                        if (outputFolder.Equals(SameCopyFolder))
                        {
                            outputFolder = FileHelper.GetDirectoryName(objFileName);
                        }
                        if (!string.IsNullOrEmpty(outputFolder))
                        {
                            if (Directory.Exists(outputFolder))
                            {
                                bool isChecked = (bool)CopyTextureFolderCheckbox.IsChecked;

                                if (!isChecked) // Dont use Texture Folder
                                {
                                    bool success = Start.Tool(ActionN64.CopyObjTextures, CopyTexturesDictionary(objFileName, outputFolder), 0);
                                    MessagesHandler.DisplayMessageSuccessFailed(success);
                                }
                                else // Use Texture Folder
                                {
                                    string textureFolder = CopyTextureFolderBox.Text;
                                    if (textureFolder != string.Empty)
                                    {
                                        if (Directory.Exists(textureFolder))
                                        {
                                            bool success = Start.Tool(ActionN64.CopyObjTextures,
                                                CopyTexturesDictionary(objFileName, outputFolder, textureFolder), ObjOptions.CopyUseTextureDir);
                                            MessagesHandler.DisplayMessageSuccessFailed(success);
                                        }
                                        else
                                        {
                                            MessagesHandler.DisplayMessageWarning(MessagesList.DirectoryNotExists);
                                            CopyTextureFolderBox.Focus();
                                        }
                                    }
                                    else
                                    {
                                        MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                                        CopyTextureFolderBox.Focus();
                                    }
                                }
                            }
                            else
                            {
                                MessagesHandler.DisplayMessageWarning(MessagesList.DirectoryNotExists);
                                CopyTextureOutputBox.Focus();
                            }
                                
                        }
                        else
                        {
                            MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                            CopyTextureOutputBox.Focus();
                        }
                    }
                    else
                    {
                        MessagesHandler.DisplayMessageWarning(MessagesList.WrongExtension, GuiHelper.ExtensionObj);
                        ObjBrowseBox.Focus();
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.FileNotExists);
                    ObjBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                ObjBrowseBox.Focus();
            }
        }


        //--------------------------------------
        //---------- Events Obj Merge ----------
        //--------------------------------------

        // Folder of obj

        private void MergeObjFolderBrowseBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MergeObjFolderBrowseBox.Text != string.Empty)
                MergeObjFolderSaveButton.IsEnabled = true;
            else
                MergeObjFolderSaveButton.IsEnabled = false;
        }

        private void MergeObjFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = GuiHelper.OpenFolderDialog();
            if (folderName != null)
            {
                MergeObjFolderBrowseBox.Text = folderName;
            }
        }

        private void MergeObjFolderBrowseBox_PreviewDrop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Folder);
            if (filePath != null)
            {
                MergeObjFolderBrowseBox.Text = filePath;
            }
        }

        private Dictionary<string, string> MergeObjFolderDictionary(string objDirectory, string objOutput)
        {
            var dict = new Dictionary<string, string>
            {
                { "ObjDir", objDirectory },
                { "ObjOutput", objOutput },
            };
            return dict;
        }

        private void MergeObjFolderSaveButton_Click(object sender, RoutedEventArgs e)
        {
            string objFolder = MergeObjFolderBrowseBox.Text;
            if (objFolder != string.Empty)
            {
                if (Directory.Exists(objFolder))
                {
                    ActionN64 action = ActionN64.MergeObjFiles;
                    string objFileName = GuiHelper.SaveFileDialog(action, objFolder);
                    if (objFileName != null)
                    {
                        bool success = Start.Tool(action, MergeObjFolderDictionary(objFolder, objFileName), 0);
                        MessagesHandler.DisplayMessageSuccessFailed(success);
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.DirectoryNotExists);
                    WrlBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                WrlBrowseBox.Focus();
            }
        }


        // Specific Obj files

        /// <summary>
        /// Add all files in the list to ObjFileList
        /// </summary>
        /// <param name="files"></param>
        private void ObjFilesAppend(ICollection<string> files)
        {
            foreach (string str in files)
            {
                if (FileHelper.IsFileExtension(str, GuiHelper.ExtensionObj)) 
                    ObjFileList.Add(str);
            }
        }

        /// <summary>
        /// Update ObjFileList with the list of given files
        /// </summary>
        /// <param name="files"></param>
        private void ObjFilesUpdate(string[] files)
        {
            if (files != null)
            {
                List<string> tmpNoDuplicate = new List<string>(ObjFileList);
                tmpNoDuplicate.AddRange(files);
                ObjFileList.Clear();
                ObjFilesAppend(new HashSet<string>(tmpNoDuplicate));
                MergeObjSpecUpdateGUI();
            }
        }

        private Dictionary<string, string> MergeObjSpecDictionary(string objOutput)
        {
            var dict = new Dictionary<string, string>
            {
                { "ObjOutput", objOutput },
            };
            return dict;
        }


        private void MergeObjSpecUpdateGUI()
        {
            if(ObjFileList.Count > 1)
            {
                MergeObjSpecSaveButton.IsEnabled = true;
            }
            else
            {
                MergeObjSpecSaveButton.IsEnabled = false;
            }
        }
        
        private void MergeObjSpecBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MergeObjSpecBox.SelectedItems.Count > 0) // Atleast 1 obj selected
            {
                MergeObjSpecDeleteButton.IsEnabled = true;
            }
            else
            {
                MergeObjSpecDeleteButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Add the obj files from the selected folder to ObjFileList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MergeObjSpecAddButton_Click(object sender, RoutedEventArgs e)
        {
            ObjFilesUpdate(GuiHelper.OpenFilesDialog(FileFilters.Obj));
        }

        /// <summary>
        /// Add the drag and dropped obj files to ObjFileList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MergeObjSpecBox_Drop(object sender, DragEventArgs e)
        {
            ObjFilesUpdate(DragDropGetPaths(e));
        }
        
        private void MergeObjSpecDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MergeObjSpecBox.SelectedItems.Count > 0) // Atleast 1 obj selected
            {
                List<string> tempObjList = new List<string>();
                foreach (var objFile in MergeObjSpecBox.SelectedItems)
                {
                    if (objFile.GetType().Equals(typeof(string)))
                        tempObjList.Add((string)objFile);
                }
                foreach (string objFile in tempObjList)
                {
                    ObjFileList.Remove(objFile);
                }
                MergeObjSpecUpdateGUI();
            }
            else
            {
                MessagesHandler.DisplayMessageWarning("No obj selected");
            }
        }

        private void MergeObjSpecSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ObjFileList.Count > 1) // If there is less than two, no point doing a merge
            {
                string directoryOutput = FileHelper.GetDirectoryName(ObjFileList[0]);

                ActionN64 action = ActionN64.MergeSpecObjFiles;
                string objFileName = GuiHelper.SaveFileDialog(action, directoryOutput);
                if (objFileName != null)
                {
                    Start.objFileList.Clear();
                    Start.objFileList.AddRange(ObjFileList);
                    bool success = Start.Tool(action, MergeObjSpecDictionary(objFileName), 0);
                    MessagesHandler.DisplayMessageSuccessFailed(success);
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning("Need atleast 2 obj to merge");
            }
        }

        //------------------------------------
        //------- Events Flip Texture --------
        //------------------------------------

        private void FlipUpdateGUI()
        {
            if (((bool)FlipHorizontallyCheckbox.IsChecked || (bool)FlipVerticallyCheckbox.IsChecked)
                && FlipBrowseBox.Text != string.Empty)
            {
                FlipSaveButton.IsEnabled = true;
            }
            else
            {
                FlipSaveButton.IsEnabled = false;
            }
        }


        private Dictionary<string, string> FlipDictionary(string imgFileName, string imgOutputName)
        {
            var dict = new Dictionary<string, string>
            {
                { "PictureFile", imgFileName },
                { "PictureOutput", imgOutputName }
            };
            return dict;
        }

        private ObjOptions GetFlipOptions()
        {
            ObjOptions options = new ObjOptions();

            // Flip Horizontally
            if ((bool)FlipHorizontallyCheckbox.IsChecked) options |= ObjOptions.FlipHorizontally;

            // Flip Vertically
            if ((bool)FlipVerticallyCheckbox.IsChecked) options |= ObjOptions.FlipVertically;

            return options;
        }
        
        private void FlipBrowseBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FlipUpdateGUI();
        }

        private void FlipBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = GuiHelper.OpenFileDialog(FileFilters.Image);
            if (fileName != null)
                FlipBrowseBox.Text = fileName;
        }

        private void FlipDragDrop_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Images);
            if (filePath != null)
            {
                FlipBrowseBox.Text = filePath;
            }
        }

        private void FlipHorizontallyCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            FlipUpdateGUI();
        }

        private void FlipVerticallyCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            FlipUpdateGUI();
        }

        private void FlipSaveButton_Click(object sender, RoutedEventArgs e)
        {
            string imgFileName = FlipBrowseBox.Text;
            if (imgFileName != string.Empty)
            {
                if (File.Exists(imgFileName))
                {
                    if (FileHelper.IsFileExtension(imgFileName, GuiHelper.ExtensionImages))
                    {
                        string directoryOutput = FileHelper.GetDirectoryName(imgFileName);

                        string imgOutputName = GuiHelper.SaveFileDialog(FileHelper.GetFileNameWithoutExtension(imgFileName), GuiHelper.ExtensionPng, FileFilters.Png, directoryOutput);
                        if (imgOutputName != null)
                        {
                            bool success = Start.Tool(ActionN64.FlipPicture, FlipDictionary(imgFileName, imgOutputName), GetFlipOptions());
                            MessagesHandler.DisplayMessageSuccessFailed(success);
                        }
                    }
                    else
                    {
                        MessagesHandler.DisplayMessageWarning(MessagesList.WrongExtensions, GuiHelper.ExtensionImages);
                        FlipBrowseBox.Focus();
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.FileNotExists);
                    FlipBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                FlipBrowseBox.Focus();
            }
        }


        //--------------------------------------
        //------- Events Mirror Texture --------
        //--------------------------------------

        private void MirrorUpdateGUI()
        {
            if (((bool)MirrorHorizontallyCheckbox.IsChecked || (bool)MirrorVerticallyCheckbox.IsChecked)
                && MirrorBrowseBox.Text != string.Empty)
            {
                MirrorSaveButton.IsEnabled = true;
            }
            else
            {
                MirrorSaveButton.IsEnabled = false;
            }
        }

        private Dictionary<string, string> MirrorDictionary(string imgFileName, string imgOutputName)
        {
            var dict = new Dictionary<string, string>
            {
                { "PictureFile", imgFileName },
                { "PictureOutput", imgOutputName }
            };
            return dict;
        }

        private ObjOptions GetMirrorOptions()
        {
            ObjOptions options = new ObjOptions();

            // Mirror Horizontally
            if ((bool)MirrorHorizontallyCheckbox.IsChecked) options |= ObjOptions.MirrorHorizontally;

            // Mirror Vertically
            if ((bool)MirrorVerticallyCheckbox.IsChecked) options |= ObjOptions.MirrorVertically;

            return options;
        }

        private void MirrorBrowseBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MirrorUpdateGUI();
        }

        private void MirrorBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = GuiHelper.OpenFileDialog(FileFilters.Image);
            if (fileName != null)
                MirrorBrowseBox.Text = fileName;
        }
        
        private void MirrorDragDrop_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Images);
            if (filePath != null)
            {
                MirrorBrowseBox.Text = filePath;
            }
        }

        private void MirrorHorizontallyCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            MirrorUpdateGUI();
        }

        private void MirrorVerticallyCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            MirrorUpdateGUI();
        }
        
        private void MirrorSaveButton_Click(object sender, RoutedEventArgs e)
        {
            string imgFileName = MirrorBrowseBox.Text;
            if (imgFileName != string.Empty)
            {
                if (File.Exists(imgFileName))
                {
                    if (FileHelper.IsFileExtension(imgFileName, GuiHelper.ExtensionImages))
                    {
                        string directoryOutput = FileHelper.GetDirectoryName(imgFileName);

                        string imgOutputName = GuiHelper.SaveFileDialog(FileHelper.GetFileNameWithoutExtension(imgFileName), GuiHelper.ExtensionPng, FileFilters.Png, directoryOutput);
                        if (imgOutputName != null)
                        {
                            bool success = Start.Tool(ActionN64.MirrorPicture, MirrorDictionary(imgFileName, imgOutputName), GetMirrorOptions());
                            MessagesHandler.DisplayMessageSuccessFailed(success);
                        }
                    }
                    else
                    {
                        MessagesHandler.DisplayMessageWarning(MessagesList.WrongExtensions, GuiHelper.ExtensionImages);
                        MirrorBrowseBox.Focus();
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.FileNotExists);
                    MirrorBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                MirrorBrowseBox.Focus();
            }
        }
        
        //--------------------------------------
        //--------- Events Bmp To Png ----------
        //--------------------------------------

        private void BmpPngUpdateGUI()
        {
            if (BmpPngTextureBrowseBox.Text != string.Empty && BmpPngAlphaBrowseBox.Text != string.Empty)
                BmpPngSaveButton.IsEnabled = true;
            else
                BmpPngSaveButton.IsEnabled = false;
        }

        private Dictionary<string, string> BmpPngDictionary(string imgFileName, string alphaFileName, string imgOutputName)
        {
            var dict = new Dictionary<string, string>
            {
                { "TextureFile", imgFileName },
                { "AlphaFile", alphaFileName },
                { "PictureOutput", imgOutputName }
            };

            return dict;
        }
        
        private void BmpPngTextureBrowseBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BmpPngUpdateGUI();
        }

        private void BmpPngTextureBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = GuiHelper.OpenFileDialog(FileFilters.Image);
            if (fileName != null)
                BmpPngTextureBrowseBox.Text = fileName;
        }

        private void BmpPngTextureDragDrop_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Images);
            if (filePath != null)
            {
                BmpPngTextureBrowseBox.Text = filePath;
            }
        }

        private void BmpPngAlphaBrowseBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BmpPngUpdateGUI();
        }

        private void BmpPngAlphaBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = GuiHelper.OpenFileDialog(FileFilters.Image);
            if (fileName != null)
                BmpPngAlphaBrowseBox.Text = fileName;
        }

        private void BmpPngAlphaDragDrop_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Images);
            if (filePath != null)
            {
                BmpPngAlphaBrowseBox.Text = filePath;
            }
        }

        private void BmpPngSaveButton_Click(object sender, RoutedEventArgs e)
        {
            string imgFileName = BmpPngTextureBrowseBox.Text;
            if (imgFileName != string.Empty)
            {
                if (File.Exists(imgFileName))
                {
                    if (FileHelper.IsFileExtension(imgFileName, GuiHelper.ExtensionImages))
                    {
                        string alphaFileName = BmpPngAlphaBrowseBox.Text;
                        if (alphaFileName != string.Empty)
                        {
                            if (File.Exists(alphaFileName))
                            {
                                if (FileHelper.IsFileExtension(alphaFileName, GuiHelper.ExtensionImages))
                                {
                                    if (ImageHelper.AreSameSize(imgFileName, alphaFileName))
                                    {
                                        string directoryOutput = FileHelper.GetDirectoryName(imgFileName);

                                        string imgOutputName = GuiHelper.SaveFileDialog(FileHelper.GetFileNameWithoutExtension(imgFileName), GuiHelper.ExtensionPng, FileFilters.Png, directoryOutput);
                                        if (imgOutputName != null)
                                        {
                                            bool success = Start.Tool(ActionN64.MakeTransparent, BmpPngDictionary(imgFileName, alphaFileName, imgOutputName), 0);
                                            MessagesHandler.DisplayMessageSuccessFailed(success);
                                        }
                                    }
                                    else
                                    {
                                        MessagesHandler.DisplayMessageWarning(MessagesList.DifferentImageSizes);
                                    }
                                }
                                else
                                {
                                    MessagesHandler.DisplayMessageWarning(MessagesList.WrongExtensions, GuiHelper.ExtensionImages);
                                    BmpPngAlphaBrowseBox.Focus();
                                }
                            }
                            else
                            {
                                MessagesHandler.DisplayMessageWarning(MessagesList.FileNotExists);
                                BmpPngAlphaBrowseBox.Focus();
                            }
                        }
                        else
                        {
                            MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                            BmpPngAlphaBrowseBox.Focus();
                        }
                    }
                    else
                    {
                        MessagesHandler.DisplayMessageWarning(MessagesList.WrongExtensions, GuiHelper.ExtensionImages);
                        BmpPngTextureBrowseBox.Focus();
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.FileNotExists);
                    BmpPngTextureBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                BmpPngTextureBrowseBox.Focus();
            }
        }


        //--------------------------------------
        //--------- Events Other Tools ---------
        //--------------------------------------

        // Negative

        private void NegativeBrowseBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NegativeBrowseBox.Text != string.Empty)
                NegativeSaveButton.IsEnabled = true;
            else
                NegativeSaveButton.IsEnabled = false;
        }

        private void NegativeBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = GuiHelper.OpenFileDialog(FileFilters.Image);
            if (fileName != null)
                NegativeBrowseBox.Text = fileName;
        }
        
        private void NegativeDragDrop_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Images);
            if (filePath != null)
            {
                NegativeBrowseBox.Text = filePath;
            }
        }

        private Dictionary<string, string> NegativeDictionary(string imgFileName, string imgOutputName)
        {
            var dict = new Dictionary<string, string>
            {
                { "PictureFile", imgFileName },
                { "PictureOutput", imgOutputName }
            };
            return dict;
        }

        private void NegativeSaveButton_Click(object sender, RoutedEventArgs e)
        {
            string imgFileName = NegativeBrowseBox.Text;
            if (imgFileName != string.Empty)
            {
                if (File.Exists(imgFileName))
                {
                    if (FileHelper.IsFileExtension(imgFileName, GuiHelper.ExtensionImages))
                    {
                        string directoryOutput = FileHelper.GetDirectoryName(imgFileName);

                        string imgOutputName = GuiHelper.SaveFileDialog(FileHelper.GetFileNameWithoutExtension(imgFileName), GuiHelper.ExtensionPng, FileFilters.Png, directoryOutput);
                        if (imgOutputName != null)
                        {
                            bool success = Start.Tool(ActionN64.NegativePicture, NegativeDictionary(imgFileName, imgOutputName), 0);
                            MessagesHandler.DisplayMessageSuccessFailed(success);
                        }
                    }
                    else
                    {
                        MessagesHandler.DisplayMessageWarning(MessagesList.WrongExtensions, GuiHelper.ExtensionImages);
                        NegativeBrowseBox.Focus();
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageWarning(MessagesList.FileNotExists);
                    NegativeBrowseBox.Focus();
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                NegativeBrowseBox.Focus();
            }
        }


        //-------------------------------------
        //--------- Events BlackList ----------
        //-------------------------------------

        // Add/Delete Games

        private void BlacklistAddGameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BlacklistAddGameBox.Text != string.Empty)
            {
                BlacklistAddGameButton.IsEnabled = true;
            }
            else
            {
                BlacklistAddGameButton.IsEnabled = false;
            }
        }

        private void BlacklistAddGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (BlacklistAddGameBox.Text != string.Empty)
            {
                GameTexturesXml game = new GameTexturesXml
                {
                    Name = BlacklistAddGameBox.Text
                };
                GamesList.Add(game);
                BlacklistAddGameBox.Text = string.Empty; // Reset the TextBox 
                SaveTexturesBlacklistXml();
            }
        }

        private void BlacklistDeleteGameSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = BlacklistDeleteGameSelect.SelectedValue;
            if (selected != null)
            {
                BlacklistDeleteGameButton.IsEnabled = true;
            }
            else
            {
                BlacklistDeleteGameButton.IsEnabled = false;
            }
            
        }

        private void BlacklistDeleteGameButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGame = BlacklistDeleteGameSelect.SelectedValue;
            if (selectedGame != null)
            {
                if (selectedGame.GetType().Equals(typeof(GameTexturesXml))) // Safe check
                {
                    GamesList.Remove((GameTexturesXml) selectedGame);
                    SaveTexturesBlacklistXml();
                }
                else
                {
                    MessagesHandler.DisplayMessageError("Error deleting the game");
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.NoGameSelected);
            }
        }

        // Add/Delete Textures

        private void GameTexturesAppend(ICollection<string> list)
        {
            foreach (string str in list)
            {
                if (FileHelper.IsFileExtension(str, GuiHelper.ExtensionImages))
                    GameTextures.Add(str);
            }
        }

        private void BlacklistTextureFilesUpdate(string[] files)
        {
            if (files != null)
            {
                List<string> tmpNoDuplicate = new List<string>(GameTextures);
                tmpNoDuplicate.AddRange(files);
                GameTextures.Clear();
                GameTexturesAppend(new HashSet<string>(tmpNoDuplicate));
                BlacklistTextureSaveButton.IsEnabled = true;
            }
        }

        private void BlacklistTextureGameSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameTextures.Clear();
            var selectedGame = BlacklistTextureGameSelect.SelectedValue;
            if (selectedGame != null)
            {
                if (selectedGame.GetType().Equals(typeof(GameTexturesXml))) // Safe check
                {
                    GameTexturesAppend(((GameTexturesXml)selectedGame).Textures);
                    BlacklistTextureAddButton.IsEnabled = true;
                }
                else
                {
                    MessagesHandler.DisplayMessageError(MessagesList.ErrorGameData);
                }
            }
            else
            {
                BlacklistTextureAddButton.IsEnabled = false;
                BlacklistTextureDeleteButton.IsEnabled = false;
                BlacklistTextureSaveButton.IsEnabled = false;
            }
        }
        

        private void BlacklistTextureBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BlacklistTextureBox.SelectedItems.Count > 0) // Atleast 1 texture path selected
            {
                BlacklistTextureDeleteButton.IsEnabled = true;
            }
            else
            {
                BlacklistTextureDeleteButton.IsEnabled = false;
            }
        }

        private void BlacklistTextureAddButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGame = BlacklistTextureGameSelect.SelectedValue;
            if (selectedGame != null)
            {
                if (selectedGame.GetType().Equals(typeof(GameTexturesXml))) // Safe check
                {
                    string[] files = GuiHelper.OpenFilesDialog(FileFilters.Image);
                    BlacklistTextureFilesUpdate(files);
                }
                else
                {
                    MessagesHandler.DisplayMessageError(MessagesList.ErrorGameData);
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.NoGameSelected);
            }
        }

        private void BlacklistTextureBox_Drop(object sender, DragEventArgs e)
        {
            var selectedGame = BlacklistTextureGameSelect.SelectedValue;
            if (selectedGame != null)
            {
                if (selectedGame.GetType().Equals(typeof(GameTexturesXml))) // Safe check
                {
                    BlacklistTextureFilesUpdate(DragDropGetPaths(e));
                }
            }
        }

        private void BlacklistTextureDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (BlacklistTextureBox.SelectedItems.Count > 0) // Atleast 1 texture path selected
            {
                List<string> tempTextures = new List<string>();
                foreach (var texture in BlacklistTextureBox.SelectedItems)
                {
                    if (texture.GetType().Equals(typeof(string)))
                        tempTextures.Add((string)texture);
                }
                foreach (string texture in tempTextures)
                {
                    GameTextures.Remove(texture);
                }
                BlacklistTextureSaveButton.IsEnabled = true;
            }
            else
            {
                MessagesHandler.DisplayMessageWarning("No texture selected");
            }
        }

        private void BlacklistTextureSaveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGame = BlacklistTextureGameSelect.SelectedValue;
            if (selectedGame != null)
            {
                if (selectedGame.GetType().Equals(typeof(GameTexturesXml))) // Safe check
                {
                    ((GameTexturesXml)selectedGame).Textures = new HashSet<string>(GameTextures);
                    SaveTexturesBlacklistXml();
                    MessagesHandler.DisplayMessageInformation(string.Format("Updated {0}",TexturesBlacklistXml));
                    BlacklistTextureSaveButton.IsEnabled = false;
                }
                else
                {
                    MessagesHandler.DisplayMessageError(MessagesList.ErrorGameData);
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.NoGameSelected);
            }
        }

        // Copy Textures

        private void BlacklistUpdateGUI()
        {
            var selectedGame = BlacklistCopyGameSelect.SelectedValue;
            if (selectedGame != null && BlacklistCopyTextureOutputBox.Text != string.Empty)
            {
                BlacklistCopyTextureButton.IsEnabled = true;
            }
            else
            {
                BlacklistCopyTextureButton.IsEnabled = false;
            }
        }
        
        private Dictionary<string, string> BlacklistCopyDictionary(string outputFolder)
        {
            var dict = new Dictionary<string, string>
            {
                { "OutputFolder", outputFolder }
            };
            return dict;
        }
        
        private void BlacklistCopyGameSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BlacklistUpdateGUI();
        }

        private void BlacklistCopyTextureOutputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlacklistUpdateGUI();
        }

        private void BlacklistCopyTextureOutputButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = GuiHelper.OpenFolderDialog();
            if (folderName != null)
                BlacklistCopyTextureOutputBox.Text = folderName;
        }


        private void BlacklistCopyTexture_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Folder);
            if (filePath != null)
            {
                BlacklistCopyTextureOutputBox.Text = filePath;
            }
        }

        private void BlacklistCopyTextureButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGame = BlacklistCopyGameSelect.SelectedValue;
            if (selectedGame != null)
            {
                if (selectedGame.GetType().Equals(typeof(GameTexturesXml))) // Safe check
                {
                    string outputFolder = BlacklistCopyTextureOutputBox.Text;

                    if (!string.IsNullOrEmpty(outputFolder))
                    {
                        if (Directory.Exists(outputFolder))
                        {
                            Start.texturesList.Clear();
                            Start.texturesList.AddRange(((GameTexturesXml)selectedGame).Textures);
                            bool success = Start.Tool(ActionN64.CopyBlacklistTextures, BlacklistCopyDictionary(outputFolder), 0);
                            MessagesHandler.DisplayMessageSuccessFailed(success);
                        }
                        else
                        {
                            MessagesHandler.DisplayMessageWarning(MessagesList.DirectoryNotExists);
                            BlacklistCopyTextureOutputBox.Focus();
                        }
                    }
                    else
                    {
                        MessagesHandler.DisplayMessageWarning(MessagesList.EmptyText);
                        BlacklistCopyTextureOutputBox.Focus();
                    }
                }
                else
                {
                    MessagesHandler.DisplayMessageError(MessagesList.ErrorGameData);
                }
            }
            else
            {
                MessagesHandler.DisplayMessageWarning(MessagesList.NoGameSelected);
            }
        }



    }

    public enum DragDropMode
    {
        Wrl,
        Obj,
        Images,
        Folder
    }

    public enum MessagesList
    {
        EmptyText,
        FileNotExists,
        DirectoryNotExists,
        WrongExtension,
        WrongExtensions,
        DifferentImageSizes,
        NoGameSelected,
        ErrorGameData
    }


    public class MessagesHandler
    {

        private const string MessageBoxTitle = "N64 Mapping Tool";


        private static string GetMessage(MessagesList msg)
        {
            return GetMessage(msg, string.Empty);
        }
        private static string GetMessage(MessagesList msg, string[] str)
        {
            return GetMessage(msg, string.Join(", ",str));
        }
        private static string GetMessage(MessagesList msg, string str)
        {
            switch (msg)
            {
                case MessagesList.EmptyText:
                    return "The field is empty";
                case MessagesList.FileNotExists:
                    return "The file doesn't exist";
                case MessagesList.DirectoryNotExists:
                    return "The directory doesn't exist";
                case MessagesList.WrongExtension:
                    return string.Format("The file doesn't have the {0} extension", str);
                case MessagesList.WrongExtensions:
                    return string.Format("The file doesn't have one of the following extension: {0}", str);
                case MessagesList.DifferentImageSizes:
                    return "The textures are not the same size";
                case MessagesList.NoGameSelected:
                    return "No game selected";
                case MessagesList.ErrorGameData:
                    return "Error getting the game data";
                default:
                    return "";
            }
        }

        public static bool AskYesNoQuestion(string message)
        {
            return ResultMessageYesNoQuestion(DisplayYesNoQuestion(message));
        }

        public static bool ResultMessageYesNoQuestion(MessageBoxResult result)
        {
            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }

        public static MessageBoxResult DisplayYesNoQuestion(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, MessageBoxTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result;
        }

        public static void DisplayMessageWarning(MessagesList msg)
        {
            DisplayMessageWarning(GetMessage(msg));
        }
        public static void DisplayMessageWarning(MessagesList msg, string[] str)
        {
            DisplayMessageWarning(GetMessage(msg, str));
        }
        public static void DisplayMessageWarning(MessagesList msg, string str)
        {
            DisplayMessageWarning(GetMessage(msg, str));
        }

        public static void DisplayMessageWarning(string message)
        {
            MessageBox.Show(message, MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void DisplayMessageInformation(string message)
        {
            MessageBox.Show(message, MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void DisplayMessageError(MessagesList msg)
        {
            DisplayMessageError(GetMessage(msg));
        }
        public static void DisplayMessageError(string message)
        {
            MessageBox.Show(message, MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void DisplayMessageSuccessFailed(bool success)
        {
            if (success)
                DisplayMessageSuccess();
            else
                DisplayMessageFailed();
        }
        private static void DisplayMessageSuccess()
        {
            DisplayMessageInformation("Success");
        }
        private static void DisplayMessageFailed()
        {
            DisplayMessageError("Error during the execution");
        }

        
    }

}
