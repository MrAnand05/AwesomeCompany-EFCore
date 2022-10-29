namespace AwesomeCompany.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public DateTime LastSalaryUpdatedUtc { get; set; }
        public List<Employee> Employees { get; set; }

    }
}
