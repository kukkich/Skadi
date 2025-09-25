using Microsoft.Extensions.Logging;

namespace Skadi;

//Todo replace with composition
public abstract class Method<TConfig>(TConfig config, ILogger logger)
{
    protected TConfig Config { get; } = config;
    protected ILogger Logger { get; } = logger;
}