using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using Wolfpack.Core.Interfaces;
using System.Linq;

namespace Wolfpack.Core.Artifacts.Formats
{
    public class TabSeparatedFormatter : IArtifactFormatter
    {
        private readonly string _tab;

        public TabSeparatedFormatter()
        {
            _tab = '\t'.ToString(CultureInfo.InvariantCulture);
        }

        public string ContentType
        {
            get { return "text/csv"; }
        }

        public Stream Serialize(object data)
        {
            Validate(data);

            Stream stream;

            if (HandleDataTable(data, out stream))
                return stream;
            if (HandleEnumerable(data, out stream))
                return stream;
            throw new ApplicationException("Failed to convert data to a stream");
        }

        private bool HandleEnumerable(object data, out Stream stream)
        {
            throw new NotImplementedException();
        }

        private bool HandleDataTable(object data, out Stream stream)
        {
            stream = null;
            var table = data as DataTable;

            if (table == null)
                return false;

            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            sw.WriteLine(string.Join(_tab, table.Columns.Cast<DataColumn>()
                .Select(dc => dc.ColumnName)
                .ToArray()));

            table.Rows.Cast<DataRow>().ToList().ForEach(
                row => sw.WriteLine(string.Join(_tab, row.ItemArray.Select(ri => ri.ToString()).ToArray())));

            sw.Flush();            
            ms.Seek(0, SeekOrigin.Begin);
            stream = ms;
            return true;
        }

        public void Validate(object data)
        {
            var dataType = data.GetType();

            if (dataType == typeof(DataTable))
                return;
            if (dataType is IEnumerable)
                throw new NotSupportedException("Yet!");

            throw new NotSupportedException(string.Format("Artifact Formatter '{0}' does not support the datatype '{1}'",
                    GetType().Name, dataType.Name));
        }
    }
}