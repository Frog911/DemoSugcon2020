using Feature.Sugcon.Models;
using Sitecore.Commerce.XA.Foundation.Catalog.Providers;
using Sitecore.Data;
using Sitecore.Framework.Conditions;
using System.Collections.Generic;

namespace Feature.Sugcon.Pipelines
{
    public class FromOrder : Sitecore.Commerce.XA.Foundation.Catalog.Pipelines.RecommendedProducts.FromOrder
    {
        public FromOrder(IRecommendedProductsProvider recommendedProductsProvider) : base(recommendedProductsProvider)
        {
        }

        public virtual void Process(CustomGetRecommendedProductsArgs args)
        {
            Condition.Requires(args, nameof(args)).IsNotNull();
            string orderId = this.GetOrderId();
            if (args.IsRecommendedProductsReady || string.IsNullOrEmpty(orderId))
            {
                return;
            }

            IEnumerable<ID> recommendedProductsIds = this.RecommendedProductsProvider.GetRecommendedProductsIds(this.GetProviderOptions(args));
            args.Success = true;
            args.RecommendedProductsIdList = recommendedProductsIds;
        }

        protected virtual BaseRecommendedProductsProviderOptions GetProviderOptions(
          CustomGetRecommendedProductsArgs args)
        {
            return new CustomOrderBasedRecommendedProductOptions
            {
                ProviderMainKey = this.GetOrderId(),
                MaxNumberOfProducts = args.MaxNumberOfRecommendedProducts,
                ProductRelationshipName = args.RelationshipFieldName,
                Tag = args.Tag
            };
        }
    }
}
