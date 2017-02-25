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

                if (_passphrase == null)
                {
                    Close();
                    return;
                }

                _secrets = (await GetRepository(_passphrase.ToClearString()).LoadAsync()).ToList();

                EntryListView.ItemsSource = _secrets;

                MasterPasswordInputBox.Focus();
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

            FieldsListView.SelectionChanged -= FieldsListViewOnSelectionChanged;
            FieldsListView.ItemsSource = secretFields;
            FieldsListView.SelectionChanged += FieldsListViewOnSelectionChanged;
            SecretsListView.SelectionChanged -= SecretsListViewOnSelectionChanged;
            SecretsListView.ItemsSource = secretSecrets;
            SecretsListView.SelectionChanged += SecretsListViewOnSelectionChanged;

            DeleteSecretButton.IsEnabled = secrets.Any();

            SetState(State.NothingSelected);
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

        async void DeleteSecretButtonOnClick(object sender, RoutedEventArgs e) =>
            await DisableUserInteractionsWhile(async () =>
            {
                var secretsToDelete = EntryListView.SelectedItems.Cast<Secret>().ToArray();

                foreach (var secret in secretsToDelete)
                    _secrets.Remove(secret);

                await GetRepository(_passphrase.ToClearString()).SaveAsync(_secrets);

                EntryListView.Items.Refresh();
            });

        void ShowSecretButtonOnClick(object sender, RoutedEventArgs e) => SetState(State.SecretSelectedShown);

        void CopySecretButtonOnClick(object sender, RoutedEventArgs e)
        {
            var item = FieldsListView.SelectedItem ?? SecretsListView.SelectedItem;

            var selectedItem = (KeyValuePair<string, string>)item;

            Clipboard.SetDataObject(SecretTextBox.Text);

            Title = $"Copied {selectedItem.Key}";
        }

        void FieldsListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e) => SetState(State.FieldSelected);

        void SecretsListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e) => SetState(State.SecretSelected);

        enum State
        {
            NothingSelected,
            FieldSelected,
            SecretSelected,
            SecretSelectedShown
        }

        static string GetSelectedItemValue(ListBox view) => 
            view.SelectedItems.Cast<KeyValuePair<string, string>>().Single().Value;

        void SetState(State newState)
        {
            switch (newState)
            {
                case State.NothingSelected:
                    SecretTextBox.Text = null;
                    SecretTextBox.Visibility = Visibility.Hidden;
                    ShowSecretButton.IsEnabled = false;
                    CopySecretButton.IsEnabled = false;
                    break;
                case State.FieldSelected:
                    SecretsListView.SelectionChanged -= SecretsListViewOnSelectionChanged;
                    SecretsListView.SelectedItem = null;
                    SecretsListView.SelectionChanged += SecretsListViewOnSelectionChanged;
                    SecretTextBox.Text = GetSelectedItemValue(FieldsListView);
                    SecretTextBox.Visibility = Visibility.Visible;
                    ShowSecretButton.IsEnabled = false;
                    CopySecretButton.IsEnabled = true;
                    break;
                case State.SecretSelected:
                    FieldsListView.SelectionChanged -= FieldsListViewOnSelectionChanged;
                    FieldsListView.SelectedItem = null;
                    FieldsListView.SelectionChanged += FieldsListViewOnSelectionChanged;
                    SecretTextBox.Text = null;
                    SecretTextBox.Visibility = Visibility.Hidden;
                    ShowSecretButton.IsEnabled = true;
                    CopySecretButton.IsEnabled = true;
                    break;
                case State.SecretSelectedShown:
                    SecretTextBox.Text = GetSelectedItemValue(SecretsListView);
                    SecretTextBox.Visibility = Visibility.Visible;
                    ShowSecretButton.IsEnabled = false;
                    CopySecretButton.IsEnabled = true;
                    break;
            }
        }
    }
}
