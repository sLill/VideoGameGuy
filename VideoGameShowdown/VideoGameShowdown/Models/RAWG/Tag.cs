﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class Tag
    {
        #region Properties..
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Id")]
        [Key]
        public string TagId { get; set; }

        [Column(TypeName = "NVARCHAR(255)")]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR(255)")]
        public string? Slug { get; set; }

        [Column(TypeName = "NVARCHAR(255)")]
        public string? Language { get; set; }

        public int? Games_Count { get; set; }

        [Column(TypeName = "NVARCHAR(2048)")]
        public string? Image_Background { get; set; }
        #endregion Properties..
    }
}

