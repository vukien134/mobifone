using Accounting.BaseDtos;


namespace Accounting.Catgories.AdjustDepreciations
{
    public class AdjustDepreciationDetailDto : CruOrgBaseDto
    {
        public string AdjustDepreciationId { get; set; }
        public string AssetToolDetailId { get; set; }
        public decimal? Amount { get; set; }
    }
}
