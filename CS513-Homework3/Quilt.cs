using Microsoft.MapPoint;
using System;
using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Bitmap class that has methods which consist of "stitching"/placing images next to each other to make one big image
/// </summary>
public class Quilt
{
    private Bitmap quilt;
    private Graphics graphics;

    private int originX;
    private int originY;

    private int maxLevelOfDetail;

	public Quilt(double latitude1, double longitude1, double latitude2, double longitude2, int maxLevelOfDetail = 23)
	{
        TileSystem.LatLongToPixelXY(latitude1, longitude1, maxLevelOfDetail, out int pixelX1, out int pixelY1);
        TileSystem.LatLongToPixelXY(latitude2, longitude2, maxLevelOfDetail, out int pixelX2, out int pixelY2);

        quilt = new Bitmap(Math.Abs(pixelX1 - pixelX2), Math.Abs(pixelY1 - pixelY2));
        graphics = Graphics.FromImage(quilt);

        originX = Math.Min(pixelX1, pixelX2);
        originY = Math.Min(pixelY1, pixelY2);

        this.maxLevelOfDetail = maxLevelOfDetail;
    }

    public void Add(Bitmap image, string quadKey)
    {
        TileSystem.QuadKeyToTileXY(quadKey, out int tileX, out int tileY, out int levelOfDetail);
        uint relativeScale = TileSystem.MapSize(maxLevelOfDetail - levelOfDetail);
        //Console.WriteLine(tileX * (int)relativeScale - originX);
        graphics.DrawImage(image, tileX * relativeScale - originX, tileY * relativeScale - originY, image.Width * relativeScale / 256, image.Height * relativeScale / 256);
    }

    public Bitmap GetImage()
    {
        return quilt;
    }

    public int getMaxLevelOfDetail()
    {
        return maxLevelOfDetail;
    }
}
