using System.Collections.Generic;

namespace ConsultaMD.Data
{
    public class RoleData
    {
        public static List<string> ApplicationRoles { get; set; } = new List<string>
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
