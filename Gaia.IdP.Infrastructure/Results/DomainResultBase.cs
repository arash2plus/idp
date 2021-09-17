namespace Gaia.IdP.Infrastructure.Results
{
    public abstract class DomainResultBase
    {
        public int Status { get; protected set;}
        public string TraceId { get; protected set; }
    }
}
