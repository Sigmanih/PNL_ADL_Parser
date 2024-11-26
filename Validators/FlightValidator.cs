using FluentValidation;
using PNL_ADL_Parser.Models;

namespace PNL_ADL_Parser.Validators;

public class FlightValidator : AbstractValidator<FlightDetails>
{
    public FlightValidator()
    {
        RuleFor(f => f.FlightNumber).NotEmpty().WithMessage("Flight number is required.");
        RuleFor(f => f.Route).NotEmpty().WithMessage("Route is required.");
        RuleFor(f => f.FlightDate).GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Flight date cannot be in the past.");
        RuleForEach(f => f.Passengers).SetValidator(new PassengerValidator());
    }
}

public class PassengerValidator : AbstractValidator<PassengerDetails>
{
    public PassengerValidator()
    {
        RuleFor(p => p.LastName).NotEmpty().WithMessage("Last name is required.");
        RuleFor(p => p.FirstName).NotEmpty().WithMessage("First name is required.");
        RuleFor(p => p.PassengerType).NotEmpty().WithMessage("Passenger type is required.");
    }
}
