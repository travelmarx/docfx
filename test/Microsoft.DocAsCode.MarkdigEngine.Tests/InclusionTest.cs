// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.MarkdigEngine.Tests
{
    using System.Collections.Generic;
    using Markdig;
    using Microsoft.DocAsCode.MarkdigEngine.Extensions;

    using Xunit;

    public class InclusionTest
    {
        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestBlockLevelInclusion_General()
        {
            var root = @"
# Hello World

Test Include File

[!include[refa](a.md)]

[!include[refa](a.md) ]

";

            var refa = @"---
title: include file
description: include file
---

# Hello Include File A

This is a file A included by another file. [!include[refb](b.md)] [!include[refb](b.md) ]

";

            var refb = @"---
title: include file
description: include file
---

# Hello Include File B
";

            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file. # Hello Include File B [!include<a href=""%7E/r/b.md"">refb</a> ]</p>

<p>[!include<a href=""a.md"">refa</a> ]</p>
";
            TestUtility.VerifyMarkup(
                root,
                expected, 
                filePath: "r/root.md",
                dependencies: new[] { "a.md", "b.md" },
                files: new Dictionary<string, string>
                {
                    { "r/a.md", refa },
                    { "r/b.md", refb },
                });
        }

        [Fact]
        [Trait("Related", "IncludeFile")]
        public void TestBlockLevelInclusion_Esacape()
        {
            var root = @"
# Hello World

Test Include File

[!include[refa](a\(x\).md)]

";

            var refa = @"
# Hello Include File A

This is a file A included by another file.
";

            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
";
            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "r/root.md",
                dependencies: new[] { "a(x).md" },
                files: new Dictionary<string, string>
                {
                    { "r/a(x).md", refa },
                });
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestBlockLevelInclusion_RelativePath()
        {
            var root = @"
# Hello World

Test Include File

[!include[refa](~/r/a.md)]

";

            var refa = @"
# Hello Include File A

This is a file A included by another file.
";

            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
";

            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "r/root.md",
                dependencies: new[] { "a.md" },
                files: new Dictionary<string, string>
                {
                    { "r/a.md", refa },
                });
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestBlockLevelInclusion_CycleInclude()
        {
            var root = @"
# Hello World

Test Include File

[!include[refa](a.md)]

";

            var refa = @"
# Hello Include File A

This is a file A included by another file.

[!include[refb](b.md)]

";

            var refb = @"
# Hello Include File B

[!include[refa](a.md)]
";

            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
<h1 id=""hello-include-file-b"">Hello Include File B</h1>
[!include[refa](a.md)]";

            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "r/root.md",
                errors: new[] { "circular-reference" },
                files: new Dictionary<string, string>
                {
                    { "r/a.md", refa },
                    { "r/b.md", refb },
                });
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestInlineLevelInclusion_General()
        {
            var root = @"
# Hello World

Test Inline Included File: \\[!include[refa](~/r/a.md)].

Test Escaped Inline Included File: \[!include[refa](~/r/a.md)].
";

            var refa = "This is a **included** token";

            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Inline Included File: \This is a <strong>included</strong> token.</p>
<p>Test Escaped Inline Included File: [!include<a href=""%7E/r/a.md"">refa</a>].</p>
";

            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "r/root.md",
                dependencies: new[] { "a.md" },
                files: new Dictionary<string, string>
                {
                    { "r/a.md", refa },
                });
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestInlineLevelInclusion_CycleInclude()
        {
            var root = @"
# Hello World

Test Inline Included File: [!include[refa](~/r/a.md)].

";

            var refa = "This is a **included** token with [!include[root](~/r/root.md)]";

            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Inline Included File: This is a <strong>included</strong> token with [!include[root](~/r/root.md)].</p>
";
            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "r/root.md",
                files: new Dictionary<string, string>
                {
                    { "r/a.md", refa },
                });
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestInlineLevelInclusion_Block()
        {
            var root = @"
# Hello World

Test Inline Included File: [!include[refa](~/r/a.md)].

";

            var refa = @"## This is a included token

block content in Inline Inclusion.";

            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Inline Included File: ## This is a included tokenblock content in Inline Inclusion..</p>
";

            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "r/root.md",
                dependencies: new[] { "a.md" },
                files: new Dictionary<string, string>
                {
                    { "r/a.md", refa },
                });
        }



        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestBlockLevelInclusion()
        {
            // -r
            //  |- root.md
            //  |- empty.md
            //  |- a
            //  |  |- refc.md
            //  |- b
            //  |  |- linkAndRefRoot.md
            //  |  |- a.md
            //  |  |- img
            //  |  |   |- img.jpg
            //  |- c
            //  |  |- c.md
            //  |- link
            //     |- link2.md
            //     |- md
            //         |- c.md
            var root = @"
[!include[linkAndRefRoot](b/linkAndRefRoot.md)]
[!include[refc](a/refc.md ""This is root"")]
[!include[refc_using_cache](a/refc.md)]
[!include[empty](empty.md)]
[!include[external](http://microsoft.com/a.md)]";

            var linkAndRefRoot = @"
Paragraph1
[link](a.md)
[!include-[link2](../link/link2.md)]
![Image](img/img.jpg)
[!include-[root](../root.md)]";
            var link2 = @"[link](md/c.md)";
            var refc = @"[!include[c](../c/c.md ""This is root"")]";
            var c = @"**Hello**";

            var expected = @"<p>Paragraph1
<a href=""%7E/r/b/a.md"">link</a>
<a href=""%7E/r/link/md/c.md"">link</a>
<img src=""%7E/r/b/img/img.jpg"" alt=""Image"" />
[!include[root](../root.md)]</p>
<p><strong>Hello</strong></p>
<p><strong>Hello</strong></p>
[!include[external](http://microsoft.com/a.md)]";

            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "r/root.md",
                dependencies: new[]
                {
                    "a/refc.md",
                    "b/linkAndRefRoot.md",
                    "c/c.md",
                    "empty.md",
                    "link/link2.md",
                    "root.md",
                },
                files: new Dictionary<string, string>
                {
                    { "r/a/refc.md", refc },
                    { "r/b/linkAndRefRoot.md", linkAndRefRoot },
                    { "r/link/link2.md", link2 },
                    { "r/c/c.md", c },
                    { "r/empty.md", string.Empty },
                });
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestBlockLevelInclusionWithSameFile()
        {
            // -r
            //  |- r.md
            //  |- a
            //  |  |- a.md
            //  |- b
            //  |  |- token.md
            //  |- c
            //     |- d
            //        |- d.md
            //  |- img
            //  |  |- img.jpg
            var r = @"
[!include[](a/a.md)]
[!include[](c/d/d.md)]
";
            var a = @"
[!include[](../b/token.md)]";
            var token = @"
![](../img/img.jpg)
[](#anchor)
[a](../a/a.md)
[](invalid.md)
[d](../c/d/d.md#anchor)
";
            var d = @"
[!include[](../../b/token.md)]";

            var expected = @"<p><img src=""%7E/r/img/img.jpg"" alt="""" />
<a href=""#anchor""></a>
<a href=""%7E/r/a/a.md"">a</a>
<a href=""%7E/r/b/invalid.md""></a>
<a href=""%7E/r/c/d/d.md#anchor"">d</a></p>" + "\n";

            var files = new Dictionary<string, string>
            {
                { "r/r.md", r },
                { "r/a/a.md", a },
                { "r/b/token.md", token },
                { "r/c/d/d.md", d },
            };

            TestUtility.VerifyMarkup(
                a,
                expected,
                filePath: "r/a/a.md",
                dependencies: new[] { "../b/token.md" },
                files: files);

            TestUtility.VerifyMarkup(
                d,
                expected,
                filePath: "r/c/d/d.md",
                dependencies: new[] { "../../b/token.md" },
                files: files);

            TestUtility.VerifyMarkup(
                d,
                $@"{expected}{expected}",
                filePath: "r/r.md",
                dependencies: new[] { "a/a.md", "b/token.md", "c/d/d.md" },
                files: files);
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestBlockLevelInclusionWithWorkingFolder()
        {
            // -r
            //  |- root.md
            //  |- b
            //  |  |- linkAndRefRoot.md
            var root = @"[!include[linkAndRefRoot](~/r/b/linkAndRefRoot.md)]";
            var linkAndRefRoot = @"Paragraph1";
            var expected = @"<p>Paragraph1</p>";

            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "r/root.md",
                files: new Dictionary<string, string>
                {
                    { "r/b/linkAndRefRoot.md", linkAndRefRoot },
                });
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestInclusion_InlineLevel()
        {
            // 1. Prepare data
            var root = @"
Inline [!include[ref1](ref1.md ""This is root"")]
Inline [!include[ref3](ref3.md ""This is root"")]
";

            var ref1 = @"[!include[ref2](ref2.md ""This is root"")]";
            var ref2 = @"## Inline inclusion do not parse header [!include[root](root.md ""This is root"")]";
            var ref3 = @"**Hello**  ";

            var expected = "<p>Inline ## Inline inclusion do not parse header [!include[root](root.md)]\nInline <strong>Hello</strong></p>\n";

            TestUtility.VerifyMarkup(
                root,
                expected,
                dependencies: new[] { "ref1.md", "ref2.md", "ref3.md", "root.md" },
                files: new Dictionary<string, string>
                {
                    { "ref1.md", ref1 },
                    { "ref2.md", ref2 },
                    { "ref3.md", ref3 },
                });
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestBlockInclude_ShouldExcludeBracketInRegex()
        {
            // 1. Prepare data
            var root = @"[!INCLUDE [azure-probe-intro-include](inc1.md)].

[!INCLUDE [azure-arm-classic-important-include](inc2.md)] [Resource Manager model](inc1.md).


[!INCLUDE [azure-ps-prerequisites-include.md](inc3.md)]";

            var expected = @"<p>inc1.</p>
<p>inc2 <a href=""inc1.md"">Resource Manager model</a>.</p>
<p>inc3</p>
";

            TestUtility.VerifyMarkup(
                root,
                expected,
                dependencies: new[] { "inc1.md", "inc2.md", "inc3.md" },
                files: new Dictionary<string, string>
                {
                    { "inc1.md", "inc1" },
                    { "inc2.md", "inc2" },
                    { "inc3.md", "inc3" },
                });
        }

        [Fact]
        [Trait("BugItem", "1101156")]
        [Trait("Related", "Inclusion")]
        public void TestBlockInclude_ImageRelativePath()
        {
            var root = @"
# Hello World

Test Include File

[!include[refa](../../include/a.md)]

";

            var refa = @"
# Hello Include File A

![img](./media/refb.png)
";

            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p><img src=""%7E/r/include/media/refb.png"" alt=""img"" /></p>
";
            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "r/parent_folder/child_folder/root.md",
                dependencies: new[]
                {
                    "../../include/a.md",
                },
                files: new Dictionary<string, string>
                {
                    { "r/include/a.md", refa },
                });
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestBlockInclude_WithYamlHeader()
        {
            var root = @"
# Hello World

Test Include File

[!include[refa](../../include/a.md)]

";

            var refa = @"---
a: b
---
body";

            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<p>body</p>
";
            TestUtility.VerifyMarkup(
                root,
                expected,
                filePath: "r/parent_folder/child_folder/root.md",
                dependencies: new[] { "../../include/a.md" },
                files: new Dictionary<string, string>
                {
                    { "r/include/a.md", refa },
                });
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestInclusionContext_CurrentFile_RootFile()
        {
            var root = "[!include[](embed)]";

            var context = new MarkdownContext(
                readFile: (path, relativeTo, _) =>
                {
                    Assert.Equal("embed", path);
                    Assert.Equal("root", relativeTo);

                    Assert.Equal("root", InclusionContext.RootFile);
                    Assert.Equal("root", InclusionContext.File);

                    return ("embed [content](c.md)", "embed");
                });

            var pipeline = new MarkdownPipelineBuilder().UseDocfxExtensions(context).Build();

            Assert.Null(InclusionContext.RootFile);
            Assert.Null(InclusionContext.File);

            using (InclusionContext.PushFile("root"))
            {
                Assert.Equal("root", InclusionContext.RootFile);
                Assert.Equal("root", InclusionContext.File);

                var result = Markdown.ToHtml(root, pipeline);

                Assert.Equal("<p>embed <a href=\"c.md\">content</a></p>", result.Trim());
                Assert.Equal("root", InclusionContext.RootFile);
                Assert.Equal("root", InclusionContext.File);
            }
            Assert.Null(InclusionContext.RootFile);
            Assert.Null(InclusionContext.File);
        }
    }
}
