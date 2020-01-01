using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadOSMTiles
{
    public partial class SaveLocationForm : Form
    {
        int m_index = 0;
        SaveLocationControl.Callback pCallback;
        List<SaveLocationControl> m_list = new List<SaveLocationControl>();
        string m_historyFileName;
        Dictionary<string, TileBlock> m_historyBlocks = new Dictionary<string, TileBlock>();
        public SaveLocationForm(string historyFileName, bool loadWithZoom)
        {
            InitializeComponent();
            this.Width = 725;
            this.KeyPreview = true;
            panel1.Width = 725;
            m_historyFileName = historyFileName;
            this.ActiveControl = txtSearch;
            chkWithZoom.Checked = loadWithZoom;
        }
        
        public void LoadHistory(Dictionary<string, TileBlock> HistoryBlocks, SaveLocationControl.Callback p)
        {
            pCallback = p;
            m_historyBlocks = HistoryBlocks;
            LoadHistory();
        }
        void LoadHistory()
        {
            panel1.Controls.Clear();
            m_list.Clear();
            m_index = 0;
            foreach (KeyValuePair<string, TileBlock> hb in m_historyBlocks)
            {
                SaveLocationControl s = AddLocation();
                s.Setup(m_index, hb.Key, hb.Value);
                m_index++;
                s.SetCallback(pCallback);
                panel1.Controls.Add(s);
                m_list.Add(s);
            }
        }

        void LoadHistory(string name)
        {
            panel1.Controls.Clear();
            m_list.Clear();
            m_index = 0;
            foreach (KeyValuePair<string, TileBlock> hb in m_historyBlocks)
            {
                if (hb.Key.Contains(name))
                {
                    SaveLocationControl s = AddLocation();
                    s.Setup(m_index, hb.Key, hb.Value);
                    m_index++;
                    s.SetCallback(pCallback);
                    panel1.Controls.Add(s);
                    m_list.Add(s);
                }
            }
        }
        public void DeleteEntry(int index, string name)
        {
            panel1.Controls.RemoveAt(index);
            m_list.RemoveAt(index);
            m_index = 0;
            foreach (SaveLocationControl s in m_list)
            {
                s.UpdateLocation(m_index);
                m_index++;
            }
            m_historyBlocks.Remove(name);
        }
        SaveLocationControl AddLocation()
        {
            SaveLocationControl s = new SaveLocationControl();
            s.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));            
            s.Name = "saveLocationControl1";
            s.Size = new System.Drawing.Size(710, 63);
            s.Location = new System.Drawing.Point(4, m_index * s.Size.Height);
            s.TabIndex = m_index;

            return s;
        }

        private void SaveLocationForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        public string SaveHistory(string fileName, Dictionary<string, TileBlock> HistoryBlocks)
        {
            try
            {
                string json = JsonConvert.SerializeObject(HistoryBlocks);
                string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);
                File.WriteAllText(fileName, jsonFormatted);
                return "ok";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
        public bool WithZoom()
        {
            return chkWithZoom.Checked;
        }
        public Dictionary<string, TileBlock> GetHistory()
        {
            return m_historyBlocks;
        }
         
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text == string.Empty)
            {
                LoadHistory();
            }
            else
            {
                LoadHistory(txtSearch.Text);
            }
        }

        private void SaveLocationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
    }
}
