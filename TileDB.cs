using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
  

namespace DownloadOSMTiles
{
    

    public class TileDB 
    {
        string m_fileName;

        public TileDB(string fileName)
        {
            m_fileName = fileName;
        }

        public string Save(List<TileBlock> tilesBlock)
        {
            try
            {
                 

                string json = JsonConvert.SerializeObject(tilesBlock);
                string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);
                File.WriteAllText(m_fileName, jsonFormatted);
                return "ok";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

          
        public string Load(out List<TileBlock> tilesBlock)
        {
            tilesBlock = null;
            try
            {
               
                if (File.Exists(m_fileName) == false)
                {
                    return "failed";
                }
                string text = File.ReadAllText(m_fileName);
                tilesBlock = JsonConvert.DeserializeObject<List<TileBlock>>(text);
                  
                return "ok";
            }
            catch (Exception err)
            {               
                return err.Message;
            }
        }

        public string Load<T>(out Dictionary<OSMXY, TileBlock> t)
        {
            t = null;
            try
            {

                if (File.Exists(m_fileName) == false)
                {
                    return "failed";
                }
                string text = File.ReadAllText(m_fileName);
                t = JsonConvert.DeserializeObject<Dictionary<OSMXY, TileBlock>>(text);

                return "ok";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
    }

}
