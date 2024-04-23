namespace Zadanie6.Controllers;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Zadanie6.DTOs;


[ApiController]
[Route("api/animals")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAllAnimals()
    {
        var response = new List<GetAnimalsResponse>();
        using(var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand("SELECT * FROM Animal", sqlConnection);
            sqlCommand.Connection.Open();
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                response.Add(new GetAnimalsResponse(
                    reader.GetInt32(0), 
                    reader.GetString(1), 
                    reader.GetString(2), 
                    reader.GetString(3), 
                    reader.GetString(4))
                );
            }
        }

        return Ok(response);
    }

    [HttpGet("{orderBy}")]
    public IActionResult GetAnimal(string orderBy = "name")
    {
        var response = new List<GetAnimalsResponse>();
        var validColumns = new[] { "name", "description", "category", "area" };
        if (!validColumns.Contains(orderBy.ToLower()))
        {
            return BadRequest("Invalid parameter. Use one of these: name,description,category,area ");
        }
        if (string.IsNullOrEmpty(orderBy))
        {
            orderBy = "name";
        }
        using var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        var sqlCommand = new SqlCommand($"SELECT * FROM Animal ORDER BY {orderBy} ASC ",sqlConnection);
        
        sqlCommand.Connection.Open();
        var reader = sqlCommand.ExecuteReader();
        while (reader.Read())
        {
            response.Add(new GetAnimalsResponse(
                reader.GetInt32(0), 
                reader.GetString(1), 
                reader.GetString(2), 
                reader.GetString(3), 
                reader.GetString(4))
            );
        }

        return Ok(response);
    }

    [HttpPost]
    public IActionResult CreateAnimal(CreateAnimalRequest request)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "INSERT INTO Animal (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area); SELECT CAST(SCOPE_IDENTITY() as int)",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@Name", request.Name);
            sqlCommand.Parameters.AddWithValue("@Description", request.Description);
            sqlCommand.Parameters.AddWithValue("@Category", request.Category);
            sqlCommand.Parameters.AddWithValue("@Area", request.Area);
            sqlCommand.Connection.Open();

            var idAnimal =(int)(decimal) sqlCommand.ExecuteScalar();

            return Created($"animals/{idAnimal}", new CreateAnimalResponse(idAnimal, request));

        }
    }

    [HttpPut("{idAnimal}")]
    public IActionResult ReplaceAnimal(int idAnimal, ReplaceAnimalRequest request)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "UPDATE Animal SET Name = @Name,Description = @Description,Category = @Category, Area=@Area WHERE IdAnimal = @Id",
                sqlConnection
                );
            sqlCommand.Parameters.AddWithValue("@Name", request.Name);
            sqlCommand.Parameters.AddWithValue("@Description", request.Description);
            sqlCommand.Parameters.AddWithValue("@Category", request.Category);
            sqlCommand.Parameters.AddWithValue("@Area", request.Area);
            sqlCommand.Parameters.AddWithValue("@Id", idAnimal);
            sqlCommand.Connection.Open();

            var affectedRows = sqlCommand.ExecuteNonQuery();
            return affectedRows == 0 ? NotFound() : NoContent();

        }
    }

    [HttpDelete("{idAnimal}")]
    public IActionResult RemoveAnimal(int idAnimal)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand("DELETE FROM Animal WHERE IdAnimal = @idAnimal", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@idAnimal", idAnimal);
            sqlCommand.Connection.Open();

            var affectedRows = sqlCommand.ExecuteNonQuery();

            return affectedRows == 0 ? NotFound() : NoContent();
        }
    }
}