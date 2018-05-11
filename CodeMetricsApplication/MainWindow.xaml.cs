using System;
using System.Windows;
using CodeMetricsLibrary;

namespace CodeMetricsApplication
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var metrics = new DzhilbMetrics(TextTextBox.Text);

                OperatorsLabel.Content = metrics.OperatorsCount;
                ClAbsoluteLabel.Content = metrics.ClAbsolute;
                ClLabel.Content = metrics.Cl;
                FModLabel.Content = metrics.FMod;
                LIfLabel.Content = metrics.LIf;
                LModLabel.Content = metrics.LMod;
                LLoopLabel.Content = metrics.LLoop;
                FOpLabel.Content = metrics.FOp;

                DictionaryTextBox.Text = metrics.GetLinesText();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
