using FluentValidation;

namespace ApiClient.Options;

public class WeatherServerOptions : BaseOptions
{
    public override string SectionName => "WhetherService";

    public string? Url { get; set; }

    public class Validator : AbstractValidator<WeatherServerOptions>
    {
        public Validator()
        {
            RuleFor(x => x.Url)
                .NotNull()
                .WithMessage($"{nameof(WeatherServerOptions)}.{nameof(WeatherServerOptions.Url)} is required.");
        }
    }
}
