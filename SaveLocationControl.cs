using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadOSMTiles
{
    public partial class SaveLocationControl : UserControl
    {
        TileBlock m_tile;
        public delegate void Callback(int code, int index, string name, TileBlock t);
        Callback pCallback;
        int m_index = -1;
        string m_name;
        public SaveLocationControl()
        {
            InitializeComponent();
        }

        public void SetCallback(Callback p)
        {
            pCallback = p;
        }
        public void Setup(int index, string name, TileBlock tile)
        {
            m_index = index;
            m_name = name;
            label1.Text = name + "   zoom: " + tile.zoom;
            label2.Text = tile.lon + "," + tile.lat + " " + tile.x + "," + tile.y + "";
            m_tile = tile;
        }
        public void GetData(out TileBlock tile, out string name)
        {
            tile = m_tile;
            name = m_name;
        }
        public void UpdateLocation(int index)
        {
            m_index = index;
            this.Location = new System.Drawing.Point(4, index * this.Size.Height);
        }
        private void btnSelect_Click(object sender, EventArgs e)
        {
            pCallback(0, m_index, m_name, m_tile);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            pCallback(1, m_index , m_name, m_tile);
        }
    }
}
