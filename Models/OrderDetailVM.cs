using System.Collections;
using System.Collections.Generic;

namespace HamBurger.Models
{
    public class OrderDetailVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable <OrderDetail> OrderDetail { get; set; }
    }
}
