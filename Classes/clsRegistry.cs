using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace gloUpdates
{
    class clsRegistry
    {
        public const string str64EMRKey = "Software\\Wow6432Node\\gloEMR";
        public const string str32EMRKey = "Software\\gloEMR";

        public const string str32PMKey = "Software\\gloPM";
        public const string str64PMKey = "Software\\Wow6432Node\\gloPM";

        public static string str32gloServiceKey = "Software\\" + gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.DynamicRegistryName;
        public static string str64gloServiceKey = "Software\\Wow6432Node\\" + gloInstallerCommandParametrs.gloInstaller.gloInstallerCommandParameters.DynamicRegistryName;

        public static bool CheckRegistryExists(string strRegKey)
        {
            bool _success = false;
            RegistryKey oKey = null;
            oKey = Registry.LocalMachine.OpenSubKey(strRegKey, false);
            try
            {
                if (oKey != null && oKey.ToString() != "")
                {
                    _success = true;
                }
            }
            catch (Exception ex)
            {
                clsGeneral.UpdateLog("Exception in CheckRegistryExists " + ex.Message.ToString() + "");
            }
            finally
            {
                if (oKey != null)
                    oKey.Close();

            }
            return _success;
        }
        public static bool WriteValue(RegistryKey ParentKey, string SubKey, string ValueName, object Value)
        {
            bool opened = false;
            try
            {
                if (ParentKey != null)
                {
                    //Open the given subkey 
                    RegistryKey Key = ParentKey.OpenSubKey(SubKey, true);

                    if (Key == null)
                    {
                        //when subkey doesn't exist create it 
                        Key = ParentKey.CreateSubKey(SubKey);
                        Key = ParentKey.OpenSubKey(SubKey, true);
                    }
                    if (Key != null)
                    {
                        opened = true;
                        Key.SetValue(ValueName, Value);
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                //MessageBox.Show(e.InnerException.ToString());
            }
            finally
            {
                if (opened)
                {
                    if (ParentKey != null)
                    {
                        ParentKey.Close();
                        ParentKey = null;
                    }
                }
            }
        }
        public static bool CreateSubKey(RegistryKey ParentKey, string SubKey)
        {
            bool opened = false;
            try
            {
                if (ParentKey != null)
                {
                    RegistryKey Key = ParentKey.OpenSubKey(SubKey, true);

                    if (Key == null)
                    {
                        //when subkey doesn't exist create it 
                        Key = ParentKey.CreateSubKey(SubKey);
                    }
                    else
                    {
                        opened = true;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
                //MessageBox.Show(e.InnerException.ToString());
            }
            finally
            {
                if (opened)
                {
                    if (ParentKey != null)
                        ParentKey.Close();
                }
            }
        }
        public static string GetRegistryValue(string strValue, string strKey)
        {
            string value = string.Empty;
            RegistryKey oKey = Registry.LocalMachine.OpenSubKey(strKey, false);
            try
            {
                if (oKey != null && oKey.ToString() != "")
                {
                    if ((oKey.GetValue(strValue) == null) == false)
                    {
                        value = oKey.GetValue(strValue).ToString().Trim();
                    }
                    else
                    {
                        value = null;
                    }
                }
            }
            //catch (System.ArgumentOutOfRangeException ex)
            //{
            //    clsGeneral.UpdateLog("Exception in GetRegistryValue "+ex.Message.ToString()+"");
            //}
            //catch (System.ArgumentNullException ex)
            //{
            //    clsGeneral.UpdateLog("Exception in GetRegistryValue " + ex.Message.ToString() + "");
            //}
            //catch (System.ArgumentException ex)
            //{
            //    clsGeneral.UpdateLog("Exception in GetRegistryValue " + ex.Message.ToString() + "");
            //}
            catch (System.Exception ex)
            {
                clsGeneral.UpdateLog("Exception in GetRegistryValue " + ex.Message.ToString() + "");
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
