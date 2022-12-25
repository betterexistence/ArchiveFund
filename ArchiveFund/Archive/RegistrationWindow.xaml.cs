using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace Archive
{
    public partial class RegistrationWindow : Window
    {
        private Base.ArciveFundEntities3 Database;
        public ObservableCollection<Base.Client> Clients;
        public RegistrationWindow(Base.ArciveFundEntities3 Database)
        {
            InitializeComponent();
            this.Database = Database;
            Clients = new ObservableCollection<Base.Client>(SourceCore.entities.Client);
        }

        private bool IsLoginValidation()
        {
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].loginClient.Equals(LoginTextBox.Text))
                {
                    MessageBox.Show("Имя пользователя занято!");
                    return true;
                }
            }
            return false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(NameTextBox.Text)) errors.AppendLine("Укажите имя");
            if (string.IsNullOrEmpty(numberPhoneTextBox.Text)) errors.AppendLine("Укажите номер");
            if (string.IsNullOrEmpty(LoginTextBox.Text)) errors.AppendLine("Укажите логин");
            if (string.IsNullOrEmpty(PasswordTextBox.Text)) errors.AppendLine("Укажите пароль");
            if (string.IsNullOrEmpty(PasswordTextRepeat.Text)) errors.AppendLine("Подтвердите пароль");
            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            Base.Client User = new Base.Client();   
            User.nameClient = NameTextBox.Text;
            User.numberPhoneClient = numberPhoneTextBox.Text;
            
            User.loginClient = LoginTextBox.Text;
            User.passwordClient = PasswordTextBox.Text;
            User.roleClient = IdRole.IsChecked.Value;
            
            if(Clients.Count != 0) if (IsLoginValidation()) return;
            
            if(PasswordTextRepeat.Text == PasswordTextBox.Text)
            {
                Database.Client.Add(User);
                Database.SaveChanges();
                new AuthorizationWindow().Show();
                Close();
            }
            else MessageBox.Show("Пароли не совпадают!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            new AuthorizationWindow().Show();
            Close();
        }
    }
}
