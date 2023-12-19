// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

// Ignore Spelling: Msg

namespace IceShell.Core.CLI.Languages;

using IceShell.Core.Api;

/// <summary>
/// Provides quick access to <see cref="LanguageService"/>.
/// </summary>
public static class LangMessage
{
    /// <summary>
    /// Gets the English language file.
    /// </summary>
    /// <returns>The English language file.</returns>
    public static LanguageFile EnglishLang()
    {
        return LanguageService.Instance.English();
    }

    /// <summary>
    /// Gets the message with the specified key.
    /// </summary>
    /// <param name="messageName">The name of the message.</param>
    /// <returns>The message if found, or the message key otherwise.</returns>
    public static string Get(string messageName)
    {
        if (LanguageService.Instance.Current().TryGet(messageName, out var currentMessage)
            && currentMessage != null)
        {
            return currentMessage;
        }

        if (LanguageService.Instance.English().TryGet(messageName, out var englishMessage)
            && englishMessage != null)
        {
            return englishMessage;
        }

        return messageName;
    }

    /// <summary>
    /// Gets the message of specified name from the current language file and interpolate it
    /// with the specified arguments via <see cref="string.Format(string, object)"/>.
    /// </summary>
    /// <param name="messageName">The key of the message.</param>
    /// <param name="arg0">The argument to fill.</param>
    /// <returns>The interpolated message.</returns>
    public static string GetFormat(string messageName, object arg0)
    {
        return string.Format(Get(messageName), arg0);
    }

    /// <summary>
    /// Gets the message of specified name from the current language file and interpolate it
    /// with the specified arguments via <see cref="string.Format(string, object, object)"/>.
    /// </summary>
    /// <param name="messageName">The key of the message.</param>
    /// <param name="arg0">The first argument to fill.</param>
    /// <param name="arg1">The second argument to fill.</param>
    /// <returns>The interpolated message.</returns>
    public static string GetFormat(string messageName, object arg0, object arg1)
    {
        return string.Format(Get(messageName), arg0, arg1);
    }

    /// <summary>
    /// Gets the message of specified name from the current language file and interpolate it
    /// with the specified arguments via <see cref="string.Format(string, object?[])"/>.
    /// </summary>
    /// <param name="messageName">The name of the message.</param>
    /// <param name="args">The format arguments.</param>
    /// <returns>The formatted language string.</returns>
    public static string GetFormat(string messageName, params object[] args)
    {
        return string.Format(Get(messageName), args);
    }

    /// <summary>
    /// Gets the message that is displayed when the command user have specified does not exist.
    /// </summary>
    /// <param name="command">The name of the command that user have specified.</param>
    /// <returns>The unknown command message.</returns>
    public static string MsgUnknownCommand(string command)
    {
        return Get(command);
    }

    /// <summary>
    /// Gets the message that is display when the process cannot be started.
    /// </summary>
    /// <returns>The unable to start process message.</returns>
    public static string MsgUnableStartProcess() => Get("shell_unable_start_process");

    /// <summary>
    /// Gets the message that is displayed when the specified executable does not exist or is in an invalid format.
    /// </summary>
    /// <param name="fileName">The name of or path to the executable.</param>
    /// <returns>The invalid executable message.</returns>
    public static string MsgInvalidExecutable(string fileName)
    {
        return GetFormat("shell_invalid_executable", fileName);
    }

    /// <summary>
    /// Gets the message that is displayed when the specified directory does not exist.
    /// </summary>
    /// <param name="directory">The name of or path to directory.</param>
    /// <returns>The directory not found message.</returns>
    public static string MsgDirectoryNotFound(string directory)
    {
        return GetFormat("generic_bad_directory", directory);
    }

    /// <summary>
    /// Gets the message that is displayed when the specified file does not exist.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The file not found message.</returns>
    public static string MsgFileNotFound(string fileName)
    {
        return GetFormat("generic_file_not_found", fileName);
    }

    /// <summary>
    /// Gets the message that is displayed when there are more arguments specified than the command can accept.
    /// </summary>
    /// <param name="current">The amount of arguments that was specified.</param>
    /// <param name="max">The maximum amount of arguments that the command can accept.</param>
    /// <returns>The too many arguments message.</returns>
    public static string MsgTooManyArguments(int current, int max)
    {
        return GetFormat("argument_surpassing_count", current, max);
    }

    /// <summary>
    /// Gets the message that is displayed when there are less arguments specified than the command requires.
    /// </summary>
    /// <param name="current">The amount of arguments that was specified.</param>
    /// <param name="required">The amount of arguments that the command requires.</param>
    /// <returns>The too less arguments message.</returns>
    public static string MsgTooFewArguments(int current, int required)
    {
        return GetFormat("argument_lower_than_count", current, required);
    }

    /// <summary>
    /// Gets the message that is displayed when an action never completes.
    /// </summary>
    /// <returns>The action never complete message.</returns>
    public static string MsgActionNeverComplete() => Get("generic_action_never_complete");

    /// <summary>
    /// Gets the message that is displayed when Nth value is required but missing.
    /// </summary>
    /// <param name="value">The Nth value that is missing.</param>
    /// <returns>The requires value message.</returns>
    public static string MsgMissingValue(int value) => GetFormat("generic_requires_value", value);
    
    /// <summary>
    /// Gets the message that is displayed when Nth value is required but missing.
    /// </summary>
    /// <param name="value">The reference name of the that is missing.</param>
    /// <returns>The requires value message.</returns>
    public static string MsgMissingValue(string value) => GetFormat("generic_requires_value", value);


    /// <summary>
    /// Gets the message that is displayed when the procedure cannot continue because it does not have access
    /// to the specified file.
    /// </summary>
    /// <param name="file">The path to or the name of the file.</param>
    /// <returns>The unauthorized file access message.</returns>
    public static string MsgUnauthorizedFile(string file) => GetFormat("generic_unauthorized_file", file);

    /// <summary>
    /// Gets the message that is displayed when there are two or more sources specified but the destination is a single file,
    /// and renaming is not available or not specified.
    /// </summary>
    /// <returns>The message.</returns>
    public static string MsgDestinationFileButMultipleSource() => Get("generic_destination_file_more");

    /// <summary>
    /// Gets the message that is displayed when the specified file exists but its presence is not allowed.
    /// </summary>
    /// <returns>The file already exists message.</returns>
    public static string MsgFileAlreadyExists(string file) => GetFormat("generic_file_exists", file);

    /// <summary>
    /// Gets the message that is displayed when the specified directory exists but its presence is not allowed.
    /// </summary>
    /// <returns>The directory already exists message.</returns>
    public static string MsgDirectoryAlreadyExists(string directory) => GetFormat("generic_directory_exists", directory);

    /// <summary>
    /// Gets the message that is displayed when the specified path is invalid.
    /// </summary>
    /// <returns>The message.</returns>
    public static string InvalidPath() => Get("generic_path_invalid");

    /// <summary>
    /// Gets the message that is associated with the specified error code.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <returns>The error code message.</returns>
    public static string MsgFromErrorCode(CommandErrorCode errorCode) => Get($"generic_command_error_{(int)errorCode}");

    /// <summary>
    /// Gets the message that is displayed when the specified argument cannot be resolved.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns>The message.</returns>
    public static string MsgCannotResolve(string arg) => GetFormat("generic_arg_cannot_resolve", arg);
}
