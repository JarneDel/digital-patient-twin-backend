#nullable enable
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace PatientGegevensService.models;

public class PatientGegevens
{
    [JsonProperty(PropertyName = "id")]
    public string? Id { get; set; }
    public string? DeviceId { get; set; }
    public General? Algemeen { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public Address? Adres { get; set; }
    public Contact? Contact { get; set; }
    public Medisch? Medisch { get; set; }
    public string? CreatedBy { get; set; }
}

public class General
{
    public string? Voornaam { get; set; }
    public string? Naam { get; set; }
    public string? Geslacht { get; set; }
    public string? GeboorteDatum { get; set; }
    public string? Geboorteland { get; set; }   
}

public class Address
{
    public string? Gemeente { get; set; }
    public string? Straat { get; set; }
    public int? Postcode { get; set; }
    public string? Nr { get; set; }
}

public class Contact
{
    public string? Email { get; set; }
    public string? Telefoon { get; set; }
}

public class Medisch
{
    public string? Bloedgroep { get; set; }
    public decimal? Lengte { get; set; }
    public decimal? Gewicht { get; set; }
}
