using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazero_Temp.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [Range(1, 100, ErrorMessage = "Display order must be 1-100")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
