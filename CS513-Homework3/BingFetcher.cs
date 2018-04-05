using System.Threading.Tasks;
using System.Drawing;
using System.Net.Http;
using System.IO;

namespace CS513_Homework3
{
    class BingFetcher
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            Bitmap image = GetTile("http://h0.ortho.tiles.virtualearth.net/tiles/a023131022213211200.jpeg?g=131");
            image.Save("tileImage.jpg");
        }

        static void GetImageInBB(double minLatitude, double minLongitude, double maxLatitude, double maxLongitude, Quilt targetQuilt)
        {
            RecursivelyGetImageinBB(minLatitude, minLongitude, maxLatitude, maxLongitude, targetQuilt, "");
            return;
        }

        static void RecursivelyGetImageinBB(double minLatitude, double minLongitude, double maxLatitude, double maxLongitude, Quilt targetQuilt, string quadKey)
        {
            targetQuilt.Add(GetTile("http://h0.ortho.tiles.virtualearth.net/tiles/a" + quadKey + ".jpeg?g=131"), quadKey); // TODO: DRAW CURRENT quadKey ONTO THE RETURN IMAGE (has to be before recursion)
            RecursivelyGetImageinBB(minLatitude, minLongitude, maxLatitude, maxLongitude, targetQuilt, quadKey += "0");
            RecursivelyGetImageinBB(minLatitude, minLongitude, maxLatitude, maxLongitude, targetQuilt, quadKey += "1");
            RecursivelyGetImageinBB(minLatitude, minLongitude, maxLatitude, maxLongitude, targetQuilt, quadKey += "2");
            RecursivelyGetImageinBB(minLatitude, minLongitude, maxLatitude, maxLongitude, targetQuilt, quadKey += "3");
        }

        static Bitmap GetTile(string URI)
        {
            Bitmap image = null;
            Task get = getTileAsync(URI).ContinueWith(x => image = x.Result);
            get.Wait();
            return image;
        }

        static async Task<Bitmap> getTileAsync(string URI)
        {
            Bitmap image = null;
            HttpResponseMessage response = await client.GetAsync(URI);
            if(response.IsSuccessStatusCode)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                MemoryStream stream = new MemoryStream(bytes);
                image = new Bitmap(Image.FromStream(stream));
            }
            return image;
        }
    }
}
