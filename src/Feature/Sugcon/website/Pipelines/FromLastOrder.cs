using Sitecore.Commerce.Providers;
using Sitecore.Commerce.XA.Foundation.Catalog.Providers;
using Sitecore.Framework.Conditions;

namespace Feature.Sugcon.Pipelines
{
    public class FromLastOrder : FromOrder
    {
        public FromLastOrder(
          IRecommendedProductsProvider recommendedProductsProvider,
          IContactCommerceInteractionProvider contactCommerceInteractionProvider)
          : base(recommendedProductsProvider)
        {
            Condition.Requires(recommendedProductsProvider, nameof(recommendedProductsProvider)).IsNotNull();
            Condition.Requires(contactCommerceInteractionProvider, nameof(contactCommerceInteractionProvider)).IsNotNull();
            this.ContactCommerceInteractionProvider = contactCommerceInteractionProvider;
        }

        protected IContactCommerceInteractionProvider ContactCommerceInteractionProvider { get; }

        protected override string GetOrderId()
        {
            return this.ContactCommerceInteractionProvider.GetContactLastOrderId();
        }
    }
}
