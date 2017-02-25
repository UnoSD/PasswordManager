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
        public AddSecretBox()
        {
            InitializeComponent();
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
            PopulateShowAndWaitAsync(new Secret(), (_, __, ___) => ClearForm(), UpdateModelOnSave);

        public Task EditSecretAsync(Secret secret) =>
            PopulateShowAndWaitAsync(secret, PopulateUi, UpdateModelOnSave);

        Secret UpdateModelOnSave(Secret secret, bool saveOnExit, Dictionary<string, string> fields, Dictionary<string, string> secrets)
        {
            if (!saveOnExit)
                return null;

            secret.Description = DescriptionTextBox.Text;

            secret.Fields.Clear();
            secret.Secrets.Clear();

            secret.Fields.AddRange(fields);
            secret.Secrets.AddRange(secrets);

            return secret;
        }

        void PopulateUi(Secret secret, Dictionary<string, string> fields, Dictionary<string, string> secrets)
        {
            ClearForm();

            DescriptionTextBox.Text = secret.Description;

            fields.AddRange(secret.Fields);
            secrets.AddRange(secret.Secrets);

            FieldsListView.Items.Refresh();
            SecretsListView.Items.Refresh();
        }

        delegate void ClearAndPopulate(Secret model, Dictionary<string, string> fieldsStorage, Dictionary<string, string> secretsStorage);

        delegate T UpdateModelFromUiIfSaved<out T>(Secret modelToUpdate, bool saveOnExit, Dictionary<string, string> fieldsStorage, Dictionary<string, string> secretsStorage);

        async Task<T> PopulateShowAndWaitAsync<T>(Secret secret, ClearAndPopulate clearAndPopulate, UpdateModelFromUiIfSaved<T> updateModelFromUiIfSaved)
        {
            var fields = new Dictionary<string, string>();
            var secrets = new Dictionary<string, string>();

            clearAndPopulate(secret, fields, secrets);

            Visibility = Visibility.Visible;

            DescriptionTextBox.Focus();

            var completionSource = new TaskCompletionSource<bool>();

            FieldsListView.ItemsSource = fields;
            SecretsListView.ItemsSource = secrets;
            
            var saveOnExit = await AddEvemtsAndWaitForUser(secrets, fields, completionSource);

            Visibility = Visibility.Collapsed;

            return updateModelFromUiIfSaved(secret, saveOnExit, fields, secrets);
        }

        async Task<bool> AddEvemtsAndWaitForUser(IDictionary<string, string> secrets, IDictionary<string, string> fields, TaskCompletionSource<bool> completionSource)
        {
            RoutedEventHandler addSecretButtonOnClick = (_, __) => AddSecretAndClear(secrets);
            RoutedEventHandler addFieldButtonOnClick = (_, __) => AddFieldAndClear(fields);
            RoutedEventHandler removeSecretButtonOnClick = (_, __) => Remove(secrets, SecretsListView);
            RoutedEventHandler removeFieldButtonOnClick = (_, __) => Remove(fields, FieldsListView);
            KeyEventHandler secretPasswordBoxOnKeyDown = (sender, e) => InvokeIfEnterKeyDown(() => AddSecretAndClear(secrets), e);
            KeyEventHandler fieldValueTextBoxOnKeyDown = (sender, e) => InvokeIfEnterKeyDown(() => AddFieldAndClear(fields), e);
            RoutedEventHandler saveButtonOnClick = (_, __) => completionSource.SetResult(true);
            RoutedEventHandler cancelButtonOnClick = (_, __) => completionSource.SetResult(false);

            AddSecretButton.Click += addSecretButtonOnClick;
            AddFieldButton.Click += addFieldButtonOnClick;
            RemoveSecretButton.Click += removeSecretButtonOnClick;
            RemoveFieldButton.Click += removeFieldButtonOnClick;
            SecretPasswordBox.KeyDown += secretPasswordBoxOnKeyDown;
            FieldValueTextBox.KeyDown += fieldValueTextBoxOnKeyDown;
            SaveButton.Click += saveButtonOnClick;
            CancelButton.Click += cancelButtonOnClick;

            var saveOnExit = await completionSource.Task;

            AddSecretButton.Click -= addSecretButtonOnClick;
            AddFieldButton.Click -= addFieldButtonOnClick;
            RemoveSecretButton.Click -= removeSecretButtonOnClick;
            RemoveFieldButton.Click -= removeFieldButtonOnClick;
            SecretPasswordBox.KeyDown -= secretPasswordBoxOnKeyDown;
            FieldValueTextBox.KeyDown -= fieldValueTextBoxOnKeyDown;
            SaveButton.Click -= saveButtonOnClick;
            CancelButton.Click -= cancelButtonOnClick;

            return saveOnExit;
        }

        void ClearForm()
        {
            DescriptionTextBox.Text =
            SecretNameTextBox.Text =
            SecretPasswordBox.Password =
            FieldNameTextBox.Text =
            FieldValueTextBox.Text =
                null;
            
            SecretsListView.ItemsSource =
            FieldsListView.ItemsSource = 
                null;
        }

        void AddSecretAndClear(IDictionary<string, string> secrets)
        {
            CheckAndAdd(secrets, SecretNameTextBox, SecretPasswordBox.Password);

            SecretNameTextBox.Text = SecretPasswordBox.Password = null;
        }

        void AddFieldAndClear(IDictionary<string, string> fields)
        {
            CheckAndAdd(fields, FieldNameTextBox, FieldValueTextBox.Text);

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

        static void ShiftFocusIfEnterKeyDown(IInputElement nextFocus, KeyEventArgs keyEventArgs) =>
            InvokeIfEnterKeyDown(() => nextFocus.Focus(), keyEventArgs);

        static void InvokeIfEnterKeyDown(Action eventHandler, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter)
                eventHandler();
        }
    }
}
