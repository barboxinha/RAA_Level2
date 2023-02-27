#region Namespaces
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Windows;
#endregion


namespace RAA_Level2
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class ViewRenumberResults : Window
    {
        private readonly List<Viewport> _renumberViewports;

        public ViewRenumberResults(List<Viewport> renumberViewports)
        {
            InitializeComponent();
            _renumberViewports = renumberViewports;

            ShowResults(_renumberViewports);
        }

        private void ShowResults(List<Viewport> renumberViewports) 
        {
            if (renumberViewports.Count > 0)
            {
                lbxResults.Items.Add("The following detail views have been renumbered successfully:\n");

                foreach (Viewport viewport in renumberViewports)
                {
                    string vportId = Convert.ToString(viewport.Id.IntegerValue);
                    string vportDetailNumber = Utils.GetBuiltinParameterValue(viewport, BuiltInParameter.VIEWPORT_DETAIL_NUMBER).ToString();
                    string vportViewName = Utils.GetBuiltinParameterValue(viewport, BuiltInParameter.VIEWPORT_VIEW_NAME).ToString();
                    string viewportItem = $"{vportDetailNumber} - {vportViewName} ({vportId})";

                    lbxResults.Items.Add(viewportItem);
                }

                lbxResults.Items.Add("\nYou can now CLOSE the window.");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
