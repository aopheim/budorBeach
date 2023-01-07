using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Interfaces;

public interface IImageConverter
{
    Task<Stream> Jpg2WebP(Stream jpgStream, CancellationToken cancellationToken);
}