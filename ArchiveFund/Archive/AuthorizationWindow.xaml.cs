using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Archive
{
    /// <summary>
    /// Interaction logic for AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public Base.Client User;
        private Base.ArciveFundEntities3 DataBase;
        public AuthorizationWindow()
        {
            InitializeComponent();
            try
            {
                DataBase = new Base.ArciveFundEntities3();
            }
            catch
            {
                MessageBox.Show("Не удалось подключиться к базе данных. Проверьте настройки подключения приложения.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
            }
        }
        //кнопка регистрации
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationWindow(DataBase).Show();
            Close();
        }
        //вход в программу
        private void AuthorizationCommit_Click(object sender, RoutedEventArgs e)
        {
            User = DataBase.Client.SingleOrDefault(U => U.loginClient == LoginText.Text && U.passwordClient == PasswordPasswordBox.Password);
            if (User != null)
            {
                new MainWindow(User).Show();
                Close();
            }
            else
            {
                MessageBox.Show("Неверно указан логин и/или пароль!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        //кнопка выхода из программы
        private void AuthorizationRollBack_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти из программы?", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                Close();
            }
        }
        //показ пароля
        private void PasswordButton_MouseEnter(object sender, MouseEventArgs e)
        {
            String Password = PasswordPasswordBox.Password;
            Visibility Visibility = PasswordPasswordBox.Visibility;
            double Width = PasswordPasswordBox.ActualWidth;
            PasswordPasswordBox.Password = PasswordTextBox.Text;
            PasswordPasswordBox.Visibility = PasswordTextBox.Visibility;
            PasswordPasswordBox.Width = PasswordTextBox.Width;
            PasswordTextBox.Text = Password;
            PasswordTextBox.Visibility = Visibility;
            PasswordTextBox.Width = Width;
        }
    }
}
