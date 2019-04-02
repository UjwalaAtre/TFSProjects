using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Rebex.Net;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Net;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Net.Sockets;

namespace gloUpdates
{
    public partial class FrmUpdates : Form
    {

        #region "variable"
         
        public static String myServiceName = "gloClinicalQueueService";

        public List<string> lstDownloadedUpdateId = null;

        static long FtpTotalBytes = 0;
        static string sBytesTotal = "";
        public static Sftp _ftp = null;
   
        public long _totalByte = 0;

        public DataTable dtUpdateDetails = null;

        public static bool isUpdateDownloaded = false;

        public static int isUpdateInstalled = 0;

        public string strLocalActionType = null;

        public static string strGlobalUpdateType = null;
        public static string strComputerName = null;
        public static string strComputerIP = null;
        public static string strAUSID = null;
        public static string AUSServicePortalURL = null;
        public static string strLoginName = null;

        public static string sDownloadFileName = null;

        public static string sDownloadPathName = null;

        public static bool isUpdateExist = false;
        
        public static bool isUpdateCancelledbyUser = false;

        public static gloGlobal.clsDatalog MappedDrvLog = new gloGlobal.clsDatalog("gloUpdates");

        private static string strAssemblyVer = "";
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "";
        }
        #endregion "variable"

        private static gloGlobal.CommonApplicationData oCAD = new gloGlobal.CommonApplicationData("TriarqHealth", "gloUpdater", true);
        private static System.Configuration.Configuration globalConfig = null; 

        public FrmUpdates(Boolean IsFromService = false)
        {
            InitializeComponent();

            strGlobalUpdateType = "GLOLDFSNIFFERSERVICEUPDATES"; //"GLOCLINICALSERVICEUPDATES";
            clsGeneral.gstrUpdatesPath = Application.StartupPath;
            strComputerName = System.Windows.Forms.SystemInformation.ComputerName;
            strLoginName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            strComputerIP = GetLocalIPAddress();
            Configuration clinicConfig = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //strAUSID = Convert.ToString(clinicConfig.AppSettings.Settings["AUSID"].Value);
            AUSServicePortalURL = Convert.ToString(clinicConfig.AppSettings.Settings["AUSWEBServiceURL"].Value);
            clinicConfig = null;

            getUpdaterConfig();

            if (globalConfig != null)
            {
                KeyValueConfigurationElement keyValue = globalConfig.AppSettings.Settings["AUSID"];
                if (keyValue != null)
                {
                    strAUSID = keyValue.Value;
                }
            }
            else
            {
                strAUSID = "";
            }

            if (string.IsNullOrEmpty(strAUSID))
            {
                pnlAUSID.Visible = true;
                pnlInstallProgress.Visible = false;
            }
            else
            {
                pnlAUSID.Visible = false;
                pnlInstallProgress.Visible = true;
            }
        } 

        private void FrmUpdates_Load(object sender, EventArgs e)
        {
            
        }
        private static void getUpdaterConfig()
        {
            if (globalConfig == null)
            {
                string ConfigName = "gloUpdater.config";
                ExeConfigurationFileMap objfile = new ExeConfigurationFileMap();
                objfile.ExeConfigFilename = Path.Combine(oCAD.ConfigFolderPath, ConfigName);
                globalConfig = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(objfile, ConfigurationUserLevel.None);
                objfile = null;
            }
        }

