

using System;
using System.Collections.Generic;
using Blend;
using Noesis;


namespace ClientUI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainUI : UserControl
    {
        public static MainUI Instance;
        private Noesis.ContextMenu _mainContextMenu;
        private Noesis.Grid _mainContainer;
        public MainUI()
        {
            Instance = this;
            InitializeComponent();
        }


        private void InitializeComponent()
        {
           Noesis.GUI.LoadComponent(this,"Assets/UI/MainUI/MainUI.xaml");

            _mainContextMenu = (Noesis.ContextMenu) FindName("MainContextMenu");
            _mainContainer = (Grid) FindName("MainContainer");
        }


        public void ShowContextMenu(List<Tuple<string,string>> options)
        {
            if (options != null && options.Count > 0)
            {
                _mainContextMenu.Items.Clear();

                foreach (var opt in options)
                {
                    _mainContextMenu.Items.Add(opt.Item2); //TODO
                }
                _mainContextMenu.Visibility = Visibility.Visible;
                _mainContextMenu.IsOpen = true;
            }
        }

        public void ShowLoginWindow(string login,string password)
        {
            LoginWindow wnd=new LoginWindow();
            _mainContainer.Children.Add(wnd);
            wnd.SetLoginPassword(login,password);
        }

        public void ShowMessageBox(string text)
        {
            MessageBox msgb=new MessageBox();
            msgb.SetText(text);
            _mainContainer.Children.Add(msgb);
        }

        public void ShowMessageBox(string text,Action callback)
        {
            MessageBox msgb = new MessageBox();
            msgb.SetText(text);
            msgb.SetAction(callback);
            _mainContainer.Children.Add(msgb);
        }
    }
}
