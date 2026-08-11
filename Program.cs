var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

var contacts = new List<Contact>();
int nextId = 1;

app.MapGet("/contacts", () => contacts);

app.MapGet("/contacts/{id}", (int id) =>
    contacts.FirstOrDefault(c => c.Id == id) is { } contact
        ? Results.Ok(contact)
        : Results.NotFound());

app.MapPost("/contacts", (Contact contact) =>
{
    if (string.IsNullOrWhiteSpace(contact.Name))
        return Results.BadRequest("Name is required.");

    contact.Id = nextId++;
    contact.Name = contact.Name.Trim();
    contact.Phone = contact.Phone?.Trim() ?? "";
    contact.Email = contact.Email?.Trim() ?? "";
    contacts.Add(contact);
    return Results.Created($"/contacts/{contact.Id}", contact);
});

app.MapPut("/contacts/{id}", (int id, Contact updated) =>
{
    var contact = contacts.FirstOrDefault(c => c.Id == id);
    if (contact is null) return Results.NotFound();
    contact.Name = updated.Name;
    contact.Phone = updated.Phone;
    contact.Email = updated.Email;
    return Results.Ok(contact);
});

app.MapDelete("/contacts/{id}", (int id) =>
{
    var contact = contacts.FirstOrDefault(c => c.Id == id);
    if (contact is null) return Results.NotFound();
    contacts.Remove(contact);
    return Results.Ok();
});

app.Run();