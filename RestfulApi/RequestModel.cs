using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace RestfulApi
{
    public class RequestModel
    {
        [Required]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
