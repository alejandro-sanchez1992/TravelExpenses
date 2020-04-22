using System;
using System.Threading.Tasks;
using TravelExpenses.Common.Models;

namespace TravelExpenses.Common.interfaces
{
    public interface IApiService
    {
        Task<Response<object>> ChangePasswordAsync(string urlBase, string servicePrefix, string controller, ChangePasswordRequest changePasswordRequest, string tokenType, string accessToken);

        Task<Response<object>> PutAsync<T>(string urlBase, string servicePrefix, string controller, T model, string tokenType, string accessToken);

        Task<Response<object>> RecoverPasswordAsync(string urlBase, string servicePrefix, string controller, EmailRequest emailRequest);

        Task<Response<object>> RegisterUserAsync(string urlBase, string servicePrefix, string controller, UserRequest userRequest);

        Task<bool> CheckConnectionAsync(string url);

        Task<Response<TokenResponse>> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest request);

        Task<Response<EmployeeResponse>> GetUserByEmail(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, EmailRequest request);

        Task<Response<object>> GetListAsync<T>(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken);

        Task<Response<object>> PostAsync<T>(string urlBase, string servicePrefix, string controller, T model, string tokenType, string accessToken);

        Task<Response<object>> GetTripAsync(string urlBase, string servicePrefix, string controller, int id, string tokenType, string accessToken);
    }
}
