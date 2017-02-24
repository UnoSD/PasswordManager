using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Paccia
{
    public partial class AddSecretBox
    {
        readonly Dictionary<string, string> _secrets = new Dictionary<string, string>();
        readonly Dictionary<string, string> _fields = new Dictionary<string, string>();

        readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);
        bool _save;

        public AddSecretBox()
        {
            InitializeComponent();

            SecretsListView.ItemsSource = _secrets;
            FieldsListView.ItemsSource = _fields;
        }

        public async Task<Secret> CreateSecretAsync()
        {
            // Reset values to defaults.

            Visibility = Visibility.Visible;

            DescriptionTextBox.Focus();

            await _resetEvent.WaitOneAsync();

            _resetEvent.Reset();

            Visibility = Visibility.Collapsed;

            if (!_save)
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

        void AddSecretButtonOnClick(object sender, RoutedEventArgs e) =>
            CheckAndAdd(_secrets, SecretNameTextBox, SecretPasswordTextBox.Password);

        void AddFieldButtonOnClick(object sender, RoutedEventArgs e) =>
            CheckAndAdd(_fields, FieldNameTextBox, FieldValueTextBox.Text);

        void CheckAndAdd(IDictionary<string, string> dictionary, TextBox keyBox, string value)
        {
            if (dictionary.ContainsKey(keyBox.Text))
            {
                MessageBox.Show($"{keyBox.Text} gia presente.");

                return;
            }

            dictionary.Add(keyBox.Text, value);

            FieldsListView.Items.Refresh();
            SecretsListView.Items.Refresh();
        }

        void SaveButtonOnClick(object sender, RoutedEventArgs e) => Exit(true);

        void CancelButtonOnClick(object sender, RoutedEventArgs e) => Exit(false);

        void Exit(bool saveOnExit)
        {
            _save = saveOnExit;

            _resetEvent.Set();
        }
    }
}
