using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Data;
using Rebex.Net;
using Microsoft.Win32;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
////using gloUpdates.SSRS;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Net;
using System.Management;
using System.Globalization;



namespace gloUpdates
{
    public class clsInstallUpdates : IDisposable
    {
        #region "Constructor and Distructor"
        public clsInstallUpdates()
        {
            lstDownloadedUpdateId = new List<string>();
        }

        ~clsInstallUpdates()
        {
            if (lstDownloadedUpdateId != null)
            {
                lstDownloadedUpdateId = null;
            }
            Dispose(false);
        }
        #endregion "Constructor and Distructor"

        #region "variable"

        public List<string> lstDownloadedUpdateId = null;

        public Sftp _ftp = null;   

        #endregion "variable"

        #region "Function"
  
        public bool InstallgloServicesPatch(string strProductName, string strFileName, string strUpdateLocation, string strProductVersion, string strUpdateType = "", string strIsModified = "No", string strAssemblyVersion = null)
        {
            bool retStatusFlag = false;

            string strAppInstallationPath = null;
            string strFilePath = null;
            string strCommand = null;
            string strOutputlogFilePath = null;
            string strAppInstalledName = null;       
            string strServiceName = null;
            string strServiceNameWithVersion = null;
            string strServiceExeName = null;
            string strSilentCmd = null;
            string StrSwitch = null;
            string strProductNameWithVersion = null;
            string strDisplayName = null;
            
            try
            {
                clsGeneral.UpdateLog("Entered in InstallgloServicesPatch " );
                string strAppInstalltionPathWithVersion = null;
                 strDisplayName = Convert.ToString(ConfigurationManager.AppSettings["GLOCLINICALQUEUESERVICE"]).Trim();

                //Fetching installed application display Name from app.config file.
                strAppInstalledName = Convert.ToString(ConfigurationManager.AppSettings[strProductName.ToUpper()]).Trim();
            //    strAppInstalledNameWithVerison = strAppInstalledName;
                strServiceName = Convert.ToString(ConfigurationManager.AppSettings[strProductName.ToUpper() + "Name"]).Trim();
                strServiceNameWithVersion = strServiceName;
                strProductNameWithVersion = strProductName;
                strServiceExeName = Convert.ToString(ConfigurationManager.AppSettings[strProductName.ToUpper() + "EXEFILENAME"]).Trim(); ;
               // StrServiceConfigName = strServiceExeName+".Config";
                
                string lastServiceName = null;
                lastServiceName = gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.GetLastServicesName(strServiceName);

                string retCode = "NotFound";
                if (!string.IsNullOrEmpty(lastServiceName))
                {
                    retCode = gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.GetExecutingPath(lastServiceName);
                }
                if (!retCode.StartsWith("NotFound") && (File.Exists(retCode)))   
                {

                    strAppInstallationPath = retCode;
                    //Fetching gloUpdates file path location that need's to be insatlled.
                    strFilePath = GetFilePathFromFileName(strFileName, strUpdateLocation);

                    string lastVersion = "";

                    string BaseKey = (strServiceName.ToUpper() == "GLOCLINICALQUEUESERVICE") ? strServiceExeName : strServiceName;
                    strAppInstalltionPathWithVersion = gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.GetExecutingDirStriped(strAppInstallationPath, lastServiceName.Length > BaseKey.Length);
                    if ((lastServiceName.Length > BaseKey.Length))
                    {
                        lastVersion = lastServiceName.Substring(BaseKey.Length + 1);

                    }

                    if (strServiceNameWithVersion.Length > strServiceName.Length)
                    {
                        strAppInstalltionPathWithVersion = string.IsNullOrEmpty(strAssemblyVersion) ? strAppInstalltionPathWithVersion : gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.GetExecutingDirVersioned(strAppInstalltionPathWithVersion, strAssemblyVersion); //strAppInstalltionPathWithVersion+=gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.Digitiser(strAssemblyVersion);
                    }

                    ////////// setting strOutputlogFilePath
                    strOutputlogFilePath = Application.StartupPath + "\\gloServices";

                    if (!Directory.Exists(strOutputlogFilePath))
                    {
                        Directory.CreateDirectory(strOutputlogFilePath);
                    }

                    strOutputlogFilePath = strOutputlogFilePath + "\\" + Convert.ToString(ConfigurationManager.AppSettings[strProductName.ToUpper() + "INSTALLLOGFILE"]).Trim();
                    strOutputlogFilePath = strOutputlogFilePath.Replace("\\\\", "\\");

                    clsGeneral.UpdateLog("Output Path for log :" + strOutputlogFilePath);
                    ////////// setting strOutputlogFilePath

                    string lastProductVersion = (String.IsNullOrEmpty(lastVersion) ? "" : gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.Digitiser(lastVersion));

                    clsGeneral.UpdateLog("calling code clsgloServices.RemoveServiceInstances ");

                    retCode = clsgloServices.RemoveServiceInstances(lastServiceName, strAppInstallationPath, lastProductVersion);

                   
                    if (retCode.StartsWith("Success"))
                    {
                        clsGeneral.UpdateLog("Stopped and Removed Instance of service name :" + lastServiceName + " RetCode: " + retCode);

                        clsGeneral.UpdateLog("Removing from Control Panel.");
                        if (UninstallfrmControlpanel(lastServiceName))
                        {
                            Application.DoEvents();
                            //write to log..
                            // clsgloServices.RemoveInstances(ServiceExeName);
                            clsGeneral.UpdateLog(lastServiceName + " with product version " + lastProductVersion + " was Removed from Control Panel successfully");
                        }
                        else
                        {
                            clsGeneral.UpdateLog(lastServiceName + " with product version " + lastProductVersion + " was Failed to remove from Control Panel successfully");
                        }

                    }
                    else
                    {
                        clsGeneral.UpdateLog("Failed to Stop and Remove Instance of service name :" + lastServiceName + " RetCode: " + retCode);


                        if (UninstallfrmControlpanel(lastServiceName))
                        {
                            Application.DoEvents();
                            //write to log..
                            // clsgloServices.RemoveInstances(ServiceExeName);
                            clsGeneral.UpdateLog(lastServiceName + " with product version " + lastProductVersion + " was Removed from Control Panel successfully");
                        }
                        else
                        {
                            clsGeneral.UpdateLog(lastServiceName + " with product version " + lastProductVersion + " was Failed to remove from Control Panel successfully");
                        }
                    }
                   
                }
                else
                {
                    //////////Installing Service as it does not exists... setting parameters

                    clsGeneral.UpdateLog("Installing Product :" + strProductNameWithVersion + ". As product is not Installed or Installation Path does not exist. with RetCode " + retCode);
                    Application.DoEvents();
                    if (string.IsNullOrEmpty(strFilePath))
                    {
                        strFilePath = GetFilePathFromFileName(strFileName, strUpdateLocation);
                        clsGeneral.UpdateLog("Installation path set as strFilePath :" + strFilePath);
                        strAppInstalltionPathWithVersion = clsValidations.GetProgramFilesPath() + "\\gloClinicalQueueService";
                        clsGeneral.UpdateLog("Installation path set as strAppInstalltionPathWithVersion :" + strAppInstalltionPathWithVersion);
                    }
                    //////////Installing Service as it does not exists... setting parameters
                }   
                //// Service Installation starts.....
                    if (!string.IsNullOrEmpty(strFilePath))
                    {

                        //if (UninstallfrmControlpanel(strAppInstalledName , strProductVersion))
                        {                            
                             
                            if (!string.IsNullOrEmpty(strOutputlogFilePath))
                                strCommand = "/i \"" + strFilePath + "\" /qn TARGETDIR=\"" + strAppInstalltionPathWithVersion + "\" ALLUSERS=1 REINSTALLMODE=amus DISABLEADVTSHORTCUTS=1 /l* \"" + strOutputlogFilePath + "_log.txt" + "\" ";
                            else
                                strCommand = "/i \"" + strFilePath + "\" /qn TARGETDIR=\"" + strAppInstalltionPathWithVersion + "\" ALLUSERS=1 REINSTALLMODE=amus DISABLEADVTSHORTCUTS=1";

                            clsGeneral.UpdateLog("Update command for MSI:" + strCommand);

                            if (strCommand != null && strCommand.Trim() != "")
                            {
                                if (clsGeneral.RunExe("", strCommand))
                                {
                                    strSilentCmd = "\"" + strAppInstalltionPathWithVersion + "\\" + strServiceExeName + "\" -allusers";
                                    StrSwitch = "/c" + strSilentCmd.Replace("\\\\", "\\");
                                    if (clsgloServices.ExecuteServiceCmd("cmd", StrSwitch))
                                    {
                                        Application.DoEvents();
                                    }
                                    retStatusFlag = true;

                                    if (string.IsNullOrEmpty(strUpdateType))
                                    {
                                       // CreateShortCutIcon(strProductNameWithVersion.ToUpper().Trim(), strAppInstalltionPathWithVersion);
                                    }
                                    else
                                    {
                                        clsgloServices.StartService(strServiceNameWithVersion);
                                        clsGeneral.UpdateLog("Started " + strDisplayName + " successfully");                                   
                                    }
                                    clsGeneral.UpdateLog("Update for Product '" + strDisplayName + "' installed successfully.");
                                }
                                else
                                {
                                    retStatusFlag = false;
                                    clsGeneral.UpdateLog("Update for Product '" + strDisplayName + "' not installed successfully.");
                                }
                            }
                        }      
                    }
                    else
                    {
                        retStatusFlag = false;
                        clsGeneral.UpdateLog("File Path for MSI:" + strFileName + " does not exist.");
                    }             
            }
            catch (Exception ex)
            {
                retStatusFlag = false;
                clsGeneral.UpdateLog("Error while applying the msi update for product name " + strDisplayName + " : " + ex.Message.ToString());
            }
            return retStatusFlag;
        } //InstallgloServicesPatch

