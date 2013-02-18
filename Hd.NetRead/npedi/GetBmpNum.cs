using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hd.NetRead.npedi
{
    #region 图形基类
    public class UnCodebase
    {
        public Bitmap bmpobj;
        public UnCodebase(Bitmap pic)
        {
            bmpobj = new Bitmap(pic);    //转换为Format32bppRgb
        }

        #region  GetGrayNumColor(System.Drawing.Color posClr) 根据RGB，计算灰度值
        /**/
        /// <summary>
        /// 根据RGB，计算灰度值
        /// </summary>
        /// <param name="posClr">Color值</param>
        /// <returns>灰度值，整型</returns>
        private int GetGrayNumColor(System.Drawing.Color posClr)
        {
            return (posClr.R * 19595 + posClr.G * 38469 + posClr.B * 7472) >> 16;
        }
        #endregion

        #region  GrayByPixels() 灰度转换,逐点方式
        /**/
        /// <summary>
        /// 灰度转换,逐点方式
        /// </summary>
        public void GrayByPixels()
        {
            for (int i = 0; i < bmpobj.Height; i++)
            {
                for (int j = 0; j < bmpobj.Width; j++)
                {
                    int tmpValue = GetGrayNumColor(bmpobj.GetPixel(j, i));
                    bmpobj.SetPixel(j, i, Color.FromArgb(tmpValue, tmpValue, tmpValue));
                }
            }
        }
        #endregion


        #region  ClearPicBorder(int borderWidth) 去图形边框
        /**/
        /// <summary>
        /// 去图形边框
        /// </summary>
        /// <param name="borderWidth"></param>
        public void ClearPicBorder(int borderWidth)
        {
            for (int i = 0; i < bmpobj.Height; i++)
            {
                for (int j = 0; j < bmpobj.Width; j++)
                {
                    if (i < borderWidth || j < borderWidth || j > bmpobj.Width - 1 - borderWidth || i > bmpobj.Height - 1 - borderWidth)
                        bmpobj.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
        }
        #endregion

        #region GrayByLine() 灰度转换,逐行方式
        /**/
        /// <summary>
        /// 灰度转换,逐行方式
        /// </summary>
        public void GrayByLine()
        {
            Rectangle rec = new Rectangle(0, 0, bmpobj.Width, bmpobj.Height);
            BitmapData bmpData = bmpobj.LockBits(rec, ImageLockMode.ReadWrite, bmpobj.PixelFormat);// PixelFormat.Format32bppPArgb);
            //    bmpData.PixelFormat = PixelFormat.Format24bppRgb;
            IntPtr scan0 = bmpData.Scan0;
            int len = bmpobj.Width * bmpobj.Height;
            int[] pixels = new int[len];
            Marshal.Copy(scan0, pixels, 0, len);

            //对图片进行处理
            int GrayValue = 0;
            for (int i = 0; i < len; i++)
            {
                GrayValue = GetGrayNumColor(Color.FromArgb(pixels[i]));
                pixels[i] = (byte)(Color.FromArgb(GrayValue, GrayValue, GrayValue)).ToArgb();      //Color转byte
            }

            bmpobj.UnlockBits(bmpData);
        }
        #endregion

        #region GetPicValidByValue(int dgGrayValue, int CharsCount) 得到有效图形并调整为可平均分割的大小
        /**/
        /// <summary>
        /// 得到有效图形并调整为可平均分割的大小
        /// </summary>
        /// <param name="dgGrayValue">灰度背景分界值</param>
        /// <param name="CharsCount">有效字符数</param>
        /// <returns></returns>
        public void GetPicValidByValue(int dgGrayValue, int CharsCount)
        {
            int posx1 = bmpobj.Width; int posy1 = bmpobj.Height;
            int posx2 = 0; int posy2 = 0;
            for (int i = 0; i < bmpobj.Height; i++)      //找有效区
            {
                for (int j = 0; j < bmpobj.Width; j++)
                {
                    int pixelValue = bmpobj.GetPixel(j, i).R;
                    if (pixelValue < dgGrayValue)     //根据灰度值
                    {
                        if (posx1 > j) posx1 = j;
                        if (posy1 > i) posy1 = i;

                        if (posx2 < j) posx2 = j;
                        if (posy2 < i) posy2 = i;
                    };
                };
            };
            // 确保能整除
            int Span = CharsCount - (posx2 - posx1 + 1) % CharsCount;   //可整除的差额数
            if (Span < CharsCount)
            {
                int leftSpan = Span / 2;    //分配到左边的空列 ，如span为单数,则右边比左边大1
                if (posx1 > leftSpan)
                    posx1 = posx1 - leftSpan;
                if (posx2 + Span - leftSpan < bmpobj.Width)
                    posx2 = posx2 + Span - leftSpan;
            }
            //复制新图
            Rectangle cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            bmpobj = bmpobj.Clone(cloneRect, bmpobj.PixelFormat);
        }
        #endregion

        #region GetPicValidByValue(int dgGrayValue) 得到有效图形,图形为类变量
        /**/
        /// <summary>
        /// 得到有效图形,图形为类变量
        /// </summary>
        /// <param name="dgGrayValue">灰度背景分界值</param>
        /// <param name="CharsCount">有效字符数</param>
        /// <returns></returns>
        public void GetPicValidByValue(int dgGrayValue)
        {
            int posx1 = bmpobj.Width; int posy1 = bmpobj.Height;
            int posx2 = 0; int posy2 = 0;
            for (int i = 0; i < bmpobj.Height; i++)      //找有效区
            {
                for (int j = 0; j < bmpobj.Width; j++)
                {
                    int pixelValue = bmpobj.GetPixel(j, i).R;
                    if (pixelValue < dgGrayValue)     //根据灰度值
                    {
                        if (posx1 > j) posx1 = j;
                        if (posy1 > i) posy1 = i;

                        if (posx2 < j) posx2 = j;
                        if (posy2 < i) posy2 = i;
                    };
                };
            };
            //复制新图
            Rectangle cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            bmpobj = bmpobj.Clone(cloneRect, bmpobj.PixelFormat);
        }
        #endregion

        #region GetPicValidByValue(Bitmap singlepic, int dgGrayValue) 得到有效图形
        /**/
        /// <summary>
        /// 得到有效图形,图形由外面传入
        /// </summary>
        /// <param name="dgGrayValue">灰度背景分界值</param>
        /// <param name="CharsCount">有效字符数</param>
        /// <returns></returns>
        public Bitmap GetPicValidByValue(Bitmap singlepic, int dgGrayValue)
        {
            int posx1 = singlepic.Width; int posy1 = singlepic.Height;
            int posx2 = 0; int posy2 = 0;
            for (int i = 0; i < singlepic.Height; i++)      //找有效区
            {
                for (int j = 0; j < singlepic.Width; j++)
                {
                    int pixelValue = singlepic.GetPixel(j, i).R;
                    if (pixelValue < dgGrayValue)     //根据灰度值
                    {
                        if (posx1 > j) posx1 = j;
                        if (posy1 > i) posy1 = i;

                        if (posx2 < j) posx2 = j;
                        if (posy2 < i) posy2 = i;
                    };
                };
            };
            //复制新图
            Rectangle cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            return singlepic.Clone(cloneRect, singlepic.PixelFormat);
        }
        #endregion

        #region GetSplitPics(int RowNum, int ColNum) 平均分割图片
        /**/
        /// <summary>
        /// 平均分割图片
        /// </summary>
        /// <param name="RowNum">水平上分割数</param>
        /// <param name="ColNum">垂直上分割数</param>
        /// <returns>分割好的图片数组</returns>
        public Bitmap[] GetSplitPics(int RowNum, int ColNum)
        {
            if (RowNum == 0 || ColNum == 0)
                return null;
            int singW = bmpobj.Width / RowNum;
            int singH = bmpobj.Height / ColNum;
            Bitmap[] PicArray = new Bitmap[RowNum * ColNum];

            Rectangle cloneRect;
            for (int i = 0; i < ColNum; i++)      //找有效区
            {
                for (int j = 0; j < RowNum; j++)
                {
                    cloneRect = new Rectangle(j * singW, i * singH, singW, singH);
                    PicArray[i * RowNum + j] = bmpobj.Clone(cloneRect, bmpobj.PixelFormat);//复制小块图
                }
            }
            return PicArray;
        }
        #endregion

        #region GetSingleBmpCode(Bitmap singlepic, int dgGrayValue) 图形解码
        /**/
        /// <summary>
        /// 返回灰度图片的点阵描述字串，1表示灰点，0表示背景
        /// </summary>
        /// <param name="singlepic">灰度图</param>
        /// <param name="dgGrayValue">背前景灰色界限</param>
        /// <returns></returns>
        public string GetSingleBmpCode(Bitmap singlepic, int dgGrayValue)
        {
            Color piexl;
            string code = "";
            for (int posy = 0; posy < singlepic.Height; posy++)
                for (int posx = 0; posx < singlepic.Width; posx++)
                {
                    piexl = singlepic.GetPixel(posx, posy);
                    if (piexl.R < dgGrayValue)    // Color.Black )
                        code = code + "1";
                    else
                        code = code + "0";
                }
            return code;
        }
        #endregion
    }
    #endregion

    #region 分析解码  class unCodeNbYang
    public class unCodeNbYang : UnCodebase
    {
        //字符表 顺序为0..9
        string[] CodeArray = new string[] {
        "001110001101100100010110001111000111100011110001111000111100011010001001101100011100",// 0
        "001100111100001100001100001100001100001100001100001100001100001100111111",// 1
        "001111000100111010000110000001100000011000000100000011000000100000010000001000010111111111111110",// 2
        "001111001001111000011000001100001100001110000011100000110000011000001111001101111100", // 3
        "000001100000011000001110000101100010011000100110010001101000011011111111000001100000011000000110", // 4
        "001111001111010000011100111110000111000011000001000001000001100010111100", // 5
        "000001110001110000110000011000000101110011100110110000111100001111000011110000110110011000111100", // 6
        "011111110111111010000010000001000000010000000100000010000000100000010000000100000001000000100000",// 7
        "001111000110001111000011110000110111011000111000001111000100011011000011110000110110011000111100",// 8
        "001111000110011011000011110000111100001111000011011000110011111000000110000011000001100011100000",// 9
        };

        // Add by fix Uncheck number 2009/12/01 begin
        string[] FixCodeArray = new string[]{
        "0000",
        "011001010001100001000100000100011000110000000011000110010001", // 1
        "00010000001111000100111010000110000001100000011000000100000011000000100000010000001000010111111111111110",//2
        "0000",
        "000001100000110000111000101100100110010011010001110000111111111000001100000110000011",//4
        "0000",
        "0000",
        "101111101110100000010000000000001000000100000100000000000010000001000000100000100000",//7
        "0000",
        "0000",
        };
        // Add by fix Uncheck number 2009/12/01 end.

        public unCodeNbYang(Bitmap pic)
            : base(pic)
        {
        }

        public string getPicnum()
        {
            GrayByPixels(); //灰度处理
            GetPicValidByValue(128, 4); //得到有效空间
            Bitmap[] pics = GetSplitPics(4, 1);     //分割

            if (pics.Length != 4)
            {
                return ""; //分割错误
            }
            else  // 重新调整大小
            {
                pics[0] = GetPicValidByValue(pics[0], 128);
                pics[1] = GetPicValidByValue(pics[1], 128);
                pics[2] = GetPicValidByValue(pics[2], 128);
                pics[3] = GetPicValidByValue(pics[3], 128);
            }

            //      if (!textBoxInput.Text.Equals(""))
            string result = "";
            char singleChar = ' ';
            {
                for (int i = 0; i < 4; i++)
                {
                    string code = GetSingleBmpCode(pics[i], 128);   //得到代码串
                    int codeLength = code.Length;                    // 得到代码串长度

                    for (int arrayIndex = 0; arrayIndex < CodeArray.Length; arrayIndex++)
                    {
                        if (CodeArray[arrayIndex].Equals(code))  //相等
                        {
                            if (arrayIndex < 10)   // 0..9
                                singleChar = (char)(48 + arrayIndex);
                            else if (arrayIndex < 36) //A..Z
                                singleChar = (char)(65 + arrayIndex - 10);
                            else
                                singleChar = (char)(97 + arrayIndex - 36);
                            result = result + singleChar;
                        }
                    }

                    // Add by fix Uncheck number 2009/12/01 begin
                    int diff = 0;

                    if (singleChar == ' ')
                    {
                        for (int arrayIndex = 0; arrayIndex < CodeArray.Length; arrayIndex++)
                        {
                            if (CodeArray[arrayIndex].Length == codeLength)
                            {
                                for (int j = 0; j < codeLength - 1; j++)
                                {
                                    if (code[j].ToString() != CodeArray[arrayIndex].ToString()[j].ToString())
                                    {
                                        diff++;
                                        if (diff > 20)
                                            break;
                                    }
                                }

                                if (diff < 21)
                                {
                                    if (arrayIndex < 10)   // 0..9
                                        singleChar = (char)(48 + arrayIndex);
                                    result = result + singleChar;
                                    break;
                                }

                                diff = 0;
                            }
                        }
                    }

                    if (singleChar == ' ')
                    {
                        for (int arrayIndex = 0; arrayIndex < FixCodeArray.Length; arrayIndex++)
                        {
                            if (FixCodeArray[arrayIndex].Length == codeLength)
                            {
                                for (int j = 0; j < codeLength - 1; j++)
                                {
                                    if (code[j].ToString() != FixCodeArray[arrayIndex].ToString()[j].ToString())
                                    {
                                        diff++;
                                        if (diff > 30)
                                            break;
                                    }
                                }

                                if (diff < 31)
                                {
                                    if (arrayIndex < 10)   // 0..9
                                        singleChar = (char)(48 + arrayIndex);
                                    result = result + singleChar;
                                    break;
                                }

                                diff = 0;
                            }
                        }
                    }
                    singleChar = ' ';
                    
                    // Add by fix Uncheck number 2009/12/01 end.
                }
            }
            return result;
        }
    }
    #endregion
}

