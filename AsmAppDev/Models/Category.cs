using AsmAppDev.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsmAppDev.Models
{
	public class Category 
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime DateCreate { get; set; }
		public bool Availability { get; set; }

		[ValidateNever]
		public string UserId { get; set; }
		[ForeignKey(nameof(UserId))]

		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }
        public bool NotificationStatus { get; set; }
    }
}
