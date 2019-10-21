using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DownloadOSMTiles
{
    public partial class MapControl : UserControl
    {
        List<TileBlock> tilesBlock = new List<TileBlock>();
        public MapControl()
        {
            InitializeComponent();
        }
        public bool LoadMapData(string mapfile, out string outMessage)
        {
            outMessage = string.Empty;
            try
            {
                TileDB db = new TileDB(mapfile);
                if (db.Load(out tilesBlock) == "ok")
                {                     
                    return true;
                }
                this.BackColor = Color.Black;
                return false;
            }
            catch (Exception err)
            {
                outMessage = err.Message;
                return false;
            }

        }
        public void ShowLatLon(float lat , float lon, int zoom)
        {
            var q = from ll in tilesBlock
                    where ll.lat >= lat && ll.lon >= lon && ll.zoom == zoom                   
                    select ll;
        }
        
        public void ShowLatLon(string name, int zoom)
        {
            try
            {
                this.Controls.Clear();
                var qresult = (from ll in tilesBlock
                               where ll.name.ToLower() == name.ToLower() && ll.zoom == zoom
                               orderby ll.x ascending
                               select ll).ToList();
                if (qresult.Count == 0)
                    return;

                int x = 0;
                int y = 0;
                int startx = qresult[0].x;
                int starty = qresult[0].y;
                for (int i = 0; i < (10 * 10); i++)
                {

                    var q = (from ll in qresult
                             where ll.x == startx && ll.y == starty
                             select ll);

                    if (q.Any())
                    {
                        TileBlock r = q.SingleOrDefault();

                        PictureBox b;
                        if (r.bitmap != null)
                        {
                            b = AddTile(x, y, i, r.bitmap);
                        }
                        else
                        {
                            b = AddTile(x, y, i, r.fileName);
                        }
                        b.SizeMode = PictureBoxSizeMode.Normal;
                        this.Controls.Add(b);
                        startx++;
                        x++;
                    }
                    else
                    {
                        startx = qresult[0].x;
                        starty++;
                        y++;
                        x = 0;
                    }

                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        PictureBox AddTile(int x , int y, int i, Bitmap b)
        {
            PictureBox pb  = new PictureBox();
            pb.Name = "pictureBox" + i;
            pb.Size = new System.Drawing.Size(b.Width,b.Height);
            pb.TabIndex = i;
            pb.TabStop = false;
            pb.Location = new System.Drawing.Point(x * pb.Size.Width, y * pb.Size.Height);
            pb.Image = b;
            return pb;
        }
        PictureBox AddTile(int x, int y, int i, string fileName)
        {
            int width = 256;
            int height = 256;
            PictureBox pb = new PictureBox();
            pb.Name = "pictureBox" + i;
            pb.Size = new System.Drawing.Size(width, height);
            pb.TabIndex = i;
            pb.TabStop = false;
            pb.Location = new System.Drawing.Point(x * pb.Size.Width, y * pb.Size.Height);
            pb.ImageLocation = fileName;
            return pb;
        }
    }
}
