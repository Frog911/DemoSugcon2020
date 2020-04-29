using Feature.Sugcon.Pipelines;
using Sitecore;
using Sitecore.Commerce.XA.Feature.Emails.Repositories;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Presentation;
using Sitecore.Pipelines;
using System.Collections.Generic;

namespace Feature.Sugcon.Repositories
{
    public class CustomEmailRecommendedProductsRepository : EmailRecommendedProductsRepository
    {
        public CustomEmailRecommendedProductsRepository(
            IModelProvider modelProvider,
            IStorefrontContext storefrontContext,
            ICatalogManager catalogManager,
            IInventoryManager inventoryManager,
            IVisitorContext visitorContext)
            : base(modelProvider, storefrontContext, catalogManager, inventoryManager, visitorContext)
        {
        }

        protected override IEnumerable<ID> GetProductsItemIdList()
        {
            CustomGetRecommendedProductsArgs productsPipelineArgs = this.GetRecommendedProductsPipelineArgs();
            CorePipeline.Run("commerce.getRecommendedProducts", productsPipelineArgs);
            return productsPipelineArgs.RecommendedProductsIdList;
        }

        protected virtual CustomGetRecommendedProductsArgs GetRecommendedProductsPipelineArgs(
          CustomGetRecommendedProductsArgs args = null)
        {
            CustomGetRecommendedProductsArgs recommendedProductsArgs = args ?? new CustomGetRecommendedProductsArgs();
            Rendering rendering = RenderingContext.CurrentOrNull.ValueOrDefault(context => context.Rendering);
            recommendedProductsArgs.ProductsQuery = rendering?.Item.Fields["Products query"]?.Value;
            recommendedProductsArgs.MaxNumberOfRecommendedProducts = MainUtil.GetInt(rendering?.Item.Fields["Max number of recommended products"]?.Value, 4);
            recommendedProductsArgs.RelationshipFieldName = rendering?.Item.Fields["Related products fields name"]?.Value;
            recommendedProductsArgs.Tag = rendering.Parameters["Tag"];
            return recommendedProductsArgs;
        }
    }
}
