using System;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Paccia
{
    public partial class PasswordInputBox
    {
        TaskCompletionSource<SecureString> _taskCompletionSource;

        public PasswordInputBox()
        {
            InitializeComponent();
        }

        void OkButtonOnClick(object sender, RoutedEventArgs e) => _taskCompletionSource.SetResult(PasswordBox.SecurePassword);

        void CancelButtonOnClick(object sender, RoutedEventArgs e) => _taskCompletionSource.SetResult(null);

        void PasswordBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OkButtonOnClick(null, null);

            if (e.Key == Key.Escape)
                CancelButtonOnClick(null, null);
        }

        public async Task<SecureString> GetPasswordAsync()
        {
            // Use keylogger prevention.
            // Maybe: 1-3-5 character of the passphrase first
            // then 2-4-6...

            _taskCompletionSource = new TaskCompletionSource<SecureString>();

            Visibility = Visibility.Visible;

            await Dispatcher.BeginInvoke
                (
                    DispatcherPriority.Input,
                    (Action)(() =>
                    {
                        PasswordBox.Focus();
                        Keyboard.Focus(PasswordBox);
                    })
                );

            var secureString = await _taskCompletionSource.Task;

            Visibility = Visibility.Collapsed;

            return secureString;
        }
    }
}
