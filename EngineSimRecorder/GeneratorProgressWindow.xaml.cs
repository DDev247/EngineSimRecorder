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

namespace EngineSimRecorder
{
    /// <summary>
    /// Interaction logic for GeneratorProgressWindow.xaml
    /// </summary>
    public partial class GeneratorProgressWindow : Window
    {
        public bool isDone = false;

        public GeneratorProgressWindow()
        {
            InitializeComponent();
            Update();
        }

        private async Task Update()
        {
            while(true)
            {
                Major.Text = MainWindow.MajorStatus;
                Minor.Text = MainWindow.MinorStatus;

                await Task.Delay(1);
            }
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            if(isDone)
            {
                Close();
            }
        }
    }
}
