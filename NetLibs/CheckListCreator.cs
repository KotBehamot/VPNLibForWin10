using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsLib
{
 
       public class CheckListCreator
        {

            public Color SelectedCheckboxColor { get; set; }
            public Color UnselectedCheckboxColor { get; set; }
            string[] listOfOption = null;
            string CheckboxName = null;
            public List<string> checkboxNameList = new List<string>(15);
            public CHeckBoxListPosition listPosition = new CHeckBoxListPosition() { x = 50, y = 150, interlinia = 20 };
            public void CreateCheckBoxList(string[] listOfOption, Form currentForm)
            {
                //  this.listOfOption = String.Format("checkbox{0}", listOfOption);
                int i = 0;
                CheckboxName = RandomString(10);
                foreach (var item in listOfOption)
                {
                    var checkbox = new CheckBox();
                    checkboxNameList.Add(String.Format(CheckboxName + "{0}", item));
                    checkbox.Name = checkboxNameList.Last();
                    //Set the location.  Notice the y-offset grows 20px with each i
                    checkbox.Location = new Point(listPosition.x, listPosition.y + (i * listPosition.interlinia));
                    checkbox.Text = item;
                    checkbox.Checked = false;
                    //Assign them all the same event handler
                    checkbox.CheckedChanged += new EventHandler(Cb_Changed);
                    //other checkbox considerations like checkbox.BringToFront()
                    //very important, you must add the checkbox the the form's controls 
                    currentForm.Controls.Add(checkbox);
                    i++;
                }
            }
            public List<Control> GetCheckBoxResultList(Form form)
            {
                List<Control> resultList = new List<Control>(15);
                foreach (var item in checkboxNameList)
                {
                    resultList.AddRange(form.Controls.Find(item, true));
                }
                return resultList;
            }
            private void InvalidateCheckboxAppearance(CheckBox cb)
            {
                // cb.Text = cb.Checked ? "B" : "1";
                cb.BackColor = cb.Checked ? SelectedCheckboxColor : UnselectedCheckboxColor;
            }
            private void Cb_Changed(object sender, EventArgs args)
            {
                InvalidateCheckboxAppearance(sender as CheckBox);
            }
            static string RandomString(int length)
            {
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                 .Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }
    public struct CHeckBoxListPosition
        {
            public int x;
            public int y;
            public int interlinia;
        }
    }


