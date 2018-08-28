using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteProcessStarter
{
    static class HelperMethods
    {
        public static bool IsTextBoxEmpty(Control txtBox, string displayMsg, bool noTrim = false, int minLength = 1)
        {
            if (txtBox.GetType() != typeof(TextBox) && txtBox.GetType() != typeof(RichTextBox)
                && txtBox.GetType() != typeof(ComboBox))
                throw new ArgumentException("Control does not have a TextBox!");

            int length = noTrim ? txtBox.Text.Length : txtBox.Text.Trim().Length;
            if (length < minLength)
            {
                MessageBox.Show(displayMsg, "Required field!",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBox.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
