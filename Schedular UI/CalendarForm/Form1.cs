using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalendarForm
{
    public partial class CalendarForm : Form
    {
        private List<Schedule> schedules;


        public CalendarForm()
        {
            InitializeComponent();

            schedules = new List<Schedule>();
        }

        private void CalendarForm_Load(object sender, EventArgs e)
        {

        }
    }
}
