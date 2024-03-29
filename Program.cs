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
        DateCompleted = new DateTime(2023, 12, 1),
    },
        new HoneyRaesAPI.Models.ServiceTicket()
    {
        Id = 2,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "The Saleoomi needed a Cauldron",
        Emergency = true,
        DateCompleted = new DateTime(2021, 1, 1),
    },
    new HoneyRaesAPI.Models.ServiceTicket()
    {
        Id = 3,
        CustomerId = 3,
        EmployeeId = null,
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

app.MapGet("/api/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/api/servicetickets/{id}", (int id) =>
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

// EXTRA - SORTING BY EMERGENCIES
app.MapGet("/servicetickets/emergencies", () =>
{
    List<HoneyRaesAPI.Models.ServiceTicket> emergencyTicketsNotDone = serviceTickets.Where(st => st.Emergency == true && st.DateCompleted == null).ToList();
    if (emergencyTicketsNotDone.Count == 0)
    {
        return Results.NotFound();
    }
    return Results.Ok(emergencyTicketsNotDone);
});

// EXTRA - SORTING BY UNASSIGNED
app.MapGet("/servicetickets/unassigned", () =>
{
    List<HoneyRaesAPI.Models.ServiceTicket> ticketsUnassigned = serviceTickets.Where(st => st.EmployeeId == null).ToList();
    if (ticketsUnassigned.Count == 0)
    {
        return Results.NotFound();
    }
    return Results.Ok(ticketsUnassigned);
});

// EXTRA - SORTING BY INACTIVE CUSTOMERS
app.MapGet("/customers/inactive", () =>
{
    List<HoneyRaesAPI.Models.ServiceTicket> completedTicktetsOlderThanAYear = new List<ServiceTicket>();
    List<HoneyRaesAPI.Models.Customer> inactiveCustomers = new List<Customer>();
    List<HoneyRaesAPI.Models.ServiceTicket> completedTickets = serviceTickets.Where(st => st.DateCompleted != null).ToList();
    for (int i = 0; i < completedTickets.Count; i++)
    {
        TimeSpan? ticketAge = DateTime.Now - completedTickets[i].DateCompleted;
        if (ticketAge?.Days >= 365)
        {
            completedTicktetsOlderThanAYear.Add(completedTickets[i]); 
        }

    }

// A loop within a loop - for each customer item, compare and see whether it is equal to the completedTicketsOlderThanAYear.
    for (int i = 0; i < customers.Count; i++)
    {
        for (int j = 0; j < completedTicktetsOlderThanAYear.Count; j++)
        {
            if (completedTicktetsOlderThanAYear[j].CustomerId == customers[i].Id)
            {
                inactiveCustomers.Add(customers[i]);
            }
        }
    
    }

    return Results.Ok(inactiveCustomers);
});

// EXTRA - AVAILABLE EMPLOYEES
// OUTPUT - A LIST OF THE EMPLOYEE DATA TYPE
// start at service tickets, filter for incomplete ones
// for each employee item, compare to service ticket customerid - if equal, remove from shallow copy of employee 

app.MapGet("/employees/available", () =>
{
    List<HoneyRaesAPI.Models.Employee> availableEmployees = employees;

    for (int i = 0; i < availableEmployees.Count; i++)
    {
        for (int j = 0; j < serviceTickets.Count; j++)
        {
            if (availableEmployees[i].Id == serviceTickets[j].EmployeeId && serviceTickets[j].DateCompleted != null)
            {
                availableEmployees.Remove(availableEmployees[i]);
            }
        }
        
    }

    return Results.Ok(availableEmployees);

});

//EXTRA - EMPLOYEE'S CUSTOMERS
// OUTPUT - LIST OF THE CUSTOMER ID TYPE
// FOR THE GIVEN EMPLOYEE ID, RETURN ALL THE CUSTOMERS FOR WHOM THEY'VE BEEN ASSIGNED TO A SERVICE TICKET
app.MapGet("/employees/completed/{id}", (int id) => 
{
    List<HoneyRaesAPI.Models.Customer> assignedCustomers = new List<Customer>();
    List<HoneyRaesAPI.Models.ServiceTicket> assignedServiceTickets = new List<ServiceTicket>();
    for (int i = 0; i < serviceTickets.Count; i++)
    {
        if (serviceTickets[i].EmployeeId == id)
        {
            assignedServiceTickets.Add(serviceTickets[i]);
        }
    
    }

    for (int i = 0; i < customers.Count; i++)
    {
        for (int j = 0; j < assignedServiceTickets.Count; j++)
        {
            if (assignedServiceTickets[j].CustomerId == customers[i].Id)
            {
                assignedCustomers.Add(customers[i]);
            }
        }

    }

    if (assignedCustomers.Count == 0)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(assignedCustomers);
    }

});

//EXTRA - EMPLOYEE OF THE MONTH
// OUTPUT - SINGULAR ITEM WITH DATA TYPE 'EMPLOYEE' - MEETS CRITERIA OF HAVING THE MOST SERVICE TICKETS
// SERVICE TICKETS, FILTER OUT BASED ON WHICH ONES WERE LAST MONTH
// FOR EACH EMPLOYEE, FIND TOTAL OF SERVICE TICKETS
// PUSH TO LIST WHICH DENOTES TICKET TOTALS AND AN EMPLOYEE ID
// ITERATE THROUGH EMPLOYEES LIST TO MATCH EMPLOYEE ID FROM LIST ABOVE. RETURN EMPLOYEE. 

app.MapGet("/employees/employeeofthemonth", () => 
{
    List<ServiceTicket> lastMonSerTicks = serviceTickets
    .Where(st => st.DateCompleted != null && st.DateCompleted.Value.Month == DateTime.Now.AddMonths(-1).Month).ToList();
    
    var employeeOfTheMonth = employees
    .OrderByDescending(e => lastMonSerTicks.Count(st => st.EmployeeId == e.Id)).FirstOrDefault();

    return Results.Ok(employeeOfTheMonth);

});

//EXTRA - PAST TICKET REVIEW
//output - List with data type 'Tickets'
// Ordered list with OLDEST first
app.MapGet("/servicetickets/pastreview", () =>
{
    List<ServiceTicket> orderedTickets = serviceTickets
    .Where(st => st.DateCompleted != null).ToList()
    .OrderByDescending(st => st.DateCompleted).ToArray()
    .Reverse().ToList();

    return Results.Ok(orderedTickets);

});



app.Run();