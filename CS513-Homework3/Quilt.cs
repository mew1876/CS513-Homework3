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
        int pixelX1, pixelX2, pixelY1, pixelY2;
        TileSystem.LatLongToPixelXY(latitude1, longitude1, maxLevelOfDetail, out pixelX1, out pixelY1);
        TileSystem.LatLongToPixelXY(latitude2, longitude2, maxLevelOfDetail, out pixelX2, out pixelY2);

        quilt = new Bitmap(Math.Abs(pixelX1 - pixelX2), Math.Abs(pixelY1 - pixelY2), PixelFormat.Format24bppRgb);
        graphics = Graphics.FromImage(quilt);

        originX = Math.Min(pixelX1, pixelX2);
        originY = Math.Min(pixelY1, pixelY2);

        this.maxLevelOfDetail = maxLevelOfDetail;
    }

    public void Add(Bitmap image, string quadKey)
    {
        int tileX, tileY, levelOfDetail;
        TileSystem.QuadKeyToTileXY(quadKey, out tileX, out tileY, out levelOfDetail);
        uint relativeScale = TileSystem.MapSize(maxLevelOfDetail - levelOfDetail);
        graphics.DrawImage(image, tileX * (int)relativeScale - originX, tileY * (int)relativeScale - originY, image.Width * (int)relativeScale, image.Height * (int)relativeScale);
    }

    public Bitmap GetImage()
    {
        return quilt;
    }
}
