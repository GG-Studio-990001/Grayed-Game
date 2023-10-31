using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CheckEditorForBuild
{
    [Test]
    public void CheckForCSFilesInAssetFolder()
    {
        string[] csFiles = System.IO.Directory.GetFiles(Application.dataPath, "*.cs", System.IO.SearchOption.TopDirectoryOnly);
        Assert.AreEqual(csFiles.Length, 0);
    }
}
