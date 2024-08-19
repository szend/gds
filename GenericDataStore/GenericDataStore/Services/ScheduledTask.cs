using GenericDataStore.Controllers;
using GenericDataStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;

namespace GenericDataStore.Services
{
    public class ScheduledTask
    {

        public async void Start()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7053/api/Users/CheckSub/s");
                var res = client.GetFromJsonAsync<object>(client.BaseAddress)?.Result;
            }
            catch (Exception)
            {


            }

        }

    }
}
