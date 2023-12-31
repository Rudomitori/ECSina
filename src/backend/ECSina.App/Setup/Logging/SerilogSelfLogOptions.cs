using ECSina.Common.Core.Configuration;

namespace ECSina.App.Setup.Logging;

public sealed class SerilogSelfLogOptions : IPositionedOptions
{
    public static string Position => "Serilog:SelfLog";
    public string? FilePath { get; set; }

    public bool IsEnabled => FilePath is { };
}
