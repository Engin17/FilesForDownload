using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Windows;


namespace XUpload
{
    public class UploadFunction
    {
        private static string uploadEscapedFileName = string.Empty;

        /// <summary>
        /// Method to select file for upload
        /// </summary>
        public static void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // If file selected
            if (openFileDialog.ShowDialog() == true)
            {
                MainWindowViewModel.TbDownloadURL = "";

                MainWindowViewModel.SelectedFilePath = openFileDialog.FileName;

                MainWindowViewModel.SelectedFileName = Path.GetFileName(openFileDialog.FileName);

                MainWindowViewModel.TbSelectedFile = "  " + Path.GetFileName(openFileDialog.FileName);

                MainWindowViewModel.LblStatus = MainWindowViewModel.StatusTextFileSelected;

                MainWindowViewModel.IsButtonUploadEnabled = true;
            }

        }

        /// <summary>
        /// Method to upload files to the FTP server
        /// </summary>
        public static void UploadFile(string filePath, string fileName)
        {

            FileStream fs = null;
            Stream rs = null;
   
            uploadEscapedFileName = string.Empty;

            try
            {
                FileInfo fileInf = new FileInfo(filePath);
                string file = filePath;
                string uploadFileName = new FileInfo(file).Name;

                //Converts a string to its escaped representation. Some characters within the filename causes problems
                uploadEscapedFileName = Uri.EscapeDataString(uploadFileName);

                string uploadUrl = MainWindowViewModel.FtpServerAdress;
                fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                string ftpUrl = string.Format("{0}/{1}", uploadUrl, uploadEscapedFileName);

                FtpWebRequest requestObj = FtpWebRequest.Create(ftpUrl) as FtpWebRequest;
                requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                requestObj.Credentials = new NetworkCredential(MainWindowViewModel.FtpUsername, MainWindowViewModel.FtpPassword);
                rs = requestObj.GetRequestStream();

                byte[] buffer = new byte[8192];
                int read = fs.Read(buffer, 0, buffer.Length);

                long transferred = 0;
                MainWindowViewModel.ProgressBarMaximum = fileInf.Length;

                while (read > 0)
                {
                    rs.Write(buffer, 0, read);

                    transferred += read;
                    MainWindowViewModel.ProgressBarValue = transferred;

                    read = fs.Read(buffer, 0, buffer.Length);
                }
                rs.Flush();

                MainWindowViewModel.IsUploadSucceeded = true;
            }
            catch (Exception ex)
            {
                MainWindowViewModel.ProgressbarVisibility = Visibility.Hidden;
                MainWindowViewModel.LblStatus = ex.Message;
                MainWindowViewModel.IsUploadSucceeded = false;
                MainWindowViewModel.IsUploadRunning = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

                if (rs != null)
                {
                    rs.Close();
                    rs.Dispose();
                }
            }

            if (MainWindowViewModel.IsUploadSucceeded)
            {
                MainWindowViewModel.ProgressbarVisibility = Visibility.Hidden;
                MainWindowViewModel.LblStatus = MainWindowViewModel.UploadSuccess;
                MainWindowViewModel.TbDownloadURL = MainWindowViewModel.StandardDownloadPath + uploadEscapedFileName;
                MainWindowViewModel.IsSelectedText = true;
                MainWindowViewModel.IsUploadRunning = false;
            }

            MainWindowViewModel.TbSelectedFile = "";
        }

        /// <summary>
        /// Method to copy the download URL to the Clipboard
        /// </summary>
        public static void CopyURLToClipboard()
        {
            Clipboard.SetText(MainWindowViewModel.StandardDownloadPath + uploadEscapedFileName);
        }


    }
}
