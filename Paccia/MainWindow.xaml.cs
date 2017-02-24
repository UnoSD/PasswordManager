using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Paccia
{
    public partial class MainWindow
    {
        readonly EncryptedRepositoryFactory<Secret> _repositoryFactory;
        IList<Secret> _secrets;
        SecureString _passphrase;

        public MainWindow(EncryptedRepositoryFactory<Secret> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;

            InitializeComponent();
        }

        Repository<Secret> GetRepository(string passphrase) => 
            _repositoryFactory.GetRepository(passphrase, Environment.MachineName, ConfigurationKey.SecretsFilePath);

        async void MainWindowOnLoaded(object sender, EventArgs e) =>
            await DisableUserInteractionsWhile(async () =>
            {
                _passphrase = await MasterPasswordInputBox.GetPasswordAsync();

                _secrets = (await GetRepository(_passphrase.ToClearString()).LoadAsync()).ToList();

                EntryListView.ItemsSource = _secrets;
            });

        Task DisableUserInteractionsWhile(Func<Task> action) =>
            DisableUserInteractionsWhile(async () => { await action(); return 0; });

        async Task<T> DisableUserInteractionsWhile<T>(Func<Task<T>> action)
        {
            MainContent.IsEnabled = false;

            var result = await action();

            MainContent.IsEnabled = true;

            return result;
        }

        void EntryListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var secrets = e.AddedItems.Cast<Secret>().ToArray();

            if (secrets.Length != 1)
                return;

            var secret = secrets.First();

            FieldsListView.ItemsSource = secret.Fields;
            SecretsListView.ItemsSource = secret.Secrets;
        }

        void EntrySearchTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (EntrySearchTextBox.Text.Length < 1)
            {
                EntryListView.ItemsSource = _secrets;
                return;
            }

            var lower = EntrySearchTextBox.Text.ToLower();

            var itemsSource = _secrets?.Where(secret => secret.Description.ToLower().Contains(lower));

            if (EntryListView != null)
                EntryListView.ItemsSource = itemsSource;
        }

        async void AddSecretButtonOnClick(object sender, RoutedEventArgs e) => 
            await DisableUserInteractionsWhile(async () =>
            {
                var secret = await AddSecretBox.CreateSecretAsync();

                if (secret == null)
                    return;

                _secrets.Add(secret);

                await GetRepository(_passphrase.ToClearString()).SaveAsync(_secrets);

                EntryListView.Items.Refresh();
            });

        void ShowSecretButtonOnClick(object sender, RoutedEventArgs e) => SecretTextBox.Visibility = Visibility.Visible;

        void CopySecretButtonOnClick(object sender, RoutedEventArgs e)
        {
            var item = FieldsListView.SelectedItem ?? SecretsListView.SelectedItem;

            var selectedItem = (KeyValuePair<string, string>)item;

            Clipboard.SetText(SecretTextBox.Text);

            Title = $"Copied {selectedItem.Key}";
        }

        void FieldsListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
                return;

            SecretsListView.SelectedItem = null;

            var selectedItem = FieldsListView.SelectedItem as KeyValuePair<string, string>?;

            SecretTextBox.Text = selectedItem?.Value;

            SecretTextBox.Visibility = Visibility.Visible;
        }

        void SecretsListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
                return;

            FieldsListView.SelectedItem = null;

            var selectedItem = SecretsListView.SelectedItem as KeyValuePair<string, string>?;

            SecretTextBox.Visibility = Visibility.Hidden;

            SecretTextBox.Text = selectedItem?.Value;
        }
    }
}
