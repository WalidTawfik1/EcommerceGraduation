using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Entities
{
    public enum Status
    {
        Pending,
        PaymentReceived,
        PaymentFailed,
        Shipped,
        Delivered,
        Cancelled,
        Success,
        Failed,
        Refunded,
        Paid
    }
}
