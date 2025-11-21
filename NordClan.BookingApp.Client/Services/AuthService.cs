using Microsoft.JSInterop;
using NordClan.BookingApp.Shared.Models;
using System.Net.Http.Json;

namespace NordClan.BookingApp.Client.Services;

public class AuthService : IAuthService
{
    private const string StorageKeyToken = "authToken";
    private const string StorageKeyUsername = "authUser";

    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;

    public AuthService(HttpClient httpClient, IJSRuntime js)
    {
        _httpClient = httpClient;
        _js = js;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var request = new LoginRequest
        {
            Username = username,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("api/Login", request);

        if (!response.IsSuccessStatusCode)
            return false;

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
            return false;

        await _js.InvokeVoidAsync("localStorage.setItem", StorageKeyToken, loginResponse.Token);
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKeyUsername, username);

        return true;
    }

    public async void Logout()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", StorageKeyToken);
        await _js.InvokeVoidAsync("localStorage.removeItem", StorageKeyUsername);
    }
}
