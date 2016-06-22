using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;

namespace JooVuuX
{
    public class mIniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key,string val,string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSectionNamesA")]
        static extern int GetSectionNamesListA(byte[] lpszReturnBuffer, int nSize, string lpFileName);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public mIniFile(string INIPath)
        {
            path = INIPath;
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public void IniWriteValue(string Section,string Key,string Value)
        {
            WritePrivateProfileString(Section,Key,Value,this.path);
        }
        
        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string Section,string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section,Key,"",temp, 255, this.path);
            return temp.ToString();
        }

        public ArrayList GetSectionsList(string FileName)
        {

            //Get The Sections List Method

            ArrayList arrSec = new ArrayList();
            byte[] buff = new byte[1024];
            GetSectionNamesListA(buff, buff.Length, FileName);
            String s = Encoding.Default.GetString(buff);
            String[] names = s.Split('\0');
            foreach (String name in names)
            {
                if (name != String.Empty)
                {
                    arrSec.Add(name);
                }
            }
            return arrSec;
        }

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW",
            SetLastError = true,
            CharSet = CharSet.Unicode, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPrivateProfileString_A(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            string lpReturnString,
            int nSize,
            string lpFilename);

        public List<string> IniReadKey(string category)
        {
            string returnString = new string(' ', 32768);
            GetPrivateProfileString_A(category, null, null, returnString, 32768, this.path);
            List<string> result = new List<string>(returnString.Split('\0'));
            result.RemoveRange(result.Count - 2, 2);
            return result;
        }

        public List<string> IniReadSection()
        {
            string returnString = new string(' ', 65536);
            GetPrivateProfileString_A(null, null, null, returnString, 65536, this.path);
            List<string> result = new List<string>(returnString.Split('\0'));
            result.RemoveRange(result.Count - 2, 2);
            return result;
        }

    }
}
