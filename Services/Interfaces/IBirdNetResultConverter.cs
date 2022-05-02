using Dtos;

namespace Services.Interfaces
{
    public interface IBirdNetResultConverter
    {
        BirdNetOutputDto ConvertJson(string json);
    }
}