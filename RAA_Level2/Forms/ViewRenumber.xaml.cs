#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
#endregion


namespace RAA_Level2
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class ViewRenumber : Window
    {
        public Document Document { get; }

        public ViewRenumber(Document doc, List<Viewport> renumberViewports)
        {
            InitializeComponent();
            Document = doc;

            if (renumberViewports != null)
            {
                // Populate selected detail viewports
                foreach (Viewport viewport in renumberViewports)
                {
                    string vportId = Convert.ToString(viewport.Id.IntegerValue);
                    string vportDetailNumber = Utils.GetBuiltinParameterValue(viewport, BuiltInParameter.VIEWPORT_DETAIL_NUMBER).ToString();
                    string vportViewName = Utils.GetBuiltinParameterValue(viewport, BuiltInParameter.VIEWPORT_VIEW_NAME).ToString();
                    string viewportItem = $"{vportId} | {vportDetailNumber} - {vportViewName}";

                    lbxPickedViews.Items.Add(viewportItem);
                }

                // Populate renumber start sequence
                var sequence = Enumerable.Range(0, 51);

                foreach (int num in sequence)
                {
                    string strNum = Convert.ToString(num);
                    cmbSequenceStart.Items.Add(strNum);
                }

                cmbSequenceStart.SelectedIndex = 0;
                btnSelect.IsEnabled = false;
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (lbxPickedViews.Items.Count == 0)
            {
                TaskDialog.Show("Warning", "No viewports have been selected yet...");
                return;
            }

            DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        internal int GetRenumberSequenceStart() 
        {
            var selectedSequenceStart = cmbSequenceStart.SelectedItem;

            return Convert.ToInt32(selectedSequenceStart);
        }
    }
}
