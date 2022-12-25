using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Archive.Pages
{
    public partial class ArchivePage : Page
    {
        bool isClientData = false;
        bool isTree = false;
        int DlgMode;
        public Base.Client User;
        public Base.Data SelectedData;
        public ObservableCollection<Base.Data> Data;
        public ObservableCollection<Base.Client> Clients;

        private ImageConverter converter = new ImageConverter();
        private string _lastBase64;
        public ArchivePage(Base.Client User)
        {
            InitializeComponent();
            Clients = new ObservableCollection<Base.Client>(SourceCore.entities.Client);
            this.User = User;
            if(User.roleClient == true)
            {
                ClientData.Visibility = Visibility.Collapsed;
            }
            else
            {
                AddGroup.Visibility = Visibility.Collapsed;
                Add.Visibility = Visibility.Collapsed;
                Copy.Visibility = Visibility.Collapsed;
                Edit.Visibility = Visibility.Collapsed;
                Delete.Visibility = Visibility.Collapsed;
            }
            UpdateGrid();
            TreeViewLoader();
            DlgLoad(0, "");
            PrintSelectedItem();
            DataContext = this;
        }

        //Генерация дерева
        private void TreeViewLoader()
        {
            treeView1.Items.Clear();
            
            //storageSection
            TreeViewItem StorageSectionItem = new TreeViewItem();
            StorageSectionItem.Header = "Список по секции хранения";
            treeView1.Items.Add(StorageSectionItem);
            var storageSectionList = SourceCore.entities.StorageSection.ToList();
            for (int i = 0; i < storageSectionList.Count; i++)
            {
                TreeViewItem ChildItem = new TreeViewItem();
                ChildItem.Header = storageSectionList[i].name;
                StorageSectionItem.Items.Add(ChildItem);
                var listData = Data.ToList();
                if (isClientData) listData = Data.Where(item => item.clientId == User.clientId).ToList();
                else listData = listData = Data.ToList();
                for (int j = 0; j < listData.Count; j++)
                {
                    if(listData[j].stoSecId == storageSectionList[i].id)
                    {
                        TreeViewItem SubChildItem = new TreeViewItem();
                        SubChildItem.Style = (Style)FindResource("TreeViewItem");
                        SubChildItem.Header = listData[j].name;
                        SubChildItem.Selected += SubChildItem_Selected;
                        ChildItem.Items.Add(SubChildItem);

                        TreeViewItem idItem = new TreeViewItem();
                        idItem.Header = listData[j].dataId;
                        idItem.Visibility = Visibility.Collapsed;
                        SubChildItem.Items.Add(idItem);
                    }
                }
            }

            //stoPeriod
            TreeViewItem StoPeriodItem = new TreeViewItem();
            StoPeriodItem.Header = "Список по периоду хранения";
            treeView1.Items.Add(StoPeriodItem);
            var storagePeriodList = SourceCore.entities.StoragePeriod.ToList();
            for (int i = 0; i < storagePeriodList.Count; i++)
            {
                TreeViewItem ChildItem = new TreeViewItem();
                ChildItem.Header = storagePeriodList[i].name;
                StoPeriodItem.Items.Add(ChildItem);
                var listData = Data.ToList();
                if (isClientData) listData = Data.Where(item => item.clientId == User.clientId).ToList();
                else listData = listData = Data.ToList();
                for (int j = 0; j < listData.Count; j++)
                {
                    if (listData[j].stoPerId == storagePeriodList[i].id)
                    {
                        TreeViewItem SubChildItem = new TreeViewItem();
                        SubChildItem.Style = (Style)FindResource("TreeViewItem");
                        SubChildItem.Header = listData[j].name;
                        SubChildItem.Selected += SubChildItem_Selected;
                        ChildItem.Items.Add(SubChildItem);

                        TreeViewItem idItem = new TreeViewItem();
                        idItem.Header = listData[j].dataId;
                        idItem.Visibility = Visibility.Collapsed;
                        SubChildItem.Items.Add(idItem);
                    }
                }
            }

            //status
            TreeViewItem StatusItem = new TreeViewItem();
            StatusItem.Header = "Список по статусу";
            treeView1.Items.Add(StatusItem);
            var statusList = SourceCore.entities.Status.ToList();
            for (int i = 0; i < statusList.Count; i++)
            {
                TreeViewItem ChildItem = new TreeViewItem();
                ChildItem.Header = statusList[i].name;
                StatusItem.Items.Add(ChildItem);
                var listData = Data.ToList();
                if (isClientData) listData = Data.Where(item => item.clientId == User.clientId).ToList();
                else listData = listData = Data.ToList();
                for (int j = 0; j < listData.Count; j++)
                {
                    if (listData[j].statusId == statusList[i].id)
                    {
                        TreeViewItem SubChildItem = new TreeViewItem();
                        SubChildItem.Style = (Style)FindResource("TreeViewItem");
                        SubChildItem.Header = listData[j].name;
                        SubChildItem.Selected += SubChildItem_Selected;
                        ChildItem.Items.Add(SubChildItem);

                        TreeViewItem idItem = new TreeViewItem();
                        idItem.Header = listData[j].dataId;
                        idItem.Visibility = Visibility.Collapsed;
                        SubChildItem.Items.Add(idItem);
                    }
                }
            }
        }
        //Вывод информации об объекте
        private void PrintSelectedItem()
        {
            SelectedData = (Base.Data)ArchiveGrid.SelectedItem;
            //data
            dataName.Content = SelectedData.name;
            dataId.Text = "Номер объекта: " + SelectedData.dataId;
            dateStart.Text = "Дата прибытия: " + ((DateTime)SelectedData.dateStart).ToString("dd.MM.yyyy");
            description.Text = SelectedData.description;
            statusTextBlock.Text = "Статус: " + SelectedData.Status.name;
            stoPerTextBlock.Text = "Срок хранения: " + SelectedData.StoragePeriod.name;
            stoSecTextBlock.Text = "Секция хранения: " + SelectedData.StorageSection.name;

            if (SelectedData.photo == null) dataImage.Source = new BitmapImage(new Uri("/Images/nullphoto.png", UriKind.RelativeOrAbsolute));
            else dataImage.Source = converter.ConvertToImage(SelectedData.photo);
            //client
            if (SelectedData.Client != null)
            {
                nameClient.Text = "Имя клиента: " + SelectedData.Client.nameClient;
                numberPhoneClient.Text = "Номер телефона: " + SelectedData.Client.numberPhoneClient;
            }
        }

        public Base.Data GetData(int id)
        {
            Base.Data selectedData = null;
            for (int i = 0; i < Data.Count; i++)
            {
                if(Data[i].dataId == id)
                {
                    selectedData = Data[i];
                }
            }
            return selectedData;
        }

        private void SubChildItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = (TreeViewItem)sender;
            TreeViewItem item = (TreeViewItem)treeViewItem.ItemContainerGenerator.Items.ElementAt(0);
            UpdateGrid(GetData((int)item.Header));
            PrintSelectedItem();
        }

        private void ArchiveGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PrintSelectedItem();
        }

        private int GetClient()
        {
            for (int i = 0; i < Clients.Count; i++)
            {
                if (clientLogin.Text.Equals(Clients[i].loginClient) && Clients[i].roleClient == false)
                {
                    return Clients[i].clientId;
                }
            }
            MessageBox.Show("Клиента под таким логином не существует или данная операция с этим логином невозможно!");
            return -1;
        }
        //кнопка принятия изменений об объекте
        private void AddCommit_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(clientLogin.Text))
                errors.AppendLine("Укажите логин клиента");
            if (string.IsNullOrEmpty(objectName.Text))
                errors.AppendLine("Укажите название объекта");
            if (string.IsNullOrEmpty(objectDescription.Text))
                errors.AppendLine("Укажите описание объекта");
            if (string.IsNullOrEmpty(objectVolume.Text))
                errors.AppendLine("Укажите объем объекта");
            if (objectstoSecId.SelectedIndex == -1)
                errors.AppendLine("Укажите группу");
            if (objectstoPerId.SelectedIndex == -1)
                errors.AppendLine("Укажите срок хранения");
            if (objectStatusId.SelectedIndex == -1)
                errors.AppendLine("Укажите статус");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (GetClient() == -1)
                return;

            if (DlgMode == 0)
            {
                var data = new Base.Data();
                data.name = objectName.Text;
                data.clientId = GetClient();
                data.description = objectDescription.Text;
                data.dateStart = DataPicker.SelectedDate;
                data.volume = int.Parse(objectVolume.Text);
                data.photo = _lastBase64;
                data.stoSecId = objectstoSecId.SelectedIndex + 1;
                data.stoPerId = objectstoPerId.SelectedIndex + 1;
                data.statusId = objectStatusId.SelectedIndex + 1;
                SourceCore.entities.Data.Add(data);
                SelectedData = data;
            }
            else
            {
                var EditData = new Base.Data();
                EditData = SourceCore.entities.Data.First(i => i.dataId == SelectedData.dataId);
                EditData.name = objectName.Text;
                EditData.clientId = GetClient();
                EditData.description = objectDescription.Text;
                EditData.dateStart = DataPicker.SelectedDate;
                EditData.volume = int.Parse(objectVolume.Text);
                EditData.photo = _lastBase64;
                EditData.stoSecId = objectstoSecId.SelectedIndex + 1;
                EditData.stoPerId = objectstoPerId.SelectedIndex + 1;
                EditData.statusId = objectStatusId.SelectedIndex + 1;
            }

            try
            {
                SourceCore.entities.SaveChanges();
                DlgLoad(0, "");
                UpdateGrid(SelectedData);
                PrintSelectedItem();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        //кнопка отмены
        private void AddRollback_Click(object sender, RoutedEventArgs e)
        {
            DlgLoad(0, "");
            UpdateGrid(SelectedData);
        }
        //кнопка добавления объекта
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            DlgLoad(1, "Добавить объект");
            DataContext = null;
            DlgMode = 0;
        }
        //кнопка копирования объекта
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (ArchiveGrid.SelectedItem != null)
            {
                DlgLoad(1, "Копировать объект");
                SelectedData = (Base.Data)ArchiveGrid.SelectedItem;
                clientLogin.Text = SelectedData.Client.loginClient;
                objectName.Text = SelectedData.name;
                objectDescription.Text = SelectedData.description;
                DataPicker.Text = SelectedData.dateStart.ToString();
                objectVolume.Text = SelectedData.volume.ToString();
                if (SelectedData.photo == null) imageData.Source = new BitmapImage(new Uri("/Images/nullphoto.png", UriKind.RelativeOrAbsolute));
                else imageData.Source = converter.ConvertToImage(SelectedData.photo);
                objectstoSecId.SelectedIndex = (int)SelectedData.stoSecId-1;
                objectstoPerId.SelectedIndex = (int)SelectedData.stoPerId-1;
                objectStatusId.SelectedIndex = (int)SelectedData.statusId-1;
                DlgMode = 0;
            }
            else
            {
                MessageBox.Show("Не выбрано ниодной строки!", "Сообщение", MessageBoxButton.OK);
            }
        }
        //кнопка редактирования объекта
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ArchiveGrid.SelectedItem != null)
            {
                DlgLoad(1, "Изменить объект");
                SelectedData = (Base.Data)ArchiveGrid.SelectedItem;
                clientLogin.Text = SelectedData.Client.loginClient;
                objectName.Text = SelectedData.name;
                objectDescription.Text = SelectedData.description;
                DataPicker.Text = SelectedData.dateStart.ToString();
                objectVolume.Text = SelectedData.volume.ToString();
                if (SelectedData.photo == null) imageData.Source = new BitmapImage(new Uri("/Images/nullphoto.png", UriKind.RelativeOrAbsolute));
                else imageData.Source = converter.ConvertToImage(SelectedData.photo);
                objectstoSecId.SelectedIndex = (int)SelectedData.stoSecId-1;
                objectstoPerId.SelectedIndex = (int)SelectedData.stoPerId-1;
                objectStatusId.SelectedIndex = (int)SelectedData.statusId-1;
            }
            else
            {
                MessageBox.Show("Не выбрано ниодной строки!", "Сообщение", MessageBoxButton.OK);
            }
        }

        public void DlgLoad(int mode, string DlgModeContent)
        {
            switch (mode)
            {
                case 0:
                    ListColumnChange.Width = new GridLength(100, GridUnitType.Star);
                    BookColumnChange.Width = new GridLength(0);
                    TreeColumnChange.Width = new GridLength(0);
                    ArchiveGrid.IsHitTestVisible = true;
                    Add.IsEnabled = true;
                    Copy.IsEnabled = true;
                    Edit.IsEnabled = true;
                    Delete.IsEnabled = true;
                    DlgMode = -1;
                    break;
                case 1:
                    ListColumnChange.Width = new GridLength(0);
                    BookColumnChange.Width = new GridLength(100, GridUnitType.Star);
                    TreeColumnChange.Width = new GridLength(0);
                    ArchiveGrid.IsHitTestVisible = false;
                    actionName.Content = DlgModeContent;
                    AddCommit.Content = DlgModeContent;
                    Add.IsEnabled = false;
                    Copy.IsEnabled = false;
                    Edit.IsEnabled = false;
                    Delete.IsEnabled = false;
                    break;
                case 2:
                    BookColumnChange.Width = new GridLength(0);
                    TreeColumnChange.Width = new GridLength(30, GridUnitType.Star);
                    ArchiveGrid.IsHitTestVisible = true;
                    actionName.Content = DlgModeContent;
                    AddCommit.Content = DlgModeContent;
                    Add.IsEnabled = true;
                    Copy.IsEnabled = true;
                    Edit.IsEnabled = true;
                    Delete.IsEnabled = true;
                    break;
                default:
                    ListColumnChange.Width = new GridLength(100, GridUnitType.Star);
                    BookColumnChange.Width = new GridLength(0);
                    TreeColumnChange.Width = new GridLength(0);
                    ArchiveGrid.IsHitTestVisible = true;
                    Add.IsEnabled = true;
                    Copy.IsEnabled = true;
                    Edit.IsEnabled = true;
                    Delete.IsEnabled = true;
                    DlgMode = -1;
                    break;
            }
        }
        //вывод информации в справочник
        public void UpdateGrid(Base.Data data = null)
        {
            if ((data == null) && (ArchiveGrid.ItemsSource != null))
            {
                data = (Base.Data)ArchiveGrid.SelectedItem;
            }
            Data = new ObservableCollection<Base.Data>(SourceCore.entities.Data);
            if (!isClientData) ArchiveGrid.ItemsSource = Data;
            else ArchiveGrid.ItemsSource = Data.Where(i => i.clientId == User.clientId);
            ArchiveGrid.SelectedItem = data ?? Data[0];
            objectStatusId.ItemsSource = SourceCore.entities.Status.ToList();
            objectstoSecId.ItemsSource = SourceCore.entities.StorageSection.ToList();
            objectstoPerId.ItemsSource = SourceCore.entities.StoragePeriod.ToList();
        }

        private void Tree_Click(object sender, RoutedEventArgs e)
        {
            if (isTree == false)
            {
                FormStackPanel.Visibility = Visibility.Collapsed;
                treeView1.Visibility = Visibility.Visible;
                isTree = true;
                DlgLoad(2, "");
            }
            else
            {
                FormStackPanel.Visibility = Visibility.Visible;
                treeView1.Visibility = Visibility.Collapsed;
                isTree = false;
                DlgLoad(0, "");
            }
        }

        private void ClientData_Click(object sender, RoutedEventArgs e)
        {
            if (isClientData == false)
            {
                isClientData = true;
                UpdateGrid(SelectedData);
                TreeViewLoader();
            }
            else
            {
                isClientData = false;
                UpdateGrid(SelectedData);
                TreeViewLoader();
            }
        }
        //кнопка удаления объекта
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить запись?", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                try
                {
                    Base.Data DeletingData = (Base.Data)ArchiveGrid.SelectedItem;
                    if (ArchiveGrid.SelectedIndex < ArchiveGrid.Items.Count - 1)
                    {
                        ArchiveGrid.SelectedIndex++;
                    }
                    else
                    {
                        if (ArchiveGrid.SelectedIndex > 0)
                        {
                            ArchiveGrid.SelectedIndex--;
                        }
                    }
                    Base.Data SelectingData = (Base.Data)ArchiveGrid.SelectedItem;
                    SourceCore.entities.Data.Remove(DeletingData);
                    SourceCore.entities.SaveChanges();
                    UpdateGrid(SelectingData);
                }
                catch
                {
                    MessageBox.Show("Невозможно удалить запись, так как она используется в других справочниках базы данных.", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None);
                }
            }
        }

        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            FormStackPanel.Visibility = Visibility.Visible;
            treeView1.Visibility = Visibility.Collapsed;
        }

        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            if (GroupNameTextBox.Text.Equals("")) return;

            switch (SelectGroupComboBox.SelectedIndex)
            {
                case 0:
                    var StoragePeriod = new Base.StoragePeriod();
                    StoragePeriod.name = GroupNameTextBox.Text;
                    SourceCore.entities.StoragePeriod.Add(StoragePeriod);
                    break;
                case 1:
                    var Status = new Base.Status();
                    Status.name = GroupNameTextBox.Text;
                    SourceCore.entities.Status.Add(Status);
                    break;
                case 2:
                    var StorageSection = new Base.StorageSection();
                    StorageSection.name = GroupNameTextBox.Text;
                    SourceCore.entities.StorageSection.Add(StorageSection);
                    break;
            }

            try
            {
                SourceCore.entities.SaveChanges();
                FormStackPanel.Visibility = Visibility.Collapsed;
                treeView1.Visibility = Visibility.Visible;
                TreeViewLoader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            FormStackPanel.Visibility = Visibility.Collapsed;
            treeView1.Visibility = Visibility.Visible;
        }
        //кнопка добавления фотографии
        private void photoPath_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
                imageData.Source = converter.ConvertToImage(_lastBase64);
            }
        }
    }
}