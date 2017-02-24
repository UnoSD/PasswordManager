using System;
using System.Collections.Generic;
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

        public async Task<Secret> CreateSecretAsync()
        {
            ClearForm();

            Visibility = Visibility.Visible;

            DescriptionTextBox.Focus();

            _saveOnExit = new TaskCompletionSource<bool>();

            var saveOnExit = await _saveOnExit.Task;

            Visibility = Visibility.Collapsed;
            
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

        void SaveButtonOnClick(object sender, RoutedEventArgs e) => _saveOnExit.SetResult(true);

        void CancelButtonOnClick(object sender, RoutedEventArgs e) => _saveOnExit.SetResult(false);
    }
}
