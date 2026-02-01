using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.DemoMvc.Infrastructure.ExternalServices;

namespace DevcraftWMS.DemoMvc.Controllers;

[Route("address-lookup")]
public sealed class AddressLookupController : Controller
{
    private readonly IIbgeClient _ibgeClient;
    private readonly IViaCepClient _viaCepClient;

    public AddressLookupController(IIbgeClient ibgeClient, IViaCepClient viaCepClient)
    {
        _ibgeClient = ibgeClient;
        _viaCepClient = viaCepClient;
    }

    [HttpGet("states")]
    public async Task<IActionResult> States(CancellationToken cancellationToken)
    {
        var states = await _ibgeClient.GetStatesAsync(cancellationToken);
        var result = states
            .Select(state => new AddressLookupItem(state.Code, state.Name))
            .ToList();

        return Ok(result);
    }

    [HttpGet("cities")]
    public async Task<IActionResult> Cities([FromQuery] string uf, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(uf))
        {
            return BadRequest(new { message = "UF is required." });
        }

        var cities = await _ibgeClient.GetCitiesAsync(uf, cancellationToken);
        var result = cities
            .Select(city => new AddressLookupItem(city.Name, city.Name))
            .ToList();

        return Ok(result);
    }

    [HttpGet("cep")]
    public async Task<IActionResult> Cep([FromQuery] string cep, CancellationToken cancellationToken)
    {
        var normalizedCep = new string((cep ?? string.Empty).Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(normalizedCep) || normalizedCep.Length < 8)
        {
            return BadRequest(new { message = "Invalid CEP." });
        }

        var response = await _viaCepClient.LookupAsync(normalizedCep, cancellationToken);
        if (response is null)
        {
            return NotFound(new { message = "CEP not found." });
        }

        var result = new CepLookupResult(
            normalizedCep,
            response.AddressLine1,
            response.District,
            response.City,
            response.State,
            "BR");

        return Ok(result);
    }

    private sealed record AddressLookupItem(string Code, string Name);

    private sealed record CepLookupResult(
        string PostalCode,
        string AddressLine1,
        string District,
        string City,
        string State,
        string Country);
}
