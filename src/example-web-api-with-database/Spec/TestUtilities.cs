namespace Spec;

/// <summary>
/// Expects a test to fail with a given reason. If the test passes, an assertion error will be thrown.
/// This is useful to ensure that a test is not accidentally passing when it is not supposed to.
/// Giving you a clear signal that your tests are poorly defined and needs to be revisited.
/// </summary>
/// <param name="reason">The reason for the failure. This will be displayed if the test passes.</param>
/// <param name="test">The test to run.</param>
public static class ExpectFailure
{
  public static void Run(string reason, Action test)
  {
    var passed = false;

    try
    {
      test();
      passed = true;
    }
    catch
    {
      // expected path
    }

    if (passed)
      Assert.Fail($"Test unexpectedly passed: {reason}");
  }

  public static async Task Run(string reason, Func<Task> test)
  {
    var passed = false;

    try
    {
      await test();
      passed = true;
    }
    catch
    {
      // expected path
    }

    if (passed)
      Assert.Fail($"Test unexpectedly passed: {reason}");
  }
}
