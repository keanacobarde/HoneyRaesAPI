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
        Emergency = false,
        DateCompleted = DateTime.Now,
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

app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
