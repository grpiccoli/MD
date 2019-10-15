namespace ConsultaMD.Services
{
    public interface IHasBasicIndexer
    {
        object this[string propertyName] { get;set; }
    }
}
