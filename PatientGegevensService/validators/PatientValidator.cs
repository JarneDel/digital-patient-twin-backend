using FluentValidation;
using PatientGegevensService.models;

namespace PatientGegevensService.validators;

public class PatientValidator : AbstractValidator<PatientGegevens>
{
    public PatientValidator()
    {
        RuleFor(g => g.Adres.Gemeente).NotEmpty().WithMessage("Gemeente is niet ingevuld");
        RuleFor(g => g.Adres.Nr).NotEmpty().WithMessage("Huisnummer is niet ingevuld");
        RuleFor(g => g.Adres.Postcode).NotEmpty().WithMessage("Postcode is niet ingevuld");
        RuleFor(g => g.Adres.Straat).NotEmpty().WithMessage("Straat is niet ingevuld");
        RuleFor(g => g.Algemeen.Geboorteland).NotEmpty().WithMessage("Geboorteland is niet ingevuld");
        RuleFor(g => g.Algemeen.GeboorteDatum).NotEmpty().WithMessage("Geboorteplaats is niet ingevuld");
        RuleFor(g => g.Algemeen.Geslacht).NotEmpty().WithMessage("Geboorteplaats is niet ingevuld");
        RuleFor(g => g.Algemeen.Naam).NotEmpty().WithMessage("Naam is niet ingevuld");
        RuleFor(g => g.Algemeen.Voornaam).NotEmpty().WithMessage("Voornaam is niet ingevuld");
        RuleFor(g => g.Contact.Email).NotEmpty().EmailAddress().WithMessage("Email is niet ingevuld of niet geldig");
        RuleFor(g => g.Contact.Telefoon).NotEmpty().WithMessage("Telefoon is niet ingevuld");
    }
}