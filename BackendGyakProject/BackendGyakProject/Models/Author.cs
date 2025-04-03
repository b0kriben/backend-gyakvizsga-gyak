using System;
using System.Collections.Generic;

namespace BackendGyakProject.Models;

public partial class Author
{
    public int? Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public DateOnly? BirthDate { get; set; }
}
