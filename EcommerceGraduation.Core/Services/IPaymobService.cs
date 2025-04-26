using EcommerceGraduation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Paymob.CashIn.Models.Callback;

namespace EcommerceGraduation.Core.Services
{
    public interface IPaymobService   
    {
       Task<Cart> CreateOrUpdatePaymentAsync(string cartId);
       Task<Order> ProcessPaymentForOrderAsync(string orderNumber);
       string GetPaymentIframeUrl(string paymentToken);
       Task<Order> UpdateOrderSuccess(string paymentIntentId);
       Task<Order> UpdateOrderFailed(string paymentIntentId);
       Task<Order> ProcessTransactionCallback(CashInCallbackTransaction callback);
       string ComputeHmacSHA512(string data, string secret);


    }

}

