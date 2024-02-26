using Microsoft.Extensions.Logging;

namespace SharpMath;

//TODO Можно добавить поддержку логирования
// Например, передавать логгер через интерфейс или
// Использовать Shared логгер, если переданный null
public abstract class Method<TConfig> 
{
    protected TConfig Config { get; }
    protected ILogger Logger { get; }

    protected Method(TConfig config, ILogger logger)
    {
        Config = config;
        Logger = logger;
    }
}