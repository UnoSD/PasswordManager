using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Paccia
{
    public partial class MainWindow
    {
        readonly EncryptedRepositoryFactory<Secret> _repositoryFactory;
        IReadOnlyCollection<Secret> _readOnlyCollection;
        string _passphrase;

        public MainWindow(EncryptedRepositoryFactory<Secret> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;

            InitializeComponent();
        }

        Repository<Secret> GetRepository(string passphrase) => _repositoryFactory.GetRepository(passphrase, Environment.MachineName, ConfigurationKey.SecretsFilePath);

        async void MainWindowOnActivated(object sender, EventArgs e)
        {
            _passphrase = "Passphrase from user input";

            _readOnlyCollection = await GetRepository(_passphrase).LoadAsync();

            PasswordListView.ItemsSource = _readOnlyCollection;
        }

        void PasswordListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var secrets = e.AddedItems.Cast<Secret>().ToArray();

            if (secrets.Length > 0)
                FieldsListView.ItemsSource = secrets.First().Fields.Concat(secrets.First().Secrets);
        }

        void PasswordSearchTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordSearchTextBox.Text.Length < 1)
            {
                PasswordListView.ItemsSource = _readOnlyCollection;
                return;
            }

            var lower = PasswordSearchTextBox.Text.ToLower();

            var itemsSource = _readOnlyCollection?.Where(secret => secret.Description.ToLower().Contains(lower));

            if (PasswordListView != null)
                PasswordListView.ItemsSource = itemsSource;
        }

        void FieldsListViewOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (KeyValuePair<string, string>)FieldsListView.SelectedItem;

            Clipboard.SetText(selectedItem.Value);

            Title = $"Copied {selectedItem.Key}";
        }

        async void AddSecretButtonOnClick(object sender, RoutedEventArgs e)
        {
            var addSecret = new AddSecretWindow();

            var secret = addSecret.CreateSecret();

            _readOnlyCollection = _readOnlyCollection.Concat(new[] { secret }).ToArray();
            
            await GetRepository(_passphrase).SaveAsync(_readOnlyCollection);

            PasswordListView.ItemsSource = _readOnlyCollection;
        }
    }
}
