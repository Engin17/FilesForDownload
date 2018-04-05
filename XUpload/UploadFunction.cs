using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XUpload
{
    public class UploadFunction
    {
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

            try
            {
                FileInfo fileInf = new FileInfo(filePath);
                string file = filePath;
                string uploadFileName = new FileInfo(file).Name;

                string uploadUrl = "ftp://downloads.seetec-video.com/";
                fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                string ftpUrl = string.Format("{0}/{1}", uploadUrl, uploadFileName);
                FtpWebRequest requestObj = FtpWebRequest.Create(ftpUrl) as FtpWebRequest;
                requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                requestObj.Credentials = new NetworkCredential("ftp12734510-xchange", "pho7tuSh");
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
                MainWindowViewModel.TbDownloadURL = MainWindowViewModel.StandardDownloadPath + fileName;
                MainWindowViewModel.IsSelectedText = true;                
            }

            MainWindowViewModel.TbSelectedFile = "";
        }

        /// <summary>
        /// Method to copy the download URL to the Clipboard
        /// </summary>
        public static void CopyURLToClipboard()
        {
            Clipboard.SetText(MainWindowViewModel.StandardDownloadPath + MainWindowViewModel.SelectedFileName);
        }


    }
}
