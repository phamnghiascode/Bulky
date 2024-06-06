using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Tiêu đề")]
        public string? Title { get; set; }
        [DisplayName("Mô tả")]
        public string? Description { get; set; }
        [Required]
        public string? ISBN { get; set; }
        [Required]
        [DisplayName("Tác giả")]
        public string? Author { get; set; }
        [Required]
        [DisplayName("Giá niêm yết")]
        [Range(1, 1000)]
        public double ListPrice { get; set; }

        [Required]
        [DisplayName("Giá 1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }

        [Required]
        [DisplayName("Giá 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [Required]
        [DisplayName("Giá 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }
        [DisplayName("Danh mục")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        [DisplayName("Ảnh")]
        public string? ImageUrl { get; set; }
    }
}
