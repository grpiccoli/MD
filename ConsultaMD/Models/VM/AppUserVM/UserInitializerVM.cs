using System.Collections.Generic;

namespace ConsultaMD.Models.VM
{
    public class UserInitializerVM
    {
        public string Name { get; set; }

        public string Role { get; set; }

        public string Claim { get; set; }

        public string Email { get; set; }

        public string Key { get; set; }

        public string Image { get; set; }

        public int? Rating { get; set; }
        public int RUN { get; set; }
        public int Carnet { get; set; }
    }
}
