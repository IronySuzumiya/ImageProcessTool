﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessTool
{
    unsafe public static class ImageProcessor
    {
        public static void Process(Bitmap bitmap, string name = "")
        {
            Preprocess(bitmap);
            UserFunction(bitmap, name);
        }

        private static void Preprocess(Bitmap bitmap)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height)
                , ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            for (int j = 0; j < data.Height; ++j)
            {
                for (int i = 0; i < data.Width; ++i)
                {
                    var color = (byte*)data.Scan0 + j * data.Stride + i * 3;
                    switch (color[0])
                    {
                        case 0x20:
                            color[0] = color[1] = color[2] = 0x00;
                            break;
                        case 0xe0:
                            color[0] = color[1] = color[2] = 0xfe;
                            break;
                        case 0xfe:
                        case 0x00:
                            break;
                        default:
                            color[0] = color[1] = color[2] = 0xfe;
                            break;
                    }
                }
            }
            for (int i = data.Width / 2 - 40; i < data.Width; ++i)
            {
                var color = (byte*)data.Scan0 + 29 * data.Stride + i * 3;
                if (color[0] == 0x00)
                {
                    color[0] = color[1] = color[2] = 0xfe;
                    break;
                }
            }
            for(int i = 0; i < data.Width; ++i)
            {
                var color = (byte*)data.Scan0 + (data.Height - 1) * data.Stride + i * 3;
                var colorToCopy = (byte*)data.Scan0 + (data.Height - 2) * data.Stride + i * 3;
                color[0] = colorToCopy[0];
                color[1] = colorToCopy[1];
                color[2] = colorToCopy[2];
            }
            bitmap.UnlockBits(data);
        }

        private static void UserFunction(Bitmap bitmap, string name = "")
        {
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height)
                , ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            var roadFeatures = new RoadFeatures(data, name);

            roadFeatures.SearchForBorders();

            roadFeatures.CalculateSlopes();
            
            switch(roadFeatures.RoadType)
            {
                case RoadTypeEnum.Curve:
                    roadFeatures.CompensateCurve();
                    break;
                case RoadTypeEnum.CrossRoad:
                    roadFeatures.CompensateCrossRoad();
                    break;
                case RoadTypeEnum.Ring:
                    roadFeatures.CompensateRingGoLeft();
                    break;
            }

            roadFeatures.HighlightBorderAndMiddleline();

            bitmap.UnlockBits(data);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
        }
    }
}
