using Feature.Sugcon.Models;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.XA.Foundation.Catalog.Providers;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Framework.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Feature.Sugcon.Providers
{
    public class CustomOrderBasedRecommendedProductsProvider : OrderBasedRecommendedProductsProvider
    {
        private const string _tagsFieldName = "Tags";
        public CustomOrderBasedRecommendedProductsProvider(
            IStorefrontContext storefrontContext,
            IOrderManager orderManager,
            IItemTypeProvider itemTypeProvider)
            : base(storefrontContext, orderManager, itemTypeProvider)
        {
        }

        public override IEnumerable<ID> GetRecommendedProductsIds(
      BaseRecommendedProductsProviderOptions baseRecommendedProductsProviderOptions)
        {
            Condition.Requires(baseRecommendedProductsProviderOptions).IsOfType(typeof(CustomOrderBasedRecommendedProductOptions));
            var ids = Enumerable.Empty<ID>();
            var recommendedProductOptions = baseRecommendedProductsProviderOptions as CustomOrderBasedRecommendedProductOptions;
            var order = this.GetOrder(recommendedProductOptions.ProviderMainKey);
            return order != null 
                ? this.CalculateCustomRecommendedProducts(order, recommendedProductOptions.MaxNumberOfProducts, recommendedProductOptions.ProductRelationshipName, recommendedProductOptions.Tag)
                : ids;
        }

        public IEnumerable<ID> CalculateCustomRecommendedProducts(
      CommerceOrder order,
      int maxNumberOfProducts,
      string productRelationshipName,
      string tag = "")
        {
            Condition.Requires(productRelationshipName).IsNotNullOrEmpty();
            List<ID> idList = new List<ID>();
            if (order.Lines.Count > 0)
            {
                var relatedProductList = string.IsNullOrEmpty(tag)
                    ? this.GetRelatedProductList(order, productRelationshipName)
                    : this.GetCustomRelatedProductList(order, productRelationshipName, tag);

                int num1 = relatedProductList.Select(x => x).Sum(y => y.Count());
                int num2 = maxNumberOfProducts < num1 ? maxNumberOfProducts : num1;
                while (idList.Count < num2)
                {
                    foreach (Queue<string> stringQueue in relatedProductList)
                    {
                        if (idList.Count != maxNumberOfProducts)
                        {
                            if (stringQueue.Count > 0)
                            {
                                ID id = new ID(stringQueue.Dequeue());
                                if (!idList.Contains(id))
                                    idList.Add(id);
                            }
                        }
                        else
                            break;
                    }
                }
            }
            return idList;
        }

        protected IEnumerable<Queue<string>> GetCustomRelatedProductList(
            CommerceOrder order,
            string productRelationshipName,
            string tag)
        {
            IEnumerable<Guid> guids = order.Lines.Select(x => x.Product.SitecoreProductItemId);
            List<Queue<string>> stringQueueList = new List<Queue<string>>();
            foreach (Guid guid in guids)
            {
                var parent = this.StorefrontContext.Context.Database.GetItem(guid.ToString());
                if (parent != null && this.ItemTypeProvider.GetItemType(parent) == Sitecore.Commerce.XA.Foundation.Common.Constants.ItemTypes.Variant)
                {
                    parent = parent.Parent;
                }

                var field = (MultilistField)parent?.Fields[productRelationshipName];

                foreach (var id in field.TargetIDs)
                {
                    var rp = this.StorefrontContext.Context.Database.GetItem(id.ToString());
                    if (rp?.Fields[_tagsFieldName]?.Value?.Split('|').Contains(tag) == true)
                    {
                        stringQueueList.Add(new Queue<string>(new List<string>() { id.ToString()}));
                    }
                }
            }
            return stringQueueList;
        }
    }
}
