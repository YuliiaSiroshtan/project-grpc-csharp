using FluentValidation;

namespace ApiClient.Options;

public class WeatherServerOptions : BaseOptions
{
    public override string SectionName => "WeatherService";

    public string? Url { get; set; }

    public class Validator : AbstractValidator<WeatherServerOptions>
    {
        public Validator()
        {
            RuleFor(x => x.Url)
                .NotNull()
                .WithMessage($"{nameof(WeatherServerOptions)}.{nameof(Url)} is required.");
        }
    }
}