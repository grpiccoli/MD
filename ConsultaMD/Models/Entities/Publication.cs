namespace ConsultaMD.Models.Entities
{
    public class Publication
    {
        public int Id { get; set; }
        public string AuthorizedBy { get; set; }
        public bool Authorized { get; set; }
        public virtual Doctor Doctor { get; set; }
        public string Name { get; set; }
    }
}
