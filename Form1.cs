using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using static DownloadOSMTiles.MapControl;

namespace DownloadOSMTiles
{
    public partial class Form1 : Form
    {
        string m_baseDir = "c:\\OSMTiles\\";
        bool m_initdone = false;
        public Form1()
        {
            InitializeComponent();
            Directory.CreateDirectory(m_baseDir);

            MapControlCallback p = new MapControlCallback(MapControlCallbackMsg);
            mapControl1.SetCallback(p);
        }

        void MapControlCallbackMsg(TileBlock tb)
        {
            lblLat.Text = tb.lat.ToString();
            lblLon.Text = tb.lon.ToString();
            lblTileX.Text = tb.x.ToString();
            lblTileY.Text = tb.y.ToString();
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


            DownloadTiles(t3, (status, msg, count) =>
            {
                MessageBox.Show("Finished");
            });
            

        }

        void PixelXYToLatLongOSM(int pixelX, int pixelY, int zoomLevel, out float latitude, out float longitude)
        {
            int mapSize = (int)Math.Pow(2, zoomLevel) * 256;
            //int tileX = (int)Math.Truncate((decimal)(pixelX / 256));
            //int tileY = (int)Math.Truncate((decimal)pixelY / 256);

            float n = (float)Math.PI - ((2.0f * (float)Math.PI * (ClipByRange(pixelY, mapSize - 1) / 256)) / (float)Math.Pow(2.0, zoomLevel));

            longitude = ((ClipByRange(pixelX, mapSize - 1) / 256) / (float)Math.Pow(2.0, zoomLevel) * 360.0f) - 180.0f;
            latitude = (180.0f / (float)Math.PI * (float)Math.Atan(Math.Sinh(n)));
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
            pixelX = (int)ClipByRange((tilex * 256) + ((X - tilex) * 256), mapSize - 1);
            pixelY = (int)ClipByRange((tiley * 256) + ((Y - tiley) * 256), mapSize - 1);
        }


        private readonly string[] _serverEndpoints = { "a", "b", "c" };
        
        public async void DownloadTiles(Tile tile, Action<bool, string, int> cb)
        {
            Directory.CreateDirectory(m_baseDir + tile.name);
            HttpClient client = new HttpClient();


            var random = new Random();
            int pixelx = tile.pixelx;
            int pixely = tile.pixely;
            int countMissing = 0;

            for (int x = tile.x; x < (tile.x + tile.x_size); x++)
            {               
                for (int y = tile.y; y < (tile.y + tile.y_size); y++)
                {
                    try
                    {
                        var url = $"http://{_serverEndpoints[random.Next(0, 2)]}.tile.openstreetmap.org/{tile.zoom}/{x}/{y}.png";
                        var data = await client.GetByteArrayAsync(url);
                        File.WriteAllBytes(m_baseDir + tile.name + "\\_" + tile.zoom + "_" + x + "_" + y + "_" + pixelx + "_" + pixely + "_" + ".png", data);
                    }
                    catch (Exception err)
                    {
                        countMissing++;                      
                    }
                    pixely += 256;
                }
                pixelx += 256;
            }
            cb(true, "finished", countMissing);
        }

        public async void DownloadTiles(List<Tile> tiles, Action<bool, string, int> cb)
        {
            Directory.CreateDirectory(m_baseDir + tiles[0].name);
            HttpClient client = new HttpClient();


            var random = new Random();
            int pixelx = tiles[0].pixelx;
            int pixely = tiles[0].pixely;
            int countMissing = 0;
            foreach (Tile tile in tiles)
            {

                for (int x = tile.x; x < (tile.x + tile.x_size); x++)
                {

                    for (int y = tile.y; y < (tile.y + tile.y_size); y++)
                    {
                        try
                        {
                            var url = $"http://{_serverEndpoints[random.Next(0, 2)]}.tile.openstreetmap.org/{tile.zoom}/{x}/{y}.png";
                            var data = await client.GetByteArrayAsync(url);
                            File.WriteAllBytes(m_baseDir + tile.name + "\\_" + tile.zoom + "_" + x + "_" + y + "_" + pixelx + "_" + pixely + "_" + ".png", data);
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
            cb(true, "finished", countMissing);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Tile> tiles = new List<Tile>();
            string mapName = txtCreateName.Text;
            if (mapName == string.Empty)
                return;
            
            int.TryParse(txtCreateSize.Text, out int size);
            if (size == 0)
                return;

            try
            {
                for (int zoom = 6; zoom < 19; zoom++)
                {
                    LatLongToPixelXYOSM(float.Parse(txtCreateLat.Text), float.Parse(txtCreateLon.Text),
                        zoom, out int pixelX, out int pixelY, out int tilex, out int tiley);
                    Tile t3 = new Tile
                    {
                        x = tilex,
                        x_size = size,
                        y = tiley,
                        y_size = size,
                        pixelx = pixelX,
                        pixely = pixelY,
                        name = mapName,
                        zoom = zoom
                    };
                    tiles.Add(t3);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;                    
            }

            DownloadTiles(tiles, (status, msg, count) =>
            {
                MessageBox.Show("Finished");
            });            
        }


        List<TileBlock> tilesBlock = new List<TileBlock>();

        
        private void button3_Click(object sender, EventArgs e)
        {
            tilesBlock = new List<TileBlock>();

            foreach (string file in Directory.EnumerateFiles(m_baseDir, "*.png", SearchOption.AllDirectories))
            {
                string[] fileparts = file.Split('_');
                var dirName = new DirectoryInfo(fileparts[0]).Name;
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
            TileDB db = new TileDB("tiles_lat_lon_db.json");         
            db.Save(tilesBlock);
            MessageBox.Show("Created");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            TileDB db = new TileDB("tiles_lat_lon_db.json");
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
            if (m_initdone == false)
                return;
            mapControl1.ShowLatLon(txtCreateName.Text, int.Parse(cmbZoom.Text));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 6; i < 19; i++)
            {
                cmbZoom.Items.Add(i.ToString());
            }
            cmbZoom.Text = "9";

            if (mapControl1.LoadMapData("tiles_lat_lon_db.json", out string outMessage) == true)
            {
                mapControl1.ShowLatLon(txtCreateName.Text, int.Parse(cmbZoom.Text));
                m_initdone = true;
            } else
            {
                MessageBox.Show(outMessage);
            }            
        }
    }
}
