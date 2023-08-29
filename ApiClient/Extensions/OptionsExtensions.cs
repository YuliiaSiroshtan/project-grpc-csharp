using ApiClient.Options;
using FluentValidation;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ApiClient.Extensions;

public static class OptionsExtensions
{
    public static TOptions LoadOptions<TOptions, TOptionsValidator>(
   IConfiguration configuration,
   IServiceCollection? services = null
)
   where TOptions : BaseOptions, new()
   where TOptionsValidator : AbstractValidator<TOptions>, new()
    {
        var options = new TOptions();
        var section = configuration.GetSection(options.SectionName);
        if (section.Value != null)
        {
            options = JsonConvert.DeserializeObject<TOptions>(section.Value);
        }
        else
        {
            section.Bind(options);
        }

        new TOptionsValidator()!.ValidateAndThrow(options);
        services?.AddSingleton<IOptions<TOptions>>(_ => Microsoft.Extensions.Options.Options.Create(options)!);

        return options!;
    }
}
