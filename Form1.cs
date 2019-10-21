using System;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;

namespace DownloadOSMTiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public struct Tile
        {
            public int x;
            public int x_size;
            public int y;
            public int y_size;
            public int zoom;
            public string name;
        }
        private void button1_Click(object sender, EventArgs e)
        {
             

            Tile t2 = new Tile
            {
                x = 74,
                x_size = 3,
                y = 95,
                y_size = 3,
                name = "new york",
                zoom = 8
            };

           
            DownloadTiles(t2);
        }
        private readonly string[] _serverEndpoints = { "a", "b", "c" };

        public async void DownloadTiles(Tile tile)
        {
            Directory.CreateDirectory("c:\\OSM_Tiles");
            HttpClient client = new HttpClient();
            var random = new Random();
            for (int x = tile.x; x < (tile.x + tile.x_size); x++)
            {
                for (int y = tile.y; y < (tile.y + tile.y_size); y++)
                {

                    try
                    {
                        var url = $"http://{_serverEndpoints[random.Next(0, 2)]}.tile.openstreetmap.org/{tile.zoom}/{x}/{y}.png";
                        var data = await client.GetByteArrayAsync(url);
                        File.WriteAllBytes("c:\\OSM_Tiles\\" + tile.name + "_" + x + "_" + y + "_" + tile.zoom + ".png", data);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);                            
                    }
                }
            }
            MessageBox.Show("Finished");
            
        }
    }
}
