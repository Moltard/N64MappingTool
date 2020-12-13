using N64Application.Tool;
using N64Application.Tool.Utils;
using N64Library.Tool.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #region Attributes

        /// <summary>
        /// Used for the Merge Obj ListBox
        /// </summary>
        private ObservableCollection<string> mergeObjFileList;

        /// <summary>
        /// Used for the BlackList Texture ListBox
        /// </summary>
        private ObservableCollection<string> blackListTexturesList;

        /// <summary>
        /// Used to store the tool settings
        /// </summary>
        private Settings toolSettings;

        #endregion

        #region Constructor

        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
            InitMergeObjList();
            InitBlackList();
        }

        #endregion

        #region Initialize GUI
        
        #region Initialize GUI - Merge Obj

        private void InitMergeObjList()
        {
            mergeObjFileList = new ObservableCollection<string>();
            MergeObjListBox.DataContext = mergeObjFileList;
        }

        #endregion

        #region Initialize GUI - BlackList

        private void InitBlackList()
        {
            toolSettings = XmlUtils.GetSettings();
            
            if (toolSettings != null)
            {
                blackListTexturesList = new ObservableCollection<string>();
                BlackListTexturesBox.DataContext = blackListTexturesList;
                toolSettings.InitAllAttributes();
                InitConfigComboBox();
            }
        }

        /// <summary>
        /// Init the datacontext of the config combobox and set the selection to a default value
        /// </summary>
        private void InitConfigComboBox()
        {
            ObjModifyObjComboBoxBlackList.DataContext = toolSettings.BlackList;
            BlackListConfigComboBox.DataContext = toolSettings.BlackList;
            BlackListTexturesComboBox.DataContext = toolSettings.BlackList;
            BlackListCopyComboBox.DataContext = toolSettings.BlackList;
            ConfigForceSelect();
        }

        /// <summary>
        /// Set the selected BlackList Config as the selection of all Config comboboxes
        /// </summary>
        /// <param name="map"></param>
        private void ConfigSetSelected(BlackListConfig config)
        {
            ObjModifyObjComboBoxBlackList.SelectedItem = config;
            BlackListConfigComboBox.SelectedItem = config;
            BlackListTexturesComboBox.SelectedItem = config;
            BlackListCopyComboBox.SelectedItem = config;
        }

        /// <summary>
        /// Force the BlackList Config Combobox to the first element if possible
        /// </summary>
        private void ConfigForceSelect()
        {
            if (toolSettings.BlackList.Count > 0)
            {
                ConfigSetSelected(toolSettings.BlackList[0]);
            }
            ConfigListUpdateGUI();
        }

        /// <summary>
        /// Update the visibility of the Config comboboxes based on how many Configs are available
        /// </summary>
        private void ConfigListUpdateGUI()
        {
            bool hasConfigs = toolSettings.BlackList.Count > 0;
            ObjModifyObjCkbBlackList.IsEnabled = hasConfigs;
            ObjModifyObjComboBoxBlackList.IsEnabled = hasConfigs;
            if (!hasConfigs)
            {
                ObjModifyObjCkbBlackList.IsChecked = false;
            }
            BlackListConfigComboBox.IsEnabled = hasConfigs;
            BlackListTexturesComboBox.IsEnabled = hasConfigs;
            BlackListCopyComboBox.IsEnabled = hasConfigs;
            BlackListConfigContainer.Visibility = hasConfigs ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion

        #endregion

        #region Events Drag and Drop

        /// <summary>
        /// Allow dragging on TextBox
        /// </summary>
        /// <param name="e"></param>
        private void DragOverTextBoxFix(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = DragDropEffects.Move;
        }

        /// <summary>
        /// Return the list of dropped path for the DragDrop event
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
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
                    case DragDropMode.Any:
                        // Get the first dragged file
                        if (droppedFilePaths.Length > 0)
                        {
                            filePath = droppedFilePaths[0];
                        }
                        break;
                    case DragDropMode.Wrl:
                        foreach (string path in droppedFilePaths)
                        {
                            // Get the first WRL dragged if multiple files are dragged
                            if (FileUtils.IsFileExtension(path, FileConstants.ExtensionWrl))
                            {
                                filePath = path;
                                break;
                            }
                        }
                        break;
                    case DragDropMode.Obj:
                        foreach (string path in droppedFilePaths)
                        {
                            // Get the first OBJ dragged if multiple files are dragged
                            if (FileUtils.IsFileExtension(path, FileConstants.ExtensionObj))
                            {
                                filePath = path;
                                break;
                            }
                        }
                        break;
                    case DragDropMode.Images:
                        foreach (string path in droppedFilePaths)
                        {
                            // Get the first Image dragged if multiple files are dragged
                            if (FileUtils.IsFileExtension(path, FileConstants.ExtensionImages))
                            {
                                filePath = path;
                                break;
                            }
                        }
                        break;
                    case DragDropMode.Directory:
                        foreach (string path in droppedFilePaths)
                        {
                            // Get the directory path of the dragged files
                            if (System.IO.Directory.Exists(path))
                            {
                                filePath = path;
                                break;
                            }
                            else if (System.IO.File.Exists(path))
                            {
                                filePath = System.IO.Path.GetDirectoryName(path);
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

        #endregion

        #region Events Generic Functions
        
        /// <summary>
        /// Create a HashSet with a given string array and a string ICollection
        /// </summary>
        /// <param name="filePathList"></param>
        /// <param name="initialList"></param>
        /// <returns></returns>
        private HashSet<string> GetHashsetFromFileList(string[] filePathList, ICollection<string> initialList = null)
        {
            HashSet<string> newFileList = initialList != null ? new HashSet<string>(initialList) : new HashSet<string>();
            foreach (string filePath in filePathList)
            {
                newFileList.Add(filePath);
            }
            return newFileList;
        }

        #endregion

        #region Events Wrl To Obj

        private void WrlFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isNotEmpty = !String.IsNullOrEmpty(WrlFileTextBox.Text);
            WrlConvertExecuteButton.IsEnabled = isNotEmpty;
        }

        private void WrlFileDrag_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Wrl);
            if (filePath != null)
            {
                WrlFileTextBox.Text = filePath;
            }
        }

        private void WrlFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileUtils.OpenFileDialog(FileFilters.Wrl);
            if (filePath != null)
            {
                WrlFileTextBox.Text = filePath;
            }
        }

        private void WrlConvertExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string wrlFilePath = WrlFileTextBox.Text;
            if (!String.IsNullOrEmpty(wrlFilePath))
            {
                if (System.IO.File.Exists(wrlFilePath))
                {
                    if (FileUtils.IsFileExtension(wrlFilePath, FileConstants.ExtensionWrl))
                    {
                        ActionN64 action = ActionN64.WrlConversion;
                        string objOutputPath = FileUtils.SaveFileDialogPresetFileName(action, 
                            System.IO.Path.GetFileNameWithoutExtension(wrlFilePath), 
                            System.IO.Path.GetDirectoryName(wrlFilePath));
                        if (!String.IsNullOrEmpty(objOutputPath))
                        {
                            ToolResult result = Start.ToolObj(action, WrlGetDataDictionary(wrlFilePath, objOutputPath), WrlGetObjOptions());
                            MessageBoxUtils.ShowMessageExecution(result);
                        }
                    }
                    else
                    {
                        MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotExtension + FileConstants.ExtensionWrl);
                        WrlFileTextBox.Focus();
                    }
                }
                else
                {
                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotFound);
                    WrlFileTextBox.Focus();
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                WrlFileTextBox.Focus();
            }
        }

        private Dictionary<string, string> WrlGetDataDictionary(string wrlFilePath, string objOutputPath)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.WrlFile, wrlFilePath },
                { DictConstants.ObjOutput, objOutputPath }
            };
            return dict;
        }

        private ObjOptions WrlGetObjOptions()
        {
            ObjOptions options = new ObjOptions();
            if (WrlReverseVertexCheckbox.IsChecked == true)
            {
                options |= ObjOptions.ReverseVertex;
            }
            return options;
        }

        #endregion

        #region Events Wrls To Obj

        private void WrlsDirectoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isNotEmpty = !String.IsNullOrEmpty(WrlsDirectoryTextBox.Text);
            WrlsConvertExecuteButton.IsEnabled = isNotEmpty;
        }

        private void WrlsDirectoryDrag_Drop(object sender, DragEventArgs e)
        {
            string directoryPath = DragDropGetPath(e, DragDropMode.Directory);
            if (directoryPath != null)
            {
                WrlsDirectoryTextBox.Text = directoryPath;
            }
        }

        private void WrlsDirectoryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string directoryPath = FileUtils.OpenDirectoryDialog();
            if (directoryPath != null)
            {
                WrlsDirectoryTextBox.Text = directoryPath;
            }
        }

        private void WrlsConvertExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string wrlDirectoryPath = WrlsDirectoryTextBox.Text;
            if (!String.IsNullOrEmpty(wrlDirectoryPath))
            {
                if (System.IO.Directory.Exists(wrlDirectoryPath))
                {

                    bool resultQuestion = MessageBoxUtils.ShowMessageYesNoQuestion(MessageBoxConstants.MessageWrlConvertRecursive);
                    if (resultQuestion)
                    {
                        ActionN64 action = ActionN64.WrlsConversion;
                        ToolResult result = Start.ToolObj(action, WrlsGetDataDictionary(wrlDirectoryPath), WrlsGetObjOptions());
                        MessageBoxUtils.ShowMessageExecution(result);
                    }
                }
                else
                {
                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageDirectoryNotFound);
                    WrlsDirectoryTextBox.Focus();
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                WrlsDirectoryTextBox.Focus();
            }
        }

        private Dictionary<string, string> WrlsGetDataDictionary(string wrlDirectoryPath)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.WrlDirectory, wrlDirectoryPath }
            };
            return dict;
        }

        private ObjOptions WrlsGetObjOptions()
        {
            ObjOptions options = new ObjOptions();
            if (WrlsReverseVertexCheckbox.IsChecked == true)
            {
                options |= ObjOptions.ReverseVertex;
            }
            return options;
        }

        #endregion

        #region Events Obj Tools

        #region Events Obj Tools - Obj File

        private void ObjFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isNotEmpty = !String.IsNullOrEmpty(ObjFileTextBox.Text);
            ObjDeleteMaterialsExecuteButton.IsEnabled = isNotEmpty;
            ObjDeleteUnusedMaterialsExecuteButton.IsEnabled = isNotEmpty;
            ObjAddMaterialsExecuteButton.IsEnabled = isNotEmpty;
            ObjModifyObjExecuteButton.IsEnabled = isNotEmpty;
            ObjObjToSmdExecuteButton.IsEnabled = isNotEmpty;
            ObjReferenceModelExecuteButton.IsEnabled = isNotEmpty;
        }

        private void ObjFileDrag_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Obj);
            if (filePath != null)
            {
                ObjFileTextBox.Text = filePath;
            }
        }

        private void ObjFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileUtils.OpenFileDialog(FileFilters.Obj);
            if (filePath != null)
            {
                ObjFileTextBox.Text = filePath;
            }
        }

        #endregion

        #region Events Obj Tools - Generic
        
        private void ObjToolsHandleAllActions(ActionN64 action)
        {
            string objFilePath = ObjFileTextBox.Text;
            if (!String.IsNullOrEmpty(objFilePath))
            {
                if (System.IO.File.Exists(objFilePath))
                {
                    if (FileUtils.IsFileExtension(objFilePath, FileConstants.ExtensionObj))
                    {
                        string fileOutputPath = FileUtils.SaveFileDialogPreset(action, System.IO.Path.GetDirectoryName(objFilePath));
                        if (!String.IsNullOrEmpty(fileOutputPath))
                        {
                            ToolResult result = Start.ToolObj(action, ObjGetDataDictionary(objFilePath, fileOutputPath, action), ObjToolsGetObjOptions(action));
                            MessageBoxUtils.ShowMessageExecution(result);
                        }
                    }
                    else
                    {
                        MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotExtension + FileConstants.ExtensionObj);
                        ObjFileTextBox.Focus();
                    }
                }
                else
                {
                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotFound);
                    ObjFileTextBox.Focus();
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                ObjFileTextBox.Focus();
            }
        }

        private Dictionary<string, string> ObjGetDataDictionary(string objFilePath, string fileOutputPath, ActionN64 action)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.ObjFile, objFilePath }
            };
            switch (action)
            {
                case ActionN64.DeleteMaterials:
                case ActionN64.DeleteUnusedMaterials:
                case ActionN64.AddMaterials:
                    dict.Add(DictConstants.ObjOutput, fileOutputPath);
                    break;
                case ActionN64.ModifyObj:
                    dict.Add(DictConstants.ObjOutput, fileOutputPath);
                    dict.Add(DictConstants.ScaleValue, ObjModifyObjTextBoxScaleUni.Text);
                    dict.Add(DictConstants.ScaleValueX, ObjModifyObjTextBoxScaleNonUniX.Text);
                    dict.Add(DictConstants.ScaleValueY, ObjModifyObjTextBoxScaleNonUniY.Text);
                    dict.Add(DictConstants.ScaleValueZ, ObjModifyObjTextBoxScaleNonUniZ.Text);
                    dict.Add(DictConstants.RotateValueX, ObjModifyObjTextBoxRotateX.Text);
                    dict.Add(DictConstants.RotateValueY, ObjModifyObjTextBoxRotateY.Text);
                    dict.Add(DictConstants.RotateValueZ, ObjModifyObjTextBoxRotateZ.Text);
                    break;
                case ActionN64.ObjToSmd:
                    dict.Add(DictConstants.SmdOutput, fileOutputPath);
                    break;
                case ActionN64.RefModelSmd:
                    dict.Add(DictConstants.SmdOutput, fileOutputPath);
                    dict.Add(DictConstants.ScaleValue, ObjReferenceModelScaleTextBox.Text);
                    break;
                default:
                    break;
            }
            return dict;
        }

        private ObjOptions ObjToolsGetObjOptions(ActionN64 action)
        {
            ObjOptions options = new ObjOptions();

            switch (action)
            {
                case ActionN64.ModifyObj:
                    if (ObjModifyObjCkbReverseVertex.IsChecked == true) { options |= ObjOptions.ReverseVertex; }
                    if (ObjModifyObjCkbMerge.IsChecked == true) { options |= ObjOptions.Merge; }
                    if (ObjModifyObjCkbSort.IsChecked == true) { options |= ObjOptions.Sort; }
                    if (ObjModifyObjCkbScaleUni.IsChecked == true) { options |= ObjOptions.UniformScale; }
                    if (ObjModifyObjCkbScaleNonUni.IsChecked == true) { options |= ObjOptions.NonUniformScale; }
                    if (ObjModifyObjCkbRotate.IsChecked == true) { options |= ObjOptions.Rotate; }
                    if (ObjModifyObjCkbBlackList.IsChecked == true)
                    {
                        options |= ObjOptions.BlackList;
                        var selected = ObjModifyObjComboBoxBlackList.SelectedItem;
                        if (selected != null)
                        {
                            var config = (BlackListConfig)selected;
                            Start.texturesList.Clear();
                            Start.texturesList.AddRange(config.Textures);
                        }
                    }
                    break;
                case ActionN64.ObjToSmd:
                    if (ObjObjToSmdRadioTexture.IsChecked == true) { options |= ObjOptions.SmdUseTextureName; }
                    break;
            }
            return options;
        }

        #endregion

        #region Events Obj Tools - Delete/Add Materials

        private void ObjDeleteMaterialsExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            ObjToolsHandleAllActions(ActionN64.DeleteMaterials);
        }

        private void ObjDeleteUnusedMaterialsExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            ObjToolsHandleAllActions(ActionN64.DeleteUnusedMaterials);
        }

        private void ObjAddMaterialsExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            ObjToolsHandleAllActions(ActionN64.AddMaterials);
        }

        #endregion

        #region Events Obj Tools - Obj to Smd/Reference Model

        private void ObjObjToSmdExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            ObjToolsHandleAllActions(ActionN64.ObjToSmd);
        }

        private void ObjReferenceModelExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            ObjToolsHandleAllActions(ActionN64.RefModelSmd);
        }

        #endregion

        #region Events Obj Tools - Modify Obj

        private void ObjModifyObjExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            ObjToolsHandleAllActions(ActionN64.ModifyObj);
        }

        private void ObjModifyObjCkbScaleUni_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = ObjModifyObjCkbScaleUni.IsChecked == true;
            ObjModifyObjTextBoxScaleUni.IsEnabled = isChecked;
            ObjModifyObjCkbScaleNonUni.IsEnabled = !isChecked;
            if (isChecked)
            {
                ObjModifyObjCkbScaleNonUni.IsChecked = false;
            }
        }

        private void ObjModifyObjCkbScaleNonUni_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = ObjModifyObjCkbScaleNonUni.IsChecked == true;
            ObjModifyObjTextBoxScaleNonUniX.IsEnabled = isChecked;
            ObjModifyObjTextBoxScaleNonUniY.IsEnabled = isChecked;
            ObjModifyObjTextBoxScaleNonUniZ.IsEnabled = isChecked;
            ObjModifyObjCkbScaleUni.IsEnabled = !isChecked;
            if (isChecked)
            {
                ObjModifyObjCkbScaleUni.IsChecked = false;
            }
        }

        private void ObjModifyObjCkbRotate_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = ObjModifyObjCkbRotate.IsChecked == true;
            ObjModifyObjTextBoxRotateX.IsEnabled = isChecked;
            ObjModifyObjTextBoxRotateY.IsEnabled = isChecked;
            ObjModifyObjTextBoxRotateZ.IsEnabled = isChecked;
        }

        private void ObjModifyObjCkbBlackList_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = ObjModifyObjCkbBlackList.IsChecked == true;
            ObjModifyObjComboBoxBlackList.IsEnabled = isChecked;
        }

        #endregion

        #endregion

        #region Events Copy Textures

        #region Events Copy Textures - GUI

        private void CopyTexturesUpdateGUI()
        {
            bool isObjFileNotEmpty = !String.IsNullOrEmpty(CopyTexturesObjFileTextBox.Text);
            bool isDirOutputChecked = CopyTexturesDirOutputCheckBox.IsChecked == true;
            bool isDirTextureChecked = CopyTexturesDirTextureCheckBox.IsChecked == true;
            
            // Lock or Unlock the Output Directory TextBox and Button
            CopyTexturesDirOutputTextBox.IsEnabled = !isDirOutputChecked;
            CopyTexturesDirOutputBrowseButton.IsEnabled = !isDirOutputChecked;

            // If unchecked, use the Directory of the Obj file as the output directory
            bool isDirOutputNotEmpty = isDirOutputChecked ? 
                true : !String.IsNullOrEmpty(CopyTexturesDirOutputTextBox.Text);

            // Lock or Unlock the Texture Directory TextBox and Button
            CopyTexturesDirTextureTextBox.IsEnabled = isDirTextureChecked;
            CopyTexturesDirTextureBrowseButton.IsEnabled = isDirTextureChecked;

            // If unchecked, load textures from the Obj and Mtl File data
            bool isDirTextureNotEmpty = isDirTextureChecked ? 
                !String.IsNullOrEmpty(CopyTexturesDirTextureTextBox.Text) : true;

            CopyTexturesExecuteButton.IsEnabled = isObjFileNotEmpty && isDirOutputNotEmpty && isDirTextureNotEmpty;
        }

        #endregion

        #region Events Copy Textures - Obj File

        private void CopyTexturesObjFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CopyTexturesUpdateGUI();
        }

        private void CopyTexturesObjFileDrag_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Obj);
            if (filePath != null)
            {
                CopyTexturesObjFileTextBox.Text = filePath;
            }
        }

        private void CopyTexturesObjFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileUtils.OpenFileDialog(FileFilters.Obj);
            if (filePath != null)
            {
                CopyTexturesObjFileTextBox.Text = filePath;
            }
        }

        #endregion

        #region Events Copy Textures - Directory Output

        private void CopyTexturesDirOutputCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            CopyTexturesUpdateGUI();
        }

        private void CopyTexturesDirOutputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CopyTexturesUpdateGUI();
        }

        private void CopyTexturesDirOutputDrag_Drop(object sender, DragEventArgs e)
        {
            string directoryPath = DragDropGetPath(e, DragDropMode.Directory);
            if (directoryPath != null)
            {
                CopyTexturesDirOutputTextBox.Text = directoryPath;
            }
        }

        private void CopyTexturesDirOutputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string directoryPath = FileUtils.OpenDirectoryDialog();
            if (directoryPath != null)
            {
                CopyTexturesDirOutputTextBox.Text = directoryPath;
            }
        }
        #endregion

        #region Events Copy Textures - Directory Texture

        private void CopyTexturesDirTextureCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            CopyTexturesUpdateGUI();
        }

        private void CopyTexturesDirTextureTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CopyTexturesUpdateGUI();
        }

        private void CopyTexturesDirTextureDrag_Drop(object sender, DragEventArgs e)
        {
            string directoryPath = DragDropGetPath(e, DragDropMode.Directory);
            if (directoryPath != null)
            {
                CopyTexturesDirTextureTextBox.Text = directoryPath;
            }
        }

        private void CopyTexturesDirTextureBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string directoryPath = FileUtils.OpenDirectoryDialog();
            if (directoryPath != null)
            {
                CopyTexturesDirTextureTextBox.Text = directoryPath;
            }
        }

        #endregion

        #region Events Copy Textures - Execution

        private void CopyTexturesExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string objFilePath = CopyTexturesObjFileTextBox.Text;
            if (!String.IsNullOrEmpty(objFilePath))
            {
                if (System.IO.File.Exists(objFilePath))
                {
                    string outputDirectoryPath = CopyTexturesDirOutputCheckBox.IsChecked == true ?
                        System.IO.Path.GetDirectoryName(objFilePath) : CopyTexturesDirOutputTextBox.Text;

                    if (!String.IsNullOrEmpty(outputDirectoryPath))
                    {
                        if (System.IO.Directory.Exists(outputDirectoryPath))
                        {
                            bool isDirTextureChecked = CopyTexturesDirTextureCheckBox.IsChecked == true;
                            string textureDirectoryPath = isDirTextureChecked ? CopyTexturesDirTextureTextBox.Text : null;

                            // If Texture Directory is unchecked, we load textures from the obj and mtl files
                            bool canCopyTextures = !isDirTextureChecked;
                            if (isDirTextureChecked)
                            {
                                if (!String.IsNullOrEmpty(textureDirectoryPath))
                                {
                                    if (System.IO.Directory.Exists(textureDirectoryPath))
                                    {
                                        canCopyTextures = true;
                                    }
                                    else
                                    {
                                        MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageDirectoryNotFound);
                                        CopyTexturesDirTextureTextBox.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                                    CopyTexturesDirTextureTextBox.Focus();
                                }
                            }
                            if (canCopyTextures)
                            {
                                bool resultQuestion = MessageBoxUtils.ShowMessageYesNoQuestion(MessageBoxConstants.MessageCopyTextures);
                                if (resultQuestion)
                                {
                                    ActionN64 action = ActionN64.CopyObjTextures;
                                    ToolResult result = Start.ToolPictures(action, CopyTexturesDictionary(objFilePath, outputDirectoryPath, textureDirectoryPath));
                                    MessageBoxUtils.ShowMessageExecution(result);
                                }
                            }
                        }
                        else
                        {
                            MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageDirectoryNotFound);
                            CopyTexturesDirOutputTextBox.Focus();
                        }
                    }
                    else
                    {
                        MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                        CopyTexturesDirOutputTextBox.Focus();
                    }
                }
                else
                {
                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotFound);
                    CopyTexturesObjFileTextBox.Focus();
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                CopyTexturesObjFileTextBox.Focus();
            }
        }

        private Dictionary<string, string> CopyTexturesDictionary(string objFilePath, string outputDirectoryPath, string textureDirectoryPath)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.ObjFile, objFilePath },
                { DictConstants.OutputDirectory, outputDirectoryPath },
                { DictConstants.TextureDir, textureDirectoryPath },
            };

            return dict;
        }

        #endregion

        #endregion

        #region Events Flip Texture
        
        private void FlipTextureUpdateGUI()
        {
            bool isTextureFileNotEmpty = !String.IsNullOrEmpty(FlipTextureFileTextBox.Text);
            bool isHorizontalChecked = FlipTextureCkbHorizontally.IsChecked == true;
            bool isVerticalChecked = FlipTextureCkbVertically.IsChecked == true;
            bool areBothChecked = isHorizontalChecked && isVerticalChecked;
            FlipTextureExecuteButton.IsEnabled = isTextureFileNotEmpty && (isHorizontalChecked || isVerticalChecked || areBothChecked);
        }

        private void FlipTextureFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FlipTextureUpdateGUI();
        }
        
        private void FlipTextureFileDrag_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Images);
            if (filePath != null)
            {
                FlipTextureFileTextBox.Text = filePath;
            }
        }

        private void FlipTextureFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileUtils.OpenFileDialog(FileFilters.Images);
            if (filePath != null)
            {
                FlipTextureFileTextBox.Text = filePath;
            }
        }

        private void FlipTextureCkbHorizontally_Changed(object sender, RoutedEventArgs e)
        {
            FlipTextureUpdateGUI();
        }

        private void FlipTextureCkbVertically_Changed(object sender, RoutedEventArgs e)
        {
            FlipTextureUpdateGUI();
        }

        private void FlipTextureExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string pictureFilePath = FlipTextureFileTextBox.Text;
            if (!String.IsNullOrEmpty(pictureFilePath))
            {
                if (System.IO.File.Exists(pictureFilePath))
                {
                    if (FileUtils.IsFileExtension(pictureFilePath, FileConstants.ExtensionImages))
                    {
                        ActionN64 action = ActionN64.FlipTexture;
                        string pictureOutputPath = FileUtils.SaveFileDialogPresetFileName(action,
                            System.IO.Path.GetFileNameWithoutExtension(pictureFilePath),
                            System.IO.Path.GetDirectoryName(pictureFilePath));
                        if (!String.IsNullOrEmpty(pictureOutputPath))
                        {
                            ToolResult result = Start.ToolPictures(action, FlipTextureGetDataDictionary(pictureFilePath, pictureOutputPath), FlipTextureGetPictureOptions());
                            MessageBoxUtils.ShowMessageExecution(result);
                        }
                    }
                    else
                    {
                        MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotExtension + FileConstants.ExtensionImages);
                        FlipTextureFileTextBox.Focus();
                    }
                }
                else
                {
                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotFound);
                    FlipTextureFileTextBox.Focus();
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                FlipTextureFileTextBox.Focus();
            }
        }

        private Dictionary<string, string> FlipTextureGetDataDictionary(string pictureFilePath, string pictureOutputPath)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.PictureFile, pictureFilePath },
                { DictConstants.PictureOutput, pictureOutputPath },
            };
            return dict;
        }


        private PictureOptions FlipTextureGetPictureOptions()
        {
            PictureOptions options = new PictureOptions();
            if (FlipTextureCkbHorizontally.IsChecked == true) {options |= PictureOptions.FlipHorizontally; }
            if (FlipTextureCkbVertically.IsChecked == true) {options |= PictureOptions.FlipVertically; }
            return options;
        }

        #endregion

        #region Events Mirror Texture

        private void MirrorTextureUpdateGUI()
        {
            bool isTextureFileNotEmpty = !String.IsNullOrEmpty(MirrorTextureFileTextBox.Text);
            bool isHorizontalChecked = MirrorTextureCkbHorizontally.IsChecked == true;
            bool isVerticalChecked = MirrorTextureCkbVertically.IsChecked == true;
            bool areBothChecked = isHorizontalChecked && isVerticalChecked;
            MirrorTextureExecuteButton.IsEnabled = isTextureFileNotEmpty && (isHorizontalChecked || isVerticalChecked || areBothChecked);
        }

        private void MirrorTextureFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MirrorTextureUpdateGUI();
        }

        private void MirrorTextureFileDrag_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Images);
            if (filePath != null)
            {
                MirrorTextureFileTextBox.Text = filePath;
            }
        }

        private void MirrorTextureFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileUtils.OpenFileDialog(FileFilters.Images);
            if (filePath != null)
            {
                MirrorTextureFileTextBox.Text = filePath;
            }
        }

        private void MirrorTextureCkbHorizontally_Changed(object sender, RoutedEventArgs e)
        {
            MirrorTextureUpdateGUI();
        }

        private void MirrorTextureCkbVertically_Changed(object sender, RoutedEventArgs e)
        {
            MirrorTextureUpdateGUI();
        }

        private void MirrorTextureExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string pictureFilePath = MirrorTextureFileTextBox.Text;
            if (!String.IsNullOrEmpty(pictureFilePath))
            {
                if (System.IO.File.Exists(pictureFilePath))
                {
                    if (FileUtils.IsFileExtension(pictureFilePath, FileConstants.ExtensionImages))
                    {
                        ActionN64 action = ActionN64.MirrorTexture;
                        string pictureOutputPath = FileUtils.SaveFileDialogPresetFileName(action,
                            System.IO.Path.GetFileNameWithoutExtension(pictureFilePath),
                            System.IO.Path.GetDirectoryName(pictureFilePath));
                        if (!String.IsNullOrEmpty(pictureOutputPath))
                        {
                            ToolResult result = Start.ToolPictures(action, MirrorTextureGetDataDictionary(pictureFilePath, pictureOutputPath), MirrorTextureGetPictureOptions());
                            MessageBoxUtils.ShowMessageExecution(result);
                        }
                    }
                    else
                    {
                        MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotExtension + FileConstants.ExtensionImages);
                        MirrorTextureFileTextBox.Focus();
                    }
                }
                else
                {
                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotFound);
                    MirrorTextureFileTextBox.Focus();
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                MirrorTextureFileTextBox.Focus();
            }
        }

        private Dictionary<string, string> MirrorTextureGetDataDictionary(string pictureFilePath, string pictureOutputPath)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.PictureFile, pictureFilePath },
                { DictConstants.PictureOutput, pictureOutputPath },
            };
            return dict;
        }


        private PictureOptions MirrorTextureGetPictureOptions()
        {
            PictureOptions options = new PictureOptions();
            if (MirrorTextureCkbHorizontally.IsChecked == true) { options |= PictureOptions.MirrorHorizontally; }
            if (MirrorTextureCkbVertically.IsChecked == true) { options |= PictureOptions.MirrorVertically; }
            return options;
        }

        #endregion

        #region Events Texture Transparency

        private void TransparentUpdateGUI()
        {
            bool isTextureFileNotEmpty = !String.IsNullOrEmpty(TransparentTextureFileTextBox.Text);
            bool isAlphaFileNotEmpty = !String.IsNullOrEmpty(TransparentAlphaFileTextBox.Text);
            TransparentTextureExecuteButton.IsEnabled = isTextureFileNotEmpty && isAlphaFileNotEmpty;
        }

        private void TransparentTextureFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TransparentUpdateGUI();
        }

        private void TransparentTextureFileDrag_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Images);
            if (filePath != null)
            {
                TransparentTextureFileTextBox.Text = filePath;
            }
        }

        private void TransparentTextureFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileUtils.OpenFileDialog(FileFilters.Images);
            if (filePath != null)
            {
                TransparentTextureFileTextBox.Text = filePath;
            }
        }

        private void TransparentAlphaFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TransparentUpdateGUI();
        }
        
        private void TransparentAlphaFileDrag_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Images);
            if (filePath != null)
            {
                TransparentAlphaFileTextBox.Text = filePath;
            }
        }

        private void TransparentAlphaFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileUtils.OpenFileDialog(FileFilters.Images);
            if (filePath != null)
            {
                TransparentAlphaFileTextBox.Text = filePath;
            }
        }

        private void TransparentTextureExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string textureFilePath = TransparentTextureFileTextBox.Text;
            if (!String.IsNullOrEmpty(textureFilePath))
            {
                if (System.IO.File.Exists(textureFilePath))
                {
                    if (FileUtils.IsFileExtension(textureFilePath, FileConstants.ExtensionImages))
                    {
                        string alphaFilePath = TransparentAlphaFileTextBox.Text;
                        if (!String.IsNullOrEmpty(alphaFilePath))
                        {
                            if (System.IO.File.Exists(alphaFilePath))
                            {
                                if (FileUtils.IsFileExtension(alphaFilePath, FileConstants.ExtensionImages))
                                {
                                    ActionN64 action = ActionN64.MakeTransparent;
                                    string pictureOutputPath = FileUtils.SaveFileDialogPresetFileName(action,
                                        System.IO.Path.GetFileNameWithoutExtension(textureFilePath),
                                        System.IO.Path.GetDirectoryName(textureFilePath));
                                    if (!String.IsNullOrEmpty(pictureOutputPath))
                                    {
                                        ToolResult result = Start.ToolPictures(action, TransparentGetDataDictionary(alphaFilePath, textureFilePath, pictureOutputPath));
                                        MessageBoxUtils.ShowMessageExecution(result);
                                    }
                                }
                                else
                                {
                                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotExtension + FileConstants.ExtensionImages);
                                    TransparentAlphaFileTextBox.Focus();
                                }
                            }
                            else
                            {
                                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotFound);
                                TransparentAlphaFileTextBox.Focus();
                            }
                        }
                        else
                        {
                            MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                            TransparentAlphaFileTextBox.Focus();
                        }
                    }
                    else
                    {
                        MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotExtension + FileConstants.ExtensionImages);
                        TransparentTextureFileTextBox.Focus();
                    }
                }
                else
                {
                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotFound);
                    TransparentTextureFileTextBox.Focus();
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                TransparentTextureFileTextBox.Focus();
            }
        }

        private Dictionary<string, string> TransparentGetDataDictionary(string alphaFilePath, string textureFilePath, string pictureOutputPath)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.AlphaFile, alphaFilePath },
                { DictConstants.TextureFile, textureFilePath },
                { DictConstants.PictureOutput, pictureOutputPath },
            };
            return dict;
        }

        #endregion

        #region Events Merge Obj Files

        #region Events Merge Obj Files - Directory

        private void MergeObjDirectoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isNotEmpty = !String.IsNullOrEmpty(MergeObjDirectoryTextBox.Text);
            MergeObjDirectoryExecuteButton.IsEnabled = isNotEmpty;
        }

        private void MergeObjDirectoryDrag_Drop(object sender, DragEventArgs e)
        {
            string directoryPath = DragDropGetPath(e, DragDropMode.Directory);
            if (directoryPath != null)
            {
                MergeObjDirectoryTextBox.Text = directoryPath;
            }
        }
        
        private void MergeObjDirectoryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string directoryPath = FileUtils.OpenDirectoryDialog();
            if (directoryPath != null)
            {
                MergeObjDirectoryTextBox.Text = directoryPath;
            }
        }

        private void MergeObjDirectoryExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string objDirectoryPath = MergeObjDirectoryTextBox.Text;
            if (!String.IsNullOrEmpty(objDirectoryPath))
            {
                if (System.IO.Directory.Exists(objDirectoryPath))
                {
                    ActionN64 action = ActionN64.MergeObjFilesDirectory;
                    string objOutputPath = FileUtils.SaveFileDialogPreset(action, objDirectoryPath);
                    if (!String.IsNullOrEmpty(objOutputPath))
                    {
                        ToolResult result = Start.ToolObj(action, MergeObjDirectoryGetDataDictionary(objDirectoryPath, objOutputPath));
                        MessageBoxUtils.ShowMessageExecution(result);
                    }
                }
                else
                {
                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageDirectoryNotFound);
                    MergeObjDirectoryTextBox.Focus();
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                MergeObjDirectoryTextBox.Focus();
            }
        }

        private Dictionary<string, string> MergeObjDirectoryGetDataDictionary(string objDirectoryPath, string objOutputPath)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.ObjDirectory, objDirectoryPath },
                { DictConstants.ObjOutput, objOutputPath },
            };
            return dict;
        }

        #endregion

        #region Events Merge Obj Files - List
        
        private void MergeObjListUpdateGUI()
        {
            // If at least 2 file are loaded, unlock the Merge button
            MergeObjListExecuteButton.IsEnabled = MergeObjListBox.Items.Count > 1;
        }

        private void MergeObjListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If at least 1 file is selected, unlock the Remove button
            MergeObjListRemoveButton.IsEnabled = MergeObjListBox.SelectedItems.Count > 0;
        }

        private void MergeObjListDrag_Drop(object sender, DragEventArgs e)
        {
            string[] filePathList = DragDropGetPaths(e);
            MergeObjListAddFiles(filePathList);
        }
        
        private void MergeObjListAddButton_Click(object sender, RoutedEventArgs e)
        {
            string[] filePathList = FileUtils.OpenFilesDialog(FileFilters.Obj);
            MergeObjListAddFiles(filePathList);
        }

        private void MergeObjListAddFiles(string[] filePathList)
        {
            if (filePathList != null)
            {
                HashSet<string> objFileList = GetHashsetFromFileList(filePathList, mergeObjFileList);
                mergeObjFileList.Clear();
                ToolObjFileListAppend(objFileList);
                MergeObjListUpdateGUI();
            }
        }


        private void ToolObjFileListAppend(ICollection<string> objFileList)
        {
            foreach (string objFilePath in objFileList)
            {
                if (objFilePath != null)
                {
                    if (FileUtils.IsFileExtension(objFilePath, FileConstants.ExtensionObj))
                    {
                        mergeObjFileList.Add(objFilePath);
                    }
                }
            }
        }

        private void MergeObjListRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MergeObjListBox.SelectedItems.Count > 0)
            {
                HashSet<string> objFileList = new HashSet<string>(MergeObjListGetListOfSelectedObj());
                for (int i = mergeObjFileList.Count - 1; i >= 0; i--)
                {
                    if (objFileList.Contains(mergeObjFileList[i]))
                    {
                        mergeObjFileList.RemoveAt(i);
                    }
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageObjMergeNone);
            }
            MergeObjListUpdateGUI();
        }

        private List<string> MergeObjListGetListOfSelectedObj()
        {
            List<string> objList = new List<string>();
            foreach (var objFile in MergeObjListBox.SelectedItems)
            {
                if (objFile.GetType().Equals(typeof(string)))
                {
                    objList.Add((string)objFile);
                }
            }
            return objList;
        }


        private void MergeObjListExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (mergeObjFileList.Count > 1)
            {
                ActionN64 action = ActionN64.MergeObjFilesList;
                string objOutputPath = FileUtils.SaveFileDialogPreset(action, System.IO.Path.GetDirectoryName(mergeObjFileList[0]));
                if (!String.IsNullOrEmpty(objOutputPath))
                {
                    Start.objFileList.Clear();
                    Start.objFileList.AddRange(mergeObjFileList);
                    ToolResult result = Start.ToolObj(action, MergeObjListGetDataDictionary(objOutputPath));
                    MessageBoxUtils.ShowMessageExecution(result);
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageObjMergeMinimum);
            }
        }

        private Dictionary<string, string> MergeObjListGetDataDictionary(string objOutputPath)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.ObjOutput, objOutputPath },
            };
            return dict;
        }

        #endregion

        #endregion

        #region Events Negative Picture

        private void NegativePictureTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isNotEmpty = !String.IsNullOrEmpty(NegativePictureTextBox.Text);
            NegativePictureExecuteButton.IsEnabled = isNotEmpty;
        }

        private void NegativePictureDrag_Drop(object sender, DragEventArgs e)
        {
            string filePath = DragDropGetPath(e, DragDropMode.Images);
            if (filePath != null)
            {
                NegativePictureTextBox.Text = filePath;
            }
        }

        private void NegativePictureBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileUtils.OpenFileDialog(FileFilters.Images);
            if (filePath != null)
            {
                NegativePictureTextBox.Text = filePath;
            }
        }

        private void NegativePictureExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string pictureFilePath = NegativePictureTextBox.Text;
            if (!String.IsNullOrEmpty(pictureFilePath))
            {
                if (System.IO.File.Exists(pictureFilePath))
                {
                    if (FileUtils.IsFileExtension(pictureFilePath, FileConstants.ExtensionImages))
                    {
                        ActionN64 action = ActionN64.NegativePicture;
                        string pictureOutputPath = FileUtils.SaveFileDialogPresetFileName(action, 
                            System.IO.Path.GetFileNameWithoutExtension(pictureFilePath), 
                            System.IO.Path.GetDirectoryName(pictureFilePath));
                        if (!String.IsNullOrEmpty(pictureOutputPath))
                        {
                            ToolResult result = Start.ToolPictures(action, NegativePictureGetDataDictionary(pictureFilePath, pictureOutputPath));
                            MessageBoxUtils.ShowMessageExecution(result);
                        }
                    }
                    else
                    {
                        MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotExtension + FileConstants.ExtensionImages);
                        NegativePictureTextBox.Focus();
                    }
                }
                else
                {
                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFileNotFound);
                    NegativePictureTextBox.Focus();
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                NegativePictureTextBox.Focus();
            }
        }


        private Dictionary<string, string> NegativePictureGetDataDictionary(string pictureFilePath, string pictureOutputPath)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.PictureFile, pictureFilePath },
                { DictConstants.PictureOutput, pictureOutputPath },
            };
            return dict;
        }

        #endregion

        #region Events Settings

        #region Events Settings - BlackList Generic 

        private void BlackListConfigShortcutButton_Click(object sender, RoutedEventArgs e)
        {
            TabSettings.IsSelected = true;
            TabSettingsBlackListConfigs.IsSelected = true;
        }

        private void BlackListSaveConfig()
        {
            bool isSuccess = toolSettings.SaveSettings();
            MessageBoxUtils.ShowMessageSettings(isSuccess);
        }

        #endregion

        #region Events Settings - BlackList Config

        private void BlackListConfigComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BlackListConfigName.DataContext = BlackListConfigComboBox.SelectedItem;
        }

        private void BlackListConfigAddButton_Click(object sender, RoutedEventArgs e)
        {
            bool wasEmpty = toolSettings.BlackList.Count == 0;
            BlackListConfig config = BlackListConfig.GetNewConfig();
            toolSettings.BlackList.Add(config);
            BlackListConfigComboBox.SelectedItem = config;
            if (wasEmpty)
            {
                // If there was no config before adding one, we force the selection on every ComboBox
                ConfigSetSelected(config);
            }
            ConfigListUpdateGUI();
        }

        private void BlackListConfigDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = BlackListConfigComboBox.SelectedItem;
            if (selected != null)
            {
                var config = (BlackListConfig)selected;
                toolSettings.BlackList.Remove(config);
                ConfigForceSelect();
            }
            ConfigListUpdateGUI();
        }

        private void BlackListConfigSaveButton_Click(object sender, RoutedEventArgs e)
        {
            BlackListSaveConfig();
        }

        #endregion

        #region Events Settings - BlackList Textures

        private void BlackListTexturesUpdateGUI()
        {
            bool isConfigSelected = BlackListTexturesComboBox.SelectedItem != null;
            BlackListTexturesAddButton.IsEnabled = isConfigSelected;
        }

        /// <summary>
        /// Clear the list of texture of the Blacklist ListBox and insert into it all values from a given list
        /// </summary>
        /// <param name="texturesList"></param>
        private void BlackListTexturesFillTexturesList(ICollection<string> texturesList)
        {
            blackListTexturesList.Clear();
            foreach (string pictureFilePath in texturesList)
            {
                blackListTexturesList.Add(pictureFilePath);
            }
        }

        private void BlackListTexturesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = BlackListTexturesComboBox.SelectedItem;
            if (selected != null)
            {
                var config = (BlackListConfig)selected;
                BlackListTexturesFillTexturesList(config.Textures);
            }
            else
            {
                // Nothing selected, clear the list
                blackListTexturesList.Clear();
            }
            BlackListTexturesUpdateGUI();
        }

        private void BlackListTexturesDrag_Drop(object sender, DragEventArgs e)
        {
            if (BlackListTexturesComboBox.SelectedItem != null)
            {
                string[] filePathList = DragDropGetPaths(e);
                BlackListTexturesAddFiles(filePathList);
            }
        }

        private void BlackListTexturesAddButton_Click(object sender, RoutedEventArgs e)
        {
            string[] filePathList = FileUtils.OpenFilesDialog(FileFilters.Images);
            BlackListTexturesAddFiles(filePathList);
        }

        private void BlackListTexturesAddFiles(string[] filePathList)
        {
            if (filePathList != null)
            {
                var selected = BlackListTexturesComboBox.SelectedItem;
                if (selected != null)
                {
                    var config = (BlackListConfig)selected;
                    HashSet<string> pictureFileList = GetHashsetFromFileList(filePathList, config.Textures);

                    config.Textures.Clear();
                    BlackListTexturesImagesAppend(config.Textures, pictureFileList);
                    BlackListTexturesFillTexturesList(config.Textures);
                    MergeObjListUpdateGUI();
                }
            }
        }

        /// <summary>
        /// Insert all image files from textureList2 into textureList1 
        /// </summary>
        /// <param name="textureList1"></param>
        /// <param name="textureList2"></param>
        private void BlackListTexturesImagesAppend(ICollection<string> textureList1, ICollection<string> textureList2)
        {
            foreach (string pictureFilePath in textureList2)
            {
                if (pictureFilePath != null)
                {
                    if (FileUtils.IsFileExtension(pictureFilePath, FileConstants.ExtensionImages))
                    {
                        textureList1.Add(pictureFilePath);
                    }
                }
            }
        }

        private void BlackListTexturesRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (BlackListTexturesBox.SelectedItems.Count > 0)
            {
                var selected = BlackListTexturesComboBox.SelectedItem;
                if (selected != null)
                {
                    var config = (BlackListConfig)selected;
                    HashSet<string> pictureFileList = new HashSet<string>(BlackListTexturesGetListOfSelectedTextures());
                    for (int i = config.Textures.Count - 1; i >= 0; i--)
                    {
                        if (pictureFileList.Contains(config.Textures[i]))
                        {
                            config.Textures.RemoveAt(i);
                        }
                    }
                    BlackListTexturesFillTexturesList(config.Textures);
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageBlackListBoxNoSelection);
            }
        }

        private List<string> BlackListTexturesGetListOfSelectedTextures()
        {
            List<string> textureList = new List<string>();
            foreach (var textureFile in BlackListTexturesBox.SelectedItems)
            {
                if (textureFile.GetType().Equals(typeof(string)))
                {
                    textureList.Add((string)textureFile);
                }
            }
            return textureList;
        }

        private void BlackListTexturesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If at least 1 file is selected, unlock the Remove button
            BlackListTexturesRemoveButton.IsEnabled = BlackListTexturesBox.SelectedItems.Count > 0;
        }

        private void BlackListTexturesSaveButton_Click(object sender, RoutedEventArgs e)
        {
            BlackListSaveConfig();
        }

        #endregion

        #region Events Settings - BlackList Copy Textures

        private void BlackListCopyUpdateGUI()
        {
            bool isConfigSelected = BlackListCopyComboBox.SelectedItem != null;
            bool isDirectoryNotEmpty = !String.IsNullOrEmpty(BlackListCopyDirOutputTextBox.Text);
            BlackListCopyExecuteButton.IsEnabled = isConfigSelected && isDirectoryNotEmpty;
        }
        
        private void BlackListCopyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BlackListCopyUpdateGUI();
        }

        private void BlackListCopyDirOutputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlackListCopyUpdateGUI();
        }

        private void BlackListCopyDrag_Drop(object sender, DragEventArgs e)
        {
            string directoryPath = DragDropGetPath(e, DragDropMode.Directory);
            if (directoryPath != null)
            {
                BlackListCopyDirOutputTextBox.Text = directoryPath;
            }
        }

        private void BlackListCopyBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string directoryPath = FileUtils.OpenDirectoryDialog();
            if (directoryPath != null)
            {
                BlackListCopyDirOutputTextBox.Text = directoryPath;
            }
        }

        private void BlackListCopyExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string outputDirectoryPath = BlackListCopyDirOutputTextBox.Text;
            if (!String.IsNullOrEmpty(outputDirectoryPath))
            {
                if (System.IO.Directory.Exists(outputDirectoryPath))
                {
                    var selected = BlackListCopyComboBox.SelectedItem;
                    if (selected != null)
                    {
                        bool resultQuestion = MessageBoxUtils.ShowMessageYesNoQuestion(MessageBoxConstants.MessageBlackListCopyTextures);
                        if (resultQuestion)
                        {
                            var config = (BlackListConfig)selected;
                            Start.texturesList.Clear();
                            Start.texturesList.AddRange(config.Textures);
                            ActionN64 action = ActionN64.CopyBlacklistTextures;
                            ToolResult result = Start.ToolPictures(action, BlackListCopyGetDataDictionary(outputDirectoryPath));
                            MessageBoxUtils.ShowMessageExecution(result);
                        }
                    }
                    else
                    {
                        MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageBlackListConfigError);
                    }
                    
                }
                else
                {
                    MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageDirectoryNotFound);
                    BlackListCopyDirOutputTextBox.Focus();
                }
            }
            else
            {
                MessageBoxUtils.ShowMessageWarning(MessageBoxConstants.MessageFieldEmpty);
                BlackListCopyDirOutputTextBox.Focus();
            }
        }

        private Dictionary<string, string> BlackListCopyGetDataDictionary(string outputDirectoryPath)
        {
            var dict = new Dictionary<string, string>
            {
                { DictConstants.OutputDirectory, outputDirectoryPath }
            };
            return dict;
        }

        #endregion

        #endregion

    }

    public enum DragDropMode
    {
        Any,
        Wrl,
        Obj,
        Images,
        Directory
    }

    public static class MessageBoxUtils
    {

        public static bool ShowMessageYesNoQuestion(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, MessageBoxConstants.MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public static void ShowMessageInformation(string message)
        {
            MessageBox.Show(message, MessageBoxConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowMessageWarning(string message)
        {
            MessageBox.Show(message, MessageBoxConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowMessageError(string message)
        {
            MessageBox.Show(message, MessageBoxConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowMessageExecution(ToolResult result)
        {
            if (result.Success)
            {
                if (result.Warn)
                {
                    ShowMessageWarning(result.Message);
                }
                else
                {
                    ShowMessageInformation(result.Message);
                }
            }
            else
            {
                ShowMessageError(result.Message);
            }
        }

        public static void ShowMessageSettings(bool success)
        {
            if (success)
                ShowMessageInformation(MessageBoxConstants.MessageSaveSettingsOK);
            else
                ShowMessageError(MessageBoxConstants.MessageSaveSettingsFailure);
        }

    }

}
