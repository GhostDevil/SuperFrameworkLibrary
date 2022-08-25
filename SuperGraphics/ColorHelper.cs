using System.Drawing;

namespace SuperFramework.SuperGraphics
{
    /// <summary>
    /// 类 名:ColorHelper
    /// 版 本:Release
    /// 日 期:2015-08-18
    /// 作 者:不良帥
    /// 描 述:颜色辅助类
    /// </summary>
    public class ColorHelper
    {
        /// <summary>
        /// 获取浅色
        /// </summary>
        /// <param name="baseColor">基本颜色</param>
        /// <param name="percentage">百分比</param>
        /// <returns>浅色颜色</returns>
        public static Color GetLighterColor(Color baseColor, int percentage)
        {
            int r0 = baseColor.R;
            int g0 = baseColor.G;
            int b0 = baseColor.B;

            int r = r0 + (int)((255 - r0) * (percentage / 100f));
            int g = g0 + (int)((255 - g0) * (percentage / 100f));
            int b = b0 + (int)((255 - b0) * (percentage / 100f));

            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;

            return Color.FromArgb(baseColor.A, r, g, b);
        }
        /// <summary>
        /// 获取深色
        /// </summary>
        /// <param name="baseColor">基本颜色</param>
        /// <param name="percentage">百分比</param>
        /// <returns>深色颜色</returns>
        public static Color GetDarkerColor(Color baseColor, int percentage)
        {
            int r0 = baseColor.R;
            int g0 = baseColor.G;
            int b0 = baseColor.B;

            int r = r0 - (int)(r0 * (percentage / 100f));
            int g = g0 - (int)(g0 * (percentage / 100f));
            int b = b0 - (int)(b0 * (percentage / 100f));

            if (r < 0)
                r = 0;
            if (g < 0)
                g = 0;
            if (b < 0)
                b = 0;

            return Color.FromArgb(baseColor.A, r, g, b);
        }
        /// <summary>
        /// 获取浅色颜色数组
        /// </summary>
        /// <param name="baseColor">基本颜色</param>
        /// <param name="arrayLength">数组长度</param>
        /// <param name="maxPercentage">最大百分比</param>
        /// <returns>返回浅色颜色数组</returns>
        public static Color[] GetLighterArrayColors(Color baseColor, int arrayLength, float maxPercentage)
        {
            if (maxPercentage < 2)
                maxPercentage = 2f;
            if (maxPercentage > 100)
                maxPercentage = 100f;

            Color[] arrc = new Color[arrayLength];
            float average = maxPercentage / arrayLength;
            for (int i = 0; i < arrayLength; i++)
            {
                arrc[arrayLength - i - 1] = GetLighterColor(baseColor, (int)(average * i));
            }
            return arrc;
        }
        /// <summary>
        /// 获取浅色颜色数组
        /// </summary>
        /// <param name="baseColor">基本颜色</param>
        /// <param name="arrayLength">数组长度</param>
        /// <returns>返回浅色颜色数组</returns>
        public static Color[] GetLighterArrayColors(Color baseColor, int arrayLength)
        {
            return GetLighterArrayColors(baseColor, arrayLength, 100f);
        }
    }
}
