using System;
using System.Collections.Generic;
//using java.util;
//using java.util.zip;
//using java.io;
using System.IO;
using System.Windows.Forms;
using Ionic.Zip;



namespace gloUpdates
{
 
    // Extraction Methods for Drug Database definitions
    public class clsUnzipDrugsFiles
    {

        #region "New Code"

        public static string ExtractZipFile(string zipfilename, string strDestinationLocation)
        {
            String reqFile = null;
            string fname = string.Empty;
            string strDirectioryName = string.Empty;
            try
            {
                string MyExtractProgress = string.Empty;

                using (ZipFile zip1 = ZipFile.Read(zipfilename))
                {
                    foreach (ZipEntry e in zip1)
                    {
                        e.Extract(strDestinationLocation, ExtractExistingFileAction.OverwriteSilently);
                    }
                }

                //string[] strDirectoyName = Directory.GetDirectories(strDestinationLocation);

                //if (strDirectoyName.Length > 0)
                //{
                //    string[] lstfile = Directory.GetFiles(strDirectoyName[0]);
                //    foreach (string strFilename in lstfile)
                //    {
                //        File.Move(strFilename, strDestinationLocation + "//" + System.IO.Path.GetFileName(strFilename));
                //    }
                //    if (Directory.Exists(strDirectoyName[0]))
                //    {
                //        Directory.Delete(strDirectoyName[0]);
                //    }
                //}
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
            return reqFile;
        }   

        #endregion "New Code"
      
    }
    //**** Extraction Methods for Drug Database definitions



}
