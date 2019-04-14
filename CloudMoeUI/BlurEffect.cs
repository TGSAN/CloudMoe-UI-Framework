using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace CloudMoeUI
{
    class BlurEffect
    {

        #region Windows7 Blur

        //Win7

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DwmBlurbehind blurBehind);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [StructLayout(LayoutKind.Sequential)]
        public struct DwmBlurbehind
        {
            public CoreNativeMethods.DwmBlurBehindDwFlags dwFlags;
            public bool Enabled;
            public IntPtr BlurRegion;
            public bool TransitionOnMaximized;
        }

        public static class CoreNativeMethods
        {
            public enum DwmBlurBehindDwFlags
            {
                DwmBbEnable = 1,
                DwmBbBlurRegion = 2,
                DwmBbTransitionOnMaximized = 4
            }
        }

        public static void EnableWin7ExtendAeroGlass(Visual visual)
        {
            try
            {
                // 为WPF程序获取窗口句柄
                //var windowInteropHelper = new WindowInteropHelper(obj);
                //IntPtr handle = windowInteropHelper.Handle;
                var visualHelperIntPtr = ((HwndSource)PresentationSource.FromVisual(visual)).Handle;

                HwndSource mainWindowSrc = HwndSource.FromHwnd(visualHelperIntPtr);

                var bb = new DwmBlurbehind
                {
                    dwFlags = CoreNativeMethods.DwmBlurBehindDwFlags.DwmBbEnable,
                    Enabled = true
                };


                DwmEnableBlurBehindWindow(visualHelperIntPtr, ref bb);

                const int dwmwaNcrenderingPolicy = 2;
                var dwmncrpDisabled = 2;

                DwmSetWindowAttribute(visualHelperIntPtr, dwmwaNcrenderingPolicy, ref dwmncrpDisabled, sizeof(int));
            }
            catch (DllNotFoundException)
            {
                Application.Current.MainWindow.Background = Brushes.White;
            }
        }

        public static void DisableWin7ExtendAeroGlass(Visual visual)
        {
            try
            {
                // 为WPF程序获取窗口句柄
                //var windowInteropHelper = new WindowInteropHelper(obj);
                //IntPtr handle = windowInteropHelper.Handle;
                var visualHelperIntPtr = ((HwndSource)PresentationSource.FromVisual(visual)).Handle;

                HwndSource mainWindowSrc = HwndSource.FromHwnd(visualHelperIntPtr);

                var bb = new DwmBlurbehind
                {
                    dwFlags = CoreNativeMethods.DwmBlurBehindDwFlags.DwmBbEnable,
                    Enabled = false
                };


                DwmEnableBlurBehindWindow(visualHelperIntPtr, ref bb);

                const int dwmwaNcrenderingPolicy = 2;
                var dwmncrpDisabled = 2;

                DwmSetWindowAttribute(visualHelperIntPtr, dwmwaNcrenderingPolicy, ref dwmncrpDisabled, sizeof(int));
            }
            catch (DllNotFoundException)
            {
                Application.Current.MainWindow.Background = Brushes.White;
            }
        }

        #endregion

        #region Windows10 Blur If >= 17134(1803) then use Acrylic Blur

        //Win10

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_INVALID_STATE = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        [DllImport("user32.dll")]
        public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public static void Win10EnableBlur(Visual visual)
        {
            //var windowHelper = new WindowInteropHelper(obj);
            //var windowHelperIntPtr = windowHelper.Handle;

            var visualHelperIntPtr = ((HwndSource)PresentationSource.FromVisual(visual)).Handle;
            //((HwndSource)PresentationSource.FromVisual(uielement)).Handle

            var accent = new AccentPolicy { AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND };

            Console.WriteLine("OS Build:" + Environment.OSVersion.Version.Build);
            //MessageBox.Show("OS Build:" + Environment.OSVersion.Version.Build);

            if (Environment.OSVersion.Version.Build >= 17134)
            {
                Console.WriteLine("Allowed use Acrylic Effect.");
                //MessageBox.Show("Allowed use Acrylic Effect.");

                int _blurOpacity = 0; /* 0-255 如果为0，颜色不能设置纯黑000000 */
                // int _blurOpacity = 32; /* 0-255 如果为0，颜色不能设置纯黑000000 */
                int _blurBackgroundColor = 0xFFFFFF; /* Drak BGR color format */
                // int _blurBackgroundColor = 0xE6E6E6; /* Drak BGR color format */

                accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;

                accent.GradientColor = (_blurOpacity << 24) | (_blurBackgroundColor & 0xFFFFFF);
            }

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(visualHelperIntPtr, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        public static void Win10DisableBlur(Visual visual)
        {
            //var windowHelper = new WindowInteropHelper(obj);
            //var windowHelperIntPtr = windowHelper.Handle;

            var visualHelperIntPtr = ((HwndSource)PresentationSource.FromVisual(visual)).Handle;
            //((HwndSource)PresentationSource.FromVisual(uielement)).Handle

            var accent = new AccentPolicy { AccentState = (AccentState)(0) };

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(visualHelperIntPtr, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        #endregion

        #region Windows10 1809 Acrylic Blur

        //internal enum AccentState
        //{
        //    ACCENT_DISABLED = 0,
        //    ACCENT_ENABLE_GRADIENT = 1,
        //    ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        //    ACCENT_ENABLE_BLURBEHIND = 3,
        //    ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        //    ACCENT_INVALID_STATE = 5
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //internal struct AccentPolicy
        //{
        //    public AccentState AccentState;
        //    public uint AccentFlags;
        //    public uint GradientColor;
        //    public uint AnimationId;
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //internal struct WindowCompositionAttributeData
        //{
        //    public WindowCompositionAttribute Attribute;
        //    public IntPtr Data;
        //    public int SizeOfData;
        //}

        //internal enum WindowCompositionAttribute
        //{
        //    // ...
        //    WCA_ACCENT_POLICY = 19
        //    // ...
        //}

        //public static void Win10AcrylicEnableBlur(Window obj)
        //{
        //    int _blurOpacity = 0;
        //    int _blurBackgroundColor = 0x990000; /* BGR color format */
        //    var windowHelper = new WindowInteropHelper(obj);

        //    var accent = new AccentPolicy();
        //    accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
        //    accent.GradientColor = (_blurOpacity << 24) | (_blurBackgroundColor & 0xFFFFFF);

        //    var accentStructSize = Marshal.SizeOf(accent);

        //    var accentPtr = Marshal.AllocHGlobal(accentStructSize);
        //    Marshal.StructureToPtr(accent, accentPtr, false);

        //    var data = new WindowCompositionAttributeData();
        //    data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
        //    data.SizeOfData = accentStructSize;
        //    data.Data = accentPtr;

        //    SetWindowCompositionAttribute(windowHelper.Handle, ref data);

        //    Marshal.FreeHGlobal(accentPtr);
        //}

        #endregion

        //General

        public static void GeneralBlurSwitcher(Visual visual, bool is_enable)
        {
            try
            {
                if (Environment.OSVersion.Version.Major <= 6 && Environment.OSVersion.Version.Minor < 2) // 是否为Win8以下（比如Win7为6.1）
                {
                    //Win7或者Vista（PS：Vista大概不能用了吧~(=ﾟωﾟ)ﾉ）
                    if (is_enable == true)
                    {
                        EnableWin7ExtendAeroGlass(visual);
                        //GlassWindow.AeroGlassCompositionEnabled = true;
                    }
                    else
                    {
                        DisableWin7ExtendAeroGlass(visual);
                        //GlassWindow.AeroGlassCompositionEnabled = false;
                    }
                }
                else
                {
                    //Win8及以上（PS：其实只有Win10能用(*ﾉωﾉ)）
                    if (is_enable == true)
                    {
                        Win10EnableBlur(visual);
                        //Win10AcrylicEnableBlur(obj);
                    }
                    else
                    {
                        Win10DisableBlur(visual);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CloudMoeUI Log: " + ex.Message);
            }
        }
    }
}
