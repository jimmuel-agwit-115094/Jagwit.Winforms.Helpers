using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Jagwit.Winforms.Helpers.Helpers
{
    /// <summary>
    /// Provides a helper method for configuring and rendering RDLC reports inside a <see cref="ReportViewer"/> control.
    /// </summary>
    public static class PrintingHelper
    {
        /// <summary>
        /// Binds a data source and optional parameters to the <paramref name="reportViewer"/>,
        /// resolves the RDLC file from the application's startup path, switches to print-layout
        /// mode, and refreshes the report.
        /// </summary>
        /// <param name="reportViewer">The <see cref="ReportViewer"/> control to configure.</param>
        /// <param name="reportName">
        /// The RDLC file name without extension (e.g. <c>"ReceiptRdlc"</c>).
        /// Resolved as <c>&lt;StartupPath&gt;\Printing\rdlc\&lt;reportName&gt;.rdlc</c>.
        /// </param>
        /// <param name="dataSource">The data source to bind (e.g. a typed <c>DataTable</c> or a list).</param>
        /// <param name="dataSetName">The dataset name declared inside the RDLC file.</param>
        /// <param name="parameters">Optional key/value pairs mapped to RDLC report parameters.</param>
        public static void PrintReport(
            ReportViewer reportViewer,
            string reportName,
            object dataSource,
            string dataSetName,
            Dictionary<string, string>? parameters = null)
        {
            try
            {
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSetName, dataSource));

                reportViewer.LocalReport.ReportPath = Path.Combine(
                    Application.StartupPath, "Printing", "rdlc", $"{reportName}.rdlc");

                reportViewer.LocalReport.SetParameters(new ReportParameter[] { });

                if (parameters != null && parameters.Count > 0)
                {
                    var reportParameters = new List<ReportParameter>();
                    foreach (var param in parameters)
                        reportParameters.Add(new ReportParameter(param.Key, param.Value));

                    reportViewer.LocalReport.SetParameters(reportParameters);
                }

                reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
                reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while printing the report: {ex}");
            }
        }
    }
}
