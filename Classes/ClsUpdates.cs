using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Ionic.Zip;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Security;
using System.Reflection;
using System.ServiceProcess;

namespace gloUpdates
{

    public static class ClsUpdates
    {     

        #region Functions    
       
        public static bool CheckService(string ServiceName)
        {

            bool success = false;
            try
            {
                ServiceController[] scServices;
                scServices = ServiceController.GetServices();
                int counter = 0;
                foreach (ServiceController scTemp in scServices)
                {
                    if (scTemp.Status == ServiceControllerStatus.Running)
                    {
                        if (scTemp.ServiceName.ToLower() == ServiceName.ToString().ToLower())
                        {
                            counter++;
                            success = true;
                        }

                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
            }
            return success;

        }
        public static bool ChkUpdates(string sServiceUpdtVersion)
        {
            Boolean bChkUpdates = false;
            Boolean isShowWarningMsg = false;

            if (!string.IsNullOrEmpty(FrmUpdates.sDownloadPathName))
            {
                if (FrmUpdates.IsFTPFileexists(FrmUpdates.sDownloadPathName, sServiceUpdtVersion))
                {
                    bChkUpdates = true;
                    if (CheckService(FrmUpdates.myServiceName))
                    {
                        isShowWarningMsg = true;
                        clsGeneral.UpdateLog("CheckService returns true hence isShowWarningMsg set to true, value - " + isShowWarningMsg.ToString());

                         // Ask user confirmation before installing updates - because service is installed & running
                        string sMsg = "Important updates are available for gloClinical Queue Service. Before you start installing updates, Please ensure that there are no pending documents to print. Would you like to install updates now?";
                        DialogResult dialogResult = MessageBox.Show(sMsg, "Check Updates", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            clsGeneral.UpdateLog("dialogResult returns True, so will start updates. bChkUpdates: " + bChkUpdates.ToString());
                            FrmUpdates.isUpdateCancelledbyUser = false;
                            clsGeneral.UpdateLog("isUpdateCancelledbyUser set to false. isUpdateCancelledbyUser: " + FrmUpdates.isUpdateCancelledbyUser.ToString());
                        }
                        else
                        {
                            clsGeneral.UpdateLog("dialogResult returns false, so updates will not be installed, will skip updates. bChkUpdates: " + bChkUpdates.ToString());
                            bChkUpdates = false;
                            clsGeneral.UpdateLog("ChkUpdates set to False. bChkUpdates: " + bChkUpdates.ToString());

                            FrmUpdates.isUpdateCancelledbyUser = true;
                            clsGeneral.UpdateLog("isUpdateCancelledbyUser set to true. isUpdateCancelledbyUser: " + FrmUpdates.isUpdateCancelledbyUser.ToString());

                        }

                    }
                    else
                    {
                        isShowWarningMsg = false;
                        clsGeneral.UpdateLog("CheckService returns false hence isShowWarningMsg set to false,no need to show warning message, value - " + isShowWarningMsg.ToString());
                    }                   
                }
                else
                {
                    if (FrmUpdates.isUpdateExist == true)
                    {                        
                        clsGeneral.UpdateLog("Updates already Exists......isUpdateExist: " + FrmUpdates.isUpdateExist.ToString());
                    }
                    else
                    {
                        clsGeneral.UpdateLog("FTP download file does not exists......ChkUpdates returns False. bChkUpdates: " + bChkUpdates.ToString());
                    }
                }
            }
            else
            {
              //  clsGeneral.UpdateLog("FTP download file does not exists......ChkUpdates returns False. bChkUpdates: " + bChkUpdates.ToString());                                   
                clsGeneral.UpdateLog("sDownloadPathName is null ");                                                   
            }
            return bChkUpdates;
        }
          
        #endregion Functions

    }
}
