using System.Windows;
using System.Windows.Media;

namespace Archive
{
    public partial class MainWindow : Window
    {
        public Base.Client User;
        public MainWindow(Base.Client User)
        {
            InitializeComponent();
            this.User = User;
            if(User.roleClient == true)
            {
                ProfileButton.Visibility = Visibility.Collapsed;
                ArchiveDataButton.Visibility = Visibility.Collapsed;
            }
            BrushConverter bc = new BrushConverter();
            ArchiveDataButton.Background = (Brush)bc.ConvertFrom("#FED6BC");
            ArchiveFrame.Navigate(new Pages.ArchivePage(User));
        }

        //Открытие страницы со справочником
        private void ArchiveDataButton_Click(object sender, RoutedEventArgs e)
        {
            ArchiveFrame.Navigate(new Pages.ArchivePage(User));
            BrushConverter bc1 = new BrushConverter();
            ProfileButton.Background = (Brush)bc1.ConvertFrom("#FDEED9");
            BrushConverter bc2 = new BrushConverter();
            ArchiveDataButton.Background = (Brush)bc2.ConvertFrom("#FED6BC");
        }

        //выход из учетной записи
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            new AuthorizationWindow().Show();
            Close();
        }
        //Открытие страницы с личным кабинетом
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc1 = new BrushConverter();
            ArchiveDataButton.Background = (Brush)bc1.ConvertFrom("#FDEED9");
            BrushConverter bc2 = new BrushConverter();
            ProfileButton.Background = (Brush)bc2.ConvertFrom("#FED6BC");
            ArchiveFrame.Navigate(new Pages.ProfilePage(User));
        }
    }
}
