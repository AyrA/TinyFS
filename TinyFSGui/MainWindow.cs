using System.Text;
using TinyFSLib;

namespace TinyFSGui
{
    public partial class MainWindow : Form
    {
        private static readonly string tempDir = Path.Combine(Path.GetTempPath(), "TinyFSGui");

        private readonly List<int> initialColumnSizes = new();

        private FS? currentFile;
        private string? currentFileName;
        private bool hasChanges = false;
        private string? encryptPassword;
        private byte[]? encryptKey;

        public MainWindow()
        {
            InitializeComponent();
            Font = new Font(Font.FontFamily, Font.SizeInPoints * 1.5f);
            foreach (var column in LvContents.Columns.OfType<ColumnHeader>())
            {
                initialColumnSizes.Add(column.Width);
            }
        }

        private void FileChange()
        {
            if (currentFile == null)
            {
                EditToolStripMenuItem.Enabled = false;
                CaseInsensitiveModeToolStripMenuItem.Checked = false;
                SetEncryptionToolStripMenuItem.Checked = false;
            }
            else
            {
                EditToolStripMenuItem.Enabled = true;
                CaseInsensitiveModeToolStripMenuItem.Checked = currentFile.IsCaseInsensitive;
                SetEncryptionToolStripMenuItem.Checked = currentFile.IsEncrypted;
            }
            hasChanges = false;
            PopulateList(false);
        }

        private void PopulateList(bool reselect)
        {
            var indexes = LvContents.SelectedIndices.OfType<int>().ToArray();
            LvContents.Items.Clear();
            if (currentFile == null)
            {
                return;
            }
            foreach (var name in currentFile.Names)
            {
                var file = currentFile[name];
                var item = LvContents.Items.Add(file.Name);
                var sizeItem = item.SubItems.Add(Tools.FormatSize(file.Data.Length));
                if (file.Flags.HasFlag(FileFlags.GZip))
                {
                    var compressed = file.Data.Length - Compression.GetCompressionGain(file.Data);
                    sizeItem.Text += string.Format(" ({0} compressed)", Tools.FormatSize(compressed));
                }
                if (file.Flags != FileFlags.None)
                {
                    var flags = new List<string>();
                    foreach (var flag in Enum.GetValues<FileFlags>())
                    {
                        if (flag != FileFlags.None && file.Flags.HasFlag(flag))
                        {
                            flags.Add(flag.ToString());
                        }
                    }
                    item.SubItems.Add(string.Join(", ", flags));
                }
                else
                {
                    item.SubItems.Add("Normal");
                }
                var preview = file.Data
                    .Take(30)
                    .Select(m => m >= 0x20 ? m : (byte)'.')
                    .ToArray();
                item.SubItems.Add(Encoding.UTF8.GetString(preview));
            }
            if (reselect)
            {
                foreach (var i in indexes.Where(m => m < LvContents.Items.Count))
                {
                    LvContents.Items[i].Selected = true;
                }
            }
            LvContents.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //Do not shrink columns beyond their initial size
            for (var i = 0; i < LvContents.Columns.Count; i++)
            {
                var column = LvContents.Columns[i];
                if (column.Width < initialColumnSizes[i])
                {
                    column.Width = initialColumnSizes[i];
                }
            }
        }

        private bool SaveTinyFs()
        {
            if (!hasChanges || currentFile == null)
            {
                return true;
            }

            if (string.IsNullOrEmpty(currentFileName))
            {
                if (SFD.ShowDialog() == DialogResult.OK)
                {
                    currentFileName = SFD.FileName;
                }
                else
                {
                    return false;
                }
            }
            try
            {
                if (currentFile.IsEncrypted)
                {
                    if (encryptKey != null)
                    {
                        currentFile.Write(currentFileName, encryptKey);
                    }
                    else if (!string.IsNullOrEmpty(encryptPassword))
                    {
                        currentFile.Write(currentFileName, encryptPassword);
                    }
                    else
                    {
                        throw new Exception("Encryption parameters have not been set. Use the 'Edit' menu to do so.");
                    }
                }
                else
                {
                    currentFile.Write(currentFileName);
                }
                hasChanges = false;
                return true;
            }
            catch (Exception ex)
            {
                Tools.BoxErr($"Cannot save TinyFS container. {ex.Message}");
            }
            return false;
        }

