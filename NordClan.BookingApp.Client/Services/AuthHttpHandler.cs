using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Headers;

namespace NordClan.BookingApp.Client.Services
{
    public class AuthHttpHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;
        private readonly NavigationManager _navigation;
        private readonly IJSRuntime _js;

        public AuthHttpHandler(IAuthService authService, NavigationManager navigation, IJSRuntime js)
        {
            _authService = authService;
            _navigation = navigation;
            _js = js;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _authService.Logout();
                _navigation.NavigateTo("/login", true);
            }

            return response;
        }
    }
}
