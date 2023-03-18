using TinyFS;

Arguments A;
try
{
    A = new Arguments(args);
}
catch (Exception ex)
{
    Console.Error.WriteLine("Failed to parse command line arguments. {0}", ex.Message);
    return byte.MaxValue;
}

try
{
    A.ExecuteMode();
}
catch (Exception ex)
{
    Console.Error.WriteLine("Error performing {0} operation. {1}", A.Mode, ex.Message);
    return 1;
}

return 0;