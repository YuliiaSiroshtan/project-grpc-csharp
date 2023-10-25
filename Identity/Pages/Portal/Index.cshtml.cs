using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Portal;

public class Index : PageModel
{
    private readonly ClientRepository _repository;

    public Index(ClientRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<ThirdPartyInitiatedLoginLink> Clients { get; private set; }

    public async Task OnGetAsync()
    {
        Clients = await _repository.GetClientsWithLoginUris();
    }
}