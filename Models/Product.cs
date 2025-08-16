using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TypicalTechTools.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product price is required")]
        [Range(0.01, 100000, ErrorMessage = "Enter a valid price between 0.01 and 100000")]
        public double ProductPrice { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string ProductDescription { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedDate { get; set; }  // Дата последнего обновления продукта

        // this is the '1' relationship identifier for the ORM

        [ValidateNever] // ✅ Отключаем валидацию для навигационного свойства
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