        private void SaveSelectedFile()
        {
            if (currentFile == null)
            {
                return;
            }
            var indexes = LvContents.SelectedIndices.OfType<int>().ToArray();
            if (indexes.Length == 0)
            {
                Tools.BoxWarn("Please select at least one file");
                return;
            }
            SFD.FileName = string.Empty;
            foreach (var index in indexes)
            {
                var f = LvContents.Items[index];
                var filter = SFD.Filter;
                try
                {
                    var entry = currentFile.GetFile(f.Text);
                    SFD.Filter = $"{f.Text}|{f.Text}|All files|*.*";
                    if (SFD.ShowDialog() == DialogResult.OK)
                    {
                        using var fs = SFD.OpenFile();
                        fs.Write(entry.Data);
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {
                    Tools.BoxErr($"Failed to save {f.Text}. {ex.Message}");
                }
                finally
                {
                    SFD.Filter = filter;
                }
            }
        }

        private void RenameSelectedFile()
        {
            if (currentFile == null)
            {
                return;
            }
            var indexes = LvContents.SelectedIndices.OfType<int>().ToArray();
            if (indexes.Length == 0)
            {
                Tools.BoxWarn("Please select a file");
                return;
            }
            else if (indexes.Length > 1)
            {
                Tools.BoxWarn("Please select exactly one file");
                return;
            }
            var item = LvContents.Items[indexes[0]];
            item.BeginEdit();
        }

        private void DeleteFiles()
        {
            if (currentFile == null)
            {
                return;
            }
            var indexes = LvContents.SelectedIndices.OfType<int>().ToArray();
            if (indexes.Length == 0)
            {
                return;
            }
            if (LvContents.Items.Count == 1)
            {
                Tools.BoxErr("You cannot delete the last file of a container. Delete the container itself instead");
                return;
            }
            var btn = Tools.BoxWarn($"Delete {indexes.Length} files", buttons: MessageBoxButtons.YesNo);
            if (btn == DialogResult.Yes)
            {
                var names = indexes.Select(m => LvContents.Items[m].Text).ToArray();
                foreach (var name in names)
                {
                    hasChanges = true;
                    currentFile.DeleteFile(name);
                }
                PopulateList(false);
            }
        }

        private bool CopySelectedFiles(bool asContent)
        {
            if (currentFile == null)
            {
                return false;
            }
            var indexes = LvContents.SelectedIndices.OfType<int>().ToArray();
            if (indexes.Length == 0)
            {
                Tools.BoxWarn("Please select at least one file first");
                return false;
            }
            if (asContent)
            {
                if (indexes.Length > 1)
                {
                    Tools.BoxWarn("Copying file contents can be done with only one file at a time");
                    return false;
                }
                var f = currentFile.GetFile(LvContents.Items[indexes[0]].Text);
                if (f.Data.Contains((byte)0))
                {
                    var result = Tools.BoxWarn("Cannot copy file contents. Due to a restriction in the Windows clipboard you cannot copy text that contains nullbytes. Want to copy it as hexadecimal instead? Selecting 'No' will copy it with nullbytes replaced with spaces.", buttons: MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes)
                    {
                        var text = string.Join(" ", f.Data.Select(m => m.ToString("X2")));
                        Clipboard.Clear();
                        Clipboard.SetText(text);
                        return true;
                    }
                    else if (result == DialogResult.No)
                    {
                        var text = Encoding.UTF8.GetString(f.Data.Select(m => m == 0 ? (byte)0x20 : m).ToArray());
                        Clipboard.Clear();
                        Clipboard.SetText(text);
                        return true;
                    }
                    return false;
                }
                else
                {
                    Clipboard.Clear();
                    Clipboard.SetText(Encoding.UTF8.GetString(f.Data));
                    return true;
                }
            }
            else
            {
                var tinyFiles = indexes.Select(m => currentFile.GetFile(LvContents.Items[m].Text)).ToArray();
                var files = PopulateTempDir(tinyFiles);
                var names = new System.Collections.Specialized.StringCollection();
                names.AddRange(files);
                Clipboard.Clear();
                Clipboard.SetFileDropList(names);
                return true;
            }
        }

        private void SetCompressionOfSelectedFiles(bool compress)
        {
            if (currentFile == null)
            {
                return;
            }
            var indexes = LvContents.SelectedIndices.OfType<int>().ToArray();
            if (indexes.Length == 0)
            {
                return;
            }
            var names = indexes.Select(m => LvContents.Items[m].Text).ToArray();
            var change = false;
            foreach (var name in names)
            {
                var f = currentFile.GetFile(name);
                if (compress)
                {
                    change |= !f.IsCompressed;
                    f.IsCompressed = true;
                }
                else
                {
                    change |= f.IsCompressed;
                    f.IsCompressed = false;
                }
            }
            if (change)
            {
                hasChanges = true;
                PopulateList(true);
            }
        }

        private bool AddFile(string fullName)
        {
            if (currentFile == null) { return false; }
            var name = Path.GetFileName(fullName);
            if (currentFile.HasFile(name))
            {
                Tools.BoxWarn($"A file with the name '{name}' already exists and will not be added");
                return false;
            }
            try
            {
                using var fs = File.OpenRead(fullName);
                if (fs.Length > FileData.MaxDataSize)
                {
                    throw new Exception($"File too big. Can be at most {FileData.MaxDataSize} bytes");
                }
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                currentFile.SetFile(name, data);
                hasChanges = true;
                return true;
            }
            catch (Exception ex)
            {
                Tools.BoxErr($"File '{name}' could not be added due to an error: {ex.Message}");
            }
            return false;
        }

        private void AddFile()
        {
            var added = false;
            var types = OFD.Filter;
            try
            {
                OFD.Filter = "All files|*.*";
                OFD.Multiselect = true;
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    foreach (var f in OFD.FileNames)
                    {
                        added |= AddFile(f);
                    }
                }
            }
            finally
            {
                OFD.Filter = types;
                OFD.Multiselect = false;
                if (added)
                {
                    PopulateList(false);
                }
            }
        }

        private string[] PopulateTempDir(FileData[] tinyData)
        {
            if (tinyData is null)
            {
                throw new ArgumentNullException(nameof(tinyData));
            }
            if (currentFile == null)
            {
                throw new InvalidOperationException();
            }

            var names = new List<string>();
            try
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
            catch
            {
                //NOOP
            }
            Directory.CreateDirectory(tempDir);
            foreach (var f in tinyData)
            {
                var name = f.Name;
                //Make name valid
                foreach (var invalids in Path.GetInvalidFileNameChars())
                {
                    name = name.Replace(invalids, '_');
                }
                var fullName = Tools.GetUniqueName(Path.Combine(tempDir, name));
                try
                {
                    File.WriteAllBytes(fullName, f.Data);
                    names.Add(fullName);
                }
                catch (Exception ex)
                {
                    Tools.BoxErr($"Cannot copy '{name}' to temp directory. {ex.Message}");
                }
            }
            return names.ToArray();
        }

        #region Event handler

        private void FileDragEnter(object sender, DragEventArgs e)
        {

            if (e.Data == null || currentFile == null)
            {
                e.Effect = DragDropEffects.None;
            }
            else if (e.Data.GetDataPresent("FileDrop"))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void FileDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data == null || currentFile == null)
            {
                e.Effect = DragDropEffects.None;
            }
            else if (e.Data.GetDataPresent("FileDrop"))
            {
                e.Effect = DragDropEffects.Copy;
                var data = (string[])e.Data.GetData("FileDrop", true);
                bool added = false;
                foreach (var filename in data)
                {
                    added |= AddFile(filename);
                }
                if (added)
                {
                    PopulateList(false);
                }
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveTinyFs())
            {
                currentFile = new FS();
                currentFileName = null;
                FileChange();
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hasChanges && currentFile != null)
            {
                var result = Tools.BoxWarn("You have unsaved changes. Save them before opening another file?", buttons: MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.Yes:
                        if (!SaveTinyFs())
                        {
                            return;
                        }
                        break;
                    case DialogResult.No:
                        break;
                }
            }
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                var isValid = false;
                var flags = FsFlags.None;
                try
                {
                    flags = FS.GetInfo(OFD.FileName);
                    isValid = true;
                }
                catch
                {
                    Tools.BoxErr("This is not a valid TinyFS file");
                }
                if (isValid)
                {
                    if (flags.HasFlag(FsFlags.Encrypted))
                    {
                        using var passwordForm = new PasswordEntry();
                        if (passwordForm.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                if (passwordForm.IsPasswordMode)
                                {
                                    currentFile = new FS(OFD.FileName, passwordForm.Password!);
                                }
                                else
                                {
                                    currentFile = new FS(OFD.FileName, passwordForm.Key);
                                }
                                currentFileName = OFD.FileName;
                                FileChange();
                            }
                            catch
                            {
                                Tools.BoxErr($"Cannot open encrypted file.\r\nDouble check that you're using the correct mode (password or key) and that you typed the value correctly.");
                            }
                        }
                    }
                    else
                    {
                        currentFile = new FS(OFD.FileName);
                        currentFileName = OFD.FileName;
                        FileChange();
                    }
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
            {
                return;
            }
            if (currentFile.Names.Length == 0)
            {
                Tools.BoxWarn("Cannot save an empty container");
                return;
            }
            SaveTinyFs();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
            {
                return;
            }
            if (currentFile.Names.Length == 0)
            {
                Tools.BoxWarn("Cannot save an empty container");
                return;
            }
            var name = currentFileName;
            currentFileName = null;
            try
            {
                if (!SaveTinyFs())
                {
                    throw null!;
                }
            }
            catch
            {
                currentFileName = name;
            }
        }

