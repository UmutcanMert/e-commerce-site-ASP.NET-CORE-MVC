using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopapp.entity
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LasrtName { get; set; }

        public string Adress { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Note { get; set; }

        public EnumOrderState OrderState { get; set; }
    
        public List<OrderItem> OrderItems { get; set; }
    }

    public enum EnumOrderState
    {
        waiting =0,
        unpaid =1,
        completed=2
    }
}