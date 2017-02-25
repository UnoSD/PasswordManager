using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Paccia
{
    public partial class AddSecretBox
    {
        // Make completely "stateless": ask for name and await, returns then ask for
        // secret then return and loop until return null etc... WaitAny(save, cancel)...
        readonly Dictionary<string, string> _secrets = new Dictionary<string, string>();
        readonly Dictionary<string, string> _fields = new Dictionary<string, string>();

        public AddSecretBox()
        {
            InitializeComponent();

            SecretsListView.ItemsSource = _secrets;
            FieldsListView.ItemsSource = _fields;
        }

        void AddSecretBoxOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var sequence = new IInputElement[]
                           {
                               DescriptionTextBox,
                               SecretNameTextBox,
                               SecretPasswordBox,
                               FieldNameTextBox,
                               FieldValueTextBox,
                               SaveButton
                           };

            for (var i = 0; i < sequence.Length - 1; i++)
            {
                var nextToFocus = sequence[i + 1];

                sequence[i].KeyDown += (_, e) => ShiftFocusIfEnterKeyDown(nextToFocus, e);
            }
        }

        public Task<Secret> CreateSecretAsync() =>
            PopulateShowAndWaitAsync
            (
                ClearForm,
                saveOnExit => UpdateModelIf(saveOnExit, new Secret())
            );

        public Task EditSecretAsync(Secret secret) =>
            PopulateShowAndWaitAsync
            (
                () => PopulateUi(secret),
                saveOnExit => UpdateModelIf(saveOnExit, secret)
            );

        Secret UpdateModelIf(bool saveOnExit, Secret secret)
        {
            if (!saveOnExit)
                return null;

            secret.Description = DescriptionTextBox.Text;

            secret.Fields.Clear();
            secret.Secrets.Clear();

            secret.Fields.AddRange(_fields);
            secret.Secrets.AddRange(_secrets);

            return secret;
        }

        void PopulateUi(Secret secret)
        {
            ClearForm();

            DescriptionTextBox.Text = secret.Description;

            foreach (var item in secret.Fields)
                _fields.Add(item.Key, item.Value);

            foreach (var item in secret.Secrets)
                _secrets.Add(item.Key, item.Value);

            FieldsListView.Items.Refresh();
            SecretsListView.Items.Refresh();
        }

        public async Task<T> PopulateShowAndWaitAsync<T>(Action clearAndPopulate, Func<bool, T> onExitAction)
        {
            clearAndPopulate();

            Visibility = Visibility.Visible;

            DescriptionTextBox.Focus();

            var completionSource = new TaskCompletionSource<bool>();

            RoutedEventHandler saveButtonOnClick = (_, __) => completionSource.SetResult(true);
            RoutedEventHandler cancelButtonOnClick = (_, __) => completionSource.SetResult(false);

            SaveButton.Click += saveButtonOnClick;
            CancelButton.Click += cancelButtonOnClick;

            var saveOnExit = await completionSource.Task;

            SaveButton.Click -= saveButtonOnClick;
            CancelButton.Click -= cancelButtonOnClick;

            Visibility = Visibility.Collapsed;

            return onExitAction(saveOnExit);
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

        void AddSecretButtonOnClick(object sender, RoutedEventArgs e)
        {
            CheckAndAdd(_secrets, SecretNameTextBox, SecretPasswordBox.Password);

            SecretNameTextBox.Text = SecretPasswordBox.Password = null;
        }

        void AddFieldButtonOnClick(object sender, RoutedEventArgs e)
        {
            CheckAndAdd(_fields, FieldNameTextBox, FieldValueTextBox.Text);

            FieldNameTextBox.Text = FieldValueTextBox.Text = null;
        }

        void CheckAndAdd(IDictionary<string, string> dictionary, TextBox keyBox, string value)
        {
            if (dictionary.ContainsKey(keyBox.Text))
            {
                // Replace with "non-popup" (maybe a banner in the window).
                MessageBox.Show($"{keyBox.Text} gia presente.");

                return;
            }

            dictionary.Add(keyBox.Text, value);

            FieldsListView.Items.Refresh();
            SecretsListView.Items.Refresh();
        }

        void RemoveSecretButtonOnClick(object sender, RoutedEventArgs e) =>
            Remove(_secrets, SecretsListView);

        void RemoveFieldButtonOnClick(object sender, RoutedEventArgs e) =>
            Remove(_fields, FieldsListView);

        void Remove(IDictionary<string, string> dictionary, ListBox view)
        {
            var pairs = view.SelectedItems.Cast<KeyValuePair<string, string>>().ToArray();

            foreach (var pair in pairs)
                dictionary.Remove(pair);

            FieldsListView.Items.Refresh();
            SecretsListView.Items.Refresh();
        }

        void SecretsListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            UpdateRemoveButtonStatus(SecretsListView, RemoveSecretButton);

        void FieldsListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            UpdateRemoveButtonStatus(FieldsListView, RemoveFieldButton);

        static void UpdateRemoveButtonStatus(ListBox view, UIElement removeButton) =>
            removeButton.IsEnabled = view.SelectedItems.Count > 0;

        void FieldValueTextBoxOnKeyDown(object sender, KeyEventArgs e) =>
            InvokeHandlerIfEnterKeyDown(AddFieldButtonOnClick, e);

        void SecretPasswordBoxOnKeyDown(object sender, KeyEventArgs e) =>
            InvokeHandlerIfEnterKeyDown(AddSecretButtonOnClick, e);

        static void ShiftFocusIfEnterKeyDown(IInputElement nextFocus, KeyEventArgs keyEventArgs) =>
            InvokeHandlerIfEnterKeyDown((_, __) => nextFocus.Focus(), keyEventArgs);

        static void InvokeHandlerIfEnterKeyDown(EventHandler<RoutedEventArgs> eventHandler, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter)
                eventHandler(null, null);
        }
    }
}
