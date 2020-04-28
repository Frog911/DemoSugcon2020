using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data;
using Sitecore.Framework.Conditions;
using System.Collections.Generic;
using System.Linq;


namespace Feature.Sugcon.Pipelines
{
    public class FromQuery : Sitecore.Commerce.XA.Foundation.Catalog.Pipelines.RecommendedProducts.FromQuery
    {
        public FromQuery(ISearchManager searchManager, IItemTypeProvider itemTypeProvider) : base(searchManager, itemTypeProvider)
        {
        }

        public virtual void Process(CustomGetRecommendedProductsArgs args)
        {
            Condition.Requires(args, nameof(args)).IsNotNull();
            if (args.IsRecommendedProductsReady || string.IsNullOrEmpty(args.ProductsQuery))
                return;
            IEnumerable<ID> idListByQuery = this.GetIdListByQuery(args);
            args.Success = true;
            args.RecommendedProductsIdList = idListByQuery;
        }

        protected virtual IEnumerable<ID> GetIdListByQuery(CustomGetRecommendedProductsArgs args)
        {
            CommerceSearchOptions searchOptions = new CommerceSearchOptions(args.MaxNumberOfRecommendedProducts, 0);
            return this.SearchManager.SearchCatalogItemsByQuery(args.ProductsQuery, string.Empty, searchOptions).SearchResultItems.Select(i => i.ID);
        }
    }
}
