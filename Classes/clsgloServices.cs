using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using System.Xml;
//using System.Security.Cryptography;
using System.Web;
using System.IO;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Diagnostics;
using Microsoft.Win32; 


namespace gloUpdates
{
    class clsgloServices
    {
        static string strPathtoProgramFiles = GetProgramFilesPath();
        public static string strPathtoClinicalService = "" + strPathtoProgramFiles + "\\gloClinicalServiceSetup";

        public const string strClinicalServiceProductCode = "{EAFA4EB0-2208-4097-A1DE-4E9EA0005017}";
        
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

        public static string RemoveServiceInstances(string AppName, string ExePath, string ProductCode, string strDomainName = "", string strDomainUser = "", string strDomainPassword = "")
        {
         
            try
            {
                return gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.StopAndRemoveServiceInTrayIcon(AppName, ExePath, ProductCode, strDomainName, strDomainUser, strDomainPassword);
               
            }
            catch (Exception Ex)
            {
                // MessageBox.Show(ex.Message);
                return "Failure: " + Ex.ToString();
            }
            
        }
        public static bool ExecuteServiceCmd(string strPath, string strCommand)
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
                    // oProcess.WaitForExit(50000);
                    if (oProcess.HasExited)
                    {
                        Success = true;
                    }
                }

            }
            catch (Exception)
            {
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
        }

        public static void StartService(string ServiceName)
        {
            //try
            //{
            //    ServiceController[] scServices;
            //    scServices = ServiceController.GetServices();
            //    int counter = 0;
            //    foreach (ServiceController scTemp in scServices)
            //    {
            //        //if (scTemp.Status == ServiceControllerStatus.Stopped)
            //        //{
            //        //System.Windows.Forms.MessageBox.Show(scTemp.ServiceName.ToLower(), "");
            //        if (ServiceName.ToUpper() != "GLOFUSION MANAGER")
            //        {
            //            if (scTemp.ServiceName.ToLower() == ServiceName.ToString().ToLower())
            //            {
            //                counter++;
            //                scTemp.Start();
            //            }
            //        }
            //        else
            //        {
            //            if (scTemp.ServiceName.ToLower() == strServiceExeName.ToLower())
            //            {
            //                counter++;
            //                scTemp.Start();
            //            }
            //        }
            //        //}
            //    }
            //}
            //catch (Exception ex)
            //{
            //    clsGeneral.UpdateLog("Error while starting the service name :'" + ServiceName + "' or '" + strServiceExeName + "'. Error Message : " + ex.Message.ToString());
            //}
            //finally
            //{
            //}
            try
            {
                gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.StartService(ServiceName);
                clsGeneral.UpdateLog("Success while starting the service name :'" + ServiceName );
            }
            catch (Exception Ex) 
            {
                clsGeneral.UpdateLog("Error while starting the service name :'" + ServiceName +  "'. Error Message : " + Ex.Message.ToString());
            }
            return;

        }
        public static bool SetAUSConfigValues(string filepath, string sAUSProductionURL, string sAUSStagingURL)
        {
            bool result = false;
            try
            {
                if (File.Exists(filepath))
                {
                    XDocument xmlDoc = XDocument.Load(filepath);

                    XElement elementProduction = xmlDoc.Root.Descendants("gloWinService.Properties.Settings")
                             .Elements("setting")
                             .Single(x => x.Attribute("name").Value == "gloWinService_net_ophit_www_gloAUSService");
                    elementProduction.Value = string.Empty;

                    elementProduction.SetElementValue("value", sAUSProductionURL);


                    XElement elementStaging = xmlDoc.Root.Descendants("gloWinService.Properties.Settings")
                            .Elements("setting")
                            .Single(x => x.Attribute("name").Value == "gloWinService_gloAUSTest_gloAUSService");
                    elementStaging.Value = string.Empty;

                    elementStaging.SetElementValue("value", sAUSStagingURL);

                    xmlDoc.Save(filepath);

                    elementProduction = null;
                    elementStaging = null;
                    result = true;
                }
                else
                {
                    result = false;
                    clsGeneral.UpdateLog("Config file not found in gloUpdate Manager service." );
                }
            }
            catch (Exception ex)
            {
                result = false;
                clsGeneral.UpdateLog("Error while starting the service name : gloUpdate Manager. Error Message : " + ex.Message.ToString());
            }
            return result;
        }
        public static void StopService(string ServiceName)
        {
            try
            {
                gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.StopService(ServiceName);
                clsGeneral.UpdateLog("Success while stoping the service name :'" + ServiceName);
            }
            catch (Exception Ex)
            {
                clsGeneral.UpdateLog("Error while stoping the service name :'" + ServiceName + "'. Error Message : " + Ex.Message.ToString());
            }
            return;
        }

        public static object GetRegistryValue(string _value, string Name)
        {
            object value = null;
            RegistryKey oKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Installer\\UserData\\S-1-5-18\\Products\\" + Name + "\\InstallProperties", true);
            try
            {
                if (oKey != null && oKey.ToString() != "")
                {
                    value = oKey.GetValue(_value);
                }
            }

            catch (System.NullReferenceException)
            {
            }
            catch (System.ArgumentOutOfRangeException)
            {
            }
            finally
            {
                if (oKey != null)
                    oKey.Close();
            }
            return value;
        }
    }
}
