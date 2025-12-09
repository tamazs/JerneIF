using Xunit;

namespace Tests;

// This makes ALL tests that use [Collection("IntegrationTests")]
// run sequentially (no parallel execution).
[CollectionDefinition("IntegrationTests", DisableParallelization = true)]
public class IntegrationTestCollection
{
    // No code needed here.
}