using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoFixture;
using budorWeb.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace budorWeb.Api.Controllers
{
    [Route("images")]
    public class ImagesController : BaseBudorController
    {
        [HttpGet(nameof(GetLatestBirdImages))]
        public List<BirdImageDto> GetLatestBirdImages(int numberOfImages = 5,
            CancellationToken cancellationToken = default)
        {
            var dtos = Fixture.CreateMany<BirdImageDto>(5).ToList();
            foreach (var dto in dtos)
                dto.Url = "https://budorbeach.blob.core.windows.net/images/2021-10-11/6-44-39.jpg";

            return dtos;
        }
    }
}