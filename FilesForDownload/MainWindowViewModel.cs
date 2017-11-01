using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FilesForDownload
{
    public class MainWindowViewModel
    {
        #region Member variables
        private static readonly string _statusTextFileSelected = "File selected";
        private static readonly string _uploadSuccess = "File successfully uploaded";
        private static readonly string _standardDownloadPath = @"https://www.seetec.eu/xchange/";

        private static string _tbSelectedFile = string.Empty;
        private static string _lblStatus = string.Empty;
        private static string _tbDownloadURL = string.Empty;

        private static string _selectedFilePath = string.Empty;

        private static bool _isButtonUploadEnabled = false;


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
        #endregion

        #region ICommand properties
        public SelectFileCommand SelectFileCommand { get; private set; }
        public UploadCommand UploadCommand { get; private set; }
        #endregion

        public MainWindowViewModel()
        {
            SelectFileCommand = new SelectFileCommand(() => UploadFunction.SelectFile());
            UploadCommand = new UploadCommand(() => UploadFile());
        }

        public void UploadFile()
        {
            if (File.Exists(SelectedFilePath))
            {
                ProgressbarVisibility = Visibility.Visible;

                Thread t = new Thread(new ThreadStart(StartUploadingFile));
                t.Start();

                IsButtonUploadEnabled = false;
            }
            else
            {
                IsButtonUploadEnabled = false;
                TbSelectedFile = "";
                LblStatus = "File doesnt exist";
            }       
        }

        public void StartUploadingFile()
        {
            UploadFunction.UploadFile(SelectedFilePath, SelectedFileName);
        }


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

    }
}
