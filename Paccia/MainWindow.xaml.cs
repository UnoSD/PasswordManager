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
        string _selected;

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
            var secrets = EntryListView.SelectedItems.Cast<Secret>().ToArray();

            IDictionary<string, string> secretFields = null;
            IDictionary<string, string> secretSecrets = null;

            if (secrets.Length == 1)
            {
                var secret = secrets.First();

                secretFields = secret.Fields;
                secretSecrets = secret.Secrets;
            }

            FieldsListView.ItemsSource = secretFields;
            SecretsListView.ItemsSource = secretSecrets;
        }

        void EntrySearchTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (EntrySearchTextBox.Text.Length < 1)
            {
                EntryListView.ItemsSource = _secrets;

                return;
            }

            var searchText = EntrySearchTextBox.Text.ToLowerInvariant();

            var filteredSecrets = _secrets?.Where(secret => secret.Description.ToLowerInvariant().Contains(searchText));
            
            EntryListView.ItemsSource = filteredSecrets;
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

        void ShowSecretButtonOnClick(object sender, RoutedEventArgs e) => ShowSelectedValue();

        void ShowSelectedValue()
        {
            SecretTextBox.Text = _selected;
            SecretTextBox.Visibility = Visibility.Visible;
            CopySecretButton.IsEnabled = true;
            ShowSecretButton.IsEnabled = false;
        }

        void CopySecretButtonOnClick(object sender, RoutedEventArgs e)
        {
            var item = FieldsListView.SelectedItem ?? SecretsListView.SelectedItem;

            var selectedItem = (KeyValuePair<string, string>)item;

            Clipboard.SetText(SecretTextBox.Text);

            Title = $"Copied {selectedItem.Key}";
        }

        string GetActiveSelectedValueAndClearOthers(ListBox active, ListBox inactive)
        {
            var activeSelected = active.SelectedItems.Cast<KeyValuePair<string, string>>().ToArray();
            var inactiveSelected = inactive.SelectedItems.Cast<KeyValuePair<string, string>>().ToArray();

            if (activeSelected.Length != 1 && inactiveSelected.Length != 1)
            {
                ShowSecretButton.IsEnabled = false;
                CopySecretButton.IsEnabled = false;
                SecretTextBox.Text = null;
                SecretTextBox.Visibility = Visibility.Hidden;

                return null;
            }

            if (!activeSelected.Any())
                return null;

            inactive.SelectedItem = null;

            return activeSelected.Single().Value;
        }

        void FieldsListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = GetActiveSelectedValueAndClearOthers(FieldsListView, SecretsListView);

            if (_selected == null)
                return;

            ShowSelectedValue();
        }

        void SecretsListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = GetActiveSelectedValueAndClearOthers(SecretsListView, FieldsListView);

            SecretTextBox.Visibility = Visibility.Hidden;
            ShowSecretButton.IsEnabled = true;
            CopySecretButton.IsEnabled = true;
        }
    }
}
