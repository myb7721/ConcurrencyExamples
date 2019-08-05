using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
namespace DeadlockDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
           Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}]");
            //Task.Run(()=> GetTask()).Wait();
            GetTask().Wait();
        }

        private async Task GetTask()
        {
            Console.WriteLine($"taskThread[{Thread.CurrentThread.ManagedThreadId}]");
            //await Task.Delay(new Random(Thread.CurrentThread.ManagedThreadId).Next((int)1E2, (int)1E4));
            await Task.Delay(new Random(Thread.CurrentThread.ManagedThreadId).Next((int)1E2, (int)1E4)).ConfigureAwait(false);
        }
    }
}
