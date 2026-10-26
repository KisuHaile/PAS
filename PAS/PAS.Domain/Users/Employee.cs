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

        public Employee(string code, string name, string department)
        {
            EmployeeCode = code;
            FullName = name;
            Department = department;
        }
    }
}
