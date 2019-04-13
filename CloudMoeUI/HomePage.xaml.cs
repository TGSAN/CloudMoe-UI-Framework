using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CloudMoeUI
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {

        #region ParentWindow 继承父级窗口

        private MainWindow _parentWin;
        public MainWindow ParentWindow
        {
            get { return _parentWin; }
            set { _parentWin = value; }
        }

        #endregion

        public HomePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ParentWindow.Maximise();
            ParentWindow.SizeAnimation(-1, 600, 250);
            ParentWindow.PageChangeToHomePage();
        }

        private void TitleBarBG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ParentWindow.ShowMaxRestoreButton == true && ParentWindow.ResizeMode != ResizeMode.NoResize)
            {
                ParentWindow.MaximiseSwitcher();
            }
        }

        private void TitleBarBG_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ParentWindow.IsWindowDraggable == true)
            {
                ParentWindow.DragMove();
            }
        }
    }
}
