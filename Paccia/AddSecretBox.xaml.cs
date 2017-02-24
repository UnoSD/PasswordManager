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

            await _resetEvent.WaitOneAsync();

            _resetEvent.Reset();

            Visibility = Visibility.Collapsed;

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
            CheckAndAdd(_secrets, SecretNameTextBox, SecretPasswordTextBox);

        void AddFieldButtonOnClick(object sender, RoutedEventArgs e) =>
            CheckAndAdd(_fields, FieldNameTextBox, FieldValueTextBox);

        void CheckAndAdd(IDictionary<string, string> dictionary, TextBox keyBox, TextBox valueBox)
        {
            if (dictionary.ContainsKey(keyBox.Text))
            {
                MessageBox.Show($"{keyBox.Text} gia presente.");

                return;
            }

            dictionary.Add(keyBox.Text, valueBox.Text);

            FieldsListView.Items.Refresh();
            SecretsListView.Items.Refresh();
        }

        void SaveButtonOnClick(object sender, RoutedEventArgs e) => _resetEvent.Set();
    }
}
