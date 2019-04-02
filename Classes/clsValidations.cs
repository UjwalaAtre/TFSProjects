using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
//using IWshRuntimeLibrary;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Management;

namespace gloUpdates
{
    public class clsValidations
    {
     
        //Services Changes

        public static int CheckMachineStatus()
        {
            int _type = 0;
            string strProcArchi = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            bool Proc64running32 = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"));
            //MessageBox.Show(strProcArchi);
            if ((strProcArchi.IndexOf("64") > 0) || (!Proc64running32))
            {

                _type = 1; //64 bit machine
                //  MessageBox.Show("64bit machine");
            }
            else
            {
                _type = 0; //32 bit machine
                //MessageBox.Show("32bit machine");
            }
            return _type;
        }

        public static string GetProgramFilesPath()
        {
            int _type = CheckMachineStatus();
            string strPathtoProgramFiles = "";
            if (_type == 1) //64 bit machine
            {
                strPathtoProgramFiles = Environment.GetEnvironmentVariable("PROGRAMFILES(x86)");
            }
            else if (_type == 0) //32 bit machine
            {
                strPathtoProgramFiles = Environment.GetEnvironmentVariable("PROGRAMFILES");
            }
            return strPathtoProgramFiles;
        }
      
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
                    //oProcess.StartInfo.Verb = "runas";
                    oProcess.StartInfo.Arguments = strCommand;

                    if (!string.IsNullOrEmpty(clsGeneral.strDomainName) && !string.IsNullOrEmpty(clsGeneral.strDomainUserName) && string.IsNullOrEmpty(clsGeneral.strDomainPassword) && strsetup.Contains("msiexec"))
                    {
                        oProcess.StartInfo.Domain = clsGeneral.strDomainName;
                        oProcess.StartInfo.UserName = clsGeneral.strDomainUserName;
                        oProcess.StartInfo.Password = clsGeneral.MakeSecureString(clsGeneral.strDomainPassword);
                    }

                    oProcess.StartInfo.UseShellExecute = false;
                    oProcess.StartInfo.CreateNoWindow = true;
                    //oProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //oProcess.StartInfo.RedirectStandardOutput = true;
                    oProcess.Start();
                    do
                    {
                        // oProcess.WaitForExit(1000);
                        oProcess.Refresh();
                        Application.DoEvents();
                    }
                    while (!oProcess.HasExited);
                    Application.DoEvents();
                    if (oProcess.ExitCode == 0 || oProcess.ExitCode == 3010)
                    {
                        Success = true;
                    }
                    else
                    {
                        clsGeneral.UpdateLog("Msi not installed successfully for file :" + strsetup + " and cmd :" + strPath + " , Exit code :" + oProcess.ExitCode);
                    }
                }
            }
            catch //(Exception ex)
            {
                // clsInstallationLogs.gloServerLog("Exception in RunExe " + ex.Message.ToString() + "");
            }
            finally
            {
                if (oProcess != null)
                {
                    oProcess.Close();
                    oProcess.Dispose();
                    oProcess = null;
                }
            }
            return Success;
        }

        private static string CreateDirectory(string path, bool IsDeleteRepace)
        {
            try
            {
                if (IsDeleteRepace)
                {
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Unable to Delete and create Directory " + path +", err "+ ex.ToString());
                path = "";
            }
            return path;
        } //CreateDirectory


        public static void WriteTextFile(string file, string text)
        {
            TextWriter tw = null;

           // CreateDirectory(Application.StartupPath + "\\Temp\\", false);

           // file = Application.StartupPath + "\\Temp\\" + file;
            file =gloGlobal.gloTSPrint.sMappedDriveFileLocation+"\\" + file;


            try
            {
                tw = new StreamWriter(file, true);
                // write a line of text (present date/time) to the file  
                tw.WriteLine(DateTime.Now);
                // write the rest of the text lines  
                tw.Write(text);
                // close the stream   
                tw.Close();

            }
            catch (Exception)
            {
            }
            finally
            {
                if (tw != null)
                {
                    tw.Dispose();
                }

            }
        }
    }
}   
