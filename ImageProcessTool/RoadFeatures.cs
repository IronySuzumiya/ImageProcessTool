using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessTool
{
    public enum RoadTypeEnum
    {
        Unknown,
        Curve,
        CrossRoad,
        Ring
    };

    unsafe public struct RoadFeatures
    {
        public int[] rightBorder;
        public int[] leftBorder;
        public int[] middleLine;
        public int[] leftSlope;
        public int[] rightSlope;
        public int[] leftZero;
        public int[] rightZero;
        public int[] middleSlope;
        public int[] middleZero;

        public ImageFrame image;
        public string name;

        public RoadFeatures(BitmapData data, string name = "")
        {
            image = new ImageFrame(data);
            this.name = name;

            rightBorder = new int[image.height];
            leftBorder = new int[image.height];
            middleLine = new int[image.height];
            leftSlope = new int[image.height];
            rightSlope = new int[image.height];
            leftZero = new int[image.height];
            rightZero = new int[image.height];
            middleSlope = new int[image.height];
            middleZero = new int[image.height];
            rightBorder = new int[image.height];

            for (int i = 0; i < image.height; ++i)
            {
                rightBorder[i] = image.width - 1;
                rightZero[i] = image.width - 1;
                middleZero[i] = image.width / 2;
            }
        }

        public void SearchForBorders()
        {
            var borderSearchStart = image.width / 2;

            for (int row = 0; row < image.height; ++row)
            {
                SearchForBordersFrom(row, borderSearchStart);

                borderSearchStart = middleLine[row] = (leftBorder[row] + rightBorder[row]) / 2;
            }
        }

        private void SearchForBordersFrom(int row, int borderSearchStart)
        {
            int col;
            for (col = borderSearchStart - 1; col >= 0; --col)
            {
                if (image.IsBlack(row, col) && image.IsWhite(row, col + 1))
                {
                    leftBorder[row] = col + 1;
                    break;
                }
            }
            if(col == -1)
            {
                leftBorder[row] = 0;
            }

            for (col = borderSearchStart; col < image.width - 1; ++col)
            {
                if (image.IsWhite(row, col) && image.IsBlack(row, col + 1))
                {
                    rightBorder[row] = col;
                    break;
                }
            }
            if(col == image.width - 1)
            {
                rightBorder[row] = image.width - 1;
            }
        }

        private int leftBorderNotFoundCnt
        {
            get
            {
                int cnt = 0;
                for(int row = 0; row < image.height; ++row)
                {
                    if(leftBorder[row] == 0)
                    {
                        ++cnt;
                    }
                }
                return cnt;
            }
        }

        private int rightBorderNotFoundCnt
        {
            get
            {
                int cnt = 0;
                for (int row = 0; row < image.height; ++row)
                {
                    if (rightBorder[row] == image.width - 1)
                    {
                        ++cnt;
                    }
                }
                return cnt;
            }
        }

        public void CalculateSlopes()
        {
            for (int row = 4; row < image.height; ++row)
            {
                double leftSlopeX = 0, leftSlopeA = 0, leftSlopeB = 0;
                double rightSlopeX = 0, rightSlopeA = 0, rightSlopeB = 0;
                double middleSlopeX = 0, middleSlopeA = 0, middleSlopeB = 0;
                for (int col = row - 4; col <= row; ++col)
                {
                    leftSlopeX += leftBorder[col];
                    leftSlopeA += col * leftBorder[col];
                    rightSlopeX += rightBorder[col];
                    rightSlopeA += col * rightBorder[col];
                    middleSlopeX += middleLine[col];
                    middleSlopeA += col * middleLine[col];
                }
                leftSlopeB = (row - 2) * leftSlopeX;
                rightSlopeB = (row - 2) * rightSlopeX;
                middleSlopeB = (row - 2) * middleSlopeX;

                leftSlope[row] = (int)((leftSlopeA - leftSlopeB) / 10.0);
                leftZero[row] = (int)((leftSlopeX / 5) - leftSlope[row] * (row - 2));
                rightSlope[row] = (int)((rightSlopeA - rightSlopeB) / 10.0);
                rightZero[row] = (int)((rightSlopeX / 5) - rightSlope[row] * (row - 2));
                middleSlope[row] = (int)((middleSlopeA - middleSlopeB) / 10.0);
                middleZero[row] = (int)((middleSlopeX / 5) - middleSlope[row] * (row - 2));
            }
        }

        public bool IsCrossRoad
        {
            get
            {
                return leftBorderNotFoundCnt > 3 && rightBorderNotFoundCnt > 3
                    && leftBorderNotFoundCnt + rightBorderNotFoundCnt > 15;
            }
        }

        public bool IsCurve
        {
            get
            {
                int blackCnt = 0;
                for (int row = image.height - 1; row >= 40; --row)
                {
                    if (image.IsBlack(row, middleLine[row]))
                    {
                        ++blackCnt;
                    }
                }
                return blackCnt > 5;
            }
        }

        public bool IsRing
        {
            get
            {
                int blackBlockRowsCnt = 0;
                int col;
                for (int row = image.height - 1; row >= 30; --row)
                {
                    if(image.IsWhite(row, image.width / 2))
                    {
                        continue;
                    }
                    for(col = image.width / 2 - 1; image.IsBlack(row, col) && col >= image.width / 2 - 10; --col) { }
                    if(col >= image.width / 2 - 10)
                    {
                        continue;
                    }
                    for (col = image.width / 2 + 1; image.IsBlack(row, col) && col <= image.width / 2 + 10; ++col) { }
                    if(col > image.width / 2 + 10)
                    {
                        ++blackBlockRowsCnt;
                    }
                }
                return blackBlockRowsCnt > 10;
            }
        }

        public RoadTypeEnum RoadType
        {
            get
            {
                return IsRing ? RoadTypeEnum.Ring :
                    IsCurve ? RoadTypeEnum.Curve :
                    IsCrossRoad ? RoadTypeEnum.CrossRoad :
                    RoadTypeEnum.Unknown;
            }
        }

        public void CompensateCurve()
        {
            int row;
            for (row = image.height - 1; row > 8; --row)
            {
                if (image.IsWhite(row, middleLine[row]))
                {
                    break;
                }
            }
            if (leftBorderNotFoundCnt > 6 && rightBorderNotFoundCnt < 2)
            {
                for (int row_ = image.height - 1; row_ > row; --row_)
                {
                    middleLine[row_] = 0;
                }
                for (int cnt = 0; cnt < 12; ++cnt)
                {
                    middleLine[row - cnt] = cnt * middleLine[row - 12] / 12;
                }
            }
            else if (rightBorderNotFoundCnt > 6 && leftBorderNotFoundCnt < 2)
            {
                for (int row_ = image.height - 1; row_ > row; --row_)
                {
                    middleLine[row_] = image.width - 1;
                }
                for (int cnt = 0; cnt < 12; ++cnt)
                {
                    middleLine[row - cnt] = image.width - 1 - cnt * (image.width - 1 - middleLine[row - 12]) / 12;
                }
            }
        }

        public void CompensateCrossRoad()
        {
            var leftCompensateStart = image.height - 1;
            var rightCompensateStart = image.height - 1;
            var leftCompensateEnd = image.height - 1;
            var rightCompensateEnd = image.height - 1;

            {
                int row = 6;
                while (row < image.height && leftBorder[row] != 0
                    && Math.Abs(leftSlope[row] - leftSlope[row - 1]) < 3) { ++row; }
                leftCompensateStart = row;
                row += 5;
                while (row < image.height
                    && (leftBorder[row] == 0 || Math.Abs(leftSlope[row] - leftSlope[row - 1]) >= 3)) { ++row; }
                row += 4;
                leftCompensateEnd = Math.Min(row, image.height - 1);
            }

            {
                int row = 6;
                while (row < image.height && rightBorder[row] != image.width - 1
                    && Math.Abs(rightSlope[row] - rightSlope[row - 1]) < 3) { ++row; }
                rightCompensateStart = row;
                row += 5;
                while (row < image.height
                    && (rightBorder[row] == image.width - 1 || Math.Abs(rightSlope[row] - rightSlope[row - 1]) >= 3)) { ++row; }
                row += 4;
                rightCompensateEnd = Math.Min(row, image.height - 1);
            }

            for (int row = leftCompensateStart; row < leftCompensateEnd; ++row)
            {
                leftBorder[row] = row * leftSlope[leftCompensateStart - 5] + leftZero[leftCompensateStart - 5];
            }

            for (int row = rightCompensateStart; row < rightCompensateEnd; ++row)
            {
                rightBorder[row] = row * rightSlope[rightCompensateStart - 5] + rightZero[rightCompensateStart - 5];
            }

            var compensateEnd = Math.Max(leftCompensateEnd, rightCompensateEnd);

            var borderSearchStart = (leftBorder[compensateEnd - 1] + rightBorder[compensateEnd - 1]) / 2;
            for (int row = compensateEnd; row < image.height; ++row)
            {
                SearchForBordersFrom(row, borderSearchStart);
                if (Math.Abs((rightBorder[row] + leftBorder[row]) - (rightBorder[row - 1] + leftBorder[row - 1])) < 10)
                {
                    borderSearchStart = (rightBorder[row] + leftBorder[row]) / 2;
                }
            }

            UpdateMiddleLine();
        }

        public void CompensateRingGoLeft()
        {
            int row;
            for (row = image.height - 1;
                row >= 30 && image.IsWhite(row, image.width / 2); --row) { }
            for (; row >= 10 && image.IsBlack(row, image.width / 2); --row) { }
            int col;
            for (col = image.width / 2; col >= 0 && image.IsBlack(row + 1, col); --col) { }
            for(int i = row; i > 0; --i)
            {
                rightBorder[i] = col + (row + 1 - i) * (rightBorder[0] - col) / row;
            }
            var borderSearchStart = col / 2;
            for (int i = row; i < image.height; ++i)
            {
                SearchForBordersFrom(i, borderSearchStart);
                if(leftBorder[i] != 0)
                {
                    borderSearchStart = (leftBorder[i] + rightBorder[i]) / 2;
                }
            }
            UpdateMiddleLine();
        }

        public void CompensateRingGoRight()
        {
            int row;
            for (row = image.height - 1;
                row >= 30 && image.IsWhite(row, image.width / 2); --row) { }
            for (; row >= 10 && image.IsBlack(row, image.width / 2); --row) { }
            int col;
            for (col = image.width / 2; col < image.width && image.IsBlack(row + 1, col); ++col) { }
            for (int i = row; i > 0; --i)
            {
                leftBorder[i] = col - (row + 1 - i) * (col - leftBorder[0]) / row;
            }
            var borderSearchStart = col + (image.width - col) / 2;
            for (int i = row; i < image.height; ++i)
            {
                SearchForBordersFrom(i, borderSearchStart);
                if(rightBorder[i] != image.width - 1)
                {
                    borderSearchStart = (leftBorder[i] + rightBorder[i]) / 2;
                }
            }
            UpdateMiddleLine();
        }

        private void UpdateMiddleLine()
        {
            for (int row = 0; row < image.height; ++row)
            {
                middleLine[row] = (leftBorder[row] + rightBorder[row]) / 2;
            }
        }

        public void HighlightBorderAndMiddleline()
        {
            for (int row = 0; row < image.height; ++row)
            {
                image.SetColor(row, middleLine[row], 0xfe, 0, 0);
                image.SetColor(row, leftBorder[row], 0xfe, 0, 0);
                image.SetColor(row, rightBorder[row], 0xfe, 0, 0);
            }
        }
    }
}
