using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Archive.Pages
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private ImageConverter converter = new ImageConverter();
        private string _lastBase64;
        public Base.Client User;
        public ProfilePage(Base.Client User)
        {
            InitializeComponent();
            this.User = User;
            Update();
        }

        private Base.Client getClient()
        {
            var clientList = SourceCore.entities.Client.ToList();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].clientId == User.clientId)
                {
                    return clientList[i];
                }
            }
            MessageBox.Show("Клиента под таким логином не существует или данная операция с этим логином невозможно!");
            return null;
        }
        //вывод информации о клиенте
        private void Update()
        {
            Base.Client client = getClient();
            clientName.Text = client.nameClient;
            if(client.sexClientId == true)
            {
                clientSexCombobox.SelectedIndex = 0;
            }
            else
            {
                clientSexCombobox.SelectedIndex = 1;
            }
            if (client.passportClient == null) clientPassport.Text = "Укажите пасспорт";
            else clientPassport.Text = client.passportClient;
            clientNumberPhone.Text = client.numberPhoneClient;
            clientLogin.Text = client.loginClient;
            clientPassword.Text = client.passwordClient;
            if(client.imageClient != null) clientImage.Source = converter.ConvertToImage(client.imageClient);
        }
        //проверка логинов
        private bool ClientLoginValidation()
        {
            var clientList = SourceCore.entities.Client.ToList();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientLogin.Text.Equals(clientList[i].loginClient))
                {
                    MessageBox.Show("Логин занят!");
                    return true;
                }
            }
            return false;
        }
        //кнопка сохранения информации о клиенте
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(clientName.Text))
                errors.AppendLine("Укажите имя");
            if (clientSexCombobox.SelectedIndex == -1)
                errors.AppendLine("Укажите пол");
            if (string.IsNullOrEmpty(clientPassport.Text))
                errors.AppendLine("Укажите пасспорт");
            if (string.IsNullOrEmpty(clientNumberPhone.Text))
                errors.AppendLine("Укажите номер телефона");
            if (string.IsNullOrEmpty(clientLogin.Text))
                errors.AppendLine("Укажите логин");
            if (string.IsNullOrEmpty(clientPassword.Text))
                errors.AppendLine("Укажите пароль");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if(clientLogin.Text != User.loginClient)
            {
                if (ClientLoginValidation()) return;
            }

            var EditClient = new Base.Client();
            EditClient = SourceCore.entities.Client.First(i => i.clientId == User.clientId);
            EditClient.nameClient = clientName.Text;
            if (clientSexCombobox.SelectedIndex == 0) EditClient.sexClientId = true;
            else EditClient.sexClientId = false;
            EditClient.passportClient = clientPassport.Text;
            EditClient.numberPhoneClient = clientNumberPhone.Text;
            EditClient.loginClient = clientLogin.Text;
            EditClient.passwordClient = clientPassword.Text;
            if(_lastBase64 != null) EditClient.imageClient = _lastBase64;

            try
            {
                SourceCore.entities.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        //изменение фотографии клиента
        private void clientImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Фотографии (*.png)|*.png";
            ofd.Title = "Выбор фотографии";
            ofd.InitialDirectory = @"D:\course2\ArchiveFund\Archive\Images";

            if (ofd.ShowDialog() == true)
            {
                if (!File.Exists(ofd.FileName))
                {
                    MessageBox.Show("Файл не существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                _lastBase64 = converter.ConvertToBase64(ofd.FileName);
                clientImage.Source = converter.ConvertToImage(_lastBase64);
            }
        }
    }
}
