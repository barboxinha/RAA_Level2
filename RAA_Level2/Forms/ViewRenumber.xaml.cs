#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows;
#endregion


namespace RAA_Level2
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class ViewRenumber : Window
    {
        public List<Viewport> RenumberViewports { get; set; }
        public Document Document { get; }

        public ViewRenumber(Document doc, List<Viewport> renumberViewports)
        {
            InitializeComponent();
            Document = doc;

            if (renumberViewports != null)
            {
                RenumberViewports = renumberViewports;

                foreach (Viewport viewport in renumberViewports)
                {
                    string viewportNumber = viewport.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER).ToString();
                    string viewportName = viewport.Name;
                    string viewportItem = $"{viewportNumber} - {viewportName}";

                    lbxPickedViews.Items.Add(viewportItem);
                }
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            // Renumber All Selected Viewports and Display Results window
            if (lbxPickedViews.Items.Count == 0)
            {
                TaskDialog.Show("Warning", "No viewports have been selected yet...");
                return;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
