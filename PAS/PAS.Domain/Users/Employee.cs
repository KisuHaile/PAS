using Domain.Common;

namespace Domain.Users
{
    public class Employee : BaseEntity
    {
        public string EmployeeCode { get; private set; }

        public string FullName { get; private set; }

        public string Department { get; private set; }

        public string? Position { get; private set; }

        public string? Email { get; private set; }

        public bool IsActive { get; private set; } = true;

        private Employee()
        {
        }

        public Employee(string code, string name, string department, string? email = null, string? position = null)
        {
            EmployeeCode = code;
            FullName = name;
            Department = department;
            Email = email;
            Position = position;
        }

        public void UpdateDetails(string name, string department, string? email = null, string? position = null)
        {
            FullName = name;
            Department = department;
            Email = email;
            Position = position;
        }
    }
}
