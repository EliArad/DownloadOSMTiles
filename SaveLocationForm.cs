using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadOSMTiles
{
    public partial class SaveLocationForm : Form
    {
        int m_index = 0;
        public SaveLocationForm()
        {
            InitializeComponent();
        }
        public void Load(Dictionary<string, TileBlock> HistoryBlocks, SaveLocationControl.Callback p)
        {
           
            foreach (KeyValuePair <string, TileBlock> hb in HistoryBlocks)
            {
                SaveLocationControl s = AddLocation();
                m_index++;
                s.Setup(hb.Key, hb.Value);
                s.SetCallback(p);
                panel1.Controls.Add(s);
            }
        }
      
        SaveLocationControl AddLocation()
        {
            SaveLocationControl s = new SaveLocationControl();
            s.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));            
            s.Name = "saveLocationControl1";
            s.Size = new System.Drawing.Size(624, 63);
            s.Location = new System.Drawing.Point(4, m_index * s.Size.Height);
            s.TabIndex = m_index;

            return s;
        }

    }
}
