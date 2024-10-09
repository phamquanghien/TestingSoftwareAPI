using System.Data;

namespace TestingSoftwareAPI.Models.Process
{
    public class SubjectProcess
    {
        public DataTable GetSubjectTableDistinct(DataTable originalDataTable)
        {
            DataTable clonedTable = originalDataTable.Copy();
            clonedTable.Columns.RemoveAt(12);
            clonedTable.Columns.RemoveAt(11);
            clonedTable.Columns.RemoveAt(10);
            clonedTable.Columns.RemoveAt(9);
            clonedTable.Columns.RemoveAt(8);
            clonedTable.Columns.RemoveAt(6);
            clonedTable.Columns.RemoveAt(5);
            clonedTable.Columns.RemoveAt(4);
            clonedTable.Columns.RemoveAt(3);
            clonedTable.Columns.RemoveAt(2);
            clonedTable.Columns.RemoveAt(0);
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