using System.Threading.Tasks;

namespace Services.Interfaces;

public interface IPictureEditService
{
    Task<string> CompressJpgToWebPFormat(string fullInputJpgPath);
}