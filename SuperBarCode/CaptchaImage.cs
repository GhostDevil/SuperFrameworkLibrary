using System;
using System.Drawing;
using System.Security.Cryptography;

namespace SuperFramework.SuperBarCode
{
    /// <summary>
    /// 日 期:2022-08-23
    /// 作 者:不良帥
    /// 描 述:图片验证码
    /// </summary>
    public class CaptchaImage
    {
        #region 私有字段
        private readonly string text;
        private Bitmap image;
        private readonly int letterCount = 4;   //验证码位数
        private readonly int letterWidth = 16;  //单个字体的宽度范围
        private readonly int letterHeight = 20; //单个字体的高度范围
        private static readonly byte[] randb = new byte[4];
        private static readonly RandomNumberGenerator rand = RandomNumberGenerator.Create();
        private readonly Font[] fonts =
        {
           new Font(new FontFamily("Times New Roman"),10 +Next(1),System.Drawing.FontStyle.Regular),
           new Font(new FontFamily("Georgia"), 10 + Next(1),System.Drawing.FontStyle.Regular),
           new Font(new FontFamily("Arial"), 10 + Next(1),System.Drawing.FontStyle.Regular),
           new Font(new FontFamily("Comic Sans MS"), 10 + Next(1),System.Drawing.FontStyle.Regular)
        };
        #endregion

        #region 公有属性
        /// <summary>
        /// 验证码
        /// </summary>
        public string Text => text;

        /// <summary>
        /// 验证码图片
        /// </summary>
        public Bitmap Image => image;
        #endregion

