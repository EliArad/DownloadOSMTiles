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
        bool m_drawXY = false;
        int m_x, m_y;
        int m_tileNumber = -1;
        bool m_drawTileNumber = false;
        public MapPictureBox(int x, int y, int tileNumber)
        {
            
            this.Paint += MapPictureBox_Paint;
            
         
            SizeMode = PictureBoxSizeMode.Normal;
            m_x = x;
            m_y = y;
            m_tileNumber = tileNumber;
        } 

        public void DrawXY(bool d)
        {
            m_drawXY = d;
            this.Refresh();
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
        public void Border(BorderStyle b)
        {
            this.BorderStyle = b;
        }

        public TileBlock GetTileProp()
        {
            return tileBlock;
        }

        private void MapPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (m_drawXY == true)
            {
                e.Graphics.DrawString(tileBlock.x + "," + tileBlock.y, this.Font, Brushes.Red, 256 / 2, 256 / 2);
            }

            if (m_drawTileNumber == true)
            {
                e.Graphics.DrawString(m_tileNumber.ToString(), this.Font, Brushes.Red, 256 / 2, 256 / 2);
            }
        }
    }
}
