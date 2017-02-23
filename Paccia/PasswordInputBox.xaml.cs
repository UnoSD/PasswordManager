using System;
using System.Security;
using System.Windows;
using System.Windows.Input;

namespace Paccia
{
    public partial class PasswordInputBox
    {
        public event EventHandler<SecureString> PasswordInserted;
        public event EventHandler Cancel;

        public PasswordInputBox()
        {
            InitializeComponent();
        }

        void OkButtonOnClick(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            PasswordInserted?.Invoke(this, PasswordBox.SecurePassword);
        }

        void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            Cancel?.Invoke(this, EventArgs.Empty);
        }

        void PasswordBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OkButtonOnClick(null, null);

            if (e.Key == Key.Escape)
                CancelButtonOnClick(null, null);
        }

        void PasswordInputBoxOnLoaded(object sender, RoutedEventArgs e) => PasswordBox.Focus();

        public void Show()
        {
            Visibility = Visibility.Visible;
            
            Focus();
        }
    }
}
