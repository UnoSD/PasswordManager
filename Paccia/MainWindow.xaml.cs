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

        void ShowSecretButtonOnClick(object sender, RoutedEventArgs e) => SetShownState();

        void CopySecretButtonOnClick(object sender, RoutedEventArgs e)
        {
            var item = FieldsListView.SelectedItem ?? SecretsListView.SelectedItem;

            var selectedItem = (KeyValuePair<string, string>)item;

            Clipboard.SetText(SecretTextBox.Text);

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

        void SetShownState() => SetState(true, 0);

        void SetState(State last) => SetState(false, last);

        void SetState(bool shown, State last)
        {
            var selectedFields = FieldsListView.SelectedItems.Cast<KeyValuePair<string, string>>().ToArray();
            var selectedSecrets = SecretsListView.SelectedItems.Cast<KeyValuePair<string, string>>().ToArray();

            var currentState = selectedSecrets.Any() && selectedFields.Any() ?
                               last :
                               selectedFields.Any() ?
                                   State.FieldSelected :
                                   selectedSecrets.Any() ?
                                   shown ?
                                       State.SecretSelectedShown :
                                       State.SecretSelected :
                                   State.NothingSelected;

            switch (currentState)
            {
                case State.NothingSelected:
                    SecretTextBox.Text = null;
                    SecretTextBox.Visibility = Visibility.Hidden;
                    ShowSecretButton.IsEnabled = false;
                    CopySecretButton.IsEnabled = false;
                    break;
                case State.FieldSelected:
                    SecretsListView.SelectedItem = null;
                    SecretTextBox.Text = selectedFields.Single().Value;
                    SecretTextBox.Visibility = Visibility.Visible;
                    ShowSecretButton.IsEnabled = false;
                    CopySecretButton.IsEnabled = true;
                    break;
                case State.SecretSelected:
                    FieldsListView.SelectedItem = null;
                    SecretTextBox.Text = null;
                    SecretTextBox.Visibility = Visibility.Hidden;
                    ShowSecretButton.IsEnabled = true;
                    CopySecretButton.IsEnabled = true;
                    break;
                case State.SecretSelectedShown:
                    SecretTextBox.Text = selectedSecrets.Single().Value;
                    SecretTextBox.Visibility = Visibility.Visible;
                    ShowSecretButton.IsEnabled = false;
                    CopySecretButton.IsEnabled = true;
                    break;
            }
        }
    }
}
