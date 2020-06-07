namespace EmployeeTimeSheetMSSQLBL.Models.Helpers
{
    public class JSONResult
    {
        public bool Success { get; set; }
        public dynamic Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}