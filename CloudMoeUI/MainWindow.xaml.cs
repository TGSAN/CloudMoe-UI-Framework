using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using Microsoft.Win32;
using Application = System.Windows.Application;
using MahApps.Metro;

namespace CloudMoeUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region CloudMoeUI Configure UI配置

        #region 预设颜色（暗色）

        System.Windows.Media.Color DarkTitleBarColorOpacity = System.Windows.Media.Color.FromArgb(64, 0, 0, 0); // 标题栏颜色
        System.Windows.Media.Color DarkWin8ColorOpacity = System.Windows.Media.Color.FromArgb(240, 31, 31, 31); // Win8颜色（轻微透明）
        System.Windows.Media.Color DarkBlurColorOpacity = System.Windows.Media.Color.FromArgb(153, 16, 16, 16); // 模糊颜色（透明） // double BlurOpacity = 0.6; // 模糊透明度（微软标准Tint为60%）
        System.Windows.Media.Color DarkBlurColorNonOpacity = System.Windows.Media.Color.FromArgb(255, 31, 31, 31); // 模糊颜色（不透明）
        double DarkNoiseEffectRatio = 0.08; // 材质强度（微软标准为4%，因为黑白色分离，所以需要两倍）

        #endregion

        #region 颜色方案1（亮色）

        System.Windows.Media.Color LightTitleBarColorOpacity = System.Windows.Media.Color.FromArgb(64, 255, 255, 255); // 标题栏颜色
        System.Windows.Media.Color LightWin8ColorOpacity = System.Windows.Media.Color.FromArgb(240, 230, 230, 230); // Win8颜色（轻微透明）
        System.Windows.Media.Color LightBlurColorOpacity = System.Windows.Media.Color.FromArgb(153, 250, 250, 250); // 模糊颜色（透明）
        System.Windows.Media.Color LightBlurColorNonOpacity = System.Windows.Media.Color.FromArgb(255, 230, 230, 230); // 模糊颜色（不透明）
        double LightNoiseEffectRatio = 0.08; // 材质强度（微软标准为4%，因为黑白色分离，所以需要两倍）

        #endregion

        System.Windows.Media.Color TitleBarColorOpacity; // 标题栏颜色
        System.Windows.Media.Color Win8ColorOpacity; // Win8颜色（轻微透明）
        System.Windows.Media.Color BlurColorOpacity; // 模糊颜色（透明）
        System.Windows.Media.Color BlurColorNonOpacity; // 模糊颜色（不透明）
        System.Windows.Media.Color TransparentColor = System.Windows.Media.Color.FromArgb(0, 0, 0, 0); // 全透明

        double NoiseEffectRatio; // 材质强度

        int BlurAnimationTime = 250; // 模糊切换动画事件（ms）

        int AnimationFrameRate = 120; // 动画目标帧率（FPS）

        #endregion

        #region CloudMoeUI Core Code (Version 1904.18053)

        #region 动画属性声明（请在非启动窗体移除此代码块）

        public readonly DependencyProperty WindowHeightAnimationProperty = DependencyProperty.Register("WindowHeightAnimation", typeof(double),
                                                                                                    typeof(MainWindow), new PropertyMetadata(OnWindowHeightAnimationChanged));

        public readonly DependencyProperty WindowWidthAnimationProperty = DependencyProperty.Register("WindowWidthAnimation", typeof(double),
                                                                                                    typeof(MainWindow), new PropertyMetadata(OnWindowWidthAnimationChanged));

        #endregion

        #region 使用Win32API主进程更改窗口大小动画

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        public enum SpecialWindowHandles
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT Rect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static void OnWindowHeightAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;

            if (window != null)
            {
                IntPtr handle = new WindowInteropHelper(window).Handle;
                var rect = new RECT();
                if (GetWindowRect(handle, ref rect))
                {
                    Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
                    double dpiX = graphics.DpiX;
                    double dpiY = graphics.DpiY;
                    double xs = graphics.DpiX / 96;
                    double ys = graphics.DpiY / 96;
                    rect.X = (int)(LastLeft * xs);
                    //rect.X = (int)window.Left;
                    rect.Y = (int)(LastTop * ys);
                    //rect.Y = (int)window.Top;
                    var height = (int)(double)e.NewValue;
                    rect.Width = (int)(window.ActualWidth * xs);
                    rect.Height = (int)(height * ys);  // double casting from object to double to int

                    SetWindowPos(handle, new IntPtr((int)SpecialWindowHandles.HWND_TOP), rect.X, rect.Y, rect.Width, rect.Height, (uint)SWP.SHOWWINDOW);
                }
            }
        }

        public double WindowHeightAnimation
        {
            get { return (double)GetValue(WindowHeightAnimationProperty); }
            set { SetValue(WindowHeightAnimationProperty, value); }
        }

        private static void OnWindowWidthAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;

            if (window != null)
            {
                IntPtr handle = new WindowInteropHelper(window).Handle;
                var rect = new RECT();
                if (GetWindowRect(handle, ref rect))
                {
                    Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
                    double dpiX = graphics.DpiX;
                    double dpiY = graphics.DpiY;
                    double xs = graphics.DpiX / 96;
                    double ys = graphics.DpiY / 96;
                    rect.X = (int)(LastLeft * xs);
                    //rect.X = (int)window.Left;
                    rect.Y = (int)(LastTop * ys);
                    //rect.Y = (int)window.Top;
                    var width = (int)(double)e.NewValue;
                    rect.Width = (int)(width * xs);
                    rect.Height = (int)(window.ActualHeight * ys); ;

                    SetWindowPos(handle, new IntPtr((int)SpecialWindowHandles.HWND_TOP), rect.X, rect.Y, rect.Width, rect.Height, (uint)SWP.SHOWWINDOW);
                }
            }
        }

        public double WindowWidthAnimation
        {
            get { return (double)GetValue(WindowWidthAnimationProperty); }
            set { SetValue(WindowWidthAnimationProperty, value); }
        }

        /// <summary>
        /// SetWindowPos Flags
        /// </summary>
        public class SWP
        {
            public static readonly int
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000;
        }

        #endregion

        #region 公用动态变量（用于标识运行状态）

        bool LastBlurBool = true; // 是否最后处于模糊（此项随窗体焦点更改）

        int MaxCutWidth = 8; // 默认最大化裁剪尺寸（宽）
        int MaxCutHeight = 8; // 默认最大化裁剪尺寸（高）

        bool IsMaxCuted = false; // 是否进行了最大化裁剪（用于判断是否需要进行窗口裁剪，此项随最大化事件更改）

        static double LastHeight = 0; // 最大化前窗口高
        static double LastWidth = 0; // 最大化前窗口宽
        static int LastTop = 0; // 动画前窗口Top（Y）坐标
        static int LastLeft = 0; // 动画前窗口Left（X）坐标

        #endregion

        #region CMUIConfig 类

        /// <summary>
        /// CMUIConfig 类，包含可对 CloudMoeUI 进行更改的属性和方法
        /// </summary>
        public partial class CMUIConfig
        {
            /// <summary>
            /// CMUIConfig.Window 类，包含可对 CloudMoeUI 窗口相关进行更改的属性和方法
            /// </summary>
            public partial class Window
            {
                //public bool sizeable_bool = true;
                //public bool Sizeable
                //{
                //    get { return this.sizeable_bool; }
                //    set
                //    {
                //        if (value != this.sizeable_bool)
                //        {
                //            //WhenValueChange();
                //        }
                //        this.sizeable_bool = value;
                //    }
                //}
            }
        }

        #endregion

        /// <summary>
        /// 获取 Windows10 透明效果设置（是否开启模糊效果，Win10 10240 开始支持）
        /// </summary>
        public string GetWindows10TransparencySetting()
        {
            string registData = "1"; // 默认1使用透明效果
            try
            {
                RegistryKey reg_HKCU = Registry.CurrentUser;
                RegistryKey reg_ThemesPersonalize = reg_HKCU.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false); // false为只读，true为可写入
                if (reg_ThemesPersonalize.GetValue("EnableTransparency") != null)  //如果值不为空
                {
                    registData = reg_ThemesPersonalize.GetValue("EnableTransparency").ToString(); // 读取值
                    //Console.WriteLine(registData);
                }
                else
                {
                    throw new Exception("找不到 EnableTransparency 值");
                }
            }
            catch
            {
                Console.WriteLine("获取透明效果设置失败，返回默认值1（使用透明效果）");
            }
            return registData;
        }

        /// <summary>
        /// 获取 Windows10 默认应用模式（App的主题色，1803内部版本17134开始支持）
        /// </summary>
        public string GetWindows10AppsLightThemeSetting()
        {
            string registData = "1"; // 默认1亮色模式
            try
            {
                RegistryKey reg_HKCU = Registry.CurrentUser;
                RegistryKey reg_ThemesPersonalize = reg_HKCU.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false); // false为只读，true为可写入
                if (reg_ThemesPersonalize.GetValue("AppsUseLightTheme") != null)  //如果值不为空（注意AppsUseLightTheme用的是Use而不是Uses）
                {
                    registData = reg_ThemesPersonalize.GetValue("AppsUseLightTheme").ToString(); // 读取值
                    //Console.WriteLine(registData);
                }
                else
                {
                    throw new Exception("找不到 AppsUseLightTheme 值");
                }
            }
            catch
            {
                Console.WriteLine("获取默认应用模式失败，返回默认值1（亮色模式）");
            }
            return registData;
        }

        /// <summary>
        /// 获取 Windows10 默认系统模式（系统的主题色主要为任务栏颜色，1903内部版本17763开始支持）
        /// </summary>
        public string GetWindows10SystemLightThemeSetting()
        {
            string registData = "0"; // 默认0暗色模式
            try
            {
                RegistryKey reg_HKCU = Registry.CurrentUser;
                RegistryKey reg_ThemesPersonalize = reg_HKCU.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false); // false为只读，true为可写入
                if (reg_ThemesPersonalize.GetValue("SystemUsesLightTheme") != null)  //如果值不为空（注意SystemUsesLightTheme用的是Uses而不是Use）
                {
                    registData = reg_ThemesPersonalize.GetValue("SystemUsesLightTheme").ToString(); // 读取值
                    //Console.WriteLine(registData);
                }
                else
                {
                    throw new Exception("找不到 SystemUsesLightTheme 值");
                }
            }
            catch
            {
                Console.WriteLine("获取默认系统模式失败，返回默认值0（暗色模式）");
            }
            return registData;
        }

        /// <summary>
        /// 模糊切换器（调用模糊类，主要为了给窗口透明度进行调整，否则不好看）
        /// </summary>
        /// <param name="visual">需要应用效果的窗口</param>
        /// <param name="switcher">true为开启模，false为关闭</param>
        public void BlurSwitcher(Visual visual, bool switcher, int animation_time = -1)
        {
            if (animation_time < 0) // 如果未指定时间，则使用默认值
            {
                animation_time = BlurAnimationTime;
            }
            if (switcher == true)
            {
                //BlurEffectV2.SetIsEnabled(this, true);
                if (GetWindows10TransparencySetting() == "1") // 如果Win10设置开启模糊则开启模糊
                {
                    BlurEffect.GeneralBlurSwitcher(visual, true);
                    //BlurEffect.GeneralBlurSwitcher(window, true);
                    //BlurRectangle.Opacity = BlurOpacity;
                    //BlurRectangle.Fill = new SolidColorBrush(BlurColorOpacity);
                    BGOpacityAnimation(true, animation_time);
                }
                else
                {
                    BlurRectangle.Fill = new SolidColorBrush(BlurColorNonOpacity); // 禁用模糊直接不透明，不使用动画
                }
                // 模糊时选择性使用材质
                if (Environment.OSVersion.Version.Major > 6) // 是否为Win10
                {
                    //NoiseEffectObject.Ratio = NoiseEffectRatio; // 设置材质强度
                    if (Environment.OSVersion.Version.Build >= 17134) // 只在1803及以上系统采用亚克力材质
                    {
                        if (GetWindows10TransparencySetting() == "1") // 如果Win10设置开启模糊则使用亚克力材质
                        {
                            NoiseEffectObject.Ratio = NoiseEffectRatio; // 设置材质强度
                        }
                        else
                        {
                            NoiseEffectObject.Ratio = 0; // 关闭材质
                        }
                    }
                    else
                    {
                        //NoiseEffectObject.Ratio = 0; // 关闭材质
                        NoiseEffectObject.Ratio = NoiseEffectRatio / 2; // 设置材质强度（更弱的）
                    }
                }
                LastBlurBool = true;
            }
            else
            {
                //BlurRectangle.Opacity = 1;
                //BlurRectangle.Fill = new SolidColorBrush(BlurColorNonOpacity);
                // 最大化时禁用模糊（因为超出工作区所以需要彻底禁用保持美观），普通模式下不禁用模糊（因为切换模糊时会变一下半透明）
                if (WindowState == WindowState.Maximized)
                {
                    BlurRectangle.Fill = new SolidColorBrush(BlurColorNonOpacity); // 最大化直接不透明，不使用动画
                    //Thread.Sleep(1000);
                    BlurEffect.GeneralBlurSwitcher(visual, false);
                }
                else
                {
                    if (GetWindows10TransparencySetting() == "0") // 如果Win10设置关闭模糊则彻底关闭模糊
                    {
                        BlurRectangle.Fill = new SolidColorBrush(BlurColorNonOpacity); // 禁用模糊直接不透明，不使用动画
                        BlurEffect.GeneralBlurSwitcher(visual, false);
                    }
                    else
                    {
                        BGOpacityAnimation(false, animation_time); // 普通情况下使用动画
                    }
                }
                NoiseEffectObject.Ratio = 0; // 关闭模糊时关闭材质
                LastBlurBool = false;
            }
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        /// <summary>
        /// 透明转换动画（是否变为透明）
        /// </summary>
        /// <param name="IsOpacity">true为变透明，false为变不透明</param>
        /// <param name="time">动画持续时间，单位ms，默认250ms</param>
        private void BGOpacityAnimation(bool IsOpacity, int time = 250)
        {
            AutoSyncThemeSetting(); // 应用Win10全局主题设置
            var storyboard = new Storyboard();
            //Timeline.SetDesiredFrameRate(storyboard, 60);
            Storyboard.SetDesiredFrameRate(storyboard, AnimationFrameRate);
            Storyboard.SetTarget(storyboard, BlurRectangle);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath("Fill.Color"));
            ColorAnimation colorAnm = new ColorAnimation();

            System.Windows.Media.Color OS_BlurColorOpacity = BlurColorOpacity; // 根据系统设置正确的透明度（主要是不支持透明的Win8系列）

            if ((Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor > 1) || (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor <= 1 && DwmIsCompositionEnabled() == false)) // 是否为Win8或者Win8.1（Win8:6.2,Win8.1:6.3），或者Vista或Win7未开启DWM
            {
                OS_BlurColorOpacity = Win8ColorOpacity; // 设置透明度
                NoiseEffectObject.Ratio = 0; // 关闭材质
            }

            if (IsOpacity == true)
            {
                colorAnm.Duration = new Duration(TimeSpan.FromMilliseconds(time));
                colorAnm.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                colorAnm.From = BlurColorNonOpacity;
                colorAnm.To = OS_BlurColorOpacity;
            }
            else
            {
                colorAnm.Duration = new Duration(TimeSpan.FromMilliseconds(time));
                colorAnm.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                colorAnm.From = OS_BlurColorOpacity;
                colorAnm.To = BlurColorNonOpacity;
            }
            storyboard.Children.Add(colorAnm);
            storyboard.Begin();
        }

        /// <summary>
        /// 窗口大小转换动画（目标宽和高）
        /// </summary>
        /// <param name="width">应用到的宽（为-1则保持不变，请勿使用ActualWidth）</param>
        /// <param name="height">应用到的高（为-1则保持不变，请勿使用ActualHeight）</param>
        /// <param name="time">动画持续时间，单位ms，默认250ms</param>
        public void SizeAnimation(int width, int height, int time = 250)
        {
            if (WindowState == WindowState.Maximized)
            {
                // 如果最大化则采用最大化前存储的值
                if (width == -1)
                {
                    width = (int)LastWidth;
                }
                if (height == -1)
                {
                    height = (int)LastHeight;
                }
                LastWidth = width; // 存储更改后的大小
                LastHeight = height; // 存储更改后的大小
                Width = width; // 应用更改
                Height = height; // 应用更改
                return;
            }
            else
            {
                // 如果没有最大化则采用当前的值
                if (width == -1)
                {
                    width = (int)ActualWidth;
                }
                if (height == -1)
                {
                    height = (int)ActualHeight;
                }
                LastWidth = ActualWidth; // 存储当前大小
                LastHeight = ActualHeight; // 存储当前大小
            }
            var storyboard = new Storyboard();
            //Timeline.SetDesiredFrameRate(storyboard, 60);
            Storyboard.SetDesiredFrameRate(storyboard, AnimationFrameRate);
            Storyboard.SetTarget(storyboard, BlurRectangle);

            DoubleAnimation width_anime = new DoubleAnimation(ActualWidth, width, TimeSpan.FromMilliseconds(time));
            Storyboard.SetTargetProperty(width_anime, new PropertyPath(WindowWidthAnimationProperty));
            //Storyboard.SetTargetProperty(width_anime, new PropertyPath(Window.WidthProperty));
            Storyboard.SetTarget(width_anime, this);
            width_anime.EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut };

            DoubleAnimation height_anime = new DoubleAnimation(ActualHeight, height, TimeSpan.FromMilliseconds(time));
            Storyboard.SetTargetProperty(height_anime, new PropertyPath(WindowHeightAnimationProperty));
            //Storyboard.SetTargetProperty(height_anime, new PropertyPath(Window.HeightProperty));
            Storyboard.SetTarget(height_anime, this);
            height_anime.EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut };

            storyboard.Children.Add(width_anime);
            storyboard.Children.Add(height_anime);

            LastLeft = (int)Left;
            LastTop = (int)Top;

            storyboard.Begin();
        }

        /// <summary>
        /// 窗口大小更改事件（最大化检测，最大化时剪切窗口，记录正常尺寸和坐标，并且停用模糊）
        /// </summary>
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Maximise();
            }
            else
            {
                Restore();
            }
        }

        /// <summary>
        /// 焦点移除事件（焦点移除时停用模糊）
        /// </summary>
        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                BlurSwitcher(this, false);
            }
            //Console.WriteLine("LostFocus");
        }

        /// <summary>
        /// 焦点获得事件（获得焦点时启用模糊）
        /// </summary>
        private void MainWindow_Activated(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                BlurSwitcher(this, true);
            }
            //Console.WriteLine("GetFocus");
        }

        /// <summary>
        /// 最大化窗口裁剪、模糊函数
        /// </summary>
        private void Maximise()
        {
            // 如果最大化立即取消模糊
            BlurSwitcher(this, false);
            //MessageBox.Show(IsMaxed.ToString());
            if (IsMaxCuted != true)
            {
                //this.WindowState = WindowState.Normal;
                this.WindowState = WindowState.Maximized;
                MaxCutWidth = (int)((ActualWidth - SystemParameters.WorkArea.Width) / 2);
                MaxCutHeight = (int)((ActualHeight - SystemParameters.WorkArea.Height) / 2);
                //MessageBox.Show(MaxCutWidth + "," + MaxCutHeight);
            }
            // 只有差值不为0时才裁剪（防止拖到顶部最大化两次导致不裁剪）
            if (MaxCutWidth != 0 && MaxCutHeight != 0)
            {
                WinBtnCmd.Margin = new Thickness(MaxCutWidth, MaxCutHeight, MaxCutWidth, -MaxCutHeight); // 最后一个值是标题栏按钮偏移值，为负是向下偏移
                MainGrid.Margin = new Thickness(MaxCutWidth, MaxCutHeight, MaxCutWidth, MaxCutHeight);
                IsMaxCuted = true;
            }
        }

        /// <summary>
        /// 还原时恢复窗口裁剪、模糊函数
        /// </summary>
        private void Restore()
        {
            // 存储最后正常尺寸
            LastHeight = ActualHeight;
            LastWidth = ActualWidth;
            // 如果已被裁剪才进行操作
            if (IsMaxCuted == true)
            {
                WinBtnCmd.Margin = new Thickness(0);
                MainGrid.Margin = new Thickness(0);
                IsMaxCuted = false;
            }
            // 如果没有模糊才进行操作
            if (LastBlurBool == false)
            {
                BlurSwitcher(this, true);
            }
        }

        /// <summary>
        /// 最大化切换器（用于双击标题栏最大化）
        /// </summary>
        public void MaximiseSwitcher()
        {

            if (this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip) // 只有 CanResize 和 CanResizeWithGrip 可以切换
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
                else
                {

                    WindowState = WindowState.Maximized;
                }
            }
        }

        /// <summary>
        /// 自动应用Win10全局主题设置（true 为应用成功）
        /// </summary>
        public bool AutoSyncThemeSetting()
        {
            try
            {
                Theme expectedTheme;
                if (GetWindows10AppsLightThemeSetting() == "1") // 是否使用亮色主题（Win10）
                {
                    TitleBarColorOpacity = LightTitleBarColorOpacity; // 标题栏颜色
                    Win8ColorOpacity = LightWin8ColorOpacity; // Win8颜色（轻微透明）
                    BlurColorOpacity = LightBlurColorOpacity; // 模糊颜色（透明）
                    BlurColorNonOpacity = LightBlurColorNonOpacity; // 模糊颜色（不透明）
                    NoiseEffectRatio = LightNoiseEffectRatio; // 材质强度
                    NoiseEffectObject.IsLight = true; // 设置材质颜色
                    expectedTheme = ThemeManager.GetTheme("Light.Blue");
                }
                else
                {
                    TitleBarColorOpacity = DarkTitleBarColorOpacity; // 标题栏颜色
                    Win8ColorOpacity = DarkWin8ColorOpacity; // Win8颜色（轻微透明）
                    BlurColorOpacity = DarkBlurColorOpacity; // 模糊颜色（透明）
                    BlurColorNonOpacity = DarkBlurColorNonOpacity; // 模糊颜色（不透明）
                    NoiseEffectRatio = DarkNoiseEffectRatio; // 材质强度
                    NoiseEffectObject.IsLight = false; // 设置材质颜色
                    expectedTheme = ThemeManager.GetTheme("Dark.Blue");
                }
                TitleBarColor.Background = new SolidColorBrush(TitleBarColorOpacity); // 设置标题栏颜色

                ThemeManager.ChangeTheme(Application.Current, expectedTheme);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 初始化 CloudMoe UI 框架（true 为初始化成功）
        /// </summary>
        public bool InitializeCloudMoeUI()
        {
            try
            {
                AutoSyncThemeSetting(); // 应用Win10全局主题设置
                AllowsTransparency = true; // 设置允许透明（必须在代码初始化后立即声明，否则有问题）
                Background = new SolidColorBrush(BlurColorNonOpacity); // 启动时整体背景不透明
                Loaded += MainWindow_Loaded;
                SizeChanged += MainWindow_SizeChanged;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 截获消息

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...
            const int WM_WININICHANGE = 0x001A;
            const int WM_SETTINGCHANGE = WM_WININICHANGE;
            // const int WM_SYSCOLORCHANGE = 0x0015;
            switch (msg)
            {
                case WM_SETTINGCHANGE: // 系统设置变更时主动更新配色方案
                    try
                    {
                        if (this.IsActive == true) // 检查是否为焦点，如果是才模糊，此处用于刷新配色
                        {
                            BlurSwitcher(this, true, 0);
                        }
                        else
                        {
                            BlurSwitcher(this, false, 0);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("接受系统消息主动更新配色方案失败。错误：" + e.Message);
                    }
                    break;
            }
            // return hwnd;
            return IntPtr.Zero;
        }

        #endregion

        #endregion

        #region Page:HomePage

        public HomePage HomePageObj = new HomePage(); // 声明一个新页面

        public void PageChangeToHomePage() // 转到首页
        {
            PageTransitioning.Content = new Frame()
            {
                Content = HomePageObj
            };

            HomePageObj.ParentWindow = this;
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            if (!InitializeCloudMoeUI()) // 初始化框架（必须在代码初始化后立即声明，否则有问题）
            {
                MessageBox.Show("初始化失败，请联系开发者。");
                Application.Current.Shutdown();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PageChangeToHomePage();
            //Console.WriteLine("Windows Handle: " + ((HwndSource)PresentationSource.FromVisual(this)).Handle.ToString());
            //MessageBox.Show("Version: " + Environment.OSVersion.Version);
            //GetWindows10AppsLightThemeSetting();
            //MessageBox.Show(Environment.OSVersion.Version.Major.ToString());
        }

        private void MetroWindow_WindowTransitionCompleted(object sender, RoutedEventArgs e)
        {
            //动画结束后使用模糊
            Activated += MainWindow_Activated;
            Deactivated += MainWindow_Deactivated;

            // 防止Win7样式出错（Win7边框和阴影）
            BlurSwitcher(this, true);
            BlurSwitcher(this, false);

            // 防止WindowChrome绘制出错
            WinChr.GlassFrameThickness = new Thickness(-1);
            WinChr.GlassFrameThickness = new Thickness(1);

            Background = new SolidColorBrush(TransparentColor); // 整体背景变透明

            if (this.IsActive == true) // 检查是否为焦点，如果是才模糊
            {
                BlurSwitcher(this, true);
            }

            // 动画结束后显示页面
            //PageChangeToHomePage();
        }
    }
}
