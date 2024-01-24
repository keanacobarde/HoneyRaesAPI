using HoneyRaesAPI.Models;

List<HoneyRaesAPI.Models.Customer> customers = new List<HoneyRaesAPI.Models.Customer>()
{ 
    new HoneyRaesAPI.Models.Customer()
    { 
        Id = 1,
        Name = "Salem",
        Address = "123 Witch Lane"
    },
    new HoneyRaesAPI.Models.Customer()
    {
        Id = 2,
        Name = "Salemoomi",
        Address = "124 Witch Lane"
    },
    new HoneyRaesAPI.Models.Customer()
    {
        Id = 3,
        Name = "Salogna",
        Address = "125 Witch Lane"
    },

};
List<HoneyRaesAPI.Models.Employee> employees = new List<HoneyRaesAPI.Models.Employee>() 
{ 
    new HoneyRaesAPI.Models.Employee()
    { 
      Id = 1,
      Name = "Keana",
      Specialty = "Dealing with Salems"
    },
    new HoneyRaesAPI.Models.Employee()
    {
      Id = 2,
      Name = "Kiki",
      Specialty = "Dealing with Saleoomis"
    },
    new HoneyRaesAPI.Models.Employee()
    {
      Id = 3,
      Name = "Keana",
      Specialty = "Dealing with Salognas"
    },
};

List<HoneyRaesAPI.Models.ServiceTicket> serviceTickets = new List<HoneyRaesAPI.Models.ServiceTicket>()
{ 
    new HoneyRaesAPI.Models.ServiceTicket()
    { 
        Id = 1,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "The Salem needed a Cauldron",
        Emergency = false,
        DateCompleted = DateTime.Now,
    },
        new HoneyRaesAPI.Models.ServiceTicket()
    {
        Id = 2,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "The Saleoomi needed a Cauldron",
        Emergency = true,
        DateCompleted = DateTime.Now,
    },
    new HoneyRaesAPI.Models.ServiceTicket()
    {
        Id = 3,
        CustomerId = 3,
        EmployeeId = 3,
        Description = "The Salogna needed a Cauldron",
        Emergency = true,
        DateCompleted = null,
    },

};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/employees", () =>
{
    return employees;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    return Results.Ok(serviceTicket);
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (When we get to it later, our SQL database will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    };
    serviceTickets.Remove(serviceTicket);
    return Results.Ok();
});

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket updatedServiceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    ticketToUpdate.EmployeeId = updatedServiceTicket.EmployeeId;
    return Results.Ok();
});

app.MapPut("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    ticketToUpdate.DateCompleted = DateTime.Now;
    return Results.Ok();
});

app.MapGet("/servicetickets/emergencies", () =>
{
    List<HoneyRaesAPI.Models.ServiceTicket> emergencyTicketsNotDone = serviceTickets.Where(st => st.Emergency == true && st.DateCompleted == null).ToList();
    if (emergencyTicketsNotDone.Count == 0)
    {
        return Results.NotFound();
    }
    return Results.Ok(emergencyTicketsNotDone);
});

app.Run();