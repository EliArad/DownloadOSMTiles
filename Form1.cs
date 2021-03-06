﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static DownloadOSMTiles.MapControl;
using static DownloadOSMTiles.MouseHook;

namespace DownloadOSMTiles
{
    public partial class Form1 : Form
    {
        string m_baseDir = "d:\\OSMTiles\\";
        bool m_initdone = false;
        string m_historyFileName = "MyHistoryBlock.json";
        public Form1()
        {
            InitializeComponent();
            cmbDrawShape.SelectedIndex = 0;
            KeyPreview = true;
            this.KeyUp += Form1_KeyUp;
            Directory.CreateDirectory(m_baseDir);
 
            MapControlCallback p = new MapControlCallback(MapControlCallbackMsg);
            MapControlZoomCallback p1 = new MapControlZoomCallback(MapControlZoomCallbackMsg);
            MapMsgCallack p2 = new MapMsgCallack(MapMsgCallackFunc);
            mapControl1.SetCallback(p,p1,p2);

            mapControl1.LoadControl(m_baseDir);
            mapControl1.LoadHistory(m_historyFileName);
            this.MouseEnter += Form1_MouseEnter;
            this.MouseLeave += Form1_MouseLeave;
            this.AllowDrop = true;

            if (AppSettings.Instance.Load("DownloadOSMTiles.json") == true)
            {
                mapControl1.LineColor = AppSettings.Instance.Config.lineColor;
                linkLabel1.LinkColor = AppSettings.Instance.Config.lineColor;
            }

        }
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);

            if (lpPoint.x > (mapControl1.Left - 20))
              mapControl1.Start();
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
          
