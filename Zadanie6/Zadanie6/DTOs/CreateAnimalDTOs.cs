using System.Diagnostics.CodeAnalysis;

namespace Zadanie6.DTOs;

using System.ComponentModel.DataAnnotations;



public record CreateAnimalRequest(
        [Required] [MaxLength(200)] string Name,
        [MaxLength(200)] string Description,
        [Required] [MaxLength(200)] string Category,
        [Required] [MaxLength(200)] string Area
);

public record CreateAnimalResponse(int Id, string Name, string Description, string Category, string Area)
{
        public CreateAnimalResponse(int id, CreateAnimalRequest request) : this(id, request.Name, request.Description,
                request.Category, request.Area) {}
};

