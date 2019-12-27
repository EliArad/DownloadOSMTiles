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
using static DownloadOSMTiles.MouseHook;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace DownloadOSMTiles
{
    public partial class MapControl : UserControl
    {
        public enum DRAW_SHAPE
        {
            NONE,
            LINE,
            CIRCLE,
            CIRCLE_FILL,
            RECT,
            TRIANGLE
        }

        const int RDW_INVALIDATE = 0x0001;
        const int RDW_ALLCHILDREN = 0x0080;
        const int RDW_UPDATENOW = 0x0100;
        [DllImport("User32.dll")]
        static extern bool RedrawWindow(IntPtr hwnd, IntPtr rcUpdate, IntPtr regionUpdate, int flags);


        [DllImport("user32")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);

        // you also need ReleaseDC
        [DllImport("user32")]
        private static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);

        int MAX_TILES = 7;
        public delegate void MapControlCallback(TileBlock t, int code, int mouseX, int mouseY);

        public delegate void MapMsgCallack(int code, string msg);
        public delegate void MapControlZoomCallback(TileBlock t, int mapX , int mapY, bool zoomIn);

        int m_lastXTile = 0;
        int m_lastYTile = 0;
        DRAW_SHAPE m_drawShape = DRAW_SHAPE.NONE;
        List<TileBlock> tilesBlock = new List<TileBlock>();
        List<MapPictureBox> m_allTiles = new List<MapPictureBox>();
        public MapControl()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            this.DoubleBuffered = true;
 
        }
	    public void LoadControl()
	    { 

           
            MouseHook.LeftMouseDownAction += new EventX2Handler(LeftMouseDownEvent);
            MouseHook.LeftMouseUpAction += new EventX2Handler(LeftMouseUpEvent);
            MouseHook.MoveMouseAction += new EventXHandler(MoveMouseEvent);
             
             
        } 

        public void Start()
        {
            MouseHook.Start();
        }
        public void Stop()
        {
            MouseHook.Stop();
        }


        Panel m_panel;
        PictureBox pictureBox1;
        List<PictureBox> m_allPB = new List<PictureBox>();
       
        public void AddBitmap()
        {
            AllowDrop = true;
            PictureBox p = new PictureBox();

            p.Location = new System.Drawing.Point(316, 258);
            p.Name = "pictureBox1";
            p.Size = new System.Drawing.Size(100, 50);
            p.TabIndex = 2;
            p.TabStop = false;
            p.ImageLocation = "images.jfif";
            p.SizeMode = PictureBoxSizeMode.StretchImage;
            Controls.Add(p);
            p.BringToFront();
           
            p.AllowDrop = true;
            m_allPB.Add(p);
        }

        public void MoveBitmap()
        {
             for(int i = 0; i < 500; i++)
             {
                m_allPB[0].Left = this.Left + i * 2;                   
             }
        }

        public bool LoadMapData(string mapfile, out string outMessage)
        {
            outMessage = string.Empty;
            try
            {
                TileDB db = new TileDB(mapfile);
                if (db.Load(out tilesBlock) == "ok")
                {
                    m_missingTiles = false;
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
                                out int tiley,
                                out int pixelX_Excact,
                                out int pixelY_Excact)

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

            pixelX_Excact = (int)(ClipByRange((tilex * 256) + ((X - tilex) * 256), mapSize - 1));
            pixelY_Excact = (int)(ClipByRange((tiley * 256) + ((Y - tiley) * 256), mapSize - 1));
        }
         
        int m_startx;
        int m_starty;
        string OSM_PATH = @"C:\OSMTiles\";
         

        public bool ShowLatLon(string name, int zoom, double lat, double lon, int mapX, int mapY, out string outMessage)
        {
            int pixelX_Excact;
            int pixelY_Excact;

            LatLongToPixelXYOSM(lat, lon, zoom,
                                out int pixelX,
                                out int pixelY,
                                out int tilex,
                                out int tiley,
                                out  pixelX_Excact,
                                out  pixelY_Excact);

            

            int x = 0;
            int y = 0;
            m_allTiles.Clear();
            this.Controls.Clear();
            outMessage = string.Empty;
            int lastCurrentPixelX = m_currentPixelX;
            int lastCurrentPixelY = m_currentPixelY;

            // draw selected tile 
            //if (AddMapTile(name, tilex, tiley, zoom, mapX, mapY,  out outMessage) == false)
            // return false;

            // draw all x until y
            for (int j = 0; j < mapY; j++)
            {
                for (int i = 0; i < MAX_TILES; i++)
                {
                    if (AddMapTile(name, tilex - mapX + i, tiley - mapY + j, zoom, i, j, out outMessage) == false)
                        return false;
                }
            }

            // draw all y to the end 
            for (int j = mapY + 1; j < MAX_TILES; j++)
            {
                for (int i = 0; i < MAX_TILES; i++)
                {
                    if (AddMapTile(name, tilex - mapX + i, tiley - mapY + j, zoom, i, j, out outMessage) == false)
                        return false;
                }
            }


            for (int i = 0; i < MAX_TILES; i++)
            {
                if (AddMapTile(name, tilex - mapX + i, tiley, zoom, i, mapY, out outMessage) == false)
                    return false;
            }


            Application.DoEvents();
            Thread.Sleep(0);
            Application.DoEvents();
            while (lastCurrentPixelX == m_currentPixelX || lastCurrentPixelY == m_currentPixelY)
            {
                Thread.Sleep(0);
                Application.DoEvents();
            }

            for (int i = 0; i < m_allTiles.Count; i++)
            {
                m_allTiles[i].Left -= pixelX_Excact - m_currentPixelX;
                m_allTiles[i].Top -= pixelY_Excact - m_currentPixelY;
            }
             
            
            m_lastXTile = MAX_TILES;
            m_lastYTile = MAX_TILES;

            return true;
        }
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);


        bool AddMapTileByLocation(string name, int tilex, int tiley, int zoom, int mapX, int mapY, out string outMessage)
        {
            outMessage = string.Empty;
            // Add Center Tile:            
            var q = (from ll in tilesBlock
                     where ll.name.ToLower() == name.ToLower() && ll.zoom == zoom &&
                     ll.x == (tilex) && ll.y == (tiley)
                     select ll).ToList();

            if (q.Count == 0)
            {
                outMessage = "Missing tiles," + (tilex) + "," + (tiley) + "," + zoom;
                return false;
            }
            if (q.Count > 1)
            {
                outMessage = "Error : Found 2 tiles for," + (tilex) + "," + (tiley) + "," + zoom;
                return false;
            }

            TileBlock r = q.SingleOrDefault();
            MapPictureBox b;

            b = AddTileByLocation(0,0,mapX, mapY, r.fileName);

            m_allTiles.Add(b);

            b.tileBlock = r;
            this.Controls.Add(b);
            return true;
        }

        bool AddMapTile(string name, int tilex , int tiley, int zoom, int mapX, int mapY, out string outMessage)
        {
            outMessage = string.Empty;
            // Add Center Tile:            
            var q = (from ll in tilesBlock
                     where ll.name.ToLower() == name.ToLower() && ll.zoom == zoom &&
                     ll.x == (tilex) && ll.y == (tiley)
                     select ll).ToList();

            if (q.Count == 0)
            {
                outMessage = "Missing tiles," + (tilex) + "," + (tiley) + "," + zoom;
                return false;
            }
            if (q.Count > 1)
            {
                outMessage = "Error : Found 2 tiles for," + (tilex) + "," + (tiley) + "," + zoom;
                return false;
            }

            TileBlock r = q.SingleOrDefault();
            MapPictureBox b;

            b = AddTile(mapX, mapY,r.fileName);

            m_allTiles.Add(b);

            b.tileBlock = r;
            this.Controls.Add(b);
            return true;
        }
         
        public bool ShowLatLon(string name, int zoom, double lat, double lon, out string outMessage)
        {
            LatLongToPixelXYOSM(lat, lon, zoom,
                                out int pixelX,
                                out int pixelY,
                                out int tilex,
                                out int tiley,
                                out int pixelX_Excact,
                                out int pixelY_Excact);


            int x = 0;
            int y = 0;           
            this.Controls.Clear();
            m_allTiles.Clear();
           
            outMessage = string.Empty;
            for (int j = 0; j < MAX_TILES; j++)
            {
                x = 0;
                for (int i = 0; i < MAX_TILES; i++)
                {
                    var q = (from ll in tilesBlock
                             where ll.name.ToLower() == name.ToLower() && ll.zoom == zoom &&
                             ll.x == (tilex + i) && ll.y == (tiley + j)
                             select ll).ToList();

                    if (q.Count == 0)
                    {
                        outMessage = "Missing tiles," + (tilex + i) + "," + (tiley + j) + "," + zoom;
                        return false;
                    }
                    if (q.Count > 1)
                    {
                        outMessage = "Error : Found 2 tiles for," + (tilex + i) + "," + (tiley + j) + "," + zoom;
                        return false;
                    }

                    TileBlock r = q.SingleOrDefault();
                    MapPictureBox b;

                    b = AddTile(x, y, r.fileName);
                    m_allTiles.Add(b);

                    b.tileBlock = r;
                    this.Controls.Add(b);
                    x++;
                }
                y++;
            }
            m_lastXTile = MAX_TILES;
            m_lastYTile = MAX_TILES;
             

            return true;
        }
         
        MapPictureBox AddTile(int x, int y, Bitmap b)
        {
            MapPictureBox pb = new MapPictureBox(x,y, m_allTiles.Count);
            pb.Name = "pictureBox" + (m_allTiles.Count);
            pb.Size = new System.Drawing.Size(b.Width, b.Height);
            pb.TabIndex = (m_allTiles.Count);
            pb.TabStop = false;
            pb.MouseWheel += Pb_MouseWheel;
            pb.MouseMove += Pb_MouseMove;
            pb.Location = new System.Drawing.Point(x * pb.Size.Width, y * pb.Size.Height);
            pb.Image = b;
            return pb;
        }

        int m_rightMostTile = 1111111;
        int m_topMostTile = 0;
        int m_bottomMostTile = 0;
        int m_LeftMostTile = 0;
        bool m_missingTiles = false;
        MapPictureBox pbCurrent;
        int m_currentPixelX;
        int m_currentPixelY;
        int m_lastMouseTileX;
        private void Pb_MouseMove(object sender, MouseEventArgs e)
        {

            pbCurrent = (MapPictureBox)sender;
            m_lastMouseTileX = e.X;
            m_currentPixelX = pbCurrent.GetTileProp().pixelx + e.X;
            m_currentPixelY = pbCurrent.GetTileProp().pixely + e.Y;           
            pMapControlCallback(pbCurrent.tileBlock, 1, e.X, e.Y);
            if (m_missingTiles == false)
            {
                string outMessage;
                m_rightMostTile = m_allTiles.Max(n => n.Right);
                if ((this.Right - this.Left) > m_rightMostTile)
                {
                    if (AddRowTilesOnTheRight(m_allTiles[0].GetTileProp().name, out outMessage) == false)
                    {
                        m_missingTiles = true;
                        pMapMsgCallack(8912, outMessage);
                    }
                }
                m_topMostTile = m_allTiles.Min(n => n.Top);
                if (m_topMostTile > this.Top)
                {
                    if (AddRowTilesOnTheTop(m_allTiles[0].GetTileProp().name, out outMessage) == false)
                    {
                        m_missingTiles = true;
                        pMapMsgCallack(8912, outMessage);
                    }
                }
                m_bottomMostTile = m_allTiles.Max(n => n.Bottom);
                if (m_bottomMostTile < this.Bottom)
                {
                    if (AddRowTilesOnTheBottom(m_allTiles[0].GetTileProp().name, out outMessage) == false)
                    {
                        m_missingTiles = true;
                        pMapMsgCallack(8912, outMessage);
                    }
                }

                m_LeftMostTile = m_allTiles.Min(n => n.Left);
                if (m_LeftMostTile > 0)
                {
                    
                    if (AddRowTilesOnTheLeft(m_allTiles[0].GetTileProp().name, out outMessage) == false)
                    {
                        m_missingTiles = true;
                        pMapMsgCallack(8912, outMessage);
                    }                    
                }
            }
        }

        public enum MOUSE_DIRECTION
        {
            NONE,
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        int INC_SIZE = 10;
        MapControlCallback pMapControlCallback;
        MapControlZoomCallback pMapControlZoomCallback;
        MapMsgCallack pMapMsgCallack;
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
        public void SetCallback(MapControlCallback p, MapControlZoomCallback p1, MapMsgCallack p2)
        {
            pMapControlCallback = p;
            pMapControlZoomCallback = p1;
            pMapMsgCallack = p2;
        }

        MapPictureBox AddTile(int x, int y, string fileName)
        {
            int width = 256;
            int height = 256;
            MapPictureBox pb = new MapPictureBox(x,y, m_allTiles.Count);
            pb.Name = "pictureBox" + (m_allTiles.Count);
            pb.Size = new System.Drawing.Size(width, height);
            pb.TabIndex = m_allTiles.Count;
            pb.TabStop = false;
            pb.MouseWheel += Pb_MouseWheel;
            pb.MouseMove += Pb_MouseMove;
            pb.Location = new System.Drawing.Point(x * pb.Size.Width, y * pb.Size.Height);
            pb.ImageLocation = fileName;
            return pb;
        }

        MapPictureBox AddTileByLocation(int x, int y, int locx, int locy, string fileName)
        {
            int width = 256;
            int height = 256;
            MapPictureBox pb = new MapPictureBox(x, y, m_allTiles.Count);
            pb.Name = "pictureBox" + (m_allTiles.Count);
            pb.Size = new System.Drawing.Size(width, height);
            pb.TabIndex = m_allTiles.Count;
            pb.TabStop = false;
            pb.MouseWheel += Pb_MouseWheel;
            pb.MouseMove += Pb_MouseMove;
            pb.Location = new System.Drawing.Point(locx, locy);
            pb.ImageLocation = fileName;
            return pb;
        }


        private void Pb_MouseWheel(object sender, MouseEventArgs e)
        {
            MapPictureBox p = sender as MapPictureBox;

            if (ModifierKeys.HasFlag(Keys.Control))
            {

                if (e.Delta > 0)
                {
                    pMapControlZoomCallback(p.tileBlock, p.X, p.Y, true);
                }
                else
                {

                    pMapControlZoomCallback(p.tileBlock, p.X, p.Y, false);
                }
            }
        }
        POINT m_lastDrawLinePoint;
        
        public void DrawCircle(POINT pt, int radius,Color color, bool draw)
        {
             
            if (draw)
            {
                IntPtr hdc = GetWindowDC(this.Handle);
                Graphics g = Graphics.FromHdc(hdc);
                Pen pen = new Pen(color);

                g.DrawEllipse(pen, pt.x - this.Left, pt.y - this.Top - 20,
                              radius + radius, radius + radius);
                 
                 
                g.Dispose();
                ReleaseDC(this.Handle, hdc);
            } 
           
        }

        void DrawFillCircle(POINT pt, int radius, Color color, bool draw)
        {
            if (draw)
            {
                IntPtr hdc = GetWindowDC(this.Handle);
                Graphics g = Graphics.FromHdc(hdc);
                Pen pen = new Pen(color);
                SolidBrush brush = new SolidBrush(color);
                g.FillEllipse(brush, pt.x - this.Left, pt.y - this.Top - 20,
                            radius + radius, radius + radius);

                g.Dispose();
                ReleaseDC(this.Handle, hdc);
            }


        }
        public void DrawLine(POINT pt, bool draw)
        {
            if (draw)
            {
                IntPtr hdc = GetWindowDC(this.Handle);
                Graphics g = Graphics.FromHdc(hdc);
                Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
                g.DrawLine(pen, m_lastDrawLinePoint.x - this.Left,
                                m_lastDrawLinePoint.y - this.Top - 20,
                                pt.x - this.Left, pt.y - this.Top - 20);
                g.Dispose();
                ReleaseDC(this.Handle, hdc);
            }
            m_lastDrawLinePoint = pt;
        }
        [DllImport("user32")]
        private static extern bool InvalidateRect(IntPtr hwnd, IntPtr rect, bool bErase);
        public void RedrawWindow()
        {
            //InvalidateRect(IntPtr.Zero, IntPtr.Zero, true);
            RedrawWindow(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);
            ////RedrawWindow(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, RDW_UPDATENOW);

        }
      
        public void ShowPixelXY(bool show)
        {
            foreach (MapPictureBox m in m_allTiles)
            {
                m.DrawPixelXY(show);
            }
        }

        public void ShowXY(bool show)
        {
            foreach (MapPictureBox m in m_allTiles)
            {
                m.DrawXY(show);
            }
        }
        public void ShowBorder(bool show)
        {
            foreach(MapPictureBox m  in m_allTiles)
            {
                m.Border(show == true ? BorderStyle.Fixed3D : BorderStyle.None);
            }
        }

        
        public bool AddRowTilesOnTheRight(string name, out string outMessage)
        {
            outMessage = string.Empty;
            int tilex = m_allTiles.Min(n => n.GetTileProp().x);
            int tiley = m_allTiles.Min(n => n.GetTileProp().y);
            int topMostTile = m_allTiles.Min(n => n.Top);
            for (int i = 0; i < MAX_TILES; i++)
            {
                if (AddDynamicTiles(name,
                                tilex + m_lastXTile,
                                tiley + i,
                                m_allTiles[0].GetTileProp().zoom,  // zoom
                                m_lastXTile, // x on the right 
                                0 + i,
                                m_rightMostTile,
                                topMostTile + i * 256,
                                out outMessage) == false)
                    return false;
            }            
            m_lastXTile += 1;
            return true;
        }

        public bool AddRowTilesOnTheTop(string name, out string outMessage)
        {
            outMessage = string.Empty;
            int tilex = m_allTiles.Min(n => n.GetTileProp().x);
            int tiley = m_allTiles.Min(n => n.GetTileProp().y);
            int topMostTile = m_allTiles.Min(n => n.Top);

            int leftMostTile = m_allTiles.Min(n => n.Left);
            
            for (int i = 0; i < MAX_TILES; i++)
            {
                if (AddDynamicTiles(name,
                                tilex + i,  // tilex
                                tiley - 1,            // tiley
                                m_allTiles[0].GetTileProp().zoom,  // zoom
                                0 + i, //  mapx
                                0,        // mapy
                                leftMostTile + i * 256,    // locx
                                topMostTile - 256,  // locy
                                out outMessage) == false)
                    return false;
            }
            
            return true;
        }
        public bool AddRowTilesOnTheBottom(string name, out string outMessage)
        {
            outMessage = string.Empty;
            int tilex = m_allTiles.Min(n => n.GetTileProp().x);
            int tiley = m_allTiles.Max(n => n.GetTileProp().y);
            int bottomMostTile = m_allTiles.Max(n => n.Bottom);

            int leftMostTile = m_allTiles.Min(n => n.Left);

            for (int i = 0; i < MAX_TILES; i++)
            {
                if (AddDynamicTiles(name,
                                tilex + i,  // tilex
                                tiley + 1,            // tiley
                                m_allTiles[0].GetTileProp().zoom,  // zoom
                                0 + i, //  mapx
                                0,        // mapy
                                leftMostTile + i * 256,    // locx
                                bottomMostTile,  // locy
                                out outMessage) == false)
                    return false;
            }
            
            return true;

        }
        public bool AddRowTilesOnTheLeft(string name, out string outMessage)
        {
            outMessage = string.Empty;
            int tilex = m_allTiles.Min(n => n.GetTileProp().x);
            int tiley = m_allTiles.Min(n => n.GetTileProp().y);

             
            int leftMostTile = m_allTiles.Min(n => n.Left);
            int TopMostTile = m_allTiles.Min(n => n.Top);

            for (int i = 0; i < MAX_TILES; i++)
            {
                if (AddDynamicTiles(name,
                                tilex -1,  // tilex
                                tiley + i,            // tiley
                                m_allTiles[0].GetTileProp().zoom,  // zoom
                                0 + i, //  mapx
                                0,        // mapy
                                leftMostTile - 256,    // locx
                                TopMostTile + i * 256,  // locy
                                out outMessage) == false)
                    return false;
            }

            return true;

        }
        bool AddDynamicTiles(string name, 
                                   int tilex, 
                                   int tiley, 
                                   int zoom, 
                                   int mapX, 
                                   int mapY, 
                                   int locx,
                                   int locy,
                                   out string outMessage)
        {
            outMessage = string.Empty;
            // Add Center Tile:            
            var q = (from ll in tilesBlock
                     where ll.name.ToLower() == name.ToLower() && ll.zoom == zoom &&
                     ll.x == (tilex) && ll.y == (tiley)
                     select ll).ToList();

            if (q.Count == 0)
            {
                outMessage = "Missing tiles," + (tilex) + "," + (tiley) + "," + zoom;
                return false;
            }
            if (q.Count > 1)
            {
                outMessage = "Error : Found 2 tiles for," + (tilex) + "," + (tiley) + "," + zoom;
                return false;
            }

            TileBlock r = q.SingleOrDefault();
            MapPictureBox b;

            b = AddTileByLocation(mapX, mapY,locx,locy, r.fileName);

            m_allTiles.Add(b);

            b.tileBlock = r;
            this.Controls.Add(b);
            return true;
        }


        public Dictionary<string, TileBlock> HistoryBlocks;
        
        public string SaveHistory(string fileName)
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
         
        public bool LoadHistory(string fileName)
        {
            try
            {                
                if (File.Exists(fileName) == false)
                {
                    HistoryBlocks = new Dictionary<string, TileBlock>();
                    return false; 
                }
                string text = File.ReadAllText(fileName);
                HistoryBlocks = JsonConvert.DeserializeObject<Dictionary<string, TileBlock>>(text);                
                return true;
            }
            catch (Exception err)
            {
                HistoryBlocks = new Dictionary<string, TileBlock>();
                return false;
            }
        }

        int m_lastMousex = 0;
        int m_lastMousey = 0;
        bool m_leftMouseDown = false;
        private void LeftMouseDownEvent()
        {
            m_leftMouseDown = true;
        }
        private void LeftMouseUpEvent()
        {
            m_leftMouseDown = false;
        }
        private void RightMouseEvent()
        {

        }

        public void DrawShape(DRAW_SHAPE s)
        {
            m_drawShape = s;
        }
        public Bitmap SnapshotMap(string fileName)
        {
            ScreenCapturer c = new ScreenCapturer(this.Left, this.Top, this.Width, this.Height);
            Bitmap result = c.Capture();
            result.Save(fileName, ImageFormat.Jpeg);
            return result;
        }

        private void MoveMouseEvent(POINT pt)
        {
             

            pMapMsgCallack(551, pt.x + "," + pt.y);
            if (m_leftMouseDown && ModifierKeys.HasFlag(Keys.Control))
            {
                if (pt.x < m_lastMousex)
                {
                    //Console.WriteLine("move to left" + pt.x + "," + pt.y);
                    MoveLeft();
                }
                else
                if (pt.x > m_lastMousex)
                {
                    //Console.WriteLine("move to right" + pt.x + "," + pt.y);
                    MoveRight();
                }
                else
                if (pt.y < m_lastMousey)
                {
                    MoveUp();
                }
                else
                if (pt.y > m_lastMousey)
                {
                    MoveDown();
                }
            }
            m_lastMousex = pt.x;
            m_lastMousey = pt.y;

            switch (m_drawShape)
            {
                case DRAW_SHAPE.LINE:
                {
                    DrawLine(pt, m_leftMouseDown);
                }
                break;                 
            } 
        }

          
        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lineToolStripMenuItem.Checked = !lineToolStripMenuItem.Checked;
            if (lineToolStripMenuItem.Checked)
                m_drawShape = DRAW_SHAPE.LINE;
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_drawShape = DRAW_SHAPE.NONE;
        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_drawShape = DRAW_SHAPE.CIRCLE;
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_drawShape = DRAW_SHAPE.RECT;
        }

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_drawShape = DRAW_SHAPE.TRIANGLE;
        }

        private void fillColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_drawShape = DRAW_SHAPE.CIRCLE_FILL;
        }
        const int SRCCOPY = 0xcc0020; // we want to copy an in memory image

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern int BitBlt(
          IntPtr hdcDest,     // handle to destination DC (device context)
          int nXDest,         // x-coord of destination upper-left corner
          int nYDest,         // y-coord of destination upper-left corner
          int nWidth,         // width of destination rectangle
          int nHeight,        // height of destination rectangle
          IntPtr hdcSrc,      // handle to source DC
          int nXSrc,          // x-coordinate of source upper-left corner
          int nYSrc,          // y-coordinate of source upper-left corner
          System.Int32 dwRop  // raster operation code
          );

    }
}
