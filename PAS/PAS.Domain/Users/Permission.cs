using System;
using Domain.Common;

namespace Domain.Users
{
    public class Permission : BaseEntity
    {
        public string PermissionName { get; private set; }

        public string Description { get; private set; }

        public Guid? RoleId { get; private set; }

        private Permission()
        {
        }

        public Permission(string name, string description, Guid? roleId = null)
        {
            PermissionName = name;
            Description = description;
            RoleId = roleId;
        }
    }
}
