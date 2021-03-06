﻿using GongSolutions.Wpf.DragDrop;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;


namespace XUpload
{
    public class MainWindowViewModel : IDropTarget, INotifyPropertyChanged
    {
        #region Member variables
        private static readonly string _statusTextFileSelected = "File Selected";
        private static readonly string _uploadSuccess = "File Successfully Uploaded";
        private static readonly string _standardDownloadPath = @"https://downloads.seetec-video.com/xchange/";
        private static readonly string _ftpServerAdress = @"ftp://downloads.seetec-video.com/";
        private static readonly string _ftpUsername = "xchange";
        private static readonly string _ftpPassword = "hT34du9?";

        private static string _tbSelectedFile = string.Empty;
        private static string _lblStatus = string.Empty;
        private static string _tbDownloadURL = string.Empty;

        private static string _selectedFilePath = string.Empty;

        private static bool _isButtonUploadEnabled = false;
        private static bool _isUploadSucceeded = false;

        private static bool _isSelectedText = false;

        private static string _assemblyVersion = string.Empty;

        private static bool _isUploadRunning = false;


        private static Visibility _progressbarVisibility = Visibility.Hidden;

        // Just a initial value for the ProgressBarMaximum. We could also set it to 1. 
        private static long _progressBarMaximum = 100;
        private static long _progressBarValue = 0;
        #endregion

        #region Property Members
        public static string SelectedFileName { get; set; }
        public static string StatusTextFileSelected
        {
            get { return _statusTextFileSelected; }
        }
        public static string UploadSuccess
        {
            get { return _uploadSuccess; }
        }
        public static string TbSelectedFile
        {
            get { return _tbSelectedFile; }
            set
            {
                _tbSelectedFile = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static string StandardDownloadPath
        {
            get { return _standardDownloadPath; }
        }

        public static string FtpServerAdress
        {
            get { return _ftpServerAdress; }
        }

        public static string FtpUsername
        {
            get { return _ftpUsername; }
        }

        public static string FtpPassword
        {
            get { return _ftpPassword; }
        }

        public static string LblStatus
        {
            get { return _lblStatus; }
            set
            {
                _lblStatus = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static string TbDownloadURL
        {
            get { return _tbDownloadURL; }
            set
            {
                _tbDownloadURL = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static string SelectedFilePath
        {
            get { return _selectedFilePath; }
            set { _selectedFilePath = value; }
        }

        public static bool IsButtonUploadEnabled
        {
            get { return _isButtonUploadEnabled; }
            set
            {
                // The upload button is only enabled if a file is selected
                if (TbSelectedFile != "")
                {
                    _isButtonUploadEnabled = value;
                }
                else
                {
                    _isButtonUploadEnabled = false;
                }

                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsUploadSucceeded
        {
            get { return _isUploadSucceeded; }
            set { _isUploadSucceeded = value; }
        }


        public static Visibility ProgressbarVisibility
        {
            get { return _progressbarVisibility; }
            set
            {
                _progressbarVisibility = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static long ProgressBarMaximum
        {
            get { return _progressBarMaximum; }
            set
            {
                _progressBarMaximum = value;
                RaiseStaticPropertyChanged();
            }
        }
        public static long ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsSelectedText
        {
            get { return _isSelectedText; }
            set
            {
                _isSelectedText = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static string AssemblyVersion
        {
            get { return _assemblyVersion; }
            set
            {
                _assemblyVersion = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsUploadRunning
        {
            get { return _isUploadRunning; }
            set { _isUploadRunning = value; }
        }

        #endregion

        #region ICommand properties

        /// <summary>
        /// Simple property to hold the 'SelectFileCommand' - when executed
        /// it will open the "open file dialog class" to select a file for the upload
        /// </summary>
        public SelectFileCommand SelectFileCommand { get; private set; }

        /// <summary>
        /// Simple property to hold the 'UploadCommand' - when executed
        /// it will start the upload to the ftp server
        /// </summary>
        public UploadCommand UploadCommand { get; private set; }

        /// <summary>
        /// Simple property to hold the 'CopyURLCommand' - when executed
        /// it will copy the download URL to the clipboard
        /// </summary>
        public CopyURLCommand CopyURLCommand { get; private set; }
        #endregion

        public MainWindowViewModel()
        {
            SelectFileCommand = new SelectFileCommand(() => UploadFunction.SelectFile());
            UploadCommand = new UploadCommand(() => UploadFile());
            CopyURLCommand = new CopyURLCommand(() => UploadFunction.CopyURLToClipboard());

            // Get the current Assembly version to display it on the main window
            AssemblyVersion = "" + GetRunningVersion();
        }

        /// <summary>
        /// Method which starts when the upload button is clicked.
        /// It does preparations and starts the upload with an other thread
        /// </summary>
        public void UploadFile()
        {
            if (File.Exists(SelectedFilePath))
            {
                ProgressbarVisibility = Visibility.Visible;

                // Start upload in a new thread
                Thread t = new Thread(new ThreadStart(StartUploadingFile));
                t.IsBackground = true; // Thread will be closed when the application exits 

                IsUploadRunning = true;

                t.Start();

                IsButtonUploadEnabled = false;
            }
            else
            {
                IsButtonUploadEnabled = false;
                TbSelectedFile = "";
                LblStatus = "File Doesnt Exist";
            }
        }

        /// <summary>
        /// Method to upload the selected file
        /// </summary>
        public void StartUploadingFile()
        {
            LblStatus = "Upload Started. Please Wait...";
            UploadFunction.UploadFile(SelectedFilePath, SelectedFileName);
        }

        /// <summary>
        /// Method to get the current Assembly information
        /// </summary>
        private string GetRunningVersion()
        {
            try
            {
                String s = "v" + Assembly.GetExecutingAssembly().GetName().Version;
                return s.Substring(0, 6);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "xxx";
            }

        }

        #region Close application members
        /// <summary>
        /// This method is calles when the window Close button is clicked, or if the user presses ALT+F4.
        /// Check if an upload process is active and ask the user if he really wants to quit the application.
        /// </summary>
        public static void OnProcessExit(object sender, CancelEventArgs e)
        {
            if (IsUploadRunning)
            {
                MessageBoxResult result = MessageBox.Show("Upload process is running. Do you really want to exit?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
        #endregion

        #region Notify static property members changed      
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        // This method is called by the set accessor of static properties.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        public static void RaiseStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDropTarget members

        /// <summary>
        /// Method for textbox selected file drag over
        /// </summary>
        public void DragOver(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null && extension != "";
            }) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        /// <summary>
        /// Method for textbox selected file drop
        /// </summary>
        public void Drop(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);

                // Update custom file textbox UI
                TbDownloadURL = "";
                SelectedFilePath = item;
                SelectedFileName = Path.GetFileName(item);
                TbSelectedFile = "  " + Path.GetFileName(item);
                LblStatus = StatusTextFileSelected;
                IsButtonUploadEnabled = true;
                CommandManager.InvalidateRequerySuggested();

                return extension != null && extension != "";
            }) ? DragDropEffects.Copy : DragDropEffects.None;
        }
        #endregion

    }
}
