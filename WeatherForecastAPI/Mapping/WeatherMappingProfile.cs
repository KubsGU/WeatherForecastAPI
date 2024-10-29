using AutoMapper;
using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Mapping
{
    public class WeatherMappingProfile : Profile
    {
        public WeatherMappingProfile()
        {
            CreateMap<Coordinate, CoordinateDto>();

            CreateMap<WeatherForecast, WeatherForecastDto>();

            CreateMap<Coordinate, WeatherResponse>()
                .ForMember(dest => dest.CoordinateId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Coordinate, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Forecast, opt => opt.Ignore());
        }
    }
}
