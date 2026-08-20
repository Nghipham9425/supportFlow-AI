using Microsoft.EntityFrameworkCore;
using SupportFlow.Domain.Entities;
using SupportFlow.Domain.Enums;

namespace SupportFlow.Infrastructure.Persistence;

public static class DevelopmentDataSeeder
{
    public static async Task SeedKnowledgeArticlesAsync(
        AppDbContext db,
        CancellationToken cancellationToken = default)
    {
        var articles = new[]
        {
            new KnowledgeArticle
            {
                Title = "Password Reset and Account Recovery",
                Category = KnowledgeArticleCategory.Account,
                Content = "If a customer cannot log in, first ask them to verify the email address associated with the account. They should open the password reset page, request a new link, and check the spam, junk, promotions, and other filtered folders. The customer should wait a few minutes before requesting another link because repeated requests can invalidate earlier links.\n\nIf the customer no longer has access to the registered email address, do not change the account email without verification. Ask for the account identifier and escalate the request to an authorized support agent. Never ask the customer to share their current password or a password reset token."
            },
            new KnowledgeArticle
            {
                Title = "Email Verification Troubleshooting",
                Category = KnowledgeArticleCategory.Account,
                Content = "When a verification email does not arrive, confirm that the customer entered the correct email address and that the mailbox is not full. Ask them to search for messages from the product support domain and to check spam, junk, promotions, and quarantine folders. Company mail servers may delay or block automated messages.\n\nThe customer can request a new verification message after checking the address. If several attempts fail, collect the approximate request time, email domain, and any visible error message. Do not mark the account as verified manually unless the identity verification process has been completed. Escalate repeated delivery failures to the account support team."
            },
            new KnowledgeArticle
            {
                Title = "Two Factor Authentication Recovery",
                Category = KnowledgeArticleCategory.Account,
                Content = "If a customer loses access to their authenticator device, ask whether they still have a saved recovery code or an active trusted session. Recovery codes should be treated like passwords and must never be requested in a public ticket. If a trusted session exists, the customer may add a new authenticator device from security settings.\n\nWhen no recovery method is available, the account must go through identity verification before two factor authentication is reset. Record the verification case and escalate it to an authorized agent. Never disable two factor authentication only because the customer knows the email address or username."
            },
            new KnowledgeArticle
            {
                Title = "Payment Failed Troubleshooting",
                Category = KnowledgeArticleCategory.Billing,
                Content = "When a payment fails, ask the customer to verify the card number, expiration date, security code, billing address, and available balance. They should also confirm that their bank allows online and international transactions when applicable. The customer may retry once or use another supported payment method.\n\nDo not ask the customer to send a full card number, security code, or banking password in a support ticket. If the error continues, collect the payment attempt time, transaction reference, currency, and safe error message. Escalate the case to billing when the bank confirms the payment was authorized but the order remains unpaid."
            },
            new KnowledgeArticle
            {
                Title = "Refund Request Policy",
                Category = KnowledgeArticleCategory.Refund,
                Content = "Customers can request a refund within fourteen days of purchase unless a different product policy applies. Ask for the order number and the email address used for payment. Confirm the purchase status and check whether the order has already been refunded or charged back.\n\nEligible refunds are normally processed within five to seven business days after approval. The customer should contact their bank if the refund was marked complete but is not visible after the expected processing period. Never promise approval before checking the order and applicable policy."
            },
            new KnowledgeArticle
            {
                Title = "Duplicate Charge Investigation",
                Category = KnowledgeArticleCategory.Billing,
                Content = "A customer may see a duplicate charge because one payment is a temporary authorization and another is the completed transaction. Ask for the order number, charge dates, amounts, and the last four digits of the payment method only. Do not request a full card number or banking credentials.\n\nCheck whether both charges have settled. Temporary authorizations often disappear automatically within several business days. If both charges are settled, attach the payment references to the billing escalation and explain that the billing team will investigate. Avoid issuing a refund before confirming that the charges are genuine duplicates."
            },
            new KnowledgeArticle
            {
                Title = "Application Error and Troubleshooting Steps",
                Category = KnowledgeArticleCategory.Technical,
                Content = "When a customer reports an application error, collect the exact error message, the page or action that caused it, the approximate time, and the device and browser used. Ask the customer to refresh the page, sign out and back in, and retry in a private window when appropriate. They should not repeatedly submit a form if it could create duplicate orders or payments.\n\nIf the issue continues, request a screenshot with personal and payment information hidden. Check whether the incident affects multiple customers before escalating. Include reproduction steps and correlation or request identifiers when available. Do not ask the customer to install untrusted software or share their password."
            },
            new KnowledgeArticle
            {
                Title = "Order Delivery and Tracking",
                Category = KnowledgeArticleCategory.Product,
                Content = "For a delayed order, ask for the order number and verify the delivery address, shipping method, and latest tracking event. Tracking information may remain unchanged for a short period while the carrier moves a package between facilities. Provide the latest confirmed estimate and avoid promising an exact delivery time unless the carrier guarantees it.\n\nIf the package is marked delivered but cannot be found, ask the customer to check with household members, building reception, and a safe delivery location. The customer should contact support again if the package is still missing after the carrier investigation window. Escalate damaged, returned, or repeatedly delayed shipments with the tracking number and order details."
            },
            new KnowledgeArticle
            {
                Title = "Subscription Cancellation and Plan Changes",
                Category = KnowledgeArticleCategory.Product,
                Content = "Customers can manage an active subscription from account settings when self-service cancellation is available. Before cancelling, explain the effective date, remaining access, and whether the current billing period is refundable under the applicable policy. A cancellation normally prevents the next renewal but does not automatically refund a completed charge.\n\nFor plan changes, confirm the target plan and explain any prorated charge or credit shown during checkout. Do not change a customer plan from a support ticket without verifying the account and receiving clear confirmation. Escalate billing discrepancies with the account email, plan name, and invoice reference."
            }
        };

        var existingTitles = await db.KnowledgeArticles
            .Select(article => article.Title)
            .ToListAsync(cancellationToken);

        var newArticles = articles
            .Where(article => !existingTitles.Contains(article.Title))
            .ToList();

        if (newArticles.Count == 0)
        {
            return;
        }

        db.KnowledgeArticles.AddRange(newArticles);
        await db.SaveChangesAsync(cancellationToken);
    }
}
