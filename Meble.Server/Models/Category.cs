﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meble.Server.Models;
public partial class Category
{
    [Key]
    public int CategoryId { get; set; }
    [Required]
    public string CategoryName { get; set; } = null!;

    public int CategoryFurnitureId { get; set; }

    public DateTime? CategoryDateOfUpdate { get; set; }
    [Required]
    public virtual Furniture CategoryFurniture { get; set; } = null!;
}
