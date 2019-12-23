using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadOSMTiles
{
    public class MapPictureBox : PictureBox
    {
        public TileBlock tileBlock;
        bool m_drawXY = true;
        int m_x, m_y;
        
        public MapPictureBox(int x, int y)
        {
            this.Paint += MapPictureBox_Paint;
            SizeMode = PictureBoxSizeMode.Normal;
            m_x = x;
            m_y = y;
        }
        public int X
        {
            get
            {
                return m_x;
            }
        }
        public int Y
        {
            get
            {
                return m_y;
            }
        }

        private void MapPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (m_drawXY == true)
            {
                e.Graphics.DrawString(tileBlock.x + "," + tileBlock.y, this.Font, Brushes.Red, 256 / 2, 256 / 2);
            }
        }
    }
}