        public bool CheckInstanceRunning(string strMessage)
        {
            bool Success = false;
            string _messageBoxCaption = "gloUpdates Installer";
            Process[] pname = null;
            DateTime oESTTime;
            TimeZone localZone = TimeZone.CurrentTimeZone;
            try
            {                
                var strAppName = new string[] { "gloClinicalQueueService"};

                pname = Process.GetProcesses(Environment.MachineName);

                var exists = pname.Any(p => strAppName.Any(t => p.ProcessName.Contains(t)));

                //if (pname.Length == 0)
                if (!exists)
                {
                    Success = true;
                }
                else
                {                  
                    oESTTime = localZone.ToLocalTime(DateTime.Now);

                    if (clsGeneral.gnAutoDownloadIntervalSetting == 0)
                    {
                        if (Convert.ToInt32(oESTTime.Hour) == Convert.ToInt32(clsGeneral.gnTimeInterval))
                        {
                            clsGeneral.UpdateLog(strMessage);

                        }
                        else
                        {
                            MessageBox.Show(strMessage, _messageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        
                        if (CheckInstanceRunning(strMessage))
                        {
                            Success = true;
                        }
                    }
                    else
                    {
                        if (MessageBox.Show(strMessage, _messageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            
                            if (CheckInstanceRunning(strMessage))
                            {
                                Success = true;
                            }
                        }
                    }
                    //}
                    Application.DoEvents();
                    //if (MessageBox.Show(strMessage, _messageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                    //{
                    //if (pname.Length > 0)
                    //{
                    //    clsGeneral.UpdateLog("Killing Process name :" + pname[0].ProcessName);
                    //    pname[0].Kill();
                    //    Success = true;
                    //}
                    //}
                }
                //}
            }
            catch (Exception ex)
            {
                Success = false;
                clsGeneral.UpdateLog("Error while Checking instance is running or not :" + ex.Message.ToString());
            }
            finally
            {
                if (pname != null)
                {
                    pname = null;
                }
            }
            return Success;
        } //CheckInstanceRunnig

        public string GetFilePathFromFileName(string strFileName, string strSearchLocation)
        {
            string strRetValue = string.Empty;
            string[] FilePath = null;
            try
            {
                FilePath = Directory.GetFiles(strSearchLocation, strFileName, SearchOption.AllDirectories);

                if (FilePath.Length >= 1)
                {
                    strRetValue = FilePath[0];
                }
            }
            catch (Exception ex)
            {
                strRetValue = null;
                clsGeneral.UpdateLog("Error while fetching the file path from file name :" + ex.Message.ToString());
            }
            finally
            {

            }
            return strRetValue;
        } //GetFilePathFromFileName

        public bool InstallgloServicesPatch_AUS(string strProductName, string strFileName, string strUpdateLocation, string strProductVersion, string strUpdateType = "", string strIsModified = "No", string strAssemblyVersion = null)
        {
            bool retStatusFlag = false;

            string strAppInstallationPath = null;
            string strFilePath = null;
            string strCommand = null;
            string strOutputlogFilePath = null;
            string strAppInstalledName = null;
            string strServiceName = null;
            string strServiceNameWithVersion = null;
            string strServiceExeName = null;
            string strSilentCmd = null;
            string StrSwitch = null;
            string strProductNameWithVersion = null;
            string strDisplayName = null;

            try
            {
               string strAppInstalltionPathWithVersion = null;

               strDisplayName = Convert.ToString(ConfigurationManager.AppSettings["GLOCLINICALQUEUESERVICE"]).Trim();
                //Fetching installed application display Name from app.config file.
                strAppInstalledName = Convert.ToString(ConfigurationManager.AppSettings[strProductName.ToUpper()]).Trim();
                //    strAppInstalledNameWithVerison = strAppInstalledName;
                strServiceName = Convert.ToString(ConfigurationManager.AppSettings[strProductName.ToUpper() + "Name"]).Trim();
                strServiceNameWithVersion = strServiceName;
                strProductNameWithVersion = strProductName;
                strServiceExeName = Convert.ToString(ConfigurationManager.AppSettings[strProductName.ToUpper() + "EXEFILENAME"]).Trim(); ;
                

                string lastServiceName = null;
                lastServiceName = gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.GetLastServicesName(strServiceName);

                string retCode = "NotFound";
                if (!string.IsNullOrEmpty(lastServiceName))
                {
                    retCode = gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.GetExecutingPath(lastServiceName);
                }
                if (!retCode.StartsWith("NotFound") && (File.Exists(retCode)))
                {

                    strAppInstallationPath = retCode;
                    //Fetching gloUpdates file path location that need's to be insatlled.
                    strFilePath = GetFilePathFromFileName(strFileName, strUpdateLocation);

                    string lastVersion = "";

                    string BaseKey = (strServiceName.ToUpper() == "GLOCLINICALQUEUESERVICE") ? strServiceExeName : strServiceName;
                    strAppInstalltionPathWithVersion = gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.GetExecutingDirStriped(strAppInstallationPath, lastServiceName.Length > BaseKey.Length);
                    if ((lastServiceName.Length > BaseKey.Length))
                    {
                        lastVersion = lastServiceName.Substring(BaseKey.Length + 1);
                    }

                    //if (strServiceNameWithVersion.Length > strServiceName.Length)
                    //{
                        strAppInstalltionPathWithVersion = string.IsNullOrEmpty(strAssemblyVersion) ? strAppInstalltionPathWithVersion : gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.GetExecutingDirVersioned(strAppInstalltionPathWithVersion, strAssemblyVersion); //strAppInstalltionPathWithVersion+=gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.Digitiser(strAssemblyVersion);
                   // }

                        strServiceNameWithVersion = lastServiceName;

                    ////////// setting strOutputlogFilePath
                    strOutputlogFilePath = Application.StartupPath + "\\gloServices";

                    if (!Directory.Exists(strOutputlogFilePath))
                    {
                        Directory.CreateDirectory(strOutputlogFilePath);
                    }

                    strOutputlogFilePath = strOutputlogFilePath + "\\" + Convert.ToString(ConfigurationManager.AppSettings[strProductName.ToUpper() + "INSTALLLOGFILE"]).Trim();
                    strOutputlogFilePath = strOutputlogFilePath.Replace("\\\\", "\\");

                    clsGeneral.UpdateLog("Output Path for log :" + strOutputlogFilePath);
                    ////////// setting strOutputlogFilePath

                    string lastProductVersion = (String.IsNullOrEmpty(lastVersion) ? "" : gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.Digitiser(lastVersion));

                    clsGeneral.UpdateLog("calling code clsgloServices.RemoveServiceInstances ");

                    retCode = clsgloServices.RemoveServiceInstances(lastServiceName, strAppInstallationPath, lastProductVersion);


                    if (retCode.StartsWith("Success"))
                    {
                        clsGeneral.UpdateLog("Stopped and Removed Instance of service name :" + lastServiceName + " RetCode: " + retCode);

                        clsGeneral.UpdateLog("Removing from Control Panel.");
                        if (UninstallfrmControlpanel(lastServiceName))
                        {
                            Application.DoEvents();
                            //write to log..
                            // clsgloServices.RemoveInstances(ServiceExeName);
                            clsGeneral.UpdateLog(lastServiceName + " with product version " + lastProductVersion + " was Removed from Control Panel successfully");
                        }
                        else
                        {
                            clsGeneral.UpdateLog(lastServiceName + " with product version " + lastProductVersion + " was Failed to remove from Control Panel successfully");
                        }

                    }
                    else
                    {
                        clsGeneral.UpdateLog("Failed to Stop and Remove Instance of service name :" + lastServiceName + " RetCode: " + retCode);


                        if (UninstallfrmControlpanel(lastServiceName))
                        {
                            Application.DoEvents();
                            //write to log..
                            // clsgloServices.RemoveInstances(ServiceExeName);
                            clsGeneral.UpdateLog(lastServiceName + " with product version " + lastProductVersion + " was Removed from Control Panel successfully");
                        }
                        else
                        {
                            clsGeneral.UpdateLog(lastServiceName + " with product version " + lastProductVersion + " was Failed to remove from Control Panel successfully");
                        }
                    }

                }
                else
                {
                    //////////Installing Service as it does not exists... setting parameters

                    clsGeneral.UpdateLog("Installing Product :" + strProductNameWithVersion + ". As product is not Installed or Installation Path does not exist. with RetCode " + retCode);
                    Application.DoEvents();
                    if (string.IsNullOrEmpty(strFilePath))
                    {
                        strFilePath = GetFilePathFromFileName(strFileName, strUpdateLocation);
                        clsGeneral.UpdateLog("Installation path set as strFilePath :" + strFilePath);
                        strAppInstalltionPathWithVersion = clsValidations.GetProgramFilesPath() + "\\gloClinicalQueueService";
                        clsGeneral.UpdateLog("Installation path set as strAppInstalltionPathWithVersion :" + strAppInstalltionPathWithVersion);
                    }
                    //////////Installing Service as it does not exists... setting parameters
                }
                //// Service Installation starts.....
                if (!string.IsNullOrEmpty(strFilePath))
                {

                    //if (UninstallfrmControlpanel(strAppInstalledName , strProductVersion))
                    {

                        if (!string.IsNullOrEmpty(strOutputlogFilePath))
                            strCommand = "/i \"" + strFilePath + "\" /qn TARGETDIR=\"" + strAppInstalltionPathWithVersion + "\" ALLUSERS=1 REINSTALLMODE=amus DISABLEADVTSHORTCUTS=1 /l* \"" + strOutputlogFilePath + "_log.txt" + "\" ";
                        else
                            strCommand = "/i \"" + strFilePath + "\" /qn TARGETDIR=\"" + strAppInstalltionPathWithVersion + "\" ALLUSERS=1 REINSTALLMODE=amus DISABLEADVTSHORTCUTS=1";

                        clsGeneral.UpdateLog("Update command for MSI:" + strCommand);

                        if (strCommand != null && strCommand.Trim() != "")
                        {
                            if (clsGeneral.RunExe("", strCommand))
                            {
                                strSilentCmd = "\"" + strAppInstalltionPathWithVersion + "\\" + strServiceExeName + "\" -silent";
                                StrSwitch = "/c" + strSilentCmd.Replace("\\\\", "\\");
                                if (clsgloServices.ExecuteServiceCmd("cmd", StrSwitch))
                                {
                                    Application.DoEvents();
                                }
                                retStatusFlag = true;

                                if (string.IsNullOrEmpty(strUpdateType))
                                {
                                    // CreateShortCutIcon(strProductNameWithVersion.ToUpper().Trim(), strAppInstalltionPathWithVersion);
                                }
                                else
                                {
                                    clsgloServices.StartService(strServiceNameWithVersion);
                                    clsGeneral.UpdateLog("Started " + strDisplayName + " successfully");
                                }
                                clsGeneral.UpdateLog("Update for Product '" + strDisplayName + "' installed successfully.");
                            }
                            else
                            {
                                retStatusFlag = false;
                                clsGeneral.UpdateLog("Update for Product '" + strDisplayName + "' not installed successfully.");
                            }
                        }
                    }
                }
                else
                {
                    retStatusFlag = false;
                    clsGeneral.UpdateLog("File Path for MSI:" + strFileName + " does not exist.");
                }
            }
            catch (Exception ex)
            {
                retStatusFlag = false;
                clsGeneral.UpdateLog("Error while applying the msi update for product name " + strDisplayName + " : " + ex.Message.ToString());
            }
            return retStatusFlag;
        } //InstallgloServicesPatch

        public int ApplyUpdates()
        {
            int retSuccess = 0;
            DataTable dtHotFixUpdateDetails = null;
            DataSet dsHotFixUpdateDetails = null;
            string strUpdateListFilePath = string.Empty;
            string strProductName = string.Empty;
            string strProductUpdateType = string.Empty;
            string strProductUpdateRelatedto = string.Empty;
            string strUpdateFileName = string.Empty;            
            string strAlertMessage = null;
            string strProductVersion = null;
            string strIsModified = null;
            string strUpdateDownloadLocation = null;
            string strDestinationFilePath = Application.StartupPath + "\\ServiceUpdates";
            FileInfo objfileinfo = null;

            Uri UriAddress = null;
            try
            {
                strUpdateDownloadLocation = clsGeneral.strUpdatedownloadLocation;

                UriAddress = new Uri(strUpdateDownloadLocation);

                if (UriAddress.IsUnc == true)
                {
                    if (!string.IsNullOrEmpty(GetLocalFromUNCPath(strUpdateDownloadLocation)))
                    {
                        strUpdateDownloadLocation = GetLocalFromUNCPath(strUpdateDownloadLocation);
                    }
                }

                if (extractZipFolder(strUpdateDownloadLocation, strDestinationFilePath))
                {
                    // pgbInstallUpdates.Value = pgbInstallUpdates.Value + 20;
                    Application.DoEvents();
                    

                    objfileinfo = new FileInfo(strDestinationFilePath);                  

                  //  strDestinationFilePath = Path.Combine(strDestinationFilePath, objfileinfo.Name.Trim().Replace(".zip", ""));

                    if (Directory.Exists(strDestinationFilePath))
                    {
                        strUpdateListFilePath = Path.Combine(strDestinationFilePath, "UpdateDetails.xml");


                        strUpdateDownloadLocation = strDestinationFilePath;
                        strAlertMessage = "Important updates are available. Please save work and close out of application so updates can be applied.";

                        dsHotFixUpdateDetails = new DataSet();

                        dsHotFixUpdateDetails.ReadXml(strUpdateListFilePath);

                        dtHotFixUpdateDetails = dsHotFixUpdateDetails.Tables["File"];

                        if (dtHotFixUpdateDetails != null)
                        {
                            if (dtHotFixUpdateDetails.Rows.Count > 0)
                            {
                                // check if service is running
                                if (CheckInstanceRunning(strAlertMessage))
                                {
                                    if (dsHotFixUpdateDetails.Tables["gloUpdates"].Rows[0]["Version"] != null)
                                    {
                                        strProductVersion = Convert.ToString(dsHotFixUpdateDetails.Tables["gloUpdates"].Rows[0]["Version"]);
                                    }

                                    foreach (DataRow dr in dtHotFixUpdateDetails.Rows)
                                    {
                                        strProductName = Convert.ToString(dr["ProductName"]).ToUpper();
                                        strProductUpdateType = Convert.ToString(dr["ProductUpdateType"]).Trim().ToUpper();
                                        strProductUpdateRelatedto = Convert.ToString(dr["ProductUpdateRelatedTo"]).ToUpper();
                                        strUpdateFileName = Convert.ToString(dr["FileName"]).Trim();
                                        //strUpdateLocation = Convert.ToString(dr["sUpdateLocation"]).Trim();
                                        if (dtHotFixUpdateDetails.Columns.Contains("IsModified"))
                                        {
                                            strIsModified = Convert.ToString(dr["IsModified"]);
                                        }
                                        else
                                            strIsModified = "No";

                                        if (strProductUpdateRelatedto == "SERVER" || strProductUpdateRelatedto == "CLIENTSERVER")
                                        {
                                             if (strProductUpdateType == "GLOSERVICES")
                                            {
                                                 //
                                                clsGeneral.UpdateLog("Creating log file connectmappeddrive.txt");
                                                clsValidations.WriteTextFile( "connectmappeddrive.txt",System.DateTime.Now.ToString());
                                                clsGeneral.UpdateLog("Log file connectmappeddrive.txt created successfully.");
                                                //

                                                if (InstallgloServicesPatch(strProductName, strUpdateFileName, strUpdateDownloadLocation, strProductVersion, "GLOSERVICES", strIsModified))
                                                {
                                                    retSuccess = 1;
                                                    clsGeneral.UpdateLog("Updates are applied successfully.");
                                                    clsGeneral.UpdateLog("strUpdateDownloadLocation" + strUpdateDownloadLocation);
                                                    clsGeneral.UpdateLog("Deleting strUpdateDownloadLocation");

                                                    try
                                                    {
                                                        if (Directory.Exists(strUpdateDownloadLocation))
                                                        {
                                                            clsGeneral.UpdateLog("strUpdateDownloadLocation" + strUpdateDownloadLocation);
                                                            Directory.Delete(strUpdateDownloadLocation,true );
                                                            clsGeneral.UpdateLog("strUpdateDownloadLocation deleted from " + strUpdateDownloadLocation);
                                                           
                                                        }
                                                        else
                                                        {
                                                            clsGeneral.UpdateLog("strUpdateDownloadLocation does not exists." + strUpdateDownloadLocation);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        clsGeneral.UpdateLog("Exception in Deleting strUpdateDownloadLocation" + ex.ToString());
                                                    }
                                                }
                                                else
                                                {
                                                    retSuccess = 0;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            retSuccess = 1;
                                            clsGeneral.UpdateLog("Updates are only pushed for Service.");
                                        }
                                    }
                                }
                                else
                                {
                                    retSuccess = 2;
                                    clsGeneral.UpdateLog("Service is in Running state.");
                                    clsGeneral.UpdateLog("Stopping Service.");

                                }
                            }
                        }
                        else
                        {
                            retSuccess = 0;
                        }
                    }
                }
                else
                {
                    retSuccess = 0;
                    clsGeneral.UpdateLog("Updates are not installed as failed in extractZipFolder");
                }
            }
            catch (Exception ex)
            {
                retSuccess = 0;
                clsGeneral.UpdateLog("Error while applying the updates for product '" + strProductName + "' and Updated type : '" + strProductUpdateType + "'. Error Message :" + ex.Message.ToString());
            }
            finally
            {

            }
            return retSuccess;
        } //ApplyHotFix

        public bool UninstallfrmControlpanel(string strApplicationName)
        {
            bool success = false;

            string strReg = string.Empty;

            RegistryKey regInstallPath = null;
            string uninstall = string.Empty;

            try
            {
                #region "new Logic using Linq"
                if (clsValidations.CheckMachineStatus() == 0)
                {
                    strReg = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
                }
                else
                {
                    strReg = "Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
                }

                regInstallPath = Registry.LocalMachine.OpenSubKey(strReg, false);

                var strInstallationPathDetail = (from name in regInstallPath.GetSubKeyNames()
                                                 let app = new InstalledApplicationPath(regInstallPath, name)
                                                 where (
                                                        app.UninstallString != null &&
                                                        app.DisplayName != null && app.DisplayName == strApplicationName                                                         
                                                       )
                                                 select app
                                                 ).ToList();

                if (strInstallationPathDetail != null)
                {
                    if (strInstallationPathDetail.Count() > 0)
                    {
                        uninstall = strInstallationPathDetail[0].UninstallString.Trim();
                    }
                }

                if (!string.IsNullOrEmpty(uninstall))
                {
                    uninstall = uninstall.Replace("/I", "/X").Remove(0, 11) + " " + "/qn";
                    if (clsValidations.RunExe("", uninstall))
                    {
                        clsGeneral.UpdateLog("Unistall application named '" + strApplicationName + "' Successfully.");
                        success = true;
                    }
                }
                else //regisrty does not exist
                {
                    success = true;
                }
                #endregion "new Logic using Linq"
            }
            catch (Exception ex)
            {
                success = false;
                clsGeneral.UpdateLog("Error while Checking application exist in ARP :" + ex.Message.ToString());
            }
            finally
            {
                if (regInstallPath != null)
                    regInstallPath.Close();
            }
            return success;
        } //UninstallfrmControlpanel
       
        public bool extractZipFolder(string strSourceFilePath, string strDestinationFilePath)
        {
            try
            {
                clsGeneral.UpdateLog("Started : Extracting Zip folder ");
            
                Application.DoEvents();

                gloUpdates.clsUnzipDrugsFiles.ExtractZipFile(strSourceFilePath, strDestinationFilePath + "\\");

                Application.DoEvents();
                clsGeneral.UpdateLog("Completed : Extracting Zip folder @location: " + strDestinationFilePath);
               
                return true;
            }
            catch (Exception ex)
            {
               clsGeneral.UpdateLog("Error while extracting the zip folder. Error Message " + ex.Message.ToString());
                return false;
            }
        } //extractZipFolder
    
        public string GetLocalFromUNCPath(string uncPath)
        {
            string strPath = string.Empty;
            ManagementScope scope = null;
            ManagementObjectSearcher searcher = null;
            SelectQuery query = null;
            try
            {
                // remove the "\\" from the UNC path and split the path
                uncPath = uncPath.Replace(@"\\", "");
                string[] uncParts = uncPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (uncParts.Length < 2)
                    strPath = "[UNRESOLVED UNC PATH: " + uncPath + "]";
                // Get a connection to the server as found in the UNC path
                if (string.IsNullOrEmpty(strPath))
                {
                    scope = new ManagementScope(@"\\" + uncParts[0] + @"\root\cimv2");
                    // Query the server for the share name
                    query = new SelectQuery("Select * From Win32_Share Where Name = '" + uncParts[1] + "'");
                    searcher = new ManagementObjectSearcher(scope, query);

                    // Get the path

                    foreach (ManagementObject obj in searcher.Get())
                    {
                        strPath = obj["path"].ToString();
                    }

                    // Append any additional folders to the local path name
                    if (uncParts.Length > 2)
                    {
                        for (int i = 2; i < uncParts.Length; i++)
                            strPath = strPath.EndsWith(@"\") ? strPath + uncParts[i] : strPath + @"\" + uncParts[i];
                    }
                }

            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("[ERROR RESOLVING UNC PATH: " + uncPath + ": " + ex.Message + "]");

                strPath = null; ;
            }
            finally
            {
                if (searcher != null)
                {
                    searcher.Dispose();
                    searcher = null;
                }
                if (query != null)
                {
                }
                if (scope != null)
                {
                    scope = null;
                }

            }
            return strPath;
        } //GetLocalFromUNCPath

        #endregion "Function"

        public int ApplyUpdates_AUSService(string AssemblyVer)
        {
            int retSuccess = 0;
            DataTable dtHotFixUpdateDetails = null;
            DataSet dsHotFixUpdateDetails = null;
            string strUpdateListFilePath = string.Empty;
            string strProductName = string.Empty;
            string strProductUpdateType = string.Empty;
            string strProductUpdateRelatedto = string.Empty;
            string strUpdateFileName = string.Empty;
            string strAlertMessage = null;
            string strProductVersion = null;
            string strIsModified = null;
            string strUpdateDownloadLocation = null;
            string strDestinationFilePath = Application.StartupPath + "\\AUSServiceUpdates";
            FileInfo objfileinfo = null;

            Uri UriAddress = null;
            try
            {
                strUpdateDownloadLocation = clsGeneral.strUpdatedownloadLocation;

                UriAddress = new Uri(strUpdateDownloadLocation);

                if (UriAddress.IsUnc == true)
                {
                    if (!string.IsNullOrEmpty(GetLocalFromUNCPath(strUpdateDownloadLocation)))
                    {
                        strUpdateDownloadLocation = GetLocalFromUNCPath(strUpdateDownloadLocation);
                    }
                }

                if (extractZipFolder(strUpdateDownloadLocation, strDestinationFilePath))
                {
                    Application.DoEvents();

                    objfileinfo = new FileInfo(strDestinationFilePath);
                                    
                    if (Directory.Exists(strDestinationFilePath))
                    {
                        strUpdateListFilePath = Path.Combine(strDestinationFilePath, "UpdateDetails.xml");


                        strUpdateDownloadLocation = strDestinationFilePath;
                        strAlertMessage = "Important updates are available. Please save work and close out of application so updates can be applied.";

                        dsHotFixUpdateDetails = new DataSet();

                        dsHotFixUpdateDetails.ReadXml(strUpdateListFilePath);

                        dtHotFixUpdateDetails = dsHotFixUpdateDetails.Tables["File"];

                        if (dtHotFixUpdateDetails != null)
                        {
                            if (dtHotFixUpdateDetails.Rows.Count > 0)
                            {
                                // check if service is running
                                if (CheckInstanceRunning(strAlertMessage))
                                {
                                    if (dsHotFixUpdateDetails.Tables["gloUpdates"].Rows[0]["Version"] != null)
                                    {
                                        strProductVersion = Convert.ToString(dsHotFixUpdateDetails.Tables["gloUpdates"].Rows[0]["Version"]);
                                    }

                                    foreach (DataRow dr in dtHotFixUpdateDetails.Rows)
                                    {
                                        strProductName = Convert.ToString(dr["ProductName"]).ToUpper();
                                        strProductUpdateType = Convert.ToString(dr["ProductUpdateType"]).Trim().ToUpper();
                                        strProductUpdateRelatedto = Convert.ToString(dr["ProductUpdateRelatedTo"]).ToUpper();
                                        strUpdateFileName = Convert.ToString(dr["FileName"]).Trim();                                        
                                        if (dtHotFixUpdateDetails.Columns.Contains("IsModified"))
                                        {
                                            strIsModified = Convert.ToString(dr["IsModified"]);
                                        }
                                        else
                                            strIsModified = "No";

                                        if (strProductUpdateRelatedto == "SERVER" || strProductUpdateRelatedto == "CLIENTSERVER")
                                        {
                                            if (strProductUpdateType == "GLOSERVICES")
                                            {
                                                //
                                                clsGeneral.UpdateLog("Creating log file connectmappeddrive.txt");
                                                clsValidations.WriteTextFile("connectmappeddrive.txt", System.DateTime.Now.ToString());
                                                clsGeneral.UpdateLog("Log file connectmappeddrive.txt created successfully.");
                                                //

                                                if (InstallgloServicesPatch_AUS(strProductName, strUpdateFileName, strUpdateDownloadLocation, strProductVersion, "GLOSERVICES", strIsModified, AssemblyVer))
                                                {
                                                    retSuccess = 1;
                                                    clsGeneral.UpdateLog("Updates are applied successfully.");
                                                    clsGeneral.UpdateLog("strUpdateDownloadLocation" + strUpdateDownloadLocation);
                                                    clsGeneral.UpdateLog("Deleting strUpdateDownloadLocation");

                                                    try
                                                    {
                                                        if (Directory.Exists(strUpdateDownloadLocation))
                                                        {
                                                            clsGeneral.UpdateLog("strUpdateDownloadLocation" + strUpdateDownloadLocation);
                                                            Directory.Delete(strUpdateDownloadLocation, true);
                                                            clsGeneral.UpdateLog("strUpdateDownloadLocation deleted from " + strUpdateDownloadLocation);

                                                        }
                                                        else
                                                        {
                                                            clsGeneral.UpdateLog("strUpdateDownloadLocation does not exists." + strUpdateDownloadLocation);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        clsGeneral.UpdateLog("Exception in Deleting strUpdateDownloadLocation" + ex.ToString());
                                                    }
                                                }
                                                else
                                                {
                                                    retSuccess = 0;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            retSuccess = 1;
                                            clsGeneral.UpdateLog("Updates are only pushed for Service.");
                                        }
                                    }
                                }
                                else
                                {
                                    retSuccess = 2;
                                    clsGeneral.UpdateLog("Service is in Running state.");
                                    clsGeneral.UpdateLog("Stopping Service.");

                                }
                            }
                        }
                        else
                        {
                            retSuccess = 0;
                        }
                    }
                }
                else
                {
                    retSuccess = 0;
                    clsGeneral.UpdateLog("Updates are not installed as failed in extractZipFolder");
                }
            }
            catch (Exception ex)
            {
                retSuccess = 0;
                clsGeneral.UpdateLog("Error while applying the updates for product '" + strProductName + "' and Updated type : '" + strProductUpdateType + "'. Error Message :" + ex.Message.ToString());
            }
            finally
            {

            }
            return retSuccess;
        }

        public bool ISRedistributable2010Exist()
        {
            bool _result = false;
            RegistryKey oKey = null;
            try
            {
                //{5D9ED403-94DE-3BA0-B1D6-71F4BDA412E6}               
                int _type = clsValidations.CheckMachineStatus();
                if (_type == 0)
                {
                    oKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\MICROSOFT\\WINDOWS\\CurrentVersion\\Uninstall\\{196BB40D-1578-3D01-B289-BEFC77A11A1E}", RegistryKeyPermissionCheck.ReadSubTree);

                    if (oKey == null)
                    {
                        oKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\MICROSOFT\\WINDOWS\\CurrentVersion\\Uninstall\\{F0C3E5D1-1ADE-321E-8167-68EF0DE699A5}", RegistryKeyPermissionCheck.ReadSubTree);
                    }
                    if (oKey == null)
                    {
                        oKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\MICROSOFT\\WINDOWS\\CurrentVersion\\Uninstall\\{5D9ED403-94DE-3BA0-B1D6-71F4BDA412E6}", RegistryKeyPermissionCheck.ReadSubTree);
                    }
                }
                else
                {
                    oKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{196BB40D-1578-3D01-B289-BEFC77A11A1E}", RegistryKeyPermissionCheck.ReadSubTree);
                    if (oKey == null)
                    {
                        oKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{F0C3E5D1-1ADE-321E-8167-68EF0DE699A5}", RegistryKeyPermissionCheck.ReadSubTree);
                    }
                    if (oKey == null)
                    {
                        oKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{5D9ED403-94DE-3BA0-B1D6-71F4BDA412E6}", RegistryKeyPermissionCheck.ReadSubTree);
                    }                  
                }

                if (oKey != null && oKey.ToString() != "")
                {
                    _result = true;
                    clsGeneral.UpdateLog("Microsoft Redistributable Package 2010 already installed.");
                }
                else
                {
                    _result = false;
                    clsGeneral.UpdateLog("Microsoft Redistributable Package 2010 is not installed.");
                }
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in ISRedistributable2010Present() :" + ex.ToString());
            }
            finally
            {
                oKey = null;
                Registry.LocalMachine.Close();
            }
            return _result;
        }


        public bool ExecuteMSI(string strsetup, string strSwitch)
        {
            bool _success = false;
            System.Diagnostics.Process oProcess = new Process();
            try
            {
                oProcess.StartInfo.FileName = strsetup;
                oProcess.StartInfo.Arguments = strSwitch;


                //if (!string.IsNullOrEmpty(clsInstallationParameter.strDomainName) && !string.IsNullOrEmpty(clsInstallationParameter.strDomainUserName) && !string.IsNullOrEmpty(clsInstallationParameter.strDomainPassword) && strsetup.Contains("msiexec"))
                //{
                //    if (IsAdmin == true && IsValid == true)
                //    {
                //        oProcess.StartInfo.Domain = clsInstallationParameter.strDomainName;
                //        oProcess.StartInfo.UserName = clsInstallationParameter.strDomainUserName;
                //        oProcess.StartInfo.Password = clsGeneral.MakeSecureString(clsInstallationParameter.strDomainPassword);
                //    }
                //}
                oProcess.StartInfo.UseShellExecute = false;
                oProcess.StartInfo.CreateNoWindow = true;
                oProcess.StartInfo.RedirectStandardOutput = false;
                oProcess.Start();
                do
                {
                    oProcess.WaitForExit(1000);
                    oProcess.Refresh();
                    Application.DoEvents();
                }
                while (!oProcess.HasExited);
                if (oProcess.ExitCode == 0 || oProcess.ExitCode == 3010 || oProcess.ExitCode == 5100) //5100 for higher version is installed
                {
                    _success = true;
                }
                clsGeneral.UpdateLog("" + strSwitch + " exit  code :" + oProcess.ExitCode + "");

            }
            catch (System.ArgumentNullException ex)
            {
                clsGeneral.UpdateLog("Exception ArgumentNullException: While Executing MSI " + strSwitch + " " + ex.Message.ToString() + "");
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception : While Executing MSI " + strSwitch + " " + ex.Message.ToString() + "");
            }
            finally
            {
                if (oProcess != null)
                {
                    oProcess.Close();
                    oProcess.Dispose();
                }
            }
            return _success;
        }

        #region "Dispose Method"

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {

                }
            }
            disposed = true;
        }

        #endregion "Dispose Method"

    } //clsInstallUpdates

    public class InstalledApplicationPath
    {
        public InstalledApplicationPath(RegistryKey uninstallKey, string keyName)
        {
            RegistryKey key = uninstallKey.OpenSubKey(keyName, false);
            try
            {
                var strdisplayName = key.GetValue("DisplayName");
                if (strdisplayName != null)
                    DisplayName = strdisplayName.ToString();

                var strInstallLocation = key.GetValue("InstallLocation");
                if (strInstallLocation != null)
                    InstallPath = strInstallLocation.ToString();

                var strUnistallString = key.GetValue("UninstallString");
                if (strUnistallString != null)
                    UninstallString = strUnistallString.ToString();

                var strDisplayVersionString = key.GetValue("DisplayVersion");
                if (strDisplayVersionString != null)
                    DisplayVersion = strDisplayVersionString.ToString();
            }
            finally
            {
                key.Close();
            }
        } //InstalledApplicationPath

        public string DisplayName { get; set; }

        public string InstallPath { get; set; }

        public string UninstallString { get; set; }

        public string DisplayVersion { get; set; }

    } //InstalledApplicationPath

} //gloUpdates 
