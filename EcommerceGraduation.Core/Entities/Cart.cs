using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGraduation.Core.Entities;

public class Cart
{
    public Cart()
    {

    }
    public Cart(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
    public string PaymentToken { get; set; }
    public string PaymentIntentId { get; set; }
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal SubAmount => Items.Sum(item => item.TotalPrice);

}
