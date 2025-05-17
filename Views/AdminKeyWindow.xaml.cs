using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HotelApp.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminKeyWindow.xaml
    /// </summary>
    public partial class AdminKeyWindow : Window
    {
        public string EnteredKey { get; private set; }

        public AdminKeyWindow()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            EnteredKey = KeyTextBox.Password.Trim();
            DialogResult = !string.IsNullOrEmpty(EnteredKey);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
