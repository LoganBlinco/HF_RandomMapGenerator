using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.Xml.Serialization;
using UnityEngine;

public class MoistureValues 
{
	public float Dryest = 0f;
	public float Dryer = 0.2f;
	public float Dry = 0.3f;
	public float Wet = 0.5f;
	public float Wetter = 0.7f;
	public float Wettest = 1f;

    public static string folderPath = @"\Resources\BiomeMapSettings\";

    public MoistureValues()
    {

    }

    public void GenerateDictionary()
    {
        BiomeInfomation.GenerateMoistureDictionary(this);
    }

    #region XML
    /*
    /// <summary>
    /// Outputs object to XML file to the Resources folder
    /// Code taken from: https://stackoverflow.com/questions/13266496/easily-write-a-whole-class-instance-to-xml-file-and-read-back-in
    /// </summary>
    /// <param name="name">File name</param>
    public void OutputToXML(string name)
    {
        // TODO init your garage..

        XmlSerializer xs = new XmlSerializer(typeof(MoistureValues));
        //TextWriter tw = new StreamWriter(@"c:\temp\garage.xml");
        string path = Application.dataPath + folderPath + name + ".xml";
        TextWriter tw = new StreamWriter(path);
        xs.Serialize(tw, this);
        tw.Close();
        Debug.Log("XML output done");
    }

    public static MoistureValues InputFromXML(string name)
    {
        string path = Application.dataPath + folderPath + name + ".xml";
        using (var sr = new StreamReader(@path))
        {
            XmlSerializer xs = new XmlSerializer(typeof(MoistureValues));
            MoistureValues temp = (MoistureValues)xs.Deserialize(sr);
            temp.GenerateDictionary();
            return temp;
        }
    }
    */
    #endregion
}
