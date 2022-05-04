using OpenCvSharp;
using System.Runtime.InteropServices;

namespace DeadBySkill;

class Program
{
    [DllImport("user32.dll")]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    static async Task Main()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Press key for skill check. Spacebar not recommended!");
        SelectKey();
        var lowWhite = new Scalar(253, 253, 253);
        var highWhite = new Scalar(255, 255, 255);
        var lowRed = new Scalar(160, 0, 0);
        var highRed = new Scalar(255, 30, 30);

        Mat GetCircle()
        {
            var sc = Screen.CaptureScreen();
            var mat = Screen.GetMatByImage(sc);
            var rgbMat = mat.CvtColor(ColorConversionCodes.BGR2RGB);
            return rgbMat.InRange(lowWhite, highWhite);
        }

        Mat GetArrow()
        {
            var sc = Screen.CaptureScreen();
            var mat = Screen.GetMatByImage(sc);
            var rgbMat = mat.CvtColor(ColorConversionCodes.BGR2RGB);
            return rgbMat.InRange(lowRed, highRed);
        }

        while (true)
        {
            start:
            await Task.Delay(50);
            var vPixelArray = new Vec3b[137, 137];
            var rPixelArray = new Vec3b[137, 137];

            var circle = GetCircle();

            for (var i = 0; i < 137; i++)
            {
                for (var j = 0; j < 137; j++)
                {
                    vPixelArray[i, j] = circle.At<Vec3b>(i, j);
                }
            }

            var circlePixelCount = vPixelArray.Cast<Vec3b>().Count(vec3b => vec3b != default);
            var isManiac = circlePixelCount > 600;
            if (circlePixelCount > 50)
            {
            search:
                var arrow = GetArrow();
                var count = 0;

                for (var i = 0; i < 137; i++)
                {
                    for (var j = 0; j < 137; j++)
                    {
                        rPixelArray[i, j] = arrow.At<Vec3b>(i, j);
                        if (rPixelArray[i, j] != default && rPixelArray[i, j] == vPixelArray[i, j])
                        {
                            if (isManiac)
                            {
                                ++count;
                            }
                            else
                            {
                                keybd_event((byte)ConsoleKey, 0, 0x0001 | 0, 0);
                                keybd_event((byte)ConsoleKey, 0, 0x0002 | 0, 0);
                                goto start;
                            }
                        }
                    }
                }

                if (isManiac && count >= 18)
                {
                    keybd_event((byte)ConsoleKey, 0, 0x0001 | 0, 0);
                    keybd_event((byte)ConsoleKey, 0, 0x0002 | 0, 0);
                }

                if (rPixelArray.Cast<Vec3b>().Any(vec3b => vec3b != default))
                {
                    goto search;
                }
                else
                {
                    continue;
                }
            }
        }
    }

    private static readonly Screen Screen = new();

    private static ConsoleKey ConsoleKey;

    private static void SelectKey()
    {
        var keyInfo = Console.ReadKey(true);
        ConsoleKey = keyInfo.Key;
        if (ConsoleKey == ConsoleKey.Spacebar)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Selected spacebar! Possible false positives!");
        }
        else
        {
            Console.WriteLine("Selected key: " + ConsoleKey);
        }
    }
}