namespace SuperForm.FormEffect
{
    /// <summary>
    /// 日 期:2015-04-21
    /// 作 者:不良帥
    /// 描 述:窗体特效枚举
    /// </summary>
    public static class FormEffectEnum
    {
        /// <summary>
        /// 靠边隐藏的类型
        /// </summary>
        public enum DockHideType
        {
            /// <summary>
            /// 不隐藏
            /// </summary>
            None = 0,
            /// <summary>
            /// 靠上边沿隐藏
            /// </summary>
            Top,
            /// <summary>
            /// 靠左边沿隐藏
            /// </summary>
            Left,
            /// <summary>
            /// 靠右边沿隐藏
            /// </summary>
            Right
        }

        /// <summary>
        /// 窗体的显示或隐藏状态
        /// </summary>
        public enum FormDockHideStatus
        {
            /// <summary>
            /// 已隐藏
            /// </summary>
            Hide = 0,

            /// <summary>
            /// 准备隐藏
            /// </summary>
            ReadyToHide,

            /// <summary>
            /// 正常显示
            /// </summary>
            ShowNormally

        }
        #region  窗体动画形式 
        /// <summary>
        /// 窗体出现方式
        /// </summary>
        public enum ShowStyle
        {
            /// <summary>
            /// 自左向右显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记
            /// </summary>
            AW_HOR_POSITIVE = 0x0001,
            /// <summary>
            /// 自右向左显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记
            /// </summary>
            AW_HOR_NEGATIVE = 0x0002,
            /// <summary>
            /// 自顶向下显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记
            /// </summary>
            AW_VER_POSITIVE = 0x0004,
            /// <summary>
            /// 自下向上显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记该标记
            /// </summary>
            AW_VER_NEGATIVE = 0x0008,
            /// <summary>
            /// 若应用了AW_HIDE标记,则使窗口向内重叠;不然向外扩大
            /// </summary>
            AW_CENTER = 0x0010,
            /// <summary>
            /// 隐蔽窗口
            /// </summary>
            AW_HIDE = 0x10000,
            /// <summary>
            /// 激活窗口,在应用了AW_HIDE标记后不要应用这个标记
            /// </summary>
            AW_ACTIVE = 0x20000,
            /// <summary>
            /// 应用滑动类型动画结果,默认为迁移转变动画类型,当应用AW_CENTER标记时,这个标记就被忽视
            /// </summary>
            AW_SLIDE = 0x40000,
            /// <summary>
            /// 应用淡入淡出结果
            /// </summary>
            AW_BLEND = 0x80000

        }
        #endregion

        /// <summary>
        /// 磁性窗体的位置属性
        /// </summary>
        public enum MagneticPosition
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }
    }
}
