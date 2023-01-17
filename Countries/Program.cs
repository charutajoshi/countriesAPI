using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.Map("/country/{countryID:int:range(1,100)}", async context =>
{
    string response = "";

    if (context.Request.RouteValues.ContainsKey("countryID"))
    {
        int id = Convert.ToInt32(context.Request.RouteValues["countryID"]); 
        var countryName = GetCountryHandler(id);

        if (countryName == "Not found")
        {
            if (context.Response.StatusCode == 200)
            {
                context.Response.StatusCode = 404; 
            }
            response = $"Could not find country name for ID {id}"; 
        }
        else if (countryName == "error")
        {
            if (context.Response.StatusCode == 200)
            {
                context.Response.StatusCode = 400;
            }
            response = $"Error while getting country name for ID {id}";
        }
        else
        {
            context.Response.StatusCode = 200; 
            response = $"Country name is {countryName} for ID {id}";
        }
    }
    else
    {
        response = $"Country ID cannot be null."; 
    }

    await context.Response.WriteAsync($"{response}\n");
});

app.Map("/countries", async context =>
{
    var countries = GetCountriesHandler();
    int statusCode = countries != null ? 200 : 400;
    string response = statusCode == 200 ? countries : "Error occurred when attempting to get countries"; 

    context.Response.StatusCode = statusCode; 
    await context.Response.WriteAsync($"{response}");
});

//app.Run(async context =>
//{
//    await context.Response.WriteAsync($"{context.Request.Path} does not exist.");
//});

app.Run();

static string GetCountriesHandler()
{
    string response = "";
    var countries = GetCountries();

    if(countries != null)
    {
        foreach (var country in countries)
        {
            response += country.Key + ". " + country.Value + "\n";
        }
    }
    else
    {
        response = $"Could not get countries";
    }

    return response; 
}

static string GetCountryHandler(int id)
{
    return GetCountry(id); 
}

static Dictionary<string, StringValues> GetCountries()
{
    Dictionary<string, StringValues> countriesDict = new Dictionary<string, StringValues>();
    countriesDict.Add("1","United States"); 
    countriesDict.Add("2","Canada"); 
    countriesDict.Add("3","United Kingdom"); 
    countriesDict.Add("4", "India"); 
    countriesDict.Add("5", "Japan");
    return countriesDict; 
}

static string GetCountry(int id)
{
    var countries = GetCountries();

    if(countries.ContainsKey(id.ToString()))
    {
        foreach(var country in countries)
        {
            if(country.Key.ToString() == id.ToString())
            {
                return country.Value.ToString();
            }
        }
    }

    return "Not found"; 
}

