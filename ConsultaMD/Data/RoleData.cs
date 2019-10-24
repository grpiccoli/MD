using System.Collections.Generic;

namespace ConsultaMD.Data
{
    public static class RoleData
    {
        public static List<string> ApplicationRoles { get; } = new List<string>
                                                            {
                                                                "Administrator",
                                                                "Editor",
                                                                "Guest",
                                                                "User",
                                                                "Doctor",
                                                                "Centre",
                                                                "Secretary"
                                                            };
    }
}
