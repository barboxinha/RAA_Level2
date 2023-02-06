using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace RAA_Level2
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class ProjectSetup : Window
    {
        private readonly string _defaultFile;
        private string _selectedFile = "";
        private List<string[]> _fileData;
        private ProjectUnits _projectUnits;

        public ProjectSetup()
        {
            InitializeComponent();
            _defaultFile = "C:\\Downloads\\ProjectSetup.csv";
            txbFilePath.Text = _defaultFile;
        }

        public string SelectedFile 
        {
            get { return _selectedFile; }
        }

        public List<string[]> FileData
        {
            get { return _fileData; }
        }

        public ProjectUnits Units
        {
            get { return _projectUnits; }
            private set { _projectUnits = value; }
        }

        public bool CreateFloorPlans 
        {
            get { return cbxCreateFloorPlans.IsChecked == true; } 
        }
        public bool CreateRCPs 
        {
            get { return cbxCreateRCPs.IsChecked == true; } 
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                RestoreDirectory = true,
                Multiselect = false,
                Filter = "csv files (*.csv)|*.csv"
            };

            if (openFile.ShowDialog(this) == true)
            {
                _selectedFile = openFile.FileName;
                txbFilePath.Text = _selectedFile;
                btnOK.IsEnabled = true;
            }
            else
            {
                // Do nothing
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFile == "")
            {
                MessageBox.Show("Please select a file (*.csv) to create Levels / Plan views from.", "Warning - No file selected");
                return;
            }

            _fileData = ReadCsvFile(_selectedFile, true);
            _projectUnits = GetProjectUnits();

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Eventually move to separate ViewModel or Util class for MVVM.
        private List<string[]> ReadCsvFile(string file, bool removeHeader = false)
        {
            List<string[]> fileData = new List<string[]>();
            string[] dataRows = File.ReadAllLines(file);

            foreach (string row in dataRows)
            {
                string[] rowCells = row.Split(',');
                fileData.Add(rowCells);
            }

            if (removeHeader)
            {
                fileData.RemoveAt(0);
            }

            return fileData;
        }

        private ProjectUnits GetProjectUnits()
        {
            return rbnImperial.IsChecked == true 
                ? ProjectUnits.Imperial 
                : ProjectUnits.Metric;
        }
    }

    public enum ProjectUnits
    {
        Metric,
        Imperial
    }
}
