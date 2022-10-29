using AwesomeCompany;
using AwesomeCompany.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(
    t=>t.UseSqlServer(builder.Configuration.GetConnectionString("SqlDbCon")));

var app = builder.Build();

app.UseHttpsRedirection();

//app.MapPut("increase-salary", async (int companyid, DatabaseContext dbContext) =>
//{
//    var company = await dbContext
//        .Set<Company>()
//        .Include(c => c.Employees)
//        .FirstOrDefaultAsync(c => c.Id == companyid);

//    if(company is null)
//    {
//        return Results.NotFound($"The company with Id '{companyid}' does not exists");
//    }

//    company.Employees.ForEach(x => x.Salary += 20);
//    company.LastSalaryUpdatedUtc = DateTime.UtcNow;
//    await dbContext.SaveChangesAsync();

//    return Results.NoContent();
//});

app.MapPut("increase-salary", async (int companyid, DatabaseContext dbContext) =>
{
    var company = await dbContext
        .Set<Company>()
        .FirstOrDefaultAsync(c => c.Id == companyid);

    if (company is null)
    {
        return Results.NotFound($"The company with Id '{companyid}' does not exists");
    }

    var transaction = await dbContext.Database.BeginTransactionAsync();
    //Without Dapper
    //await dbContext.Database.ExecuteSqlInterpolatedAsync(
    //    $"UPDATE Employees SET Salary = Salary+130 WHERE Companyid = {company.Id} ");

    //With Dapper

    await dbContext.Database.GetDbConnection().ExecuteAsync(
        "UPDATE Employees SET Salary = Salary+130 WHERE Companyid = @Companyid ", new {companyid = company.Id},
        transaction.GetDbTransaction()
        );

    //company.Employees.ForEach(x => x.Salary += 20);
    company.LastSalaryUpdatedUtc = DateTime.UtcNow;
    await dbContext.SaveChangesAsync();

    await dbContext.Database.CommitTransactionAsync();

    return Results.NoContent();
});

app.Run();


// Add services to the container.
//builder.Services.AddRazorPages();


// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapRazorPages();

