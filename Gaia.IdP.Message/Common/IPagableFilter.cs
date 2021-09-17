namespace Gaia.IdP.Message.Common
{
    public interface IPagableFilter : IFilter
    {
        int? Skip { get; set; }
        int? Limit { get; set; }
        string Sort { get; set; }
    }
}
