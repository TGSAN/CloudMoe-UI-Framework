using System.Windows;

namespace CloudMoeUI
{
    /// <summary>
    /// WindowTest.xaml 的交互逻辑
    /// </summary>
    public partial class WindowTest : Window
    {
        public WindowTest()
        {
            InitializeComponent();
            BlurEffectV2.SetIsEnabled(this, true);
        }
    }
}
