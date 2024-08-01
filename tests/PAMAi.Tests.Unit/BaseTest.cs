using System.Diagnostics;

namespace PAMAi.Tests.Unit;
internal class BaseTest
{
    [OneTimeSetUp]
    public void Setup()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        Trace.Flush();
    }
}
