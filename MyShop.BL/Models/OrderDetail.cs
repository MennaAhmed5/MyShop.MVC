﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Entities.Models
{
	public class OrderDetail
	{
		public int Id { get; set; }	

		public int OrderId { get; set; }

		[ValidateNever]
		public OrderHeader OrderHeader { get; set; }	

		public int ProductId { get; set; }

		public Product Product { get; set; }

		public int Count { get; set; }

		public decimal Price { get; set; }
	}
}
