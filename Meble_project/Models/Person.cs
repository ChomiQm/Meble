using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class Person
{
    public int PersonId { get; set; }

    public string PersonFirstName { get; set; } = null!;

    public string PersonSurname { get; set; } = null!;

    public string PersonAddress { get; set; } = null!;

    public string PersonPhone { get; set; } = null!;

    public string PersonMail { get; set; } = null!;

    public DateTime? PersonDateOfUpdate { get; set; }

    public virtual ICollection<ClientOrder> ClientOrders { get; } = new List<ClientOrder>();

    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();

    public virtual ICollection<Supplier> Suppliers { get; } = new List<Supplier>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}