        #region 构造函数
        public CaptchaImage(int letter=4)
        {
            letterCount = letter;
            text = GetNumber(letterCount);
            CreateImage();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="max">最大值</param>
        private static int Next(int max)
        {
            rand.GetBytes(randb);
            int value = BitConverter.ToInt32(randb, 0);
            value %= (max + 1);
            if (value < 0) value = -value;
            return value;
        }

        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        private static int Next(int min, int max)
        {
            int value = Next(max - min) + min;
            return value;
        }
        #endregion

        #region 生成随机字符

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns>返回生成的随机数组字符串</returns>
        public string GetNumber(int length, bool sleep = false)
        {
            if (sleep) System.Threading.Thread.Sleep(3);
            string result = "";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
        #endregion

        #region 生成随机字母与数字

        /// <summary>
        /// 生成随机字母与数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns>返回生成的字母与数组随机字符串</returns>
        public string GetStr(int length, bool sleep = false)
        {
            if (sleep) System.Threading.Thread.Sleep(3);
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }

        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns>返回生成的纯字母随机字符串</returns>
        public string GetStrChar(int length, bool sleep = false)
        {
            if (sleep) System.Threading.Thread.Sleep(3);
            char[] Pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }

        /// <summary>
        /// 生成随机字符码含中文
        /// </summary>
        /// <param name="codeLen">字符串长度</param>
        /// <param name="zhCharsCount">中文字符数</param>
        /// <returns></returns>
        public string GetStrWithChinese(int codeLen, int zhCharsCount)
        {
            Random rnd = new Random(unchecked((int)DateTime.Now.Ticks));
            string ChineseChars = "的一是在不了有和人这中大为上个国我以要他时来用们生到作地于出就分对成会可主发年动同工也能下过子说产种面而方后多定行学法所民得经十三之进着等部度家电力里如水化高自二理起小物现实加量都两体制机当使点从业本去把性好应开它合还因由其些然前外天政四日那社义事平形相全表间样与关各重新线内数正心反你明看原又么利比或但质气第向道命此变条只没结解问意建月公无系军很情者最立代想已通并提直题党程展五果料象员革位入常文总次品式活设及管特件长求老头基资边流路级少图山统接知较将组见计别她手角期根论运农指几九区强放决西被干做必战先回则任取据处队南给色光门即保治北造百规热领七海口东导器压志世金增争济阶油思术极交受联什认六共权收证改清己美再采转更单风切打白教速花带安场身车例真务具万每目至达走积示议声报斗完类八离华名确才科张信马节话米整空元况今集温传土许步群广石记需段研界拉林律叫且究观越织装影算低持音众书布复容儿须际商非验连断深难近矿千周委素技备半办青省列习响约支般史感劳便团往酸历市克何除消构府称太准精值号率族维划选标写存候毛亲快效斯院查江型眼王按格养易置派层片始却专状育厂京识适属圆包火住调满县局照参红细引听该铁价严";
            string EnglishOrNumChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            char[] chs = new char[codeLen];
            int index;
            for (int i = 0; i < zhCharsCount; i++)
            {
                index = rnd.Next(0, codeLen);
                if (chs[index] == '\0')
                    chs[index] = ChineseChars[rnd.Next(0, ChineseChars.Length)];
                else
                    --i;
            }
            for (int i = 0; i < codeLen; i++)
            {
                if (chs[i] == '\0')
                    chs[i] = EnglishOrNumChars[rnd.Next(0, EnglishOrNumChars.Length)];
            }

            return new string(chs, 0, chs.Length);
        }

        #endregion

        #region 绘制验证码
        /// <summary>
        /// 绘制验证码
        /// </summary>
        /// <param name="chaos">增加噪点</param>
        public void CreateImage(bool chaos = false)
        {
            int int_ImageWidth = text.Length * letterWidth;
            Bitmap image = new Bitmap(int_ImageWidth, letterHeight);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(System.Drawing.Color.White);

                Color[] Colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
                if (chaos)
                {
                    //Pen pen = new Pen(Colors[rnd.Next(Colors.Length - 1)], 0);//噪点颜色：随机色，灰色噪点太简单。
                    Pen pen = new Pen(Color.LightGray, 0);//噪点颜色：灰色
                    int c = text.Length * 10;
                    for (int i = 0; i < c; i++)
                    {
                        int x = Next(image.Width);
                        int y = Next(image.Height);
                        g.DrawRectangle(pen, x, y, 1, 1);
                    }

                    int x1 = Next(image.Width / 4);
                    int y1 = Next(image.Height);
                    int x2 = Next(3 * image.Width / 4, image.Width);
                    int y2 = Next(image.Height);
                    Pen pen1 = new Pen(Colors[Next(Colors.Length - 1)], 1);
                    g.DrawLine(pen1, x1, y1, x2, y2);

                    for (int l = 0; l < 3; l++)
                    {
                        x1 = Next(image.Width / 4);
                        y1 = Next(image.Height);
                        x2 = Next(3 * image.Width / 4, image.Width);
                        y2 = Next(image.Height);
                        pen1 = new Pen(Color.LightGray, 1);
                        g.DrawLine(pen1, x1, y1, x2, y2);
                    }

                }

                for (int i = 0; i < 2; i++)
                {
                    int x1 = Next(image.Width - 1);
                    int x2 = Next(image.Width - 1);
                    int y1 = Next(image.Height - 1);
                    int y2 = Next(image.Height - 1);
                    g.DrawLine(new Pen(System.Drawing.Color.Silver), x1, y1, x2, y2);
                }

                int _x = -12;
                for (int int_index = 0; int_index < text.Length; int_index++)
                {
                    _x += Next(12, 16);
                    int _y = Next(-2, 2);
                    string str_char = text.Substring(int_index, 1);
                    str_char = Next(1) == 1 ? str_char.ToLower() : str_char.ToUpper();
                    Brush newBrush = new SolidBrush(GetRandomColor());
                    Point thePos = new Point(_x, _y);
                    g.DrawString(str_char, fonts[Next(fonts.Length - 1)], newBrush, thePos);
                }
                for (int i = 0; i < 10; i++)
                {
                    int x = Next(image.Width - 1);
                    int y = Next(image.Height - 1);
                    image.SetPixel(x, y, System.Drawing.Color.FromArgb(Next(0, 255), Next(0, 255), Next(0, 255)));
                }
                image = TwistImage(image, true, Next(1, 3), Next(4, 6));
                g.DrawRectangle(new Pen(System.Drawing.Color.LightGray, 1), 0, 0, int_ImageWidth - 1, (letterHeight - 1));
            }
            this.image = image;
        }

        /// <summary>
        /// 字体随机颜色
        /// </summary>
        public Color GetRandomColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);
            int int_Red = RandomNum_First.Next(180);
            int int_Green = RandomNum_Sencond.Next(180);
            int int_Blue = (int_Red + int_Green > 300) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            return System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue);
        }

        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高,一般为3</param>
        /// <param name="dPhase">波形的起始相位,取值区间[0-2*PI)</param>
        public Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            double PI = 6.283185307179586476925286766559;
            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            Graphics graph = Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();
            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = bXDir ? (PI * (double)j) / dBaseAxisLen : (PI * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);
                    int nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    int nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            srcBmp.Dispose();
            return destBmp;
        }
        #endregion
    }
}
