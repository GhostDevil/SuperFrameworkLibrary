using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SuperFramework.SuperWindows.WPFEffect
{
    public class ChineseBrushCanvas : InkCanvas
    {
        public ChineseBrushCanvas()
        {
            //当然要换上我们特地搞出来的ChinesebrushRenderer
            this.DynamicRenderer = new ChineseBrushRenderer();
        }


        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            //感兴趣的童鞋，注释这一句看看？
            this.Strokes.Remove(e.Stroke);

            this.Strokes.Add(new ChineseBrushStroke(e.Stroke.StylusPoints, this.DefaultDrawingAttributes.Color,this));
        }
    }
    public class ChineseBrushStroke : Stroke
    {

        private ImageSource imageSource;
        private readonly double width = 16;

        public ChineseBrushStroke(StylusPointCollection stylusPointCollection, Color color,DependencyObject dependency) : base(stylusPointCollection)
        {
            
            if (DesignerProperties.GetIsInDesignMode(dependency))//App.Current.MainWindow
                return;
            var dv = new DrawingVisual();
            var size = 90;
            using (var conext = dv.RenderOpen())
            {
                conext.PushOpacityMask(new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ConfigInfo\\pen.png", UriKind.Absolute))));
                conext.DrawRectangle(new SolidColorBrush(color), null, new Rect(0, 0, size, size));
                conext.Close();
            }
            var rtb = new RenderTargetBitmap(size, size, 96d, 96d, PixelFormats.Pbgra32);
            rtb.Render(dv);
            imageSource = BitmapFrame.Create(rtb);

            //Freezable 类提供特殊功能，以便在使用修改或复制开销很大的对象时帮助提高应用程序性能
            //WPF中的Frozen（冻结）与线程及其他相关问题
            imageSource.Freeze();
        }

        //卡顿就是该函数造成，每写完一笔就会调用，当笔画过长，后果可想而知~
        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            if (this.StylusPoints?.Count < 1)
                return;

            var p1 = new Point(double.NegativeInfinity, double.NegativeInfinity);
            var w1 = this.width + 20;


            for (int i = 0; i < StylusPoints.Count; i++)
            {
                var p2 = (Point)this.StylusPoints[i];

                var vector = p1 - p2;

                var dx = (p2.X - p1.X) / vector.Length;
                var dy = (p2.Y - p1.Y) / vector.Length;

                var w2 = this.width;
                if (w1 - vector.Length > this.width)
                    w2 = w1 - vector.Length;

                for (int j = 0; j < vector.Length; j++)
                {
                    var x = p2.X;
                    var y = p2.Y;

                    if (!double.IsInfinity(p1.X) && !double.IsInfinity(p1.Y))
                    {
                        x = p1.X + dx;
                        y = p1.Y + dy;
                    }

                    drawingContext.DrawImage(imageSource, new Rect(x - w2 / 2.0, y - w2 / 2.0, w2, w2));

                    p1 = new Point(x, y);

                    if (double.IsInfinity(vector.Length))
                        break;
                }
            }
        }
    }
    public class ChineseBrushRenderer : DynamicRenderer
    {
        private ImageSource imageSource;
        private readonly double width = 16;

        protected override void OnDrawingAttributesReplaced()
        {
            if (DesignerProperties.GetIsInDesignMode(this.Element))
                return;

            base.OnDrawingAttributesReplaced();

            var dv = new DrawingVisual();
            var size = 90;
            using (var conext = dv.RenderOpen())
            {
                //[关键]OpacityMask了解下？也许有童鞋想到的办法是，各种颜色的图片来一张？
                conext.PushOpacityMask(new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images\\pen.png", UriKind.Absolute))));
                //用颜色生成画笔画一个矩形
                conext.DrawRectangle(new SolidColorBrush(this.DrawingAttributes.Color), null, new Rect(0, 0, size, size));
                conext.Close();
            }
            var rtb = new RenderTargetBitmap(size, size, 96d, 96d, PixelFormats.Pbgra32);
            rtb.Render(dv);
            imageSource = BitmapFrame.Create(rtb);
            //[重要]此乃解决卡顿问题的关键！
            imageSource.Freeze();
        }

        protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints, Geometry geometry, Brush fillBrush)
        {
            var p1 = new Point(double.NegativeInfinity, double.NegativeInfinity);
            var p2 = new Point(0, 0);
            var w1 = this.width + 20;

            for (int i = 0; i < stylusPoints.Count; i++)
            {
                p2 = (Point)stylusPoints[i];

                //两点相减得到一个向量[高中数学知识了解下？]
                var vector = p1 - p2;

                //得到 x、y的变化值
                var dx = (p2.X - p1.X) / vector.Length;
                var dy = (p2.Y - p1.Y) / vector.Length;

                var w2 = this.width;
                if (w1 - vector.Length > this.width)
                    w2 = w1 - vector.Length;

                //为啥又来一个for？图像重叠，实现笔画的连续性，感兴趣的童鞋可以把for取消掉看看效果
                for (int j = 0; j < vector.Length; j++)
                {
                    var x = p2.X;
                    var y = p2.Y;

                    if (!double.IsInfinity(p1.X) && !double.IsInfinity(p1.Y))
                    {
                        x = p1.X + dx;
                        y = p1.Y + dy;
                    }

                    //画图，没啥可说的
                    drawingContext.DrawImage(imageSource, new Rect(x - w2 / 2.0, y - w2 / 2.0, w2, w2));

                    //再把新的坐标赋值给p1，以序后来
                    p1 = new Point(x, y);

                    if (double.IsInfinity(vector.Length))
                        break;

                }
            }
        }
    }
}
