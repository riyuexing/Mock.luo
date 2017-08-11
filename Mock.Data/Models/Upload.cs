﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mock.Data.Models
{
    [Table("Upload")]
    public class Upload
    {
        public Upload()
        {
            this.UploadEntry = new HashSet<UploadEntry>();
        }

        [Required,Key]
        public int Id { get; set; }
        public DateTime? AddTime { get; set; }
        [StringLength(50)]
        public string UserName { get; set; }
        public int? SortCode { get; set; }

        public virtual ICollection<UploadEntry> UploadEntry { get; set; }
    }
}
