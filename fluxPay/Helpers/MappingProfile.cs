using AutoMapper;

using fluxPay.DTOs;
using fluxPay.Interfaces;
using FluxPay.Models;


namespace fluxPay.Helpers
{
    public class MappingProfiles : Profile
    {  
        public MappingProfiles()
     {
         CreateMap<TempUser, UserDto>();
     }

    }
     
    
}