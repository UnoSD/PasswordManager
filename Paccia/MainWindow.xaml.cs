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
        IReadOnlyCollection<Secret> _readOnlyCollection;

        public MainWindow()
        {
            InitializeComponent();
        }

        async void MainWindowOnActivated(object sender, EventArgs e)
        {
            var repository = new PasswordRepository(new Configuration(new HardcodedConfigurationDefaults()), new BinarySerializer<IEnumerable<Secret>>());

            _readOnlyCollection = await repository.LoadPasswordsAsync();

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

            var repository = new PasswordRepository(new Configuration(new HardcodedConfigurationDefaults()), new BinarySerializer<IEnumerable<Secret>>());

            await repository.SavePasswordsAsync(_readOnlyCollection);

            PasswordListView.ItemsSource = _readOnlyCollection;
        }
    }
}
