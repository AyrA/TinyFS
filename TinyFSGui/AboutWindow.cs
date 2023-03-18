using System.Reflection;

namespace TinyFSGui
{
    partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();
            Text = string.Format("About {0}", AssemblyTitle);
            LabelProductName.Text = AssemblyProduct;
            LabelVersion.Text = string.Format("Version {0}", AssemblyVersion);
            LabelCopyright.Text = AssemblyCopyright;
            LabelCompanyName.Text = AssemblyCompany;
            TextBoxDescription.Text = AssemblyDescription;
        }

        #region Assembly Attribute Accessors

        private static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            }
        }

        private static string AssemblyVersion
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version("0.0.0.0");
                return version.ToString();
            }
        }

        private static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "<NONE:AssemblyProduct>";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        private static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "<NONE:AssemblyCopyright>";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        private static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "<NONE:AssemblyCompany>";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        private static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "<NONE:AssemblyDescription>";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        #endregion
    }
}
