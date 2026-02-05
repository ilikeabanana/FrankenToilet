using System.IO;
using System.Reflection;
using FrankenToilet.Core;
using UnityEngine;

namespace FrankenToilet.dolfelive;

public static class BundleLoader
{
    public static AssetBundle bundle;
    
    static BundleLoader()
    {
        Assembly asm = typeof(Plugin).Assembly;
        
        LogHelper.LogDebug(string.Join(", ", asm.GetManifestResourceNames()));  
        
        using Stream resStream = asm.GetManifestResourceStream("FrankenToilet.dolfelive.dolfe.bundle");
        bundle = AssetBundle.LoadFromStream(resStream);
    }
}