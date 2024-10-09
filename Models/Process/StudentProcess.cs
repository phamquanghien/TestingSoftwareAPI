using System.Data;

namespace TestingSoftwareAPI.Models.Process
{
    public class StudentProcess
    {
        public DataTable GetStudentTableDistinct(DataTable originalDataTable)
        {
            //string[] columnNamesToClone = { "Column0", "Column2", "Column3", "Column4" };
            DataTable clonedTable = originalDataTable.Copy();
            clonedTable.Columns.RemoveAt(12);
            clonedTable.Columns.RemoveAt(11);
            clonedTable.Columns.RemoveAt(10);
            clonedTable.Columns.RemoveAt(9);
            clonedTable.Columns.RemoveAt(8);
            clonedTable.Columns.RemoveAt(7);
            clonedTable.Columns.RemoveAt(6);
            clonedTable.Columns.RemoveAt(5);
            clonedTable.Columns.RemoveAt(1);
            var distinctDataTable = clonedTable.Clone();
            try {
                var uniqueRows = new HashSet<string>();
                foreach (DataRow row in clonedTable.Rows)
                {
                    var value = row[0]?.ToString();
                    if (!string.IsNullOrEmpty(value) && uniqueRows.Add(value))
                    {
                        distinctDataTable.ImportRow(row);
                    }
                }
            } catch{
            }
            return distinctDataTable;
        }
    }
}