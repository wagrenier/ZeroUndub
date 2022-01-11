using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using ZeroUndubProcess;

namespace ZeroUndub
{
    public partial class MainWindow : Window
    {
        private const string _windowName = "PS2 Fatal Frame Undubber";

        public MainWindow()
        {
            RestorationOptions = new Options
            {
                IsUndub = false,
                IsModelImport = false,
                IsSubtitleInject = false
            };

            InitializeComponent();
        }

        private string JpIsoFile { get; set; }
        private string EuIsoFile { get; set; }
        private bool IsUndubLaunched { get; set; }

        private Options RestorationOptions { get; }

        private void CbUndubChecked(object sender, RoutedEventArgs e)
        {
            var newVal = UndubCheckBox.IsChecked == true;
            RestorationOptions.IsUndub = newVal;
        }

        private void CbModelImportChecked(object sender, RoutedEventArgs e)
        {
            var newVal = ImportModelsCheckBox.IsChecked == true;
            RestorationOptions.IsModelImport = newVal;
        }

        private void CbSubtitleChecked(object sender, RoutedEventArgs e)
        {
            var newVal = ImportSubtitlesCheckBox.IsChecked == true;
            RestorationOptions.IsSubtitleInject = newVal;
        }

        private void UndubGame(object sender, DoWorkEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(JpIsoFile) || string.IsNullOrWhiteSpace(EuIsoFile))
            {
                MessageBox.Show("Please select the files before!", _windowName);
                return;
            }

            MessageBox.Show("Copying the EU ISO, this may take a few minutes! pz_redux will be created in the same folder as the EU ISO.", _windowName);
            IsUndubLaunched = true;
            
            (sender as BackgroundWorker)?.ReportProgress(5);

            var importer = new ZeroFileImporter(EuIsoFile, JpIsoFile, RestorationOptions);

            var task = Task.Factory.StartNew(() =>
            {
                importer.RestoreGame();
                
            });

            while (!importer.Reporter.IsCompleted)
            {
                (sender as BackgroundWorker)?.ReportProgress(100 * importer.Reporter.FilesCompleted / importer.Reporter.TotalFiles);
                Thread.Sleep(100);
            }

            (sender as BackgroundWorker)?.ReportProgress(100);

            if (!importer.Reporter.IsSuccess)
            {
                MessageBox.Show($"The program failed with the following message: {importer.Reporter.ErrorMessage}",
                    _windowName);
                return;
            }

            MessageBox.Show("Don't forget to enable the 60Hz mode once in game", _windowName);
            MessageBox.Show("All Done! Enjoy the game :D", _windowName);
        }

        private void LaunchUndubbing(object sender, EventArgs e)
        {
            if (IsUndubLaunched)
            {
                return;
            }

            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            worker.DoWork += UndubGame;
            worker.ProgressChanged += WorkerProgressChanged;

            worker.RunWorkerAsync();
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private void FileSelectorClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Select the EU ISO", _windowName);
            var euFileDialog = new OpenFileDialog
            {
                Filter = "iso files (*.iso)|*.iso|All files (*.*)|*.*",
                Title = "Select the EU ISO"
            };

            if (euFileDialog.ShowDialog() == true)
            {
                EuIsoFile = euFileDialog.FileName;
            }
            
            MessageBox.Show("Select the JP ISO", _windowName);
            var jpFileDialog = new OpenFileDialog
            {
                Filter = "iso files (*.iso)|*.iso|All files (*.*)|*.*",
                Title = "Select the JP ISO"
            };

            if (jpFileDialog.ShowDialog() == true)
            {
                JpIsoFile = jpFileDialog.FileName;
            }
        }
    }
}