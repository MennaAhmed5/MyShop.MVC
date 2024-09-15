using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Implementations
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

		public void Update(OrderHeader orderHeader)
		{
			var orderFromDB = _context.OrderHeaders.Update(orderHeader);
		}

		public void UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus)
		{
			var orderFromDb = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if(orderFromDb != null)
			{
				orderFromDb.OrderStatus = OrderStatus;
				if(PaymentStatus != null)
				{
					orderFromDb.PaymentStatus = PaymentStatus;	
				}
			}
		}
	}
}
