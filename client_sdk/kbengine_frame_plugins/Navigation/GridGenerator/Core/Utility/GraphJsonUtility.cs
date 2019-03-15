using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using Newtonsoft.Json;

namespace KBEngine
{

    public class GraphJsonUtility : MonoBehaviour
    {

        static public string filePath = "";

        private static Dictionary<string, string> Dic_Value = new Dictionary<string, string>();

        private static string FileName
        {
            get { return "fileName"; }
        }
        private static string FolderName
        {
            get
            {
                return "FolderName";
            }
        }

        //初始化方法 如有需要，可重载初始化方法
        public static void Init(string pFolderName, string pFileName)
        {
            Dic_Value.Clear();
            Read();
        }

        //读取文件及json数据加载到Dictionary中
        private static void Read()
        {
            if (!Directory.Exists(FolderName))
            {
                Directory.CreateDirectory(FolderName);
            }

            if (File.Exists(FileName))
            {
                FileStream fs = new FileStream(FileName, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                JsonData values = JsonMapper.ToObject(sr.ReadToEnd());
                foreach (var key in values.Keys)
                {
                    Dic_Value.Add(key, values[key].ToString());
                }

                if (fs != null)
                {
                    fs.Close();
                }

                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        //将Dictionary数据转成json保存到本地文件
        private static void Save()
        {
            string values = JsonMapper.ToJson(Dic_Value);
            Debug.Log(values);
            if (!Directory.Exists(FolderName))
            {
                Directory.CreateDirectory(FolderName);
            }

            FileStream file = new FileStream(FileName, FileMode.Create);

            byte[] bts = System.Text.Encoding.UTF8.GetBytes(values);

            file.Write(bts, 0, bts.Length);

            if (file != null)
            {
                file.Close();
            }

        }

        public static bool ExportGridDatas(object gridData)
        {
            if (filePath == "")
                filePath = Application.dataPath + @"/GraphCaches/JsonGridGraphData.json";

            string values = JsonConvert.SerializeObject(gridData);
            StreamWriter sw = new StreamWriter(filePath);
            sw.Write(values);
            sw.Close();
            sw.Dispose();
            return true;
        } 
    }
}

