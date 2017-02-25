using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Paccia
{
    public partial class AddSecretBox
    {
        // Make completely "stateless": ask for name and await, returns then ask for
        // secret then return and loop until return null etc... WaitAny(save, cancel)...
        readonly Dictionary<string, string> _secrets = new Dictionary<string, string>();
        readonly Dictionary<string, string> _fields = new Dictionary<string, string>();

        TaskCompletionSource<bool> _saveOnExit;

        public AddSecretBox()
        {
            InitializeComponent();

            SecretsListView.ItemsSource = _secrets;
            FieldsListView.ItemsSource = _fields;
        }

        public async Task<Secret> CreateSecretAsync() => await ClearPrepareShowAndWaitAsync(() =>
            {
                
            },
            saveOnExit =>
            {
                if (!saveOnExit)
                    return null;

                var secret = new Secret
                {
                    Description = DescriptionTextBox.Text
                };

                foreach (var item in _fields)
                    secret.Fields.Add(item);

                foreach (var item in _secrets)
                    secret.Secrets.Add(item);

                return secret;
            });

        public Task EditSecretAsync(Secret secret) => ClearPrepareShowAndWaitAsync(() =>
            {
                DescriptionTextBox.Text = secret.Description;

                foreach (var item in secret.Fields)
                    _fields.Add(item.Key, item.Value);

                foreach (var item in secret.Secrets)
                    _secrets.Add(item.Key, item.Value);

                FieldsListView.Items.Refresh();
                SecretsListView.Items.Refresh();
            },
            saveOnExit =>
            {
                if (!saveOnExit)
                    return false;

                secret.Description = DescriptionTextBox.Text;

                secret.Fields.Clear();

                foreach (var item in _fields)
                    secret.Fields.Add(item);

                secret.Secrets.Clear();

                foreach (var item in _secrets)
                    secret.Secrets.Add(item);

                return true;
            });

        public async Task<T> ClearPrepareShowAndWaitAsync<T>(Action prepare, Func<bool, T> after)
        {
            ClearForm();

            prepare();

            Visibility = Visibility.Visible;

            DescriptionTextBox.Focus();

            _saveOnExit = new TaskCompletionSource<bool>();

            var saveOnExit = await _saveOnExit.Task;

            Visibility = Visibility.Collapsed;

            return after(saveOnExit);
        }

        void ClearForm()
        {
            DescriptionTextBox.Text =
                    SecretNameTextBox.Text =
                            SecretPasswordBox.Password =
                                    FieldNameTextBox.Text =
                                            FieldValueTextBox.Text =
                                                    null;

            _fields.Clear();
            _secrets.Clear();

            SecretsListView.Items.Refresh();
            FieldsListView.Items.Refresh();
        }

        void AddSecretButtonOnClick(object sender, RoutedEventArgs e) =>
            CheckAndAdd(_secrets, SecretNameTextBox, SecretPasswordBox.Password, () => SecretNameTextBox.Text = SecretPasswordBox.Password = null);

        void AddFieldButtonOnClick(object sender, RoutedEventArgs e) =>
            CheckAndAdd(_fields, FieldNameTextBox, FieldValueTextBox.Text, () => FieldNameTextBox.Text = FieldValueTextBox.Text = null);

        void RemoveSecretButtonOnClick(object sender, RoutedEventArgs e) => 
            Remove(_secrets, SecretsListView);

        void RemoveFieldButtonOnClick(object sender, RoutedEventArgs e) =>
            Remove(_fields, FieldsListView);

        void CheckAndAdd(IDictionary<string, string> dictionary, TextBox keyBox, string value, Action clear)
        {
            if (dictionary.ContainsKey(keyBox.Text))
            {
                // Replace with "non-popup" (maybe a banner in the window).
                MessageBox.Show($"{keyBox.Text} gia presente.");

                return;
            }

            dictionary.Add(keyBox.Text, value);

            clear();

            FieldsListView.Items.Refresh();
            SecretsListView.Items.Refresh();
        }

        void Remove(IDictionary<string, string> dictionary, ListBox view)
        {
            var pairs = view.SelectedItems.Cast<KeyValuePair<string, string>>().ToArray();

            foreach (var pair in pairs)
                dictionary.Remove(pair);

            FieldsListView.Items.Refresh();
            SecretsListView.Items.Refresh();
        }

        void SaveButtonOnClick(object sender, RoutedEventArgs e) => _saveOnExit.SetResult(true);

        void CancelButtonOnClick(object sender, RoutedEventArgs e) => _saveOnExit.SetResult(false);

        void SecretsListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e) => 
            UpdateRemoveButtonStatus(SecretsListView, RemoveSecretButton);

        void FieldsListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            UpdateRemoveButtonStatus(FieldsListView, RemoveFieldButton);

        static void UpdateRemoveButtonStatus(ListBox view, UIElement removeButton) => 
            removeButton.IsEnabled = view.SelectedItems.Count > 0;
    }
}
