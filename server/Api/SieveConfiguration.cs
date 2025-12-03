using DataAccess;
using Sieve.Services;

namespace Api;

public class SieveConfiguration : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Transaction>(t => t.User.FullName)
            .CanFilter()
            .CanSort()
            .HasName("fullName");
    }
}