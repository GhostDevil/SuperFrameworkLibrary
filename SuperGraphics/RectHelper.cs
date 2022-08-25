using System.Drawing;

namespace SuperFramework.SuperGraphics
{

    /// <summary>
    /// 日 期:2015-08-18
    /// 作 者:不良帥
    /// 描 述:该类提供一些静态方法来处理Rectangle的常用操作
    /// </summary>
    public class RectHelper
    {
        /// <summary>
        /// 将rect的Width和Height分别加1，并返回新的结果
        /// </summary>
        /// <param name="rect">原始的方框</param>
        /// <returns>增加宽高后的方框</returns>
        public static Rectangle IncreaseWH(Rectangle rect)
        {
            rect.Width++;
            rect.Height++;
            return rect;
        }

        /// <summary>
        /// 将rect的Width和Height分别减1
        /// </summary>        
        /// <param name="rect">Rectangle</param>
        /// <returns>新的Rectangle</returns>
        public static Rectangle DecreaseWH(Rectangle rect)
        {
            rect.Width--;
            rect.Height--;
            return rect;
        }

        /// <summary>
        /// 判断rect1是否等于rect2, 或完全包含了rect2
        /// </summary>
        /// <param name="rect1">Rectangle</param>
        /// <param name="rect2">Rectangle</param>
        /// <returns>true：等于或包含；false：不等于且未包含</returns>

        public static bool EqualOrFullyContains(Rectangle rect1, Rectangle rect2)
        {
            return rect2.Left >= rect1.Left && rect2.Top >= rect1.Top &&
                rect2.Right <= rect1.Right && rect2.Bottom <= rect1.Bottom;
        }
    }
}
