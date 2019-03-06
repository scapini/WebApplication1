using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Model
{
    public class Albums
    {
        [Key]
        public int CollectionId { get; set; }
        public string ArtistName { get; set; }
        public string CollectionName { get; set; }
        public int ArtistId { get; set; }
    }
}
