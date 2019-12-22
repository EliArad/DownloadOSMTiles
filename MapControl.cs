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
        public delegate void MapControlCallback(TileBlock t, int code ,
                                                MOUSE_DIRECTION direction, string msg);

        public delegate void MapControlZoomCallback(TileBlock t, bool zoomIn);

        bool m_mouseDown = false;
        List<TileBlock> tilesBlock = new List<TileBlock>();
        List<MapPictureBox> m_allTiles = new List<MapPictureBox>();
        public MapControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
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
        double ClipByRange(double n, double range)
        {
            return n % range;
        }

        double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }
        void PixelXYToLatLongOSM(int pixelX, int pixelY, int zoomLevel, out double latitude, out double longitude)
        {
            int mapSize = (int)Math.Pow(2, zoomLevel) * 256;
            //int tileX = (int)Math.Truncate((decimal)(pixelX / 256));
            //int tileY = (int)Math.Truncate((decimal)pixelY / 256);

            double n = (double)Math.PI - ((2.0f * (double)Math.PI * (ClipByRange(pixelY, mapSize - 1) / 256)) / (double)Math.Pow(2.0, zoomLevel));

            longitude = ((ClipByRange(pixelX, mapSize - 1) / 256) / (double)Math.Pow(2.0, zoomLevel) * 360.0f) - 180.0f;
            latitude = (180.0f / (double)Math.PI * (double)Math.Atan(Math.Sinh(n)));
        }
        void LatLongToPixelXYOSM(double latitude, double longitude, int zoomLevel,
                                out int pixelX,
                                out int pixelY,
                                out int tilex,
                                out int tiley)
        {
            double MinLatitude = -85.05112878f;
            double MaxLatitude = 85.05112878f;
            double MinLongitude = -180;
            double MaxLongitude = 180;
            double mapSize = (float)Math.Pow(2, zoomLevel) * 256;

            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            longitude = Clip(longitude, MinLongitude, MaxLongitude);

            double X = (double)((longitude + 180.0f) / 360.0f * (double)(1 << zoomLevel));
            double Y = (double)((1.0 - Math.Log(Math.Tan(latitude * (Math.PI / 180.0)) + 1.0 / Math.Cos(latitude * (Math.PI / 180.0))) / Math.PI) / 2.0 * (1 << zoomLevel));

            tilex = (int)(Math.Truncate(X));
            tiley = (int)(Math.Truncate(Y));
            pixelX = tilex * 256;
            pixelY = tiley * 256;
        }
        public void ShowLatLon(float lat, float lon, int zoom)
        {
            var q = from ll in tilesBlock
                    where ll.lat >= lat && ll.lon >= lon && ll.zoom == zoom
                    select ll;
        }
        int m_startx;
        int m_starty;
        string OSM_PATH = @"C:\OSMTiles\";
        public bool ShowLatLon(string name, int zoom, double lat, double lon, out string outMessage)
        {
            LatLongToPixelXYOSM(lat, lon, zoom,
                                out int pixelX,
                                out int pixelY,
                                out int tilex,
                                out int tiley);

            //string fileName = string.Format("OSM_PATH{0}\\_{1}_{2}_{3}_{4}_{5}.png", name, zoom, tilex, tiley, pixelX, pixelY);
            int x = 0;
            int y = 0;
            int tileIndex = 0;
            m_allTiles.Clear();
            this.Controls.Clear();
            outMessage = string.Empty;
            for (int j = 0; j < 7; j++)
            {
                x = 0;
                for (int i = 0; i < 7; i++)
                {
                    var q = (from ll in tilesBlock
                             where ll.name.ToLower() == name.ToLower() && ll.zoom == zoom &&
                             ll.x == (tilex + i) && ll.y == (tiley + j)
                             select ll).ToList();

                    if (q.Count == 0)
                    {
                        outMessage = (tilex + i) + "," + (tiley + j) + "   Zoom: " + zoom;
                        return false;
                    }
                    if (q.Count > 1)
                    {
                        outMessage = "Error : Found 2 tiles for: " + (tilex + i) + "," + (tiley + j) + "Zoom: " + zoom;
                        return false;
                    }


                    TileBlock r = q.SingleOrDefault();

                    MapPictureBox b;
                    if (r.bitmap != null)
                    {
                        b = AddTile(x, y, tileIndex, r.bitmap);
                    }
                    else
                    {
                        b = AddTile(x, y, tileIndex, r.fileName);
                    }
                    m_allTiles.Add(b);
                    b.SizeMode = PictureBoxSizeMode.Normal;
                    b.tileBlock = r;
                    this.Controls.Add(b);
                    x++;
                    tileIndex++;
                }
                y++;
            }
            return true;
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
                m_startx = qresult[0].x;
                m_starty = qresult[0].y;
                for (int j = 0; j < 5; j++)
                {
                    for (int i = 0; i < 10000; i++)
                    {

                        var q = (from ll in qresult
                                 where ll.x == m_startx && ll.y == m_starty
                                 select ll);

                        if (q.Count() > 0)
                        {
                            TileBlock r = q.SingleOrDefault();

                            MapPictureBox b;
                            if (r.bitmap != null)
                            {
                                b = AddTile(x, y, i, r.bitmap);
                            }
                            else
                            {
                                b = AddTile(x, y, i, r.fileName);
                            }
                            m_allTiles.Add(b);
                            b.SizeMode = PictureBoxSizeMode.Normal;
                            b.tileBlock = r;
                            this.Controls.Add(b);
                            m_startx++;
                            x++;
                        }
                        else
                            break;
                    }
                    m_startx = qresult[0].x;
                    m_starty++;
                    y++;
                    x = 0;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        MapPictureBox AddTile(int x, int y, int i, Bitmap b)
        {
            MapPictureBox pb = new MapPictureBox();
            pb.Name = "pictureBox" + i;
            pb.Size = new System.Drawing.Size(b.Width, b.Height);
            pb.TabIndex = i;
            pb.TabStop = false;
            pb.MouseMove += Pb_MouseMove;
            pb.MouseDown += Pb_MouseDown;
            pb.MouseUp += Pb_MouseUp;
            pb.MouseWheel += Pb_MouseWheel;
            pb.Location = new System.Drawing.Point(x * pb.Size.Width, y * pb.Size.Height);
            pb.Image = b;
            return pb;
        }

        private void Pb_MouseUp(object sender, MouseEventArgs e)
        {
            m_direction = MOUSE_DIRECTION.NONE;
            m_mouseDown = false;
        }

        int m_firstMouseX;
        int m_firstMouseY;

        int m_mouseX;
        int m_mouseY;
        MOUSE_DIRECTION m_direction = MOUSE_DIRECTION.NONE;
        public enum MOUSE_DIRECTION
        {
            NONE,
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        private void Pb_MouseDown(object sender, MouseEventArgs e)
        {
            m_mouseDown = true;
        }
        int INC_SIZE = 10;
        MapControlCallback pMapControlCallback;
        MapControlZoomCallback pMapControlZoomCallback;
        public void MoveLeft()
        {
            for (int i = 0; i < m_allTiles.Count; i++)
            {
                m_allTiles[i].Left -= INC_SIZE;
            }
        }
        public void MoveRight()
        {
            for (int i = 0; i < m_allTiles.Count; i++)
            {
                m_allTiles[i].Left += INC_SIZE;
            }
        }
        public void MoveDown()
        {
            for (int i = 0; i < m_allTiles.Count; i++)
            {
                m_allTiles[i].Top += INC_SIZE;
            }
        }
        public void MoveUp()
        {
            for (int i = 0; i < m_allTiles.Count; i++)
            {
                m_allTiles[i].Top -= INC_SIZE;
            }
        }
        public void SetCallback(MapControlCallback p, MapControlZoomCallback p1)
        {
            pMapControlCallback = p;
            pMapControlZoomCallback = p1;
        }
        int m_lastMouseX = -1;
        int m_lastMouseY = -1;
        private void Pb_MouseMove(object sender, MouseEventArgs e)
        {
            MapPictureBox p = sender as MapPictureBox;

            if (Math.Abs(m_lastMouseX - e.X) <= 5 && Math.Abs(m_lastMouseY - e.Y) <= 5)
                return;

            
            m_mouseX = e.X;
            m_mouseY = e.Y;
            
            if (m_mouseDown && m_mouseX < m_lastMouseX)
            {               
                m_direction = MOUSE_DIRECTION.LEFT;
                m_lastMouseX = e.X;
            } else 
            if (m_mouseDown && m_mouseX > m_lastMouseX)
            {
                m_direction = MOUSE_DIRECTION.RIGHT;
                m_lastMouseX = e.X;
            }
            else
            if (m_mouseDown && m_mouseY > m_lastMouseY)
            {
                m_direction = MOUSE_DIRECTION.DOWN;
                m_lastMouseY = e.Y;
            }
            else
            if (m_mouseDown && m_mouseY < m_lastMouseY)
            {
                m_direction = MOUSE_DIRECTION.UP;
                m_lastMouseY = e.Y;
            }

            pMapControlCallback(p.tileBlock , 0 , m_direction, string.Empty);
                
        }

        MapPictureBox AddTile(int x, int y, int i, string fileName)
        {
            int width = 256;
            int height = 256;
            MapPictureBox pb = new MapPictureBox();
            pb.Name = "pictureBox" + i;
            pb.Size = new System.Drawing.Size(width, height);
            pb.TabIndex = i;
            pb.TabStop = false;
            pb.MouseMove += Pb_MouseMove;
            pb.MouseDown += Pb_MouseDown;
            pb.MouseUp += Pb_MouseUp;
            pb.MouseWheel += Pb_MouseWheel;
            pb.Location = new System.Drawing.Point(x * pb.Size.Width, y * pb.Size.Height);
            pb.ImageLocation = fileName;
            return pb;
        }
       
        private void Pb_MouseWheel(object sender, MouseEventArgs e)
        {
            MapPictureBox p = sender as MapPictureBox;

            if (e.Delta > 0)
            {
                pMapControlZoomCallback(p.tileBlock, true);
            }
            else
            {

                pMapControlZoomCallback(p.tileBlock, false);
            }
        }
    }
}
