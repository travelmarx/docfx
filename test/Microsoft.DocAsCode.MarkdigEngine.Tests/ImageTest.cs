// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.MarkdigEngine.Tests
{
    using System.Collections.Generic;
    using Xunit;

    public class ImageTest
    {
        [Fact]
        public void ImageTestBlockGeneral()
        {
            var source = @":::image type=""content"" source=""example.jpg"" alt-text=""example"":::";
            var expected = @"<img src=""example.jpg"" alt=""example"">";

            TestUtility.VerifyMarkup(source, expected);
        }

        [Fact]
        public void ComplexImageTestBlockGeneral()
        {
            var source = @"
:::image type=""icon"" source=""example.svg"":::

:::image type=""complex"" source=""example.jpg"" alt-text=""example""::: 
Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.
:::image-end:::

:::image source=""example.jpg"" alt-text=""example"":::
";

            var expected = @"<img role=""presentation"" src=""example.svg"">
<img alt=""example"" aria-describedby=""a00f6"" src=""example.jpg"">
<div id=""a00f6"" class=""visually-hidden"">
<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>
</div>
<img alt=""example"" src=""example.jpg"">
";

            TestUtility.VerifyMarkup(source, expected);
        }

        [Fact]
        public void ImageWithIconTypeTestBlockGeneral()
        {
            var source = @":::image type=""icon"" source=""example.svg"":::";

            var expected = @"<img role=""presentation"" src=""example.svg"">";

            TestUtility.VerifyMarkup(source, expected);
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestImageBlockSrcResolveInToken()
        {
            // -r
            //  |- r.md
            //  |- b
            //  |  |- token.md
            //  |  |- img.jpg
            var r = @"
[!include[](b/token.md)]
";
            var token = @"
:::image source=""example.jpg"" type=""content"" alt-text=""example"":::
";

            var expected = @"<img alt=""example"" src=""~/r/b/example.jpg"">
";
            TestUtility.VerifyMarkup(r, expected, filePath: "r/r.md", files: new Dictionary<string, string>
            {
                { "r/b/token.md", token }
            });
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestComplexImageBlockSrcResolveInToken()
        {
            // -r
            //  |- r.md
            //  |- b
            //  |  |- token.md
            //  |  |- img.jpg
            var r = @"
[!include[](b/token.md)]
";
            var token = @"
:::image source=""example.jpg"" type=""complex"" alt-text=""example""::: 
Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.
:::image-end:::
";

            var expected = @"<img src=""~/r/b/example.jpg"" alt=""example"" aria-describedby=""e68bf"">
<div id=""e68bf"" class=""visually-hidden"">
<p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>
</div>
";
            TestUtility.VerifyMarkup(r, expected, filePath: "r/r.md", files: new Dictionary<string, string>
            {
                { "r/b/token.md", token }
            });
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestImageWithIconTypeBlockSrcResolveInToken()
        {
            // -r
            //  |- r.md
            //  |- b
            //  |  |- token.md
            //  |  |- img.jpg
            var r = @"
[!include[](b/token.md)]
";
            var token = @"
:::image source=""example.svg"" type=""icon"" alt-text=""example"":::
";

            var expected = @"<img role=""presentation"" src=""~/r/b/example.svg"">
";
            TestUtility.VerifyMarkup(r, expected, filePath: "r/r.md", files: new Dictionary<string, string>
            {
                { "r/b/token.md", token }
            });
        }

        [Fact]
        public void ImageBlockTestBlockClosed()
        {
            var source = @":::image source=""example.jpg"" type=""complex"" alt-text=""example"":::Lorem Ipsum
:::image-end:::";

            TestUtility.VerifyMarkup(source, null, new[] { "invalid-image" });
        }

        [Fact]
        public void ImageTestNotImageBlock()
        {
            var source = @":::row:::
:::column:::
    This is where your content goes.
:::column-end:::
:::row-end:::
";
            var expected = @"<section class=""row"">
< div class=""column"">
<p>This is where your content goes.</p>
</div>
</section>
";

            TestUtility.VerifyMarkup(source, expected);
        }

    }
}
