namespace ConsultaMD.Models.Entities
{
    public class DigitalSignature
    {
        public int Id { get; set; }
        public int NaturalId { get; set; }
        public Natural Natural { get; set; }
        public string PathToKey { get; set; }
    }
}
