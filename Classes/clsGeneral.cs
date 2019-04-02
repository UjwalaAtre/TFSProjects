using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
//using Shell32;
using Ionic.Zip;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Security.Principal;
////using System.DirectoryServices.AccountManagement;
using System.Security;
using System.Configuration;


namespace gloUpdates
{

    public static class clsConfiguration
    {
        public static Configuration Config = null;
        public static string ServiceType = null;
        public static string ftp = "";
        public static string user = "";
        public static string pwd = "";
        public static string ftppath = "";
    }

    // The Class is mainly Static methods used through out service for different purpose like logging and most general methods.
    public static class clsGeneral
    {
        public static string gstrWebService = "Production";
             
        public static Int32 gnTimeInterval = 1;
        public static int gnAutoDownloadIntervalSetting = 1;
        public static string gstrUpdatesPath = null;
      
        public static string strFtpHostName = string.Empty;
        public static string strFtpUserId = string.Empty;
        public static string strFtpPwd = string.Empty;
        public static string strFtpFolderPath = string.Empty;

        public static bool isLoggingEnable = true;

        public static string strDomainName = null;
        public static string strDomainUserName = null;
        public static string strDomainPassword = null;

        public static string strUpdatedownloadLocation = null;

        public static bool RunExe(string strPath, string strCommand)
        {
            bool Success = false;
            string strsetup = string.Empty;
            string strWinpath = Environment.GetEnvironmentVariable("WINDIR").ToString();//gets the WIndir path
            System.Diagnostics.Process oProcess = new Process();
            if (strPath == "")
            {
                strsetup = ("" + strWinpath + "\\System32\\msiexec.exe");
            }
            else
            {
                strsetup = strPath;
            }
            try
            {
                if (oProcess != null)
                {
                    oProcess.StartInfo.FileName = strsetup;
                    oProcess.StartInfo.Arguments = strCommand;
                    oProcess.StartInfo.UseShellExecute = false;
                    oProcess.StartInfo.CreateNoWindow = true;

                    //oProcess.StartInfo.RedirectStandardOutput = true;
                    oProcess.Start();
                    if (!string.IsNullOrEmpty(strDomainName) && !string.IsNullOrEmpty(strDomainUserName) && !string.IsNullOrEmpty(strDomainPassword) && strsetup.Contains("msiexe"))
                    {
                        oProcess.StartInfo.Domain = strDomainName;
                        oProcess.StartInfo.UserName = strDomainUserName;
                        oProcess.StartInfo.Password = MakeSecureString(strDomainPassword);
                    }
                    do
                    {
                        oProcess.WaitForExit(1000);
                        oProcess.Refresh();
                        Application.DoEvents();
                    }
                    while (!oProcess.HasExited);
                    Application.DoEvents();
                    if (oProcess.ExitCode == 0 || oProcess.ExitCode == 3010)
                    {
                        Success = true;
                    }
                    //clsGeneral.UpdateLog(strCommand);
                    clsGeneral.UpdateLog(oProcess.ExitCode.ToString());
                }

            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in RunExe " + ex.Message.ToString() + "");
            }
            finally
            {
                if (oProcess != null)
                {
                    oProcess.Close();
                    oProcess.Dispose();
                }
            }
            return Success;
        } //RunExe

        public static SecureString MakeSecureString(string text)
        {
            SecureString secure = null;
            try
            {
                secure = new SecureString();

                foreach (char c in text)
                {
                    secure.AppendChar(c);
                }
            }
            catch (Exception ex)
            {
                secure = null;
               clsGeneral.UpdateLog("Error while creating secure string :" + ex.Message.ToString());

            }
            return secure;
        } //MakeSecureString

        public static void UpdateLog(String strLogMessage)
        {
            StreamWriter objFile = null;
            try
            {
                if (isLoggingEnable)
                {
                    objFile = new StreamWriter(Application.StartupPath + "\\gloUpdatesInstaller.log", true);
                    objFile.WriteLine(System.DateTime.Now.ToString() + System.DateTime.Now.Millisecond.ToString() + ": " + strLogMessage);
                    objFile.Close();
                }

            }
            catch (Exception)
            {

            }
            finally
            {
                objFile = null;
            }

        }
        public static void DeleteFiles(string dirpath)
        {
            try
            {
                System.IO.DirectoryInfo direc = new DirectoryInfo(dirpath);

                foreach (FileInfo file in direc.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in DeleteFiles() :" + ex.ToString());
            }
        }
         
        #region "Download methods"

        public static string GetZipFileName(string strLocation)
        {
            string ZipFileName = string.Empty;
            try
            {
                string[] strZipFileName = new string[100];
                char[] textdelimator = { '/' };
                strZipFileName = strLocation.Split(textdelimator);
                if (strZipFileName.Length > 0)
                {
                    for (int i = 0; i < strZipFileName.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(strZipFileName[i].ToString()))
                        {
                            if (strZipFileName[i].ToString().ToLower().EndsWith(".zip"))
                            {
                                ZipFileName = strZipFileName[i].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in GetZipFileName " + ex.Message.ToString() + "");
            }
            return ZipFileName;
        } //GetZipFileName

        public static void GetFtpCredentials()
        {
            try
            {                

                clsGeneral.strFtpHostName =clsConfiguration.ftp;
                clsGeneral.strFtpUserId  =clsConfiguration.user;
                clsGeneral.strFtpPwd  =clsConfiguration.pwd;
                clsGeneral.strFtpFolderPath = clsConfiguration.ftppath ;
             
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in GetFtpCredentials " + ex.Message.ToString() + "");
            }
        } //GetFtpCredentials

        public static void UpdateLocation(string strProductLocation)
        {
            strUpdatedownloadLocation = strProductLocation;
            clsGeneral.UpdateLog("Updated Location for Client Update: " + strProductLocation);
                       
        }

        public static string GetFileSize(long bytes)
        {
            string sValues = string.Empty;
            try
            {
                if (bytes <= 0)
                {
                    return "";
                }

                double Dfiles = ConvertBytesToMegabytes(bytes);

                if (Dfiles > 1024)
                {
                    double dMbFiles = ConvertMegabytesToGigabytes(Dfiles);

                    if (dMbFiles.ToString().Length >= 3)
                    {
                        sValues = dMbFiles.ToString().Substring(0, 3) + "GB";
                    }
                    else
                    {
                        sValues = dMbFiles.ToString() + " GB";
                    }
                }
                else
                {
                    if (Dfiles.ToString().Length >= 3)
                    {
                        sValues = Dfiles.ToString().Substring(0, 3) + " MB";
                    }
                    else
                    {
                        sValues = Dfiles.ToString() + " MB";
                    }

                }

            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in GetFileSize " + ex.Message.ToString() + "");
            }
            return sValues;
        }

        public static double ConvertBytesToMegabytes(long bytes)
        {
            try
            {
                return (bytes / 1024f) / 1024f;
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in ConvertBytesToMegabytes " + ex.Message.ToString() + "");
                return 0;
            }
        }

        public static double ConvertMegabytesToGigabytes(double megabytes) // SMALLER
        {
            // 1024 megabyte in a terabyte
            try
            {
                return megabytes / 1024.0;
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in ConvertMegabytesToGigabytes " + ex.Message.ToString() + "");
                return 0;

            }
        }

        #endregion "Download methods"
     
    } //clsGeneral
} //gloUpdates
