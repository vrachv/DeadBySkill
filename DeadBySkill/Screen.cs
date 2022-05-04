using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;

namespace DeadBySkill;

public class Screen
{
    public Image CaptureScreen()
    {
        return CaptureWindow(User32.GetDesktopWindow());
    }

    private Image CaptureWindow(IntPtr handle)
    {
        var hdcSrc = User32.GetWindowDC(handle);
        var width = 140;
        var height = 140;
        var hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
        var hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
        var hOld = GDI32.SelectObject(hdcDest, hBitmap);

        GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 890, 470, GDI32.SRCCOPY);
        GDI32.SelectObject(hdcDest, hOld);
        GDI32.DeleteDC(hdcDest);
        User32.ReleaseDC(handle, hdcSrc);

        var img = Image.FromHbitmap(hBitmap);
        GDI32.DeleteObject(hBitmap);

        return img;
    }

    public Mat GetMatByImage(Image image)
    {
        using var bmp = new Bitmap(image);
        return BitmapConverter.ToMat(bmp);
    }
}