        private void LvContents_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentFile == null)
            {
                return;
            }
            switch (e.KeyData & ~Keys.Modifiers)
            {
                case Keys.Enter:
                    e.Handled = e.SuppressKeyPress = true;
                    SaveSelectedFile();
                    break;
                case Keys.Insert:
                    e.Handled = e.SuppressKeyPress = true;
                    AddFile();
                    break;
                case Keys.Delete:
                    e.Handled = e.SuppressKeyPress = true;
                    DeleteFiles();
                    break;
                case Keys.F2:
                    e.Handled = e.SuppressKeyPress = true;
                    RenameSelectedFile();
                    break;
                case Keys.C:
                    e.Handled = e.SuppressKeyPress = true;
                    if (e.Control)
                    {
                        CopySelectedFiles(e.Shift);
                    }
                    else
                    {
                        SetCompressionOfSelectedFiles(!EnableCompressionToolStripMenuItem.Checked);
                    }
                    break;
                default:
                    break;
            }
        }

        private void LvContents_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (currentFile == null)
            {
                e.CancelEdit = true;
                return;
            }
            var item = LvContents.Items[e.Item];
            if (string.IsNullOrEmpty(e.Label))
            {
                e.CancelEdit = true;
                return;
            }
            if (e.Label == item.Text)
            {
                e.CancelEdit = true;
                return;
            }
            if (currentFile.HasFile(e.Label))
            {
                e.CancelEdit = true;
                Tools.BoxErr("A file with this name already exists in this TinyFS container");
                return;
            }
            else
            {
                currentFile.GetFile(item.Text).Name = e.Label;
                hasChanges = true;
            }
        }

        private void LvContents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentFile == null)
            {
                EnableCompressionToolStripMenuItem.Checked = false;
                return;
            }
            var indexes = LvContents.SelectedIndices.OfType<int>().ToArray();
            if (indexes.Length == 0)
            {
                EnableCompressionToolStripMenuItem.Checked = false;
                return;
            }
            var items = indexes.Select(m => LvContents.Items[m]).ToArray();
            if (indexes.Length == 1)
            {
                EnableCompressionToolStripMenuItem.Checked = currentFile.GetFile(items[0].Text).Flags.HasFlag(FileFlags.GZip);
            }
            else
            {
                var flags = items.Select(m => currentFile.GetFile(m.Text).Flags.HasFlag(FileFlags.GZip)).ToArray();
                EnableCompressionToolStripMenuItem.Checked = flags.Contains(true);
            }
        }

        private void LvContents_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (currentFile == null)
            {
                return;
            }
            var items = LvContents.SelectedItems
                .OfType<ListViewItem>()
                .Select(m => currentFile.GetFile(m.Text))
                .ToArray();
            if (items.Length == 0)
            {
                return;
            }
            var names = PopulateTempDir(items);
            var data = new DataObject(DataFormats.FileDrop, names);
            DoDragDrop(data, DragDropEffects.Move);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hasChanges)
            {
                var result = Tools.BoxWarn("You have unsaved changes. Save them before exiting?", buttons: MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    case DialogResult.Yes:
                        if (!SaveTinyFs())
                        {
                            e.Cancel = true;
                            return;
                        }
                        break;
                    case DialogResult.No:
                        break;
                }
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
            catch (Exception ex)
            {
                Tools.BoxErr($"Failed to delete temporary files in '{tempDir}'. {ex.Message}");
            }
        }

        private void AddFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFile();
        }

        private void SetEncryptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
            {
                return;
            }
            if (currentFile.IsEncrypted)
            {
                SetEncryptionToolStripMenuItem.Checked = currentFile.IsEncrypted = false;
                encryptKey = null;
                encryptPassword = null;
                hasChanges = true;
                Tools.BoxInfo("Encryption disabled. Save the file to persist your changes.");
            }
            else
            {
                using var pw1 = new PasswordEntry();
                if (pw1.ShowDialog() == DialogResult.OK)
                {
                    Tools.BoxInfo($"Please confirm your {(pw1.IsPasswordMode ? "password" : "key")} by entering it again.");
                    using var pw2 = new PasswordEntry();
                    if (pw2.ShowDialog() == DialogResult.OK)
                    {
                        if (pw1.IsPasswordMode != pw2.IsPasswordMode)
                        {
                            Tools.BoxErr("You cannot mix the password and key type");
                            return;
                        }
                        if (pw1.IsPasswordMode)
                        {
                            if (pw1.Password != pw2.Password)
                            {
                                Tools.BoxErr("The passwords do not match");
                                return;
                            }
                            else
                            {
                                encryptPassword = pw1.Password;
                                SetEncryptionToolStripMenuItem.Checked = currentFile.IsEncrypted = true;
                            }
                        }
                        else if (pw1.Key != null && pw2.Key != null)
                        {
                            if (!pw1.Key.SequenceEqual(pw2.Key))
                            {
                                Tools.BoxErr("The keys do not match");
                                return;
                            }
                            else
                            {
                                encryptKey = pw1.Key;
                                SetEncryptionToolStripMenuItem.Checked = currentFile.IsEncrypted = true;
                            }
                        }
                        else
                        {
                            Tools.BoxErr("Error setting password or key. Please try again");
                            return;
                        }
                        hasChanges = true;
                        Tools.BoxInfo("Encryption enabled");
                    }
                }
            }
        }

        private void OptimizeCompressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile == null || currentFile.Names.Length == 0)
            {
                return;
            }
            var hasCompressionChange = false;
            foreach (var name in currentFile.Names)
            {
                var f = currentFile.GetFile(name);
                if (f.IsCompressionRecommended())
                {
                    hasCompressionChange |= !f.Flags.HasFlag(FileFlags.GZip);
                    f.Flags |= FileFlags.GZip;
                }
                else
                {
                    hasCompressionChange |= f.Flags.HasFlag(FileFlags.GZip);
                    f.Flags &= ~FileFlags.GZip;
                }
            }
            if (hasCompressionChange)
            {
                hasChanges = true;
                PopulateList(true);
                Tools.BoxInfo("Compression settings of all files are now optimized");
            }
            else
            {
                Tools.BoxInfo("Compression settings of all files are already optimized");
            }
        }

        private void CaseInsensitiveModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
            {
                return;
            }
            try
            {
                currentFile.IsCaseInsensitive = !currentFile.IsCaseInsensitive;
                CaseInsensitiveModeToolStripMenuItem.Checked = currentFile.IsCaseInsensitive;
                hasChanges = true;
            }
            catch (Exception ex)
            {
                Tools.BoxErr(ex.Message);
            }
        }

        private void SaveFileToDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSelectedFile();
        }

        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameSelectedFile();
        }

        private void EnableCompressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressionOfSelectedFiles(!EnableCompressionToolStripMenuItem.Checked);
        }

        private void DeleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFiles();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dlg = new AboutWindow();
            dlg.ShowDialog();
        }

        #endregion
    }
}