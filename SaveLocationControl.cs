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
        public delegate void Callback(TileBlock t);
        Callback pCallback;
        public SaveLocationControl()
        {
            InitializeComponent();
        }

        public void SetCallback(Callback p)
        {
            pCallback = p;
        }
        public void Setup(string name, TileBlock tile)
        {
            label1.Text = name + "   zoom: " + tile.zoom;
            label2.Text = tile.lon + "," + tile.lat + " " + tile.x + "," + tile.y + "";
            m_tile = tile;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            pCallback(m_tile);
        }
    }
}
