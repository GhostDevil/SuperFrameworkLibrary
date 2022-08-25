using System;
using System.Drawing;

namespace SuperFramework.SuperAlgorithm
{
    public static class MapHelper
    {
        #region 地图上两点之间的实际直线距离单位M
        /// <summary>
        /// 地球半径米 
        /// </summary>
        private const double EARTH_RADIUS = 6378137.0;
        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// 地图上两点之间的直线距离单位M
        /// </summary>
        /// <param name="lat1">A点经度</param>
        /// <param name="lng1">A点纬度</param>
        /// <param name="lat2">B点经度</param>
        /// <param name="lng2">B点纬度</param>
        /// <returns>两点直线距离</returns>
        /**
         * 1、公式中经纬度均用弧度表示；
         * 2、Lat1 Lng1 分别表示A点经、纬度，Lat2 Lng2 分别表示B点经纬度；
         * 3、a=Lng1 -Lng2 为两点纬度之差 b=Lat1 – Lat2 为两点经度之差；
         * 4、6378.137为地球半径（公里）；
         * 返回单位米
         * */
        public static double GetDistanceByMap(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLat2 = Rad(lat2);
            double a = radLat1 - radLat2;
            double b = Rad(lng1) - Rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
            Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s *= EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        #endregion

        #region 计算两条直线的交点（修正后的）

        /// <summary>
        /// 计算两条直线的交点（修正后的）
        /// </summary>
        /// <param name="lineFirstStar">L1的点1坐标</param>
        /// <param name="lineFirstEnd">L1的点2坐标</param>
        /// <param name="lineSecondStar">L2的点1坐标</param>
        /// <param name="lineSecondEnd">L2的点2坐标</param>
        /// <returns></returns>
        public static PointF GetIntersection(PointF lineFirstStar, PointF lineFirstEnd, PointF lineSecondStar, PointF lineSecondEnd)
        {
            /*
             * L1，L2都存在斜率的情况：
             * 直线方程L1: ( y - y1 ) / ( y2 - y1 ) = ( x - x1 ) / ( x2 - x1 ) 
             * => y = [ ( y2 - y1 ) / ( x2 - x1 ) ]( x - x1 ) + y1
             * 令 a = ( y2 - y1 ) / ( x2 - x1 )
             * 有 y = a * x - a * x1 + y1   .........1
             * 直线方程L2: ( y - y3 ) / ( y4 - y3 ) = ( x - x3 ) / ( x4 - x3 )
             * 令 b = ( y4 - y3 ) / ( x4 - x3 )
             * 有 y = b * x - b * x3 + y3 ..........2
             * 
             * 如果 a = b，则两直线平等，否则， 联解方程 1,2，得:
             * x = ( a * x1 - b * x3 - y1 + y3 ) / ( a - b )
             * y = a * x - a * x1 + y1
             * 
             * L1存在斜率, L2平行Y轴的情况：
             * x = x3
             * y = a * x3 - a * x1 + y1
             * 
             * L1 平行Y轴，L2存在斜率的情况：
             * x = x1
             * y = b * x - b * x3 + y3
             * 
             * L1与L2都平行Y轴的情况：
             * 如果 x1 = x3，那么L1与L2重合，否则平等
             * 
            */
            float a = 0, b = 0;
            int state = 0;
            if (lineFirstStar.X != lineFirstEnd.X)
            {
                a = (lineFirstEnd.Y - lineFirstStar.Y) / (lineFirstEnd.X - lineFirstStar.X);
                state |= 1;
            }
            if (lineSecondStar.X != lineSecondEnd.X)
            {
                b = (lineSecondEnd.Y - lineSecondStar.Y) / (lineSecondEnd.X - lineSecondStar.X);
                state |= 2;
            }
            switch (state)
            {
                case 0: //L1与L2都平行Y轴
                    {
                        if (lineFirstStar.X == lineSecondStar.X)
                        {
                            //throw new Exception("两条直线互相重合，且平行于Y轴，无法计算交点。");
                            return new PointF(0, 0);
                        }
                        else
                        {
                            //throw new Exception("两条直线互相平行，且平行于Y轴，无法计算交点。");
                            return new PointF(0, 0);
                        }
                    }
                case 1: //L1存在斜率, L2平行Y轴
                    {
                        float x = lineSecondStar.X;
                        float y = (lineFirstStar.X - x) * (-a) + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
                case 2: //L1 平行Y轴，L2存在斜率
                    {
                        float x = lineFirstStar.X;
                        //网上有相似代码的，这一处是错误的。你可以对比case 1 的逻辑 进行分析
                        //源code:lineSecondStar * x + lineSecondStar * lineSecondStar.X + p3.Y;
                        float y = (lineSecondStar.X - x) * (-b) + lineSecondStar.Y;
                        return new PointF(x, y);
                    }
                case 3: //L1，L2都存在斜率
                    {
                        if (a == b)
                        {
                            // throw new Exception("两条直线平行或重合，无法计算交点。");
                            return new PointF(0, 0);
                        }
                        float x = (a * lineFirstStar.X - b * lineSecondStar.X - lineFirstStar.Y + lineSecondStar.Y) / (a - b);
                        float y = a * x - a * lineFirstStar.X + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
            }
            // throw new Exception("不可能发生的情况");
            return new PointF(0, 0);
        }
        #endregion
    }
}
