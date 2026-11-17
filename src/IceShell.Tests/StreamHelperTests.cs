// SPDX-FileCopyrightText: 2023, 2026 WithLithum
// SPDX-License-Identifier: GPL-3.0-or-later

using IceShell.Core.Commands.Utilities;

namespace IceShell.Tests;

public class StreamHelperTests
{
    [Fact]
    public void CopyText_LargeStream_Succeed()
    {
        // Arrange
        // Duplicate a large string for testing purpose
        const string sourceA = "AABBCCDDEEFFGGHHIIJJKKLLMMNNOOPPQQRRSSTTUUVVWWXXYYZZ";
        string sourceB = $"{sourceA}{sourceA}{sourceA}{sourceA}{sourceA}";
        string origin = $"{sourceB}{sourceB}{sourceB}{sourceB}{sourceB}";

        var reader = new StringReader(origin);
        var writer = new StringWriter();

        // Act
        StreamHelper.CopyText(reader, writer);

        // Assert
        Assert.Equal(origin, writer.ToString());
    }
}
