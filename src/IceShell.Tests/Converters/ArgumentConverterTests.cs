namespace IceShell.Tests.Converters;

using IceShell.Core.Api;
using IceShell.Core.Commands.Argument;
using IceShell.Core.Exceptions;

public class ArgumentConverterTests
{
    [Test]
    public void TryConvert_Enum()
    {
        var converter = new ArgumentConvertService();
        Assert.That(converter.TryConvert("ParsingFailure", typeof(CommandErrorCode), out var result) 
                    && result?.Equals(CommandErrorCode.ParsingFailure) == true);
    }

    [Test]
    public void EnumConverter_Convert()
    {
        var converter = new EnumArgumentConverter();
        Assert.That(converter.TryConvert("BadArgument", typeof(CommandErrorCode), out var result) 
                    && object.Equals(result, CommandErrorCode.BadArgument));
    }
    
    [Test]
    public void EnumConverter_ConvertIsCaseInsensitive()
    {
        var converter = new EnumArgumentConverter();
        Assert.That(converter.TryConvert("BAdARgUMeNT", typeof(CommandErrorCode), out var result) 
                    && object.Equals(result, CommandErrorCode.BadArgument));
    }
    
    [Test]
    public void UInt8Converter_Convert()
    {
        var converter = new ByteArgumentConverter();
        Assert.That(converter.TryConvert("255", typeof(CommandErrorCode), out var result) 
                    && object.Equals(result, (byte)255));
    }
    
    [Test]
    public void Int16Converter_Convert()
    {
        var converter = new Int16ArgumentConverter();
        Assert.That(converter.TryConvert("255", typeof(CommandErrorCode), out var result) 
                    && object.Equals(result, (byte)255));
    }
}
