using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.DataAccess.Enums
{
    public class OrderAtt : Attribute
    {
        public readonly int Order;

        public OrderAtt(int order)
        {
            Order = order;
        }
    }
    public class EnumCollection
    {
        public enum LayerType
        {
            B1Layer = 1,
            B2Layer = 2,
            B3Layer = 3
        }
    }
 
}
