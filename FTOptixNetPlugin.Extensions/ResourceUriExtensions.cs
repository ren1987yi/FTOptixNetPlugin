#define FTOptix_V1_6
using FTOptix.Core;
using System.Text.RegularExpressions;

namespace FTOptixNetPlugin.Extensions
{
    public static class ResourceUriExtensions
    {
        public static ResourceUri ToResourceUri(this string value)
        {

            const string namespace_string = "ns=";
            const string project_shortcut = "%PROJECTDIR%";
            const string application_shortcut = "%APPLICATIONDIR%";
            value = value.Trim();
            if (value.StartsWith(namespace_string))
            {
                var x = value.IndexOf(';', namespace_string.Length);
                if (x == -1)
                {
                    
                }
                else
                {
                    value = value.Substring(x + 1);
                }

            }


            var pattern_folder = @"^%(.*?)%";
            var match = Regex.Match(value, pattern_folder);
            if (match.Success)
            {
                switch (match.Value.ToUpper())
                {
                    case project_shortcut:
                        {

                            var subpath = value.Substring(project_shortcut.Length);

                            if (subpath.StartsWith("/") || subpath.StartsWith("\\"))
                            {
                                subpath = subpath.Substring(1);
                            }
                            return ResourceUri.FromProjectRelativePath(subpath);
                        }

                        break;
                    case application_shortcut:
                        {

                            var subpath = value.Substring(application_shortcut.Length);
                            if (subpath.StartsWith("/") || subpath.StartsWith("\\"))
                            {
                                subpath = subpath.Substring(1);
                            }
                            return ResourceUri.FromApplicationRelativePath(subpath);
                        }
                        break;
                    default:
                        var pattern_number = @"^%USB(\d+)%";
                        var usbMatch = Regex.Match(match.Value, pattern_number);
                        if (usbMatch.Success)
                        {

                            var g = usbMatch.Groups.GetValueOrDefault("1");
                            if (g != null)
                            {
                                var usb_number = uint.Parse(g.Value);

                                var subpath = value.Substring(match.Value.Length);
                                if (subpath.StartsWith("/") || subpath.StartsWith("\\"))
                                {
                                    subpath = subpath.Substring(1);
                                }


                                return ResourceUri.FromUSBRelativePath(usb_number, subpath);
                            }

                        }

                        return null;

                        break;
                }

            }
            else
            {
                return ResourceUri.FromAbsoluteFilePath(value);

            }

        }
    }
}
