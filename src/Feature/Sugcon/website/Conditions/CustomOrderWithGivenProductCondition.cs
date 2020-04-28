using Sitecore.XConnect;
using Sitecore.XConnect.Segmentation.Predicates;
using System;
using System.Linq;
using System.Linq.Expressions;
using Sitecore.Commerce.Providers;
using Sitecore.Framework.Rules;
using System.Web.Mvc;
using Sitecore.Commerce.CustomModels.Facets;

namespace Feature.Sugcon.Conditions
{
    public class CustomOrderWithGivenProductCondition : ICondition, IMappableRuleEntity, IContactSearchQueryFactory
    {
        private readonly IContactCommerceInteractionProvider _provider;
        public CustomOrderWithGivenProductCondition()
        {
            _provider = DependencyResolver.Current.GetService<IContactCommerceInteractionProvider>();
        }

        public string ProductID { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            var contactOrders = _provider.GetContactOrders(context.Fact<Contact>(null));

            return contactOrders != null
                && contactOrders.Any(order => order.CartLines.Any(line => line.Product.ProductId == this.ProductID));
        }

        public Expression<Func<Contact, bool>> CreateContactSearchQuery(
          IContactSearchQueryContext context)
        {
            return contact => contact.GetFacet<CommerceInteractionsCache>("CommerceInteractionsCache").Orders.Any(order => order.CartLines.Any(line => line.Product.ProductId == this.ProductID));
        }
    }
}
