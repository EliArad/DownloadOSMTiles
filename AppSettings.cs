using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
  

namespace DownloadOSMTiles
{
    
    public struct AppConfig
    {
        public Color lineColor;
        public bool  LoadWithZoom;
        
    }
    public class AppSettings
    {
        string m_fileName;

        public AppConfig Config;
        private AppSettings()
        {
            
        }

        public string Save()
        {
            try
            { 

                string json = JsonConvert.SerializeObject(Config);
                string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);
                File.WriteAllText(m_fileName, jsonFormatted);
                return "ok";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        private static AppSettings instance = null;
        private static readonly object padlock = new object();
        public static AppSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new AppSettings();
                        }
                    }
                }
                return instance;
            }
        } 

        public bool Load(string fileName)
        {
            m_fileName = fileName;
            try
            {
               
                if (File.Exists(m_fileName) == false)
                {
                    return false;
                }
                string text = File.ReadAllText(m_fileName);
                Config = JsonConvert.DeserializeObject<AppConfig>(text);
                  
                return true;
            }
            catch (Exception err)
            {               
                return false;
            }
        }
         
    }

}
