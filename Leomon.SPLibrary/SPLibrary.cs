/*
 * Author : leomon
 * Email : 1964416932@qq.com
 * Website : null
 * Reference : null
 * */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Leomon.SPLibrary
{
    /// <summary>
    /// 该类用来存放或者读取配置信息。存放配置信息前，需要创建一个Editor类型的对象，
    /// 将配置信息写入后方可以写入。
    /// first created: 2014/01/21.
    /// modified time:2014/02/06.
    /// version:1.0.4.0
    /// author:leomon.
    /// </summary>
    public class SharedPreferences
    {
        private bool hasFile = false;
        private string fileName = string.Empty;
        private XElement configDoc = null;
        private Dictionary<string, string> stringDictionary = new Dictionary<string, string>();
        private Dictionary<string, bool> booleanDictionary = new Dictionary<string, bool>();
        private Dictionary<string, int> int32Dictionary = new Dictionary<string, int>();
        private Dictionary<string, float> floatDictionary = new Dictionary<string, float>();

        /// <summary>
        /// 判断配置文件是否存在。true表示存在。
        /// </summary>
        public bool ConfigFileExists
        {
            get
            {
                return hasFile;
            }
        }
        /// <summary>
        /// 构造方法：初始化。
        /// </summary>
        public SharedPreferences()
        {
            this.fileName = "defaultSetting.xml";
        }

        /// <summary>
        /// 构造方法：初始化。
        /// </summary>
        /// <param name="fileName">文件名或文件路径+文件名，若存在则可以执行读写操作。否则会自动创建。</param>
        /// <param name="load">表示是否需要加载指定的配置文件</param>
        public SharedPreferences(string fileName, bool load)
        {
            this.fileName = fileName;
            hasFile = File.Exists(fileName);
            if (hasFile && load)
            {
                LoadMyFile(fileName);
            }
        }

        /// <summary>
        /// 方法：读取配置文件。
        /// </summary>
        /// <param name="fileName">文件名或文件路径+文件名</param>
        private void LoadMyFile(string fileName)
        {
            int32Dictionary.Clear();
            stringDictionary.Clear();
            booleanDictionary.Clear();
            floatDictionary.Clear();
            //将配置信息加载到内存中。
            try
            {
                configDoc = XElement.Load(fileName);
            }
            catch (System.Xml.XmlException)
            {
                File.Delete(fileName);
            }          
        }

        /// <summary>
        /// 查找匹配的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetValue(string key)
        {
            string tempStr = "";
            try
            {
                var value = (from xml in configDoc.Descendants(key) select xml).First();
                tempStr = (string)value;
            }
            catch (Exception)
            {
                tempStr = "null";
            }

            return tempStr;
        }

        /// <summary>
        /// 保存配置信息。
        /// </summary>
        /// <param name="editor">editor类型的配置信息。</param>
        public void Save(Editor editor)
        {
            //依次写入配置信息到文件中并保存。为了方便起见都是直接覆盖上次的配置文件的。
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    using (XmlTextWriter xw = new XmlTextWriter(stream, Encoding.UTF8))
                    {
                        xw.Formatting = Formatting.Indented;
                        xw.WriteStartDocument();
                        xw.WriteComment("配置文件为软件自动生成，非软件的编写者不要尝试修改。若出现异常，直接删除该配置文件即可。");
                        xw.WriteComment("By Leomon. All rights reserved. " + DateTime.Today.ToShortDateString());
                        xw.WriteStartElement("configuration");
                        foreach (var item in editor.BooleanDictionary)  //再写入bool类型的配置信息
                        {
                            xw.WriteElementString(item.Key, item.Value.ToString());
                        }
                        foreach (var item in editor.Int32Dictionary)    //写入int型的。。。
                        {
                            xw.WriteElementString(item.Key, item.Value.ToString());
                        }
                        foreach (var item in editor.DoubleDictionary)   //写入浮点型数据的配置信息
                        {
                            xw.WriteElementString(item.Key, item.Value.ToString());
                        }
                        foreach (var item in editor.StringDictionary)    //先写string类型的配置信息（其实都是string 类型的。）
                        {
                            xw.WriteElementString(item.Key, item.Value);
                        }
                        xw.WriteEndElement();
                        xw.WriteEndDocument();
                    }
                }
                catch (Exception)
                {
                }
            }

        }
        /// <summary>
        /// 获取string类型的配置信息
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defValue">你设定的返回值，用于在查找失败后返回。</param>
        /// <returns>若存在，则返回对应字符串值，否则返回defValue设定值。</returns>
        public string GetString(String key, String defValue)
        {
            //使用linq to xml方法查找：
            string value = GetValue(key);
            if (value == "null")
            {
                value = defValue;
            }
            return value;
        }

        /// <summary>
        /// 获取浮点类型的配置信息
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defValue">你设定的返回值，用于在查找失败后返回</param>
        /// <returns>若存在，则返回对应浮点型值，否则返回defValue设定值。</returns>
        public double GetDouble(String key, double defValue)
        {
            string value = GetValue(key);
            double dValue = 0;
            if (!double.TryParse(value, out dValue))
            {
                dValue = defValue;
            }
            return dValue;
        }

        /// <summary>
        /// 获取整型的配置信息
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defValue">你设定的返回值，用于在查找失败后返回</param>
        /// <returns>若存在，则返回对应int型值，否则返回defValue设定值。</returns>
        public Int32 GetInt32(String key, Int32 defValue)
        {
            string value = GetValue(key);
            int iValue = 0;
            if (!Int32.TryParse(value, out iValue))
            {
                iValue = defValue;
            }
            return iValue;
        }

        /// <summary>
        /// 获取bool类型的配置信息
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defValue">你设定的返回值，用于在查找失败后返回</param>
        /// <returns>若存在，则返回对应bool类型值，否则返回defValue设定值。</returns>
        public Boolean GetBoolean(String key, Boolean defValue)
        {
            string value = GetValue(key);
            bool bValue = false;
            if (!bool.TryParse(value, out bValue))
            {
                bValue = defValue;
            }
            return bValue;
        }
    }

    /// <summary>
    /// 该类用来记录待存放配置信息，将配置信息组织成一定的格式，交给MySharedPreferences类中的方法来处理。目前，仅可以处理针对string,bool,int,Double型的配置。
    /// </summary>
    public class Editor
    {
        #region 字段+属性
        /// <summary>
        /// 字段：用来存放string类型的属性信息。
        /// </summary>
        private Dictionary<string, string> stringDictionary = new Dictionary<string, string>();

        /// <summary>
        /// 属性：获取或设置string类型的配置信息。
        /// </summary>
        public Dictionary<string, string> StringDictionary
        {
            get { return stringDictionary; }
            set { stringDictionary = value; }
        }

        /// <summary>
        /// 字段：用来存放bool类型的配置信息。
        /// </summary>
        private Dictionary<string, bool> booleanDictionary = new Dictionary<string, bool>();

        /// <summary>
        /// 属性：获取或返回bool类型的配置信息。
        /// </summary>
        public Dictionary<string, bool> BooleanDictionary
        {
            get { return booleanDictionary; }
            set { booleanDictionary = value; }
        }

        /// <summary>
        /// 字段：用来存放int类型的配置信息。
        /// </summary>
        private Dictionary<string, int> int32Dictionary = new Dictionary<string, int>();

        /// <summary>
        /// 属性：获取或返回int类型的配置信息。
        /// </summary>
        public Dictionary<string, int> Int32Dictionary
        {
            get { return int32Dictionary; }
            set { int32Dictionary = value; }
        }

        /// <summary>
        /// 字段：用来存放double类型的配置信息。
        /// </summary>
        private Dictionary<string, double> doubleDictionary = new Dictionary<string, double>();

        /// <summary>
        /// 属性：获取或返回double类型的配置信息。
        /// </summary>
        public Dictionary<string, double> DoubleDictionary
        {
            get { return doubleDictionary; }
            set { doubleDictionary = value; }
        }
        #endregion
        /// <summary>
        /// 构造方法：创建并初始化Editor类型对象。
        /// </summary>
        public Editor()
        {
            StringDictionary.Clear();
            Int32Dictionary.Clear();
            BooleanDictionary.Clear();
            DoubleDictionary.Clear();
        }

        /// <summary>
        /// 构造方法：创建并初始化Editor类型对象。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">配置信息</param>
        public Editor(String key, String value)
        {
            StringDictionary.Clear();
            PutString(key, value);
        }

        /// <summary>
        ///  构造方法：创建并初始化Editor类型对象。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">配置信息</param>
        public Editor(String key, Boolean value)
        {
            BooleanDictionary.Clear();
            PutBoolean(key, value);
        }

        /// <summary>
        /// 构造方法：创建并初始化Editor类型对象。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">配置信息</param>
        public Editor(String key, Int32 value)
        {
            Int32Dictionary.Clear();
            PutInt32(key, value);
        }

        /// <summary>
        /// 构造方法：创建并初始化Editor类型对象。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">配置信息</param>
        public Editor(String key, double value)
        {
            DoubleDictionary.Clear();
            PutDouble(key, value);
        }

        /// <summary>
        /// 方法：存入字符串类型的配置信息。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">配置信息</param>
        public void PutString(string key, string value)
        {
            StringDictionary.Add(key, value);
        }

        /// <summary>
        /// 方法：存入bool类型的配置信息。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">配置信息</param>
        public void PutBoolean(string key, bool value)
        {
            BooleanDictionary.Add(key, value);
        }

        /// <summary>
        /// 方法：存入int类型的配置信息。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">配置信息</param>
        public void PutInt32(string key, int value)
        {
            Int32Dictionary.Add(key, value);
        }

        /// <summary>
        /// 方法：存入double类型的配置信息。
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">配置信息</param>
        public void PutDouble(string key, double value)
        {
            DoubleDictionary.Add(key, value);
        }

        /// <summary>
        /// 方法：判断是否拥有需要存储的配置信息。
        /// </summary>
        /// <returns>true表示有需要存储的配置，false表示不需要调用存储的方法。</returns>
        public bool HasContent()
        {
            bool hasContent = false;
            if (StringDictionary.Count == 0 && BooleanDictionary.Count == 0 &&
                Int32Dictionary.Count == 0 && DoubleDictionary.Count == 0)
            {
                hasContent = false;
            }
            else
            {
                hasContent = true;
            }
            return hasContent;
        }
    }
}
