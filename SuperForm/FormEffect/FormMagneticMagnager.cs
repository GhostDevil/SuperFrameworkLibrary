using System;
using System.Drawing;
using System.Windows.Forms;
using static SuperForm.FormEffect.FormEffectEnum;

namespace SuperForm.FormEffect
{
    /// <summary>
    /// 日 期:2014-12-10
    /// 作 者:不良帥
    /// 描 述:磁性吸附窗体
    /// </summary>
    public class FormMagneticMagnager
    {
        MagneticPosition Pos;//位置属性
        Form MainForm, ChildForm;
        bool IsFirstPos;//是否第一次定位ChildForm子窗体
        private int step;//磁性子窗体ChildForm移动步长
        private Point LocationPt;//定位点
        delegate void LocationDel();//移动子窗体的委托
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="_MainForm">主窗体</param>
        /// <param name="_ChildForm">子窗体</param>
        /// <param name="_pos">位置属性</param>
        public FormMagneticMagnager(Form _MainForm, Form _ChildForm, MagneticPosition _pos)
        {
            IsFirstPos = false;
            step = 20;
            MainForm = _MainForm;
            ChildForm = _ChildForm;
            Pos = _pos;
            MainForm.LocationChanged += new EventHandler(MainForm_LocationChanged);
            ChildForm.LocationChanged += new EventHandler(ChildForm_LocationChanged);
            MainForm.SizeChanged += new EventHandler(MainForm_SizeChanged);
            ChildForm.SizeChanged += new EventHandler(ChildForm_SizeChanged);
            ChildForm.Load += new EventHandler(ChildForm_Load);
            MainForm.Load += new EventHandler(MainForm_Load);
        }
        void ChildForm_Load(object sender, EventArgs e)
        {
            GetLocation();
        }
        void MainForm_Load(object sender, EventArgs e)
        {
            GetLocation();
        }
        void MainForm_LocationChanged(object sender, EventArgs e)
        {
            GetLocation();
        }
        void MainForm_SizeChanged(object sender, EventArgs e)
        {
            GetLocation();
        }
        void ChildForm_SizeChanged(object sender, EventArgs e)
        {
            GetLocation();
        }
        void GetLocation()//定位子窗体
        {
            if (ChildForm == null)
                return;
            if (Pos == MagneticPosition.Left)
                LocationPt = new Point(MainForm.Left - ChildForm.Width, MainForm.Top);
            else if (Pos == MagneticPosition.Top)
                LocationPt = new Point(MainForm.Left, MainForm.Top - ChildForm.Height);
            else if (Pos == MagneticPosition.Right)
                LocationPt = new Point(MainForm.Right, MainForm.Top);
            else if (Pos == MagneticPosition.Bottom)
                LocationPt = new Point(MainForm.Left, MainForm.Bottom);
            ChildForm.Location = LocationPt;
        }
        void ChildForm_LocationChanged(object sender, EventArgs e)//当窗体位置移动后
        {
            if (!IsFirstPos)
            {
                IsFirstPos = true;
                return;
            }
            LocationDel del = new LocationDel(OnMove);//委托
            MainForm.BeginInvoke(del);//调用
        }

        void OnMove()//移动子窗体
        {
            if (ChildForm.Left > LocationPt.X)
                if (ChildForm.Left - LocationPt.X > step)
                    ChildForm.Left -= step;
                else
                    ChildForm.Left = LocationPt.X;
            else if (ChildForm.Left < LocationPt.X)
                if (ChildForm.Left - LocationPt.X < -step)
                    ChildForm.Left += step;
                else
                    ChildForm.Left = LocationPt.X;
            if (ChildForm.Top > LocationPt.Y)
                if (ChildForm.Top - LocationPt.Y > step)
                    ChildForm.Top -= step;
                else
                    ChildForm.Top = LocationPt.Y;

            else if (ChildForm.Top < LocationPt.Y)
                if (ChildForm.Top - LocationPt.Y < -step)
                    ChildForm.Top += step;
                else
                    ChildForm.Top = LocationPt.Y;
        }

    }
}