            mapControl1.Stop();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cmbZoom.Text = "9";
            }
            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveLocation();
            }
        }

        void SaveLocation()
        {
            if (txtLocationName.Text == string.Empty)
            {
                MessageBox.Show("Please specify location name to save");
                return;
            }
            if (mapControl1.HistoryBlocks.ContainsKey(txtLocationName.Text) == false)
            {
                TileBlock s = new TileBlock();

                s.x = int.Parse(lblTileX.Text);
                s.y = int.Parse(lblTileY.Text);
                s.pixelx = int.Parse(lblPixelX.Text);
                s.pixely = int.Parse(lblPixelY.Text);

                s.lat = double.Parse(lblLat.Text);
                s.lon = double.Parse(lblLon.Text);
                s.zoom = int.Parse(cmbZoom.Text);
                s.name = txtCreateName.Text;
                mapControl1.HistoryBlocks.Add(txtLocationName.Text, s);
                mapControl1.SaveHistory(m_historyFileName);
                MessageBox.Show("Saved");
            }
            else
            {
                TileBlock s = mapControl1.HistoryBlocks[txtLocationName.Text];
                s.x = int.Parse(lblTileX.Text);
                s.y = int.Parse(lblTileY.Text);
                s.pixelx = int.Parse(lblPixelX.Text);
                s.pixely = int.Parse(lblPixelY.Text);

                s.lat = double.Parse(lblLat.Text);
                s.lon = double.Parse(lblLon.Text);
                s.zoom = int.Parse(cmbZoom.Text);
                s.name = txtCreateName.Text;
                mapControl1.HistoryBlocks[txtLocationName.Text] = s;
                mapControl1.SaveHistory(m_historyFileName);
                MessageBox.Show("Saved");
            }
        }

       
        GeoCoordinate sCoord = null;
        GeoCoordinate eCoord = null;

        List<double> m_latList = new List<double>();
        List<double> m_lonList = new List<double>();
        void MapMsgCallackFunc(int code, string msg)
        {
            switch (code)
            {
                case 9511:
                {
                    m_latList.Add(double.Parse(lblLat.Text));
                    m_lonList.Add(double.Parse(lblLon.Text));
                }
                break;
                case 883:
                {
                    if (eCoord == null || sCoord == null)
                    { 
                        MessageBox.Show("Please mark points before calling calculate");
                        return;
                    }                        
                    double distanceInMeters = sCoord.GetDistanceTo(eCoord);
                    MessageBox.Show("Distance In meters: " + distanceInMeters + Environment.NewLine +
                                    "Distance In Km: " + distanceInMeters / 1000.0 + Environment.NewLine);
                }
                break;
                case 881:
                {                     
                    sCoord = new GeoCoordinate(double.Parse(lblLat.Text), double.Parse(lblLon.Text));
                }
                break;
                case 882:
                {
                    eCoord = new GeoCoordinate(double.Parse(lblLat.Text), double.Parse(lblLon.Text));
                }
                break;
                case 521:
                {
                    SaveLocation();
                }
                break;
                case 551:
                {
                    lblMouseXY.Text = msg;
                }
                break;
                case 8912:
                {
                    string[] s = msg.Split(',');
                    MessageBox.Show("Missing tiles: " + msg);
                    if (s[0] == "Missing tiles")
                    {
                        DialogResult d = MessageBox.Show("Do you want to download missing tiles?", "ELI OSM Control", MessageBoxButtons.YesNo);
                        if (d == DialogResult.Yes)
                        {
                            DownloadFromXY(int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
                        }
                    }
                }
                break;
            }
        }
        void MapControlZoomCallbackMsg(TileBlock tb , int mapX, int mapY, bool zoomIn)
        {
            
            string outMessage;
            if (zoomIn == true)
            {
                if (mapControl1.ShowLatLon(tb.name, tb.zoom + 1, m_latitude, m_longitude, mapX, mapY, out outMessage) == false)
                {
                    string[] s = outMessage.Split(',');
                    MessageBox.Show("Missing tiles: " + outMessage);
                    if (s[0] == "Missing tiles")
                    {
                        DialogResult d = MessageBox.Show("Do you want to download missing tiles?", "ELI OSM Control", MessageBoxButtons.YesNo);
                        if (d == DialogResult.Yes)
                        {
                            DownloadFromXY(int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
                            return;
                        }
                    }
                }
                else
                {
                    cmbZoom.Text = (tb.zoom + 1).ToString();
                }
            }
            else
            {
                if (mapControl1.ShowLatLon(tb.name, tb.zoom - 1, m_latitude, m_longitude, mapX, mapY, out outMessage) == false)
                {
                    string[] s = outMessage.Split(',');
                    MessageBox.Show("Missing tiles: " + outMessage);
                    if (s[0] == "Missing tiles")
                    {
                        DialogResult d = MessageBox.Show("Do you want to download missing tiles?", "ELI OSM Control", MessageBoxButtons.YesNo);
                        if (d == DialogResult.Yes)
                        {
                            DownloadFromXY(int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
                            return;
                        }
                    }
                }
                else
                {
                    cmbZoom.Text = (tb.zoom - 1).ToString();
                }
            }
            mapControl1.ShowXY(showXYToolStripMenuItem.Checked);
            mapControl1.ShowBorder(showBorderToolStripMenuItem.Checked);
        }
        double m_latitude;
        double m_longitude;
        void MapControlCallbackMsg(TileBlock tb, int code, int mouseX, int mouseY)
        {

            lblTileX.Text = tb.x.ToString();
            lblTileY.Text = tb.y.ToString();
            int px = tb.pixelx + mouseX;
            int py = tb.pixely + mouseY;
            lblPixelX.Text = (px).ToString();
            lblPixelY.Text = (py).ToString();

            PixelXYToLatLongOSM(px, py, tb.zoom, out m_latitude, out m_longitude);
            lblLat.Text = m_latitude.ToString();
            lblLon.Text = m_longitude.ToString();




        }
        private void button1_Click(object sender, EventArgs e)
        { 

            Tile t2 = new Tile
            {
                x = 17,
                x_size = 3,
                y = 23,
                y_size = 3,
                name = "new york",
                zoom = 6
            };

            LatLongToPixelXYOSM(40.90039f, -74.18632f, 8, out int pixelX, out int pixelY, out int tilex , out int tiley);
            Tile t3 = new Tile
            {
                x = tilex,
                x_size = 3,
                y = tiley,
                y_size = 3,
                name = "new york",
                zoom = 8
            };


            DownloadTiles(t3, (status, msg, countMissing, countDownload) =>
            {
                MessageBox.Show("Finished");
            });
            

        }

        void PixelXYToLatLongOSM(int pixelX, int pixelY, int zoomLevel, out double latitude, out double longitude)
        {
            int mapSize = (int)Math.Pow(2, zoomLevel) * 256;
            //int tileX = (int)Math.Truncate((decimal)(pixelX / 256));
            //int tileY = (int)Math.Truncate((decimal)pixelY / 256);

            double n = (float)Math.PI - ((2.0f * (double)Math.PI * (ClipByRange(pixelY, mapSize - 1) / 256)) / (double)Math.Pow(2.0, zoomLevel));

            longitude = ((ClipByRange(pixelX, mapSize - 1) / 256) / (double)Math.Pow(2.0, zoomLevel) * 360.0f) - 180.0f;
            latitude = (180.0f / (float)Math.PI * (double)Math.Atan(Math.Sinh(n)));
        }
         

        float ClipByRange(float n, float range)
        {
            return n % range;
        }

        float Clip(float n, float minValue, float maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }
        
        void LatLongToPixelXYOSM(float latitude, float longitude, int zoomLevel, 
                                 out int pixelX, 
                                 out int pixelY,
                                 out int tilex,
                                 out int tiley)
        {
            float MinLatitude = -85.05112878f;
            float MaxLatitude = 85.05112878f;
            float MinLongitude = -180;
            float MaxLongitude = 180;
            float mapSize = (float)Math.Pow(2, zoomLevel) * 256;

            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            longitude = Clip(longitude, MinLongitude, MaxLongitude);

            float X = (float)((longitude + 180.0f) / 360.0f * (float)(1 << zoomLevel));
            float Y = (float)((1.0 - Math.Log(Math.Tan(latitude * (Math.PI / 180.0)) + 1.0 / Math.Cos(latitude * (Math.PI / 180.0))) / Math.PI) / 2.0 * (1 << zoomLevel));

            tilex = (int)(Math.Truncate(X));
            tiley = (int)(Math.Truncate(Y));
            pixelX = tilex * 256;
            pixelY = tiley * 256;
        }


        private readonly string[] _serverEndpoints = { "a", "b", "c" };
        
        public async void DownloadTiles(Tile tile, Action<bool, string, int,int> cb)
        {
            Directory.CreateDirectory(m_baseDir + tile.name);
            HttpClient client = new HttpClient();


            var random = new Random();
            int pixelx = tile.pixelx;
            int pixely = tile.pixely;
            int countMissing = 0;
            int countDownload = 0;
            m_stopDownload = false;
            for (int x = tile.x; x < (tile.x + tile.x_size); x++)
            {               
                for (int y = tile.y; y < (tile.y + tile.y_size); y++)
                {
                    if (m_stopDownload == true)
                    {
                        m_stopDownload = false;
                        return;
                    }

                    try
                    {
                        string fileName = m_baseDir + tile.name + "\\_" + tile.zoom + "_" + x + "_" + y + "_" + pixelx + "_" + pixely + "_" + ".png";
                        if (File.Exists(fileName) == false)
                        {
                            var url = $"http://{_serverEndpoints[random.Next(0, 2)]}.tile.openstreetmap.org/{tile.zoom}/{x}/{y}.png";
                            var data = await client.GetByteArrayAsync(url);
                            File.WriteAllBytes(fileName, data);
                            Thread.Sleep(400);
                            countDownload++;
                        }
                        else
                        {
                            countDownload++;
                        }
                    }
                    catch (Exception err)
                    {
                        countMissing++;                      
                    }
                    pixely += 256;
                }
                pixelx += 256;
            }
            cb(true, "finished", countMissing, countDownload);
        }

        public async void DownloadTiles(List<Tile> tiles, Action<bool, string, int,int> cb)
        {
            Directory.CreateDirectory(m_baseDir + tiles[0].name);
            HttpClient client = new HttpClient();


            var random = new Random();
            int pixelx = tiles[0].pixelx;
            int pixely = tiles[0].pixely;
            int countMissing = 0;
            int countDownload = 0;
            m_stopDownload = false;
            foreach (Tile tile in tiles)
            {
                if (m_stopDownload == true)
                {
                    m_stopDownload = false;
                    return;
                }
                for (int x = tile.x; x < (tile.x + tile.x_size); x++)
                {

                    for (int y = tile.y; y < (tile.y + tile.y_size); y++)
                    {
                        try
                        {
                            string fileName = m_baseDir + tile.name + "\\_" + tile.zoom + "_" + x + "_" + y + "_" + pixelx + "_" + pixely + "_" + ".png";
                            if (File.Exists(fileName) == false)
                            {
                                var url = $"http://{_serverEndpoints[random.Next(0, 2)]}.tile.openstreetmap.org/{tile.zoom}/{x}/{y}.png";
                                var data = await client.GetByteArrayAsync(url);
                                File.WriteAllBytes(fileName, data);
                                Thread.Sleep(400);
                                countDownload++;
                            }
                            else
                            {
                                countDownload++;
                            }
                        }
                        catch (Exception err)
                        {
                            countMissing++;
                        }
                        pixely += 256;
                    }
                    pixelx += 256;
                }
            }
            cb(true, "finished", countMissing, countDownload);
        }
 
        public async void DownloadTilesFromList(List<Tile> tiles, Action<bool, string, int, int> cb)
        {
            mapControl1.Stop();
            Directory.CreateDirectory(m_baseDir + tiles[0].name);
            HttpClient client = new HttpClient();

            m_stopDownload = false;
            var random = new Random();
             
            int countMissing = 0;
            int countDownload = 0;
            foreach (Tile tile in tiles)
            {
                if (m_stopDownload == true)
                {
                    m_stopDownload = false;
                    return;
                }
                if (Directory.Exists(m_baseDir + tile.name + "\\" + tile.zoom) == false)
                {
                    Directory.CreateDirectory(m_baseDir + tile.name + "\\" + tile.zoom);
                }

                try
                {
                    string fileName = m_baseDir + tile.name + "\\_" + tile.zoom + "_" + tile.x + "_" + tile.y + "_" + tile.pixelx + "_" + tile.pixely + "_" + ".png";
                    if (File.Exists(fileName) == false)
                    {
                        var url = $"http://{_serverEndpoints[random.Next(0, 2)]}.tile.openstreetmap.org/{tile.zoom}/{tile.x}/{tile.y}.png";
                        var data = await client.GetByteArrayAsync(url);
                        File.WriteAllBytes(m_baseDir + tile.name + "\\" + tile.zoom + "\\" + Path.GetFileName(fileName), data);
                        Thread.Sleep(2000);
                        countDownload++;
                        string s = string.Format("{0}:{1} [ {2},{3},{4} ]", countDownload, tiles.Count,tile.x,tile.y, tile.zoom);
                        Console.WriteLine(s);
                        label13.Text = s;                        
                    }
                    else
                    {
                        countDownload++;
                        string s = string.Format("{0}:{1}", countDownload, tiles.Count);
                        Console.WriteLine(s);
                        label13.Text = s;
                    }
                }
                catch (Exception err)
                {
                    countMissing++;
                }
                   
            }
            if (cb != null)
                cb(true, "finished", countMissing, countDownload);
        }
         
        List<TileBlock> tilesBlock = new List<TileBlock>();

        
        private void button3_Click(object sender, EventArgs e)
        {
            if (txtCreateName.Text == string.Empty)
            {
                MessageBox.Show("Please select area");
                return;
            }
            string area = txtCreateName.Text;

            tilesBlock = new List<TileBlock>();
          
            foreach (string file in Directory.EnumerateFiles(m_baseDir + area, "*.png", SearchOption.AllDirectories))
            {
                string[] fileparts = file.Split('_');                
                string [] s = fileparts[0].Split(Path.DirectorySeparatorChar);
                var dirName = s[2];
                TileBlock t = new TileBlock();
                t.x = int.Parse(fileparts[2]);
                t.y = int.Parse(fileparts[3]);
                t.pixelx = int.Parse(fileparts[4]);
                t.pixely = int.Parse(fileparts[5]);
                t.zoom = int.Parse(fileparts[1]);
                PixelXYToLatLongOSM(t.pixelx, t.pixely, t.zoom, out t.lat, out t.lon);
                t.fileName = file;        
                t.name = dirName;
                tilesBlock.Add(t);

            }
            TileDB db = new TileDB(m_baseDir   + area + "\\tiles_lat_lon_db.json");         
            db.Save(tilesBlock);
            MessageBox.Show("Created");

            if (mapControl1.LoadMapData(m_baseDir  + area + "\\tiles_lat_lon_db.json", out string outMessage) == true)
            {
                //mapControl1.ShowLatLon(txtCreateName.Text, int.Parse(cmbZoom.Text));
                m_initdone = true;
            }
            else
            {
                MessageBox.Show(outMessage);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string area = txtCreateName.Text;
            TileDB db = new TileDB(m_baseDir + area + "\\tiles_lat_lon_db.json");
            db.Load(out tilesBlock);   
            for(int i = 0; i < tilesBlock.Count; i++)
            {
                TileBlock t = tilesBlock[i];
                t.bitmap = new Bitmap(Image.FromFile(tilesBlock[i].fileName));
                tilesBlock[i] = t;
            }            
        }

        private void cmbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbZoom2.Text = cmbZoom.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 6; i < 19; i++)
            {
                cmbZoom.Items.Add(i.ToString());
                cmbZoom2.Items.Add(i.ToString());
            }
            cmbZoom.Text = "9";
            cmbZoom2.Text = "9";
            string area = txtCreateName.Text;
            if (mapControl1.LoadMapData(m_baseDir  + area + "\\tiles_lat_lon_db.json", out string outMessage) == true)
            {
                //mapControl1.ShowLatLon(txtCreateName.Text, int.Parse(cmbZoom.Text));
                m_initdone = true;
            } else
            {
                MessageBox.Show(outMessage);
            }            
        }

        private void btnCreateFromRegion_Click(object sender, EventArgs e)
        {
            int startTilex = int.Parse(txtStartX.Text);
            int startTiley = int.Parse(txtStartY.Text);
             
            int zoom = int.Parse(cmbZoom.Text);
            List<Tile> tiles = new List<Tile>();
            int orig_size = int.Parse(txtDownloadCount.Text);
            int size = 7;
            

            // IN ZOOM , the X and Y are plus 256 and the pixel is multiplx 256

            for (int z = zoom; z <= 18; z++)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        Tile t3 = new Tile
                        {
                            x_size = 1,
                            y_size = 1,                          
                            name = txtCreateName.Text,
                            zoom = z
                        };
                        t3.x = startTilex + j;
                        t3.y = startTiley + i;
                        t3.pixelx = t3.x * 256;
                        t3.pixely = t3.y * 256;
                        tiles.Add(t3);
                    }
                }
                startTilex = startTilex * 2;
                startTiley = startTiley * 2;
            }

            DownloadTilesFromList(tiles, (status, msg, countMissing, countDownload) =>
            {
                MessageBox.Show("Finished download: " + countDownload);
            });
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (mapControl1.ShowLatLon(txtCreateName.Text, 
                                       int.Parse(cmbZoom.Text),
                                       double.Parse(txtCreateLat.Text), 
                                       double.Parse(txtCreateLon.Text),
                                       out string outMessage) == false)
            {
                
                string[] s = outMessage.Split(',');
                MessageBox.Show("Missing tiles: " + outMessage);
                if (s[0] == "Missing tiles")
                {
                    DialogResult d = MessageBox.Show("Do you want to download missing tiles?", "ELI OSM Control", MessageBoxButtons.YesNo);
                    if (d == DialogResult.Yes)
                    {
                        DownloadFromXY(int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
                        return;
                    }
                }
            }
            mapControl1.ShowXY(showXYToolStripMenuItem.Checked);
            mapControl1.ShowBorder(showBorderToolStripMenuItem.Checked);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //DownloadFromPixelXAndPixelY();
            int startTilex = int.Parse(txtStartX.Text);
            int startTiley = int.Parse(txtStartY.Text);
            int zoom = int.Parse(txtDownloadCount.Text);

            DownloadFromXY(startTilex, startTiley , zoom);
        }
        void DownloadFromPixelXAndPixelY()
        {
            int startPixelTilex = int.Parse(txtStartX.Text);
            int startPixelTiley = int.Parse(txtStartY.Text);
            int zoom = int.Parse(txtDownloadCount.Text);
            List<Tile> tiles = new List<Tile>();
            int orig_size = int.Parse(txtDownloadCount.Text);
            int size = 15;
            // IN ZOOM , the X and Y are plus 256 and the pixel is multiplx 256

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Tile t3 = new Tile
                    {
                        x_size = 1,
                        y_size = 1,
                        pixelx = startPixelTilex + j * 256,
                        pixely = startPixelTiley + i * 256,
                        name = txtCreateName.Text,
                        zoom = zoom
                    };
                    t3.x = t3.pixelx / 256;
                    t3.y = t3.pixely / 256;
                    tiles.Add(t3);
                }
            }


            DownloadTilesFromList(tiles, (status, msg, countMissing, countDownload) =>
            {
                MessageBox.Show("Finished download: " + countDownload);
            });
        }

        void DownloadFromXY(int startTilex, int startTiley, int zoom)
        {
            
            List<Tile> tiles = new List<Tile>();
            int orig_size = int.Parse(txtDownloadCount.Text);
            int size = 8;
            // IN ZOOM , the X and Y are plus 256 and the pixel is multiplx 256

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Tile t3 = new Tile
                    {
                        x_size = 1,
                        y_size = 1,                       
                        name = txtCreateName.Text,
                        zoom = zoom
                    };
                    t3.x = startTilex + j;
                    t3.y = startTiley + i;
                    t3.pixelx = t3.x * 256;
                    t3.pixely = t3.y * 256;
                    tiles.Add(t3);
                }

            }
             
            DownloadTilesFromList(tiles, (status, msg, countMissing, countDownload) =>
            {
                MessageBox.Show("Finished download: " + countDownload);
            });
        }

        bool m_stopDownload = false;
        private void button6_Click(object sender, EventArgs e)
        {
            m_stopDownload = true;
             
        }

        private void button7_Click(object sender, EventArgs e)
        {
           mapControl1.RedrawWindow();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MouseHook.Stop();
            AppSettings.Instance.Config.lineColor = mapControl1.LineColor;
            AppSettings.Instance.Save();

            mapControl1.SaveHistory(m_historyFileName);
        }

        private void cmbDrawShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            mapControl1.DrawShape((DRAW_SHAPE)cmbDrawShape.SelectedIndex);
        }

        private void showBorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showBorderToolStripMenuItem.Checked = !showBorderToolStripMenuItem.Checked;
            mapControl1.ShowBorder(showBorderToolStripMenuItem.Checked);
        }

        private void showXYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showXYToolStripMenuItem.Checked = !showXYToolStripMenuItem.Checked;
            mapControl1.ShowXY(showXYToolStripMenuItem.Checked);
        }
          
        private void cmbZoom2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbZoom.Text = cmbZoom2.Text;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            mapControl1.AddRowTilesOnTheLeft("israel", out string outMessage);
        }

        public void CallbackMsg(int code, int index, string name, TileBlock tile)
        {
            
            if (code == 0)
            {
                int zoom = saveLocationForm.WithZoom() == false ? int.Parse(cmbZoom.Text) : tile.zoom;
                if (mapControl1.ShowLatLon(tile.name, zoom, tile.lat, tile.lon, 1, 1, out string outMessage) == false)
                {
                    string[] s = outMessage.Split(',');
                    MessageBox.Show("Missing tiles: " + outMessage);
                    if (s[0] == "Missing tiles")
                    {
                        DialogResult d = MessageBox.Show("Do you want to download missing tiles?", "ELI OSM Control", MessageBoxButtons.YesNo);
                        if (d == DialogResult.Yes)
                        {
                            DownloadFromXY(int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
                            return;
                        }
                    }
                }
                mapControl1.ShowXY(showXYToolStripMenuItem.Checked);
                mapControl1.ShowBorder(showBorderToolStripMenuItem.Checked);

                if (saveLocationForm != null)
                    mapControl1.SetHistory(saveLocationForm.GetHistory());

                if (saveLocationForm != null)
                {
                    AppSettings.Instance.Config.LoadWithZoom = saveLocationForm.WithZoom();
                    saveLocationForm.Close();
                }
                saveLocationForm = null;

            }
            if (code == 1)
            {
                saveLocationForm.DeleteEntry(index, name);

            }
        }
       
        SaveLocationForm saveLocationForm;
        private void button2_Click(object sender, EventArgs e)
        {
            SaveLocationControl.Callback p = new SaveLocationControl.Callback(CallbackMsg);


            if (saveLocationForm != null)
            {
                AppSettings.Instance.Config.LoadWithZoom = saveLocationForm.WithZoom();
                saveLocationForm.Close();
                saveLocationForm = null;            
            }
            saveLocationForm = new SaveLocationForm(m_historyFileName, AppSettings.Instance.Config.LoadWithZoom);
            saveLocationForm.LoadHistory(mapControl1.HistoryBlocks, p);
            saveLocationForm.ShowDialog();
            if (saveLocationForm != null)
                mapControl1.SetHistory(saveLocationForm.GetHistory());
            return;
           
        }

        private void snapshotToolStripMenuItem_Click(object sender, EventArgs e)
        {

            mapControl1.SnapshotMap("map_capture.jpg");
            System.Diagnostics.Process.Start("map_capture.jpg");

        }
         
        private void addAirplainToolStripMenuItem_Click(object sender, EventArgs e)
        {

            mapControl1.AddBitmap();

            var t = new Thread(() =>
            {
                mapControl1.MoveBitmap();
            });
            t.Start();

        }

        private void showPixelXYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showPixelXYToolStripMenuItem.Checked = !showPixelXYToolStripMenuItem.Checked;
            mapControl1.ShowPixelXY(showPixelXYToolStripMenuItem.Checked);
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            mapControl1.LoadLocation(txtCreateName.Text, "home", showPixelXYToolStripMenuItem.Checked, showBorderToolStripMenuItem.Checked, 3, false);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (eCoord == null || sCoord == null)
            {
                MessageBox.Show("Please mark points before calling calculate");
                return;
            }
            
            double distanceInMeters = sCoord.GetDistanceTo(eCoord);
            MessageBox.Show("Distance In meters: " + distanceInMeters + Environment.NewLine +
                            "Distance In Km: " + distanceInMeters /1000.0 + Environment.NewLine);
        }

        private void btnCalcLineDistance_Click(object sender, EventArgs e)
        {
            
            GeoCoordinate sc = null;
            GeoCoordinate ec = null;
            double distanceInMeters = 0;
            
            for (int i = 0; i < m_latList.Count - 1; i+=1)
            {
                sc = new GeoCoordinate(m_latList[i], m_lonList[i]);
                ec = new GeoCoordinate(m_latList[i+1], m_lonList[i+1]);
                distanceInMeters+= sc.GetDistanceTo(ec);
            }

            MessageBox.Show("Line distance :" + distanceInMeters / 1000.0 + " Km");
            m_latList.Clear();
            m_lonList.Clear();
            checkBox1.Checked = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                m_latList.Clear();
                m_lonList.Clear();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = true;
            colorDlg.AnyColor = true;
            colorDlg.SolidColorOnly = false;
            colorDlg.Color = linkLabel1.LinkColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                linkLabel1.LinkColor = colorDlg.Color;
                mapControl1.LineColor = linkLabel1.LinkColor;
            }
        }

        string GetFileName()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                 
                Title = "Import Location CSV Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "csv",
                Filter = "CSV files (*.CSV)|*.csv",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = false
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            return string.Empty;
        }
        private void importLocationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = GetFileName();
            if (fileName == string.Empty)
                return;

            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                int count = 0;
                     
                while (true)
                {
                    line = sr.ReadLine();
                    try
                    {
                        if (line == null)
                            break;
                        if (line == string.Empty)
                            continue;
                        string[] s = line.Split(',');
                        for (int i = 0; i < s.Length; i++)
                        {
                            s[i] = s[i].Trim('\t');
                        }
                        TileBlock t = new TileBlock
                        {
                            lat = double.Parse(s[1]),
                            lon = double.Parse(s[2]),
                            name = txtCreateName.Text,
                            zoom = 9
                        };
                        if (mapControl1.AddHistoryBlock(s[0], t) == true)
                        {
                            count++;
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message + Environment.NewLine + line);
                    }
                }
                MessageBox.Show("Number of Places imported are: " + count);
            }
        }
    }
}
