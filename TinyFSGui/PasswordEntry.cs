namespace TinyFSGui
{
    public partial class PasswordEntry : Form
    {
        public string? Password { get; private set; }
        public byte[]? Key { get; private set; }
        public bool IsPasswordMode { get; private set; }

        public PasswordEntry()
        {
            InitializeComponent();
            Font = new Font(Font.FontFamily, Font.SizeInPoints * 1.5f);
            TbPassword.Focus();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (RbPassword.Checked)
            {
                IsPasswordMode = true;
                if (string.IsNullOrEmpty(TbPassword.Text))
                {
                    MessageBox.Show("Please enter a password", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Password = TbPassword.Text;
                DialogResult = DialogResult.OK;
            }
            else
            {
                IsPasswordMode = false;
                if (string.IsNullOrEmpty(TbKey.Text))
                {
                    MessageBox.Show("Please enter a key", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    var key = Convert.FromBase64String(TbKey.Text);
                    if (key.Length != 32)
                    {
                        throw new InvalidDataException("Invalid key");
                    }
                    Key = key;
                    DialogResult = DialogResult.OK;
                }
                catch (FormatException)
                {
                    MessageBox.Show("Key is not valid Base64", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (InvalidDataException)
                {
                    MessageBox.Show("Key does not decode to a 32 byte AES key", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RbPassword_CheckedChanged(object sender, EventArgs e)
        {
            TbPassword.Enabled = RbPassword.Checked;
            TbKey.Enabled = !RbPassword.Checked;
            if (TbPassword.Enabled)
            {
                TbPassword.Focus();
            }
            else
            {
                TbKey.Focus();
            }
        }
    }
}
