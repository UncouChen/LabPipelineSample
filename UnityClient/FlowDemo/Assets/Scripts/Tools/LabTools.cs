using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LabData
{
    public class LabTools
    {

        public static string DataPath => Application.dataPath;
        /// <summary>
        /// 创建存储数据的文件夹
        /// </summary>
        /// <param name="floderName"></param>
        /// <param name="isNew"></param>
        public static string CreatSaveDataFolder(string floderName, bool isNew = false)
        {
            if (Directory.Exists(floderName))
            {
                if (isNew)
                {
                    var tempPath = floderName + "_" + DateTime.Now.Millisecond.ToString();
                    Directory.CreateDirectory(tempPath);
                    return tempPath;
                }

                Debug.Log("Folder Has Existed!");
                return floderName;
            }
            else
            {
                Directory.CreateDirectory(floderName);
                Debug.Log("Success Create: " + floderName);
                return floderName;
            }
        }
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="path"></param>
        public static void CreatData(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();

                Debug.Log("Success Create: " + path);
            }
            else
            {
                Debug.Log("File Has Existed!");
            }
        }
        /// <summary>
        /// 获取Enum的Description内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(T obj)
        {
            var type = obj.GetType();
            FieldInfo field = type.GetField(Enum.GetName(type, obj));
            DescriptionAttribute descAttr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (descAttr == null)
            {
                return string.Empty;
            }

            return descAttr.Description;
        }

        /// <summary>
        /// 根据Config类型反序列化，如果是需要覆盖旧的config，传入true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isNew"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T GetConfig<T>(bool isNew = false,string filePath="/GameData") where T : class, new()
        {
            var path = DataPath+filePath + "/" + "ConfigData";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = path + "/" + typeof(T).Name + ".json";


            if (isNew && File.Exists(path))
            {
                File.Delete(path);
            }
            if (!File.Exists(path))
            {
                var json = JsonConvert.SerializeObject(new T());
                var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }

            StreamReader sr = new StreamReader(path);
            var data = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
            sr.Close();
            return data;
        }

        /// <summary>
        /// 创建对应数据的文件夹
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CreateDataFolder<T>(string filePath = "/GameData") where T : class
        {
            var path = DataPath+ filePath + "/" + typeof(T).Name;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 写数据，数据类型必须继承LabDataBase，dataName为需要写的数据名字，isOverWrite选择是否要覆盖已有文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="dataName"></param>
        /// <param name="isOverWrite"></param>
        /// <returns></returns>
        public static void WriteData<T>(T t, string dataName, bool isOverWrite = false, string filePath = "/GameData") where T : class, new()
        {
            var path = DataPath+filePath + "/" + typeof(T).Name + "/" + dataName + ".json";

            if (!File.Exists(path))
            {
                var json = JsonConvert.SerializeObject(t);
                var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
            else if (File.Exists(path) && isOverWrite)
            {
                var json = JsonConvert.SerializeObject(t);
                var fs = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite);
                fs.Close();
                fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
            else
            {
                Debug.LogError("需要重写数据，请在参数中设置isOverWrite=true");
            }
        }

        /// <summary>
        /// 通过类型T和名字获取指定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="dataName"></param>
        /// <returns></returns>
        public static T GetData<T>(string dataName, string filePath = "/GameData") where T : class
        {
            var path = DataPath+ filePath + "/" + typeof(T).Name + "/" + dataName + ".json";

            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);
                var data = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                sr.Close();
                return data;

            }
            else
            {
                Debug.LogError("数据文件不存在！");
                return null;
            }
        }

    }
}

