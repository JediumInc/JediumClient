using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JediumCore;
using Noesis;

namespace Blend
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : UserControl
    {
        private Button _doLoginButton;

        public LoginWindow()
        {
            Noesis.GUI.LoadComponent(this, "Assets/UI/MainUI/Windows/LoginWindow.xaml");
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _doLoginButton = (Button) FindName("LoginButton");
            _doLoginButton.Click += _doLoginButton_Click;
            //  Noesis.GUI.LoadComponent(this, "Assets/ClientUI/MainUI.xaml");
            //
            //  _mainContextMenu = (Noesis.ContextMenu)FindName("MainContextMenu");
        }

        public void SetLoginPassword(string login, string password)
        {
            TextBox LoginBox = (TextBox)FindName("Login");
            PasswordBox psswd = (PasswordBox)FindName("Password");

            LoginBox.Text = login;
            psswd.Password = password;
        }

        private async void _doLoginButton_Click(object sender, RoutedEventArgs e)
        {

            TextBox LoginBox = (TextBox) FindName("Login");
            PasswordBox psswd = (PasswordBox) FindName("Password");


          ((Panel)this.Parent).Children.Remove(this);

            await Test.Instance.DoLogin(LoginBox.Text, psswd.Password);
        }
    }
}
