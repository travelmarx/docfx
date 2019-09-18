// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.MarkdigEngine.Tests
{
    using System.Collections.Generic;
    using Xunit;

    public class MarkdigServiceTest
    {
        [Fact]
        [Trait("Related", "MarkdigService")]
        public void MarkdigServiceTest_ParseAndRender_Simple()
        {
            var markdown = @"# title

```yaml
key: value
```";

            var expected = @"<h1 id=""title"">title</h1>
<pre><code class=""lang-yaml"">key: value
</code></pre>
";

            TestUtility.VerifyMarkup(markdown, expected);
        }

        [Fact]
        [Trait("Related", "MarkdigService")]
        public void MarkdigServiceTest_ParseAndRender_Inclusion()
        {
            // -x
            //  |- root.md
            //  |- b
            //  |  |- linkAndRefRoot.md
            var root = @"[!include[linkAndRefRoot](~/x/b/linkAndRefRoot.md)]";
            var linkAndRefRoot = @"Paragraph1";

            var expected = @"<p>Paragraph1</p>";

            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "x/root.md",
                dependencies: new[] { "b/linkAndRefRoot.md" },
                files: new Dictionary<string, string>
                {
                    { "x/b/linkAndRefRoot.md", linkAndRefRoot },
                });
        }

        [Fact]
        [Trait("Related", "MarkdigService")]
        public void MarkdigServiceTest_ParseInline()
        {
            var markdown = @"# I am a heading";
            var expected = @"<h1 id=""i-am-a-heading"">I am a heading</h>";

            TestUtility.VerifyMarkup(markdown, expected);
        }
    }
}
