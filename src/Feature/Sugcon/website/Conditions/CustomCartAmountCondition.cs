using Sitecore.Commerce.CustomModels.Models;
using Sitecore.Commerce.CustomModels.PageEvents;
using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Segmentation.Predicates;
using System.Linq;

namespace Feature.Sugcon.Conditions
{
    public class CustomCartAmountCondition : ICondition, IMappableRuleEntity
    {
        public decimal Number { get; set; }

        public NumericOperationType Comparison { get; set; }

        public virtual bool Evaluate(IRuleExecutionContext context)
        {
            Interaction interaction = context.Fact<Contact>(null).Interactions.Where(i => i.Events.OfType<LinesAddedToCartPageEvent>().Any()
            || i.Events.OfType<LinesRemovedFromCartPageEvent>().Any() 
            || i.Events.OfType<LinesUpdatedOnCartPageEvent>().Any()).OrderByDescending(i => i.LastModified).FirstOrDefault();
            if (interaction != null)
            {
                Event @event = interaction.Events.Where(e =>
                {
                    switch (e)
                    {
                        case LinesAddedToCartPageEvent _:
                        case LinesRemovedFromCartPageEvent _:
                            return true;
                        default:
                            return e is LinesUpdatedOnCartPageEvent;
                    }
                }).OrderByDescending(e => e.Timestamp).FirstOrDefault();
                if (@event != null)
                {
                    Cart cart = this.GetCart(@event);
                    return cart != null && cart.CartLines != null && cart.Total != null && this.Comparison.Evaluate(cart.Total.Amount, this.Number);
                }
            }
            return true;
        }

        protected virtual Cart GetCart(Event @event)
        {
            switch (@event)
            {
                case LinesAddedToCartPageEvent _:
                case LinesRemovedFromCartPageEvent _:
                case LinesUpdatedOnCartPageEvent _:
                    return ((CartLinesPageEvent)@event).Cart;
                default:
                    return null;
            }
        }
    }
}
