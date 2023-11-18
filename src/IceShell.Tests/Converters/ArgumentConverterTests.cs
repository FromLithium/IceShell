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
        converter.AddDefaultConverters();
        Assert.That(converter.TryConvert("ParsingFailure", typeof(CommandErrorCode), out var result));
        Assert.That(result, Is.EqualTo(CommandErrorCode.ParsingFailure));
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
        Assert.That(converter.TryConvert("255", typeof(byte), out var result) 
                    && object.Equals(result, (byte)255));
    }
    
    [Test]
    public void Int16Converter_Convert()
    {
        var converter = new Int16ArgumentConverter();
        Assert.That(converter.TryConvert("32767", typeof(short), out var result) 
                    && object.Equals(result, (short)32767));
    }
    
    [Test]
    public void Int32Converter_Convert()
    {
        var converter = new Int32ArgumentConverter();
        Assert.That(converter.TryConvert("255555", typeof(int), out var result) 
                    && object.Equals(result, 255555));
    }
}
