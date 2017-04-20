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

        private int leftBorderNotFoundCnt;
        private int rightBorderNotFoundCnt;

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

            leftBorderNotFoundCnt = 0;
            rightBorderNotFoundCnt = 0;

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

        private void SearchForBordersFrom(int row, int borderSearchStart, bool modifyNotFoundCnt = true)
        {
            if(!SearchForLeftBorderFrom(row, borderSearchStart) && modifyNotFoundCnt)
            {
                ++leftBorderNotFoundCnt;
            }
            if(!SearchForRightBorderFrom(row, borderSearchStart) && modifyNotFoundCnt)
            {
                ++rightBorderNotFoundCnt;
            }
        }

        private bool SearchForLeftBorderFrom(int row, int borderSearchStart)
        {
            for (int col = borderSearchStart - 1; col >= 0; --col)
            {
                if (image.IsBlack(row, col))
                {
                    leftBorder[row] = col;
                    return true;
                }
            }
            leftBorder[row] = 0;
            return false;
        }

        private bool SearchForRightBorderFrom(int row, int borderSearchStart)
        {
            for (int col = borderSearchStart; col < image.width; ++col)
            {
                if (image.IsBlack(row, col))
                {
                    rightBorder[row] = col;
                    return true;
                }
            }
            rightBorder[row] = image.width - 1;
            return false;
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
            var leftCompensateStart = image.height;
            var rightCompensateStart = image.height;
            var leftCompensateEnd = image.height;
            var rightCompensateEnd = image.height;

            {
                int row = 6;
                while (row < image.height && leftBorder[row] != 0
                    && Math.Abs(leftSlope[row] - leftSlope[row - 1]) < 3 && leftSlope[row] * leftSlope[row - 1] >= 0) { ++row; }
                leftCompensateStart = row;
            }

            {
                int row = 6;
                while (row < image.height && rightBorder[row] != image.width - 1
                    && Math.Abs(rightSlope[row] - rightSlope[row - 1]) < 3 && leftSlope[row] * leftSlope[row - 1] >= 0) { ++row; }
                rightCompensateStart = row;
            }

            for (int row = leftCompensateStart; row < leftCompensateEnd; ++row)
            {
                leftBorder[row] = row * leftSlope[leftCompensateStart - 5] + leftZero[leftCompensateStart - 5];
                if(image.IsBlack(row, leftBorder[row]))
                {
                    leftCompensateEnd = row;
                    break;
                }
            }

            for (int row = rightCompensateStart; row < rightCompensateEnd; ++row)
            {
                rightBorder[row] = row * rightSlope[rightCompensateStart - 5] + rightZero[rightCompensateStart - 5];
                if (image.IsBlack(row, rightBorder[row]))
                {
                    rightCompensateEnd = row;
                    break;
                }
            }

            int borderSearchStart;

            if (leftCompensateEnd < rightCompensateEnd)
            {
                borderSearchStart = (leftBorder[leftCompensateEnd - 1] + rightBorder[leftCompensateEnd - 1]) / 2;
                for (int row = leftCompensateEnd; row < rightCompensateEnd; ++row)
                {
                    SearchForLeftBorderFrom(row, borderSearchStart);
                    //if (Math.Abs((rightBorder[row] + leftBorder[row]) - (rightBorder[row - 1] + leftBorder[row - 1])) < 10)
                    //{
                        borderSearchStart = (rightBorder[row] + leftBorder[row]) / 2;
                    //}
                }
            }
            else if (leftCompensateEnd > rightCompensateEnd)
            {
                borderSearchStart = (leftBorder[rightCompensateEnd - 1] + rightBorder[rightCompensateEnd - 1]) / 2;
                for (int row = rightCompensateEnd; row < leftCompensateEnd; ++row)
                {
                    SearchForRightBorderFrom(row, borderSearchStart);
                    //if (Math.Abs((rightBorder[row] + leftBorder[row]) - (rightBorder[row - 1] + leftBorder[row - 1])) < 10)
                    //{
                        borderSearchStart = (rightBorder[row] + leftBorder[row]) / 2;
                    //}
                }
            }

            var compensateEnd = Math.Max(leftCompensateEnd, rightCompensateEnd);

            borderSearchStart = (leftBorder[compensateEnd - 1] + rightBorder[compensateEnd - 1]) / 2;
            for (int row = compensateEnd; row < image.height; ++row)
            {
                SearchForBordersFrom(row, borderSearchStart, false);
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
