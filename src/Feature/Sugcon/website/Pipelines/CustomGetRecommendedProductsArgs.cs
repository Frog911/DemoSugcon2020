using Sitecore.Commerce.XA.Foundation.Catalog.Pipelines.RecommendedProducts;

namespace Feature.Sugcon.Pipelines
{
    public class CustomGetRecommendedProductsArgs : GetRecommendedProductsArgs
    {
        public string Tag { get; set; }
    }
}
