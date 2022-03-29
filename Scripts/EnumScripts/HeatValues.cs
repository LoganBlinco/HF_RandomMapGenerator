using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.Xml.Serialization;
using UnityEngine;

namespace eLF_RandomMaps
{
    public class HeatValues
    {
        public float Coldest = 0f;
        public float Colder = 0.2f;
        public float Cold = 0.3f;
        public float Hot = 0.5f;
        public float Hotter = 0.7f;
        public float Hottest = 1f;

        //public static string folderPath = @"\Resources\BiomeMapSettings\";

        public HeatValues()
        {

        }

        public void GenerateDictionary()
        {
            BiomeInfomation.GenerateHeatDictionary(this);
        }

        #region XML

        /// <summary>
        /// Outputs object to XML file to the Resources folder
        /// Code taken from: https://stackoverflow.com/questions/13266496/easily-write-a-whole-class-instance-to-xml-file-and-read-back-in
        /// </summary>
        /// <param name="name">File name</param>
        /// 
        /*
        public void OutputToXML(string name)
        {
            // TODO init your garage..

            XmlSerializer xs = new XmlSerializer(typeof(HeatValues));
            //TextWriter tw = new StreamWriter(@"c:\temp\garage.xml");
            string path = Application.dataPath + folderPath + name + ".xml";
            TextWriter tw = new StreamWriter(path);
            xs.Serialize(tw, this);
            tw.Close();
            Debug.Log("XML output done");
        }

        public static HeatValues InputFromXML(string name)
        {
            string path = Application.dataPath + folderPath + name + ".xml";
            using (var sr = new StreamReader(@path))
            {
                XmlSerializer xs = new XmlSerializer(typeof(HeatValues));
                HeatValues temp = (HeatValues)xs.Deserialize(sr);
                temp.GenerateDictionary();
                return temp;
            }
        }
        */
        #endregion
    }
}