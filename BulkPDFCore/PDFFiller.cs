﻿//Sample license text.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkPDF
{
    public delegate string DelGetFilename(int dataSourceRow);
    public delegate void DelSetPercent(int percent);
    public delegate bool DelIsAborted();

    public static class PDFFiller
    {
        public static void CreateFiles(PDF pdf, bool finalize, IDataSource dataSource, Dictionary<string, PDFField> pdfFields, string outputDir
            , DelGetFilename GetFilename, DelSetPercent setPercent = null, DelIsAborted isAborted = null)
        {
            pdf.ResetFieldValue();
            dataSource.ResetRowCounter();
            for (int dataSourceRow = 1; dataSourceRow <= dataSource.PossibleRows; dataSourceRow++)
            {
                // DataSource
                foreach (var field in pdfFields.Keys)
                {
                    if (pdfFields[field].UseValueFromDataSource)
                    {
                        var value = dataSource.GetField(dataSource.Columns.FindIndex(x => x == pdfFields[field].DataSourceValue) + 1);
                        pdf.SetFieldValue(field, value, pdfFields[field].MakeReadOnly);
                    }
                }

                // PDF
                pdf.SaveFilledPDF(outputDir + GetFilename(dataSourceRow), finalize);
                pdf.ResetFieldValue();
                dataSource.NextRow();

                // Progress
                if (setPercent != null)
                    setPercent((int)((float)dataSourceRow / (float)dataSource.PossibleRows * (float)100));

                // Abort?
                if (isAborted != null)
                    if (isAborted())
                        break;
            }
            dataSource.ResetRowCounter();
        }
    }
}
