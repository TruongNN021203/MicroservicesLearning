using AuthenticationApi.Application.DTOs;
using Ecommerce.SharedLibrary.Responses;


namespace AuthenticationApi.Application.Interface;

public interface IUser
{
    Task<Response> Register(AppUserDTO appUserDTO);
    Task<Response> Login(LoginDTO loginDTO);
    Task<GetUserDTO> GetUser(int userId);
}
