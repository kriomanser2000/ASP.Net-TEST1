using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.AspNetCore.Identity;
namespace WebApplicationTest;
internal class Car
{
    public int Id { set; get; }
    public float MaxSpeed { set; get; }
    public string Model { set; get; }
    public string Description { set; get; }
    public Car()
    {
        Id = 0;
        MaxSpeed = 0.0F;
        Model = string.Empty;
        Description = string.Empty;
    }
}
public class Program
{
    public static void Main(string[] args)
    {
        List<Car> listOfCars = new List<Car>()
        {
            new Car() { Id = 1, MaxSpeed = 180F, Model = "FirstModel", Description = "FirstDescription" },
            new Car() { Id = 2, MaxSpeed = 120F, Model = "SecondModel", Description = "SecondDescription" },
            new Car() { Id = 3, MaxSpeed = 220F, Model = "ThirdModel", Description = "ThirdDescription" }
        };
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        app.Run(async (context) =>
        {
            var request = context.Request;
            var response = context.Response;
            var path = context.Request.Path;
            string expressionForNumber = "^/api/cars/([0-9]+)$";
            if (path == "/api/cars" && request.Method == "GET")
            {
                await response.WriteAsJsonAsync(listOfCars);
            }
            else if (Regex.IsMatch(path, expressionForNumber) && request.Method == "GET")
            {
                int intVariable = int.Parse(path.Value?.Split("/")[3]);
                try
                {
                    Car myCar = listOfCars.FirstOrDefault((inCar) => inCar.Id == intVariable);
                    if (myCar is null)
                    {
                        response.StatusCode = 404;
                        await response.WriteAsJsonAsync(new
                        {
                            message = "Car is not Found!"
                        });
                    }
                    else
                    {
                        await response.WriteAsJsonAsync(myCar);
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = 505;
                    await response.WriteAsJsonAsync(new
                    {
                        message = "Server ERROR!"
                    });
                }
            }
            else if (path == "/api/cars" && request.Method == "POST")
            {
                try
                {
                    Car tempCar = await request.ReadFromJsonAsync<Car>();                    
                    if (tempCar != null)                     
                    {
                        response.StatusCode = 404;
                        await response.WriteAsJsonAsync(new
                        {
                            message = "Car is not Found!"
                        });
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = 505;
                    await response.WriteAsJsonAsync(new
                    {
                        message = "Server ERROR!"
                    });
                }
            }
            else if (path == "/api/cars" && request.Method == "PUT")
            {
                try
                {
                    Car tempCar = await request.ReadFromJsonAsync<Car>();
                    if (tempCar != null)
                    {
                        Car existingCar = listOfCars.FirstOrDefault(c => c.Id == tempCar.Id);
                        if (existingCar != null)
                        {
                            existingCar.MaxSpeed = tempCar.MaxSpeed;
                            existingCar.Model = tempCar.Model;
                            existingCar.Description = tempCar.Description;
                            await response.WriteAsJsonAsync(existingCar);
                        }
                        else
                        {
                            response.StatusCode = 484;
                            await response.WriteAsJsonAsync(new
                            {
                                message = "Car is not found!"
                            });
                        }
                    }
                    else
                    {
                        response.StatusCode = 404;
                        await response.WriteAsJsonAsync(new
                        {
                            message = "Car si not found!"
                        });
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = 500;
                    await response.WriteAsJsonAsync(new
                    {
                        message = "Server Error"
                    });
                }
            }
            else if (path == "/api/cars" && request.Method == "DELETE")
            {
                try
                {
                    int Id = -1;
                    int.TryParse(path.Value?.Split("/")[3], out Id);
                    if (Id != -1)
                    {
                        int index = listOfCars.FindIndex(x => x.Id == Id);
                        listOfCars.RemoveAt(index);
                        await response.WriteAsJsonAsync(listOfCars);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        await response.WriteAsJsonAsync(new
                        {
                            message = "Car not found!"
                        });
                    }
                }
                catch(Exception ex)
                {
                    response.StatusCode = 505;
                    await response.WriteAsJsonAsync(new
                    {
                        message = "Server error"
                    });
                }
            }
        });
        app.Run();
    }
}