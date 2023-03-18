namespace TinyFSGui
{
    public static class Tools
    {
        private static readonly string[] sizes = "Bytes,KB,MB,GB,TB,EB".Split(',');

        public static string FormatSize(double size)
        {
            int index = 0;
            while (size >= 1000 && index < sizes.Length - 1)
            {
                ++index;
                size /= 1000.0;
            }
            if (index == 0)
            {
                return $"{size} {sizes[index]}";
            }
            return $"{size:0.00} {sizes[index]}";
        }

        public static DialogResult BoxInfo(string text, string? title = null, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return Box(text, title, buttons, MessageBoxIcon.Information);
        }

        public static DialogResult BoxWarn(string text, string? title = null, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return Box(text, title, buttons, MessageBoxIcon.Warning);
        }

        public static DialogResult BoxErr(string text, string? title = null, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return Box(text, title, buttons, MessageBoxIcon.Error);
        }

        private static DialogResult Box(string text, string? title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (title == null)
            {
                foreach (var form in Application.OpenForms)
                {
                    var f = (Form)form;
                    if (f.Focused && f.Visible)
                    {
                        title = f.Text; break;
                    }
                }
            }
            return MessageBox.Show(text, title ?? icon.ToString(), buttons, icon);
        }

        public static string GetUniqueName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException($"'{nameof(fullName)}' cannot be null or empty.", nameof(fullName));
            }
            fullName = Path.GetFullPath(fullName);
            if (!File.Exists(fullName))
            {
                return fullName;
            }
            var dir = Path.GetDirectoryName(fullName) ?? Environment.CurrentDirectory;
            var name = Path.GetFileNameWithoutExtension(fullName);
            var ext = Path.GetExtension(fullName);
            int i = 1;
            string finalName;
            do
            {
                finalName = Path.Combine(dir, $"{name} ({i++}){ext}");
            } while (File.Exists(finalName));
            return finalName;
        }
    }
}
