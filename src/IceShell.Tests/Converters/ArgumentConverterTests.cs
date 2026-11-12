namespace IceShell.Tests.Converters;

using IceShell.Core.Api;
using IceShell.Core.Commands.Argument;

public class ArgumentConverterTests
{
    [Fact]
    public void TryConvert_Enum()
    {
        var converter = new ArgumentConvertService();
        converter.AddDefaultConverters();
        Assert.True(converter.TryConvert("ParsingFailure", typeof(CommandErrorCode), out var result));
        Assert.Equal(CommandErrorCode.ParsingFailure, result);
    }

    [Fact]
    public void EnumConverter_Convert()
    {
        var converter = new EnumArgumentConverter();
        Assert.True(converter.TryConvert("BadArgument", typeof(CommandErrorCode), out var result) 
                    && object.Equals(result, CommandErrorCode.BadArgument));
    }
    
    [Fact]
    public void EnumConverter_ConvertIsCaseInsensitive()
    {
        var converter = new EnumArgumentConverter();
        Assert.True(converter.TryConvert("BAdARgUMeNT", typeof(CommandErrorCode), out var result) 
                    && object.Equals(result, CommandErrorCode.BadArgument));
    }
    
    [Fact]
    public void UInt8Converter_Convert()
    {
        var converter = new ByteArgumentConverter();
        Assert.True(converter.TryConvert("255", typeof(byte), out var result) 
                    && object.Equals(result, (byte)255));
    }
    
    [Fact]
    public void Int16Converter_Convert()
    {
        var converter = new Int16ArgumentConverter();
        Assert.True(converter.TryConvert("32767", typeof(short), out var result) 
                    && object.Equals(result, (short)32767));
    }
    
    [Fact]
    public void Int32Converter_Convert()
    {
        var converter = new Int32ArgumentConverter();
        Assert.True(converter.TryConvert("255555", typeof(int), out var result) 
                    && object.Equals(result, 255555));
    }
}
