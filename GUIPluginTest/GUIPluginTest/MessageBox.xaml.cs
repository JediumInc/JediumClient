using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Noesis;

namespace Blend
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : UserControl
    {

        private Action _callback;
        public MessageBox()
        {
            //Noesis.GUI.LoadComponent(this, "G:\\work\\JediumV2\\Client2\\GUIPluginTest\\GUIPluginTest\\bin\\Debug\\MessageBox.xaml");
            StreamReader sr=new StreamReader("G:\\work\\JediumV2\\Client2\\GUIPluginTest\\GUIPluginTest\\bin\\Debug\\MessageBox.xaml");
            string cnt = sr.ReadToEnd();
            sr.Close();
           
           // this.
            this.Content = cnt;
            InitializeComponent();
        }

        private void InitializeComponent()
        {

           

            Button okbtn = (Button) FindName("OKButton");

            okbtn.Click += Okbtn_Click;

        }

        public void SetAction(Action clb)
        {
            _callback = clb;
        }

        public void SetText(string text)
        {
            TextBlock message = (TextBlock)FindName("Message");
            message.Text = text;
        }

        private void Okbtn_Click(object sender, RoutedEventArgs e)
        {
            

            ((Panel)this.Parent).Children.Remove(this);

            _callback?.Invoke();
        }
    }
}