        private bool UpdateAUSID(string AUSID)
        {
            bool result = false;
         //   Configuration clinicConfig = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
           
            try
            {
               
                if (string.IsNullOrEmpty(strAUSID))
                {
                    using (gloAUSWebService.gloAUSService ausWEBService = new gloAUSWebService.gloAUSService())
                    {
                        ausWEBService.Credentials = System.Net.CredentialCache.DefaultCredentials;
                        ausWEBService.Url = AUSServicePortalURL;
                        string _Key = ausWEBService.Login("sarika@ophit.net", "spX12ss@!!21nasik");
                        if (ausWEBService.CheckAusIDExist(AUSID, _Key))
                        {
                            globalConfig.AppSettings.Settings.Add("AUSID", AUSID);
                            globalConfig.Save(ConfigurationSaveMode.Modified);
                            clsGeneral.UpdateLog("Updating updated AUS ID - completed, value: " + AUSID);
                            strAUSID = AUSID;
                            AddServiceAuditLog("New AUSID Set","AUSID Set","All","");                          
                            result = true;
                        }
                        else
                        {
                            MessageBox.Show("Enter valid AUS ID. ", "gloUpdates Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Error in UpdateAUSID : " + ex.ToString());
            }
            finally
            {
              //  clinicConfig = null;
            }
            return result;
        }

        private static void AddServiceAuditLog(string sDescription, string status, string servicename, string sUpdateVersion)
        {
            try
            {
                using (gloAUSWebService.gloAUSService ausWEBService = new gloAUSWebService.gloAUSService())
                {
                    ausWEBService.Url = AUSServicePortalURL;
                    string _Key = ausWEBService.Login("sarika@ophit.net", "spX12ss@!!21nasik");
                    ausWEBService.InsertgloServicesAuditLog(strAUSID, sDescription, strComputerIP, strComputerName, strLoginName, status, servicename, sUpdateVersion, DateTime.Now, _Key);
                }
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Error in AddServiceAuditLog : " + ex.ToString());
            }
        }
        private void FrmUpdates_Shown(Object sender, EventArgs e)
        {
            try
            {
               
                bool IsAusUpdate = false;
              
                if (!string.IsNullOrEmpty(strAUSID))
                {
                    Configuration ausconfig = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    IsAusUpdate = Convert.ToBoolean(ausconfig.AppSettings.Settings["ISAUSUpdate"].Value);

                    if (IsAusUpdate)
                    {
                        lblProgress.Text = "Updating gloUpdates Manager Service. This may take few minutes.";
                        clsGeneral.UpdateLog("Updating AUS Config File.");                        
                        GetAUSServiceUpdate();                       
                    }
                    else
                    {
                        getUpdates();
                    }
                    globalConfig = null;
                }
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Error in FrmUpdates_Shown : " + ex.ToString());
                this.Close();
                Application.Exit();
            }
        }



        private void GetAUSServiceUpdate()
        {
            Configuration ausConfig = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);            
            string retCode = "";
            string lastServiceName = null;
            string sUpdatelogLocation = null;
            string strServiceName = null;
            string sServiceUpdateVersion = null ;
            setPanels("start", true);          
            string passwrd = null;

            //AddServiceAuditLog("gloUpdates Manager Installation Started."); 

            try
            {
                sServiceUpdateVersion = string.Empty;//Convert.ToString(ausConfig.AppSettings.Settings["gloAUSServiceUpdatesVersion"].Value); 
                strServiceName = Convert.ToString(ausConfig.AppSettings.Settings["GLOAUSSERVICENAME"].Value);
                lastServiceName = gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.GetLastServicesName(strServiceName);
                //string sProductionURL = Convert.ToString(ausConfig.AppSettings.Settings["strAUSProductionURL"].Value);
                //string sStagingURL = Convert.ToString(ausConfig.AppSettings.Settings["strAUSStagingURL"].Value);

                AddServiceAuditLog("gloUpdates Manager Installation Started.", "Started", strServiceName, sServiceUpdateVersion);

                lblProgress.Text = "Updating " + strServiceName + " Service. This may take few minutes.";
                lblFailed.Text = strServiceName + "  Service Failed to Update." + Environment.NewLine + "Try Again or Contact System Administrator.";
                lblSuccess.Text = strServiceName + "  Service Successfully Updated.";

                if (!string.IsNullOrEmpty(lastServiceName))
                {
                    string[] ver = lastServiceName.Split('_');
                    strAssemblyVer = ver[1].ToString().Replace(".", "");
                    sServiceUpdateVersion = Convert.ToString(ausConfig.AppSettings.Settings["gloAUSServiceUpdatesVersion"].Value) + strAssemblyVer + ".zip";
                    retCode = gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.GetExecutingPath(lastServiceName);
                }
                else
                {
                    clsGeneral.UpdateLog(strServiceName + "  service not found or not running mode.");
                    return;
                }

                if (!retCode.StartsWith("NotFound") && (File.Exists(retCode)))
                {

                    clsGeneral.gstrWebService = ausConfig.AppSettings.Settings["FTPConnection"].Value.ToString().ToUpper();

                    //

                    if (clsGeneral.gstrWebService.ToUpper().Trim() == "TEST")
                    {
                        clsConfiguration.ftp = ausConfig.AppSettings.Settings["sftp"].Value.ToString();
                        clsConfiguration.user = ausConfig.AppSettings.Settings["suser"].Value.ToString();
                        passwrd = ausConfig.AppSettings.Settings["spwd"].Value.ToString();
                        clsConfiguration.ftppath = ausConfig.AppSettings.Settings["strStagingFTPAUSUpdatesDownloadPath"].Value.ToString().ToUpper() + strAssemblyVer + "//";
                    }
                    else
                    {
                        clsConfiguration.ftp = ausConfig.AppSettings.Settings["pftp"].Value.ToString();
                        clsConfiguration.user = ausConfig.AppSettings.Settings["puser"].Value.ToString();
                        passwrd = ausConfig.AppSettings.Settings["ppwd"].Value.ToString();
                        clsConfiguration.ftppath = ausConfig.AppSettings.Settings["strProductionFTPAUSUpdatesDownloadPath"].Value.ToString().ToUpper() + strAssemblyVer + "//";
                    }

                    clsConfiguration.ftp = clsEncryption.DecryptFromBase64String(clsConfiguration.ftp, "12345678");
                    clsConfiguration.user = clsEncryption.DecryptFromBase64String(clsConfiguration.user, "12345678");
                    clsConfiguration.pwd = clsEncryption.DecryptFromBase64String(passwrd, "12345678");
                    //   
                    clsGeneral.UpdateLog("Service is Pointed to the server : " + clsGeneral.gstrWebService.ToString());

                    sUpdatelogLocation = MappedDrvLog.GetLogPath();

                    gloGlobal.gloTSPrint.sMappedDriveFileLocation = sUpdatelogLocation;
                    clsGeneral.UpdateLog("MappedDrvLog log location set: " + sUpdatelogLocation);

                    //Staging or Production - set values
                    if (DownloadUpdates_AUSService(sServiceUpdateVersion))
                    {
                        isUpdateDownloaded = true;
                        clsGeneral.UpdateLog("Updates are available and downloaded for Service.");
                    }
                    else
                    {
                        if (FrmUpdates.isUpdateExist == true)
                        {
                            setPanels("Exist");
                            clsGeneral.UpdateLog("Updates already Exists......isUpdateExist: " + FrmUpdates.isUpdateExist.ToString());
                        }

                        isUpdateDownloaded = false;
                    }
                    if (isUpdateDownloaded)
                    {
                        clsGeneral.UpdateLog("Starting - Installing Automatic Updates");

                        Application.DoEvents();

                        if (InstallUpdates_AUSService())
                        {
                            isUpdateInstalled = 1;


                            setPanels("Success");

                            clsGeneral.UpdateLog("gloUpdates Done !!");
                            string[] sUpdatedFileName = sDownloadFileName.Split('/');

                            ////////// deleting zip file
                            string sZipfile = null;
                            sZipfile = clsGeneral.strUpdatedownloadLocation;
                            clsGeneral.UpdateLog("Downloaded zip file " + sZipfile);
                            if (File.Exists(sZipfile))
                            {
                                File.Delete(sZipfile);
                                clsGeneral.UpdateLog("Zip file deleted - " + sZipfile);
                            }
                            ////////// deleting zip file

                            sUpdatedFileName = null;

                        }
                        else
                        {

                            if (FrmUpdates.isUpdateExist == true)
                            {
                                setPanels("Exist");
                                clsGeneral.UpdateLog("Updates already Exists......isUpdateExist: " + FrmUpdates.isUpdateExist.ToString());
                            }
                            else
                            {
                                setPanels("Failed");
                                AddServiceAuditLog("gloUpdates Manager Installation Failed.", "Failed", strServiceName, Convert.ToString(sDownloadFileName));
                                clsGeneral.UpdateLog("gloUpdates Failed !!");
                            }
                        }
                    }
                }
                else
                {
                    clsGeneral.UpdateLog(strServiceName + " service not found.");
                }

            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("GetAUSURlUpdate function : " + ex.ToString());
                this.Close();
                Application.Exit();
            }
            finally
            {
                AddServiceAuditLog("gloUpdates Manager Installation Completed.", "Completed", strServiceName, Convert.ToString(sDownloadFileName));
                ausConfig = null;        
                this.Close();               
                Application.Exit();                 
            }
        }
        public void getUpdates()
        {
            string sUpdatelogLocation = null;
            setPanels("start", true);
            string sServiceUpdateVersion = null;
            string passwrd =null;
            Configuration config1 = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
           
            try
            {                                 
                    sServiceUpdateVersion = config1.AppSettings.Settings["gloClinicalServiceUpdatesVersion"].Value.ToString();                                 

                    clsGeneral.gstrWebService = config1.AppSettings.Settings["FTPConnection"].Value.ToString().ToUpper();
                    AddServiceAuditLog("gloLDSSniffer Service Installation Started.", "Started", config1.AppSettings.Settings["GLOCLINICALQUEUESERVICE"].Value.ToString(), sServiceUpdateVersion);
                    //

                    if (clsGeneral.gstrWebService.ToUpper().Trim() == "TEST")
                    {
                        clsConfiguration.ftp = config1.AppSettings.Settings["sftp"].Value.ToString();
                        clsConfiguration.user = config1.AppSettings.Settings["suser"].Value.ToString();
                        passwrd = config1.AppSettings.Settings["spwd"].Value.ToString();                       
                        clsConfiguration.ftppath = config1.AppSettings.Settings["strStagingFTPUpdatesDownloadPath"].Value.ToString().ToUpper();                                                
                    }
                    else
                    {
                        clsConfiguration.ftp = config1.AppSettings.Settings["pftp"].Value.ToString();                        
                        clsConfiguration.user = config1.AppSettings.Settings["puser"].Value.ToString();                       
                        passwrd = config1.AppSettings.Settings["ppwd"].Value.ToString();                        
                        clsConfiguration.ftppath = config1.AppSettings.Settings["strProductionFTPUpdatesDownloadPath"].Value.ToString().ToUpper();                        
                    }

                    clsConfiguration.ftp = clsEncryption.DecryptFromBase64String(clsConfiguration.ftp, "12345678");
                    clsConfiguration.user = clsEncryption.DecryptFromBase64String(clsConfiguration.user, "12345678");                    
                    clsConfiguration.pwd = clsEncryption.DecryptFromBase64String(passwrd, "12345678");
                    //   
                    clsGeneral.UpdateLog("Service is Pointed to the server : " + clsGeneral.gstrWebService.ToString());

                    using (clsInstallUpdates objInstallUpdate = new clsInstallUpdates())
                    {
                        if (objInstallUpdate.ISRedistributable2010Exist() == false)
                        {
                            clsGeneral.GetFtpCredentials();
                            if (CheckFTPConnection())
                            {
                                lblProgress.Text = "Installing VC++ redistributable X86 package. This may take few minutes.";
                                string remotPath = config1.AppSettings.Settings["strFTPPrerequisitesPath"].Value.ToString().ToUpper() + "MSRedistributable2010/vcredist_x86.exe";
                                string strpath = Application.StartupPath.ToString() + @"\MSRedistributable2010";

                                clsGeneral.DeleteFiles(strpath);

                                if (_ftp.FileExists(remotPath))
                                {
                                    if (!Directory.Exists(strpath)) { Directory.CreateDirectory(strpath); }
                                    if (DownloadFiles(remotPath, strpath))
                                    {
                                        strpath += "\\vcredist_x86.exe";
                                        if (objInstallUpdate.ExecuteMSI(strpath, "/Q") == false)
                                        {
                                            lblFailed.Text = "VC++ redistributable X86 installation failed." + Environment.NewLine + "Try Again or Contact System Administrator.";
                                            setPanels("Failed");
                                            clsGeneral.UpdateLog("VC++ redistributable X86 installation Failed.");
                                            // clsGeneral.DeleteFiles(strpath);                                           
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    clsGeneral.UpdateLog("vcredist_x86.exe file not exist on sFtp server.");
                                }
                            }
                        }
                        else
                        {
                            clsGeneral.UpdateLog("VC++ redistributable X86 already installed.");
                        }
                    }
                    lblProgress.Text = "Updating gloLDSSniffer Service. This may take few minutes.";
                    clsGeneral.UpdateLog("Inside getUpdates");

                    clsGeneral.UpdateLog("setting location set for sMappedDriveFileLocation ");

                    sUpdatelogLocation = MappedDrvLog.GetLogPath();

                    gloGlobal.gloTSPrint.sMappedDriveFileLocation = sUpdatelogLocation;
                    clsGeneral.UpdateLog("MappedDrvLog log location set: " + sUpdatelogLocation);

                    //Staging or Production - set values
                    if (DownloadUpdates(strGlobalUpdateType, sServiceUpdateVersion))
                    {
                        isUpdateDownloaded = true;
                        clsGeneral.UpdateLog("Updates are available and downloaded for Service.");
                    }
                    else
                    {
                        if (FrmUpdates.isUpdateExist == true)
                        {
                            setPanels("Exist");
                            clsGeneral.UpdateLog("Updates already Exists......isUpdateExist: " + FrmUpdates.isUpdateExist.ToString());
                        }                      
                        
                        isUpdateDownloaded = false;
                    }
                    if (isUpdateDownloaded)
                    {
                        clsGeneral.UpdateLog("Starting - Installing Automatic Updates");

                        Application.DoEvents();

                        if (InstallUpdates(strGlobalUpdateType))
                        {
                            isUpdateInstalled = 1;


                            setPanels("Success");
                          //  Thread.Sleep(20000);
                            //MessageBox.Show("Done !! ");
                            clsGeneral.UpdateLog("gloUpdates Done !!");
                            clsGeneral.UpdateLog("Updating updated file version in Config settings");
                            string[] sUpdatedFileName = sDownloadFileName.Split('/');

                            config1.AppSettings.Settings["gloClinicalServiceUpdatesVersion"].Value = sUpdatedFileName[sUpdatedFileName.Length - 1];
                            config1.Save(ConfigurationSaveMode.Modified);
                            clsGeneral.UpdateLog("Updating updated file version in Config settings - completed, value: " + sUpdatedFileName[sUpdatedFileName.Length - 1]);


                            ////////// deleting zip file
                            string sZipfile = null;
                            sZipfile = clsGeneral.strUpdatedownloadLocation ;
                            clsGeneral.UpdateLog("Downloaded zip file " + sZipfile);
                            if (File.Exists(sZipfile))
                            {
                                File.Delete(sZipfile);
                                clsGeneral.UpdateLog("Zip file deleted - " + sZipfile);
                            }
                            ////////// deleting zip file

                            sUpdatedFileName = null;
                       
                        }
                        else
                        {
                           // MessageBox.Show("Failed !! ");                          
                            ////pnlInstallProgress.Controls["picUpdating"].Visible = false;
                            ////pnlInstallProgress.Controls["lblProgress"].Visible = false;
                            ////pnlInstallProgress.Controls["lblFailed"].Visible = true;
                            ////pnlInstallProgress.Controls["picFailed"].Visible = true;                            
                            
                           // Thread.Sleep(50000);
                            if (FrmUpdates.isUpdateExist == true)
                            {
                                setPanels("Exist");
                                clsGeneral.UpdateLog("Updates already Exists......isUpdateExist: " + FrmUpdates.isUpdateExist.ToString());
                            }
                            else
                            {
                                setPanels("Failed");
                                clsGeneral.UpdateLog("gloUpdates Failed !!");
                            }
                        }
                    }
                    else
                    {

                        if (FrmUpdates.isUpdateExist == true)
                        {
                            setPanels("Exist");
                            clsGeneral.UpdateLog("Updates already Exists......isUpdateExist: " + FrmUpdates.isUpdateExist.ToString());
                        }
                        else
                        {
                            if (FrmUpdates.isUpdateCancelledbyUser == true)
                            {
                                clsGeneral.UpdateLog("Updates Cancelled by User. ");
                            }
                            else
                            {
                                setPanels("Failed");
                                AddServiceAuditLog("gloLDSSniffer Service Installation Failed.", "Failed", config1.AppSettings.Settings["GLOCLINICALQUEUESERVICE"].Value.ToString(), Convert.ToString(sDownloadFileName));
                                clsGeneral.UpdateLog("Updates not downloaded or not available from FTP. ");
                            }
                        }
                       
                        ////pnlInstallProgress.Controls["picUpdating"].Visible = false;
                        ////pnlInstallProgress.Controls["lblProgress"].Visible = false;
                        ////pnlInstallProgress.Controls["lblFailed"].Visible = true;
                        ////pnlInstallProgress.Controls["picFailed"].Visible = true;                        
                        
                       // Thread.Sleep(20000);
                       
                    }

                    clsGeneral.UpdateLog("Inside getUpdates - Started updating Twain DLL");

                    string sDLLPath = gloGlobal.clsMISC.GetEnvironmentSysFolder()+"\\";
                    string _twainSource = null;
                    _twainSource = Application.StartupPath+ @"\Twain\twain_32.dll";
                    ProcessXcopy(_twainSource, sDLLPath );
                    clsGeneral.UpdateLog("Inside getUpdates - _twainSource1: " + _twainSource + " Copied to: " + sDLLPath);                    

                    clsGeneral.UpdateLog("Inside getUpdates - Finished updating Twain DLL");
             }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("getUpdates function : " + ex.ToString());
                clsGeneral.UpdateLog("Exiting gloUpdates Application ");
                this.Close();               
                Application.Exit();           
            }
            finally
            {
                AddServiceAuditLog("gloLDSSniffer Service Installation Completed.", "Completed", config1.AppSettings.Settings["GLOCLINICALQUEUESERVICE"].Value.ToString(), Convert.ToString(sDownloadFileName));
                sServiceUpdateVersion = null;
                config1 = null;                
                this.Close();               
                Application.Exit();                 
            }
        }

    
        #region Functions

        private static void ProcessXcopy(string SolutionDirectory, string TargetDirectory)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            //Give the name as Xcopy
            startInfo.FileName = "xcopy";
            //make the window Hidden
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //Send the Source and destination as Arguments to the process
            startInfo.Arguments = "\"" + SolutionDirectory + "\"" + " " + "\"" + TargetDirectory + "\"" + @" /e /y /I";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                startInfo = null;
            }
        }

        void setPanels(string sflag, bool bIsstart=false)
        {
            if (bIsstart == true)
            {
                pnlInstallProgress.Controls["picUpdating"].Visible = true;
                pnlInstallProgress.Controls["lblProgress"].Visible = true;
                Application.DoEvents();
            }
            else
            {
                pnlInstallProgress.Controls["picUpdating"].Visible = false;
                pnlInstallProgress.Controls["lblProgress"].Visible = false;
                pnlInstallProgress.Controls["lbl" + sflag].Visible = true;
                pnlInstallProgress.Controls["pic" + sflag].Visible = true;
                Application.DoEvents();
                Thread.Sleep(5000);
            }
            ////pnlInstallProgress.Controls["pic"+sflag].BringToFront();
            ////pnlInstallProgress.Controls["lbl"+sflag].BringToFront();
          
        }

        public static bool DownloadUpdates(string strUpdateType, string sServiceUpdtVersion)
        {
            bool retSuccess = false;
            string strZipFileName = null;
            string strPathttoZipFile = null;
            ////string strFtpCredentials = null;
            ////string _Access = null;
            string strDownloadPath = null;

            DataTable dtUpdates = null;
         
            try
            {
                clsGeneral.UpdateLog("In function DownloadUpdates for Update Type: " + strUpdateType);
                
                Application.DoEvents();
              
                clsGeneral.GetFtpCredentials();

                clsGeneral.UpdateLog("In function DownloadUpdates after GetFtpCredentials for Update Type:  " + strUpdateType);

                #region GLOCLINICALQUEUESERVICE
                if (strUpdateType.ToUpper() == "GLOLDFSNIFFERSERVICEUPDATES") //"GLOCLINICALSERVICEUPDATES"
                {
                    if (CheckFTPConnection())
                    {
                        clsGeneral.UpdateLog("Connected successfully to FTP");

                        ////strDownloadPath = strDownloadPath + sServiceUpdtVersion; /////+ dtUpdates.Rows[0]["sProductVersion"].ToString();
                        ////clsGeneral.UpdateLog("strDownloadPath Updated with Version: " + strDownloadPath + " for UpdateType: " + strUpdateType);
                        strDownloadPath = clsGeneral.strFtpFolderPath;

                        if (!String.IsNullOrEmpty(strDownloadPath))
                        {
                            if (IsFTPFileexists(strDownloadPath, sServiceUpdtVersion))
                            {
                                clsGeneral.UpdateLog("FTP download file exists");
                                clsGeneral.UpdateLog("FTP download location:" + strDownloadPath);

                                strDownloadPath = strDownloadPath + sDownloadFileName; /////+ dtUpdates.Rows[0]["sProductVersion"].ToString();
                                clsGeneral.UpdateLog("strDownloadPath Updated with new Version: " + strDownloadPath + " for UpdateType: " + strUpdateType);

                                Application.DoEvents();
                            }
                            else
                            {
                                clsGeneral.UpdateLog("Unable to find the download file in FTP location.");
                            }
                        }
                        else
                        {
                            clsGeneral.UpdateLog("FTP download path does not exist.");
                        }
                    }
                    else
                    {
                        clsGeneral.UpdateLog("Unable to connect to FTP");
                    }
                }
                #endregion GLOCLINICALQUEUESERVICE
               
                clsGeneral.UpdateLog("FTP Download Path :" + strDownloadPath);

                if ((!String.IsNullOrWhiteSpace(strDownloadPath) || strDownloadPath != "") && !String.IsNullOrEmpty(sDownloadFileName))
                {
                    Application.DoEvents();
                 
                    if (DownloadFiles(strDownloadPath, clsGeneral.gstrUpdatesPath))
                    {
                        //string strUpdateDownloadLocation = strPathttoZipFile;
                        clsGeneral.UpdateLog("Downloaded Update from Path :" + strDownloadPath);

                        //if zip file alredy exists delete the existing zip file from that location..
                        strZipFileName = clsGeneral.GetZipFileName(strDownloadPath);
                        strPathttoZipFile = clsGeneral.gstrUpdatesPath + "\\" + strZipFileName;

                        clsGeneral.UpdateLog("Install started for strUpdateType:" + strUpdateType);

                        if (strUpdateType.ToUpper() == "GLOLDFSNIFFERSERVICEUPDATES") //"GLOCLINICALSERVICEUPDATES"
                        {
                            clsGeneral.UpdateLocation(strPathttoZipFile);                           
                        }                      
                        retSuccess = true;
                    }
                    else
                    {
                        clsGeneral.UpdateLog("Failed to download update from Path :" + strDownloadPath);
                        if (!String.IsNullOrEmpty(strUpdateType))
                        {
                            clsGeneral.UpdateLog("Failed to download update for strUpdateType :" + strUpdateType);   
                        }                        
                    }
                }
                else
                {
                    clsGeneral.UpdateLog("Updates are already applied or does not available.");
                }           
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Error while downloading or Applying the Update: " + ex.Message.ToString());
                retSuccess = false;
            }
            finally
            {           
                if (_ftp != null)
                {
                    if (_ftp.State == SftpState.Connecting)
                    {
                        _ftp.Disconnect();
                    }

                    _ftp.Dispose();
                    _ftp = null;
                }

                if (dtUpdates != null)
                {
                    dtUpdates.Dispose();
                    dtUpdates = null;
                }
            }
            return retSuccess;
        } //DownloadUpdates

        public static bool InstallUpdates(string strUpdateType)
        {
            bool isUpdateInstalled = false;
            try
            {
               if (ApplyUpdates())
               {
                   isUpdateInstalled = true;
                   clsGeneral.UpdateLog("Update Installed successfully");
               }
            }
            catch (Exception ex)
            {
                isUpdateInstalled = false;
                clsGeneral.UpdateLog("Error while installing update. Error Message:" + ex.Message.ToString());
            }
            finally
            {
            }
            return isUpdateInstalled;
        } //InstallUpdates


        public static bool ApplyUpdates()
        {
            clsGeneral.UpdateLog("Entering ApplyUpdates from frmUpdates form.");
            int retSuccess = 0;
            clsInstallUpdates objInstallUpdate = null;
            objInstallUpdate = new clsInstallUpdates();
            retSuccess = objInstallUpdate.ApplyUpdates();
            clsGeneral.UpdateLog("Exiting ApplyUpdates from frmUpdates form, retSuccess: " + retSuccess.ToString());
            if (retSuccess == 1)
            {
                return true;
            }
            else
            {
                return false;
            }           
        } 

        #endregion Functions

        #region "AUS Update"
        public static bool DownloadUpdates_AUSService(string sServiceUpdtVersion)
        {
            bool retSuccess = false;
            string strZipFileName = null;
            string strPathttoZipFile = null;
            ////string strFtpCredentials = null;
            ////string _Access = null;
            string strDownloadPath = null;

            DataTable dtUpdates = null;

            try
            {
                clsGeneral.UpdateLog("In function DownloadUpdates_AUSService.");

                Application.DoEvents();

                clsGeneral.GetFtpCredentials();

                clsGeneral.UpdateLog("In function DownloadUpdates after GetFtpCredentials.");

                #region UPDATEMANAGERSERVICE

                if (CheckFTPConnection())
                {
                    clsGeneral.UpdateLog("Connected successfully to FTP");

                    strDownloadPath = clsGeneral.strFtpFolderPath;

                    if (!String.IsNullOrEmpty(strDownloadPath))
                    {
                        if (IsAUSFTPFileexists(strDownloadPath, sServiceUpdtVersion))
                        {                           
                            clsGeneral.UpdateLog("FTP download location:" + strDownloadPath);

                            strDownloadPath = strDownloadPath + sDownloadFileName; 
                            clsGeneral.UpdateLog("strDownloadPath Updated with new Version: " + strDownloadPath);

                            Application.DoEvents();
                        }
                        else
                        {
                            clsGeneral.UpdateLog("Unable to find the download file in FTP location.");
                        }
                    }
                    else
                    {
                        clsGeneral.UpdateLog("FTP download path does not exist.");
                    }
                }
                else
                {
                    clsGeneral.UpdateLog("Unable to connect to FTP");
                }

                #endregion UPDATEMANAGERSERVICE

                clsGeneral.UpdateLog("FTP Download Path :" + strDownloadPath);

                if ((!String.IsNullOrWhiteSpace(strDownloadPath) || strDownloadPath != "") && !String.IsNullOrEmpty(sDownloadFileName))
                {
                    Application.DoEvents();

                    if (DownloadFiles(strDownloadPath, clsGeneral.gstrUpdatesPath))
                    {
                        //string strUpdateDownloadLocation = strPathttoZipFile;
                        clsGeneral.UpdateLog("Downloaded Update from Path :" + strDownloadPath);

                        //if zip file alredy exists delete the existing zip file from that location..
                        strZipFileName = clsGeneral.GetZipFileName(strDownloadPath);
                        strPathttoZipFile = clsGeneral.gstrUpdatesPath + "\\" + strZipFileName;

                        clsGeneral.UpdateLocation(strPathttoZipFile);
                        
                        retSuccess = true;
                    }
                    else
                    {
                        clsGeneral.UpdateLog("Failed to download update from Path :" + strDownloadPath);                      
                        
                    }
                }
                else
                {
                    clsGeneral.UpdateLog("Updates are already applied or does not available.");
                }
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Error while downloading or Applying the Update: " + ex.Message.ToString());
                retSuccess = false;
            }
            finally
            {
                if (_ftp != null)
                {
                    if (_ftp.State == SftpState.Connecting)
                    {
                        _ftp.Disconnect();
                    }

                    _ftp.Dispose();
                    _ftp = null;
                }

                if (dtUpdates != null)
                {
                    dtUpdates.Dispose();
                    dtUpdates = null;
                }
            }
            return retSuccess;
        }
        public static bool InstallUpdates_AUSService()
        {
            bool isUpdateInstalled = false;
            try
            {
                if (ApplyUpdates_AUSService())
                {
                    isUpdateInstalled = true;
                    clsGeneral.UpdateLog("Update Installed successfully");
                }
            }
            catch (Exception ex)
            {
                isUpdateInstalled = false;
                clsGeneral.UpdateLog("Error while installing update. Error Message:" + ex.Message.ToString());
            }
            finally
            {
            }
            return isUpdateInstalled;
        }
        public static bool ApplyUpdates_AUSService()
        {
            clsGeneral.UpdateLog("Entering ApplyUpdates from frmUpdates form.");
            int retSuccess = 0;
            clsInstallUpdates objInstallUpdate = null;
            objInstallUpdate = new clsInstallUpdates();
            retSuccess = objInstallUpdate.ApplyUpdates_AUSService(strAssemblyVer);
            clsGeneral.UpdateLog("Exiting ApplyUpdates from frmUpdates form, retSuccess: " + retSuccess.ToString());
            if (retSuccess == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        } 

        #endregion ""


        #region "FTP Connection Function"

        private static void _ftp_TransferProgress(object sender, SftpTransferProgressEventArgs e)
        {
            try
            {
               sBytesTotal = clsGeneral.GetFileSize(e.BytesTransferred);
               ////////clsGeneral.UpdateLog("FtpTotalBytes: " + sBytesTotal);

               Application.DoEvents();
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in _ftp_TransferProgress " + ex.Message.ToString() + "");
            }
        } //_ftp_TransferProgress

        private static void _ftp_BatchTransferProgress(object sender, SftpBatchTransferProgressEventArgs e)
        {
            string sStatus = "";
           
            try
            {
                if (e.State == SftpTransferState.Uploading)
                {
                }
                else
                {
                }

                //Check downloaded file size                                         
                switch (e.Operation)
                {
                    case SftpBatchTransferOperation.HierarchyRetrievalStarted:
                        //sStatus= "Retrieving hierarchy...";
                        sStatus= "Caluclating size and number of files to be downloaded...";
                        Application.DoEvents();
                        clsGeneral.UpdateLog(sStatus);
                        break;
                    case SftpBatchTransferOperation.HierarchyRetrievalFailed:
                        sStatus= "Retrieve hierarchy failed.";
                        Application.DoEvents();
                        clsGeneral.UpdateLog(sStatus);
                        break;
                    case SftpBatchTransferOperation.HierarchyRetrieved:
                        // set the bytes total info                                              
                        sStatus= "Hierarchy retrieved.";
                       sBytesTotal = clsGeneral.GetFileSize(e.BytesTotal);
                        FtpTotalBytes = e.BytesTotal;
                        //lblBytesTotal.Text = e.BytesTotal.ToString();
                        Application.DoEvents();
                        clsGeneral.UpdateLog(sStatus);
                        clsGeneral.UpdateLog("FtpTotalBytes: "+FtpTotalBytes.ToString());
                        break;
                    case SftpBatchTransferOperation.DirectoryProcessingStarted:
                        sStatus= "Processing directory.";
                        Application.DoEvents();
                        clsGeneral.UpdateLog(sStatus);
                        break;
                    case SftpBatchTransferOperation.DirectoryProcessingFailed:
                        sStatus= "Directory processing failed.";
                        clsGeneral.UpdateLog(sStatus);
                        break;
                    case SftpBatchTransferOperation.DirectorySkipped:
                        sStatus= "Directory skipped.";
                        clsGeneral.UpdateLog(sStatus);
                        break;
                    case SftpBatchTransferOperation.DirectoryCreated:
                        sStatus= "Directory created.";
                        clsGeneral.UpdateLog(sStatus);
                        break;
                    case SftpBatchTransferOperation.FileProcessingStarted:
                        sStatus= "Processing file...";
                        Application.DoEvents();
                        clsGeneral.UpdateLog(sStatus);
                        break;
                    case SftpBatchTransferOperation.FileTransferStarting:
                       // pbFile.Value = 0;
                      //  pbFile.Maximum = (int)e.CurrentFileLength;
                        sStatus= "Transferring file...";
                        Application.DoEvents();
                        //clsGeneral.UpdateLog(sStatus);
                        break;
                    case SftpBatchTransferOperation.FileProcessingFailed:
                        sStatus= "File processing failed.";
                        clsGeneral.UpdateLog(sStatus);
                        break;
                    case SftpBatchTransferOperation.FileSkipped:
                        sStatus= "File skipped.";
                        clsGeneral.UpdateLog(sStatus);
                        break;
                    case SftpBatchTransferOperation.FileTransferred:
                        sStatus= "File transferred.";
                        clsGeneral.UpdateLog(sStatus);
                        break;
                }

                // process any application events to prevent the windown from freezing
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in _ftp_BatchTransferProgress " + ex.Message.ToString() + "");
            }
        } //_ftp_BatchTransferProgress

        public static bool IsFTPFileexists(string path,string sServiceUpdtVersion)
        {
            bool success = false;
            try
            {

                string[] files = _ftp.GetNameList(path);//_ftp.GetRawList(path,FtpListingType.NameList );
                
                if (files.Length>0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i].Substring(files[i].Length - 4, 4).ToLower() == ".zip")
                        {
                           // files[i] = files[i].Remove(0, path.Trim().Length);
                            if (files[i].ToString().Trim() != sServiceUpdtVersion.Trim())
                            {
                                success = true;
                                sDownloadFileName = files[i].ToString();
                                clsGeneral.UpdateLog("IsFTPFileexists on FTP: " + files[i].ToString());
                                break;
                            }
                            else
                            {
                                isUpdateExist = true;
                                clsGeneral.UpdateLog("IsFTPFileexists on FTP, but updates are already applied to the service; fileversion: " + files[i].ToString());
                            }
                        }
                    }
                }
                
                ////if (_ftp.FileExists(path))
                ////{
                ////    success = true;
                ////}
                ////else
                ////{
                ////   // if (_ftp.FileExists(                    
                ////}
                 
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in IsFTPFileexists " + ex.Message.ToString() + "");
            }
            return success;
        } //IsFTPFileexists

        public static bool IsAUSFTPFileexists(string path, string sServiceUpdtVersion)
        {
            bool success = false;
            try
            {

                string[] files = _ftp.GetNameList(path);//_ftp.GetRawList(path,FtpListingType.NameList );

                if (files.Length > 0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i].Substring(files[i].Length - 4, 4).ToLower() == ".zip")
                        {
                            //files[i] = files[i].Remove(0, path.Trim().Length);
                            if (files[i].ToString().Trim() == sServiceUpdtVersion.Trim())
                            {
                                success = true;
                                sDownloadFileName = files[i].ToString();
                                clsGeneral.UpdateLog("IsAUSFTPFileexists on FTP: " + files[i].ToString());
                                break;
                            }
                            else
                            {
                                isUpdateExist = true;
                                clsGeneral.UpdateLog("IsAUSFTPFileexists on FTP, but updates are already applied to the service; fileversion: " + files[i].ToString());
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in IsAUSFTPFileexists " + ex.Message.ToString() + "");
            }
            return success;
        }

        public static bool CheckFTPConnection()
        {
            bool success = false;
            try
            {
                clsGeneral.UpdateLog("Connecting to FTP...");

                // connect and login
                _ftp = new Sftp();
                //FtpSecurity security = FtpSecurity.Explicit;
                //TlsParameters parameters = new TlsParameters();
                //parameters.CertificateVerifier = new Verifier();
                clsGeneral.UpdateLog("Validating Ftp hostname...");
                _ftp.Connect(clsGeneral.strFtpHostName);
                clsGeneral.UpdateLog("Valid Ftp hostname...");
                clsGeneral.UpdateLog("Validating Ftp credentials");
                _ftp.Login(clsGeneral.strFtpUserId, clsGeneral.strFtpPwd);
                clsGeneral.UpdateLog("Connection established successfully to ftp");
                success = true;
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in Check FTP Connection " + ex.Message.ToString() + "");
            }
            return success;
        } //CheckFTPConnection

        public static bool DownloadFiles(string remotePath, string localPath)
        {
            bool success = false;
            if (!Connect())
            {
                //return;               
            }
            try
            {
                // download files
                Application.DoEvents();
                _ftp.GetFiles(remotePath, localPath, SftpBatchTransferOptions.Recursive);
                Application.DoEvents();

                success = true;
                clsGeneral.UpdateLog("Download finished successfully");
            }
            catch (Exception ex)
            {
                SftpException sx = ex as SftpException;
                clsGeneral.UpdateLog("Exception in DownloadFiles " + ex.Message.ToString() + "");
                
            }
            finally
            {
                Disconnect();
                //SetState(false);
            }
            return success;
        } //DownloadFiles

        private static bool Connect()
        {
            try
            {
                // register batch transfer events
                _ftp.BatchTransferProgress += new SftpBatchTransferProgressEventHandler(_ftp_BatchTransferProgress);
                //_ftp.BatchTransferProblemDetected += new FtpBatchTransferProblemDetectedEventHandler(_ftp_BatchTransferProblemDetected);

                // register single file progress event
                _ftp.TransferProgress += new SftpTransferProgressEventHandler(_ftp_TransferProgress);

                return true;
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in Connect " + ex.Message.ToString() + "");
                clsGeneral.UpdateLog("Unable to connec to FTP");
                return false;
            }
        } //Connect

        public static void Disconnect()
        {
            try
            {
                _ftp.Disconnect();
                _ftp.Dispose();
                _ftp = null;
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in Disconnect " + ex.Message.ToString() + "");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAUSID.Text == "")
                {
                    MessageBox.Show("Enter AUS ID. ", "gloUpdates Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (UpdateAUSID(txtAUSID.Text) != false)
                {
                    bool IsAusUpdate = false;
                    Configuration ausconfig = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                    IsAusUpdate = Convert.ToBoolean(ausconfig.AppSettings.Settings["ISAUSUpdate"].Value);
                    pnlAUSID.Visible = false;
                    pnlInstallProgress.Visible = true;
                    if (IsAusUpdate)
                    {
                        clsGeneral.UpdateLog("Updating AUS Config File.");
                        GetAUSServiceUpdate();
                    }
                    else
                    {
                        getUpdates();
                    }
                }
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in btnSave_Click : " + ex.Message.ToString() + "");
                this.Close();
                Application.Exit();
            }
            finally
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

         //RetrieveFTPUpdateDownloadPath

        #endregion "FTP Connection Function"
    }

}
