using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Lib_K_Relay;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.DataObjects;
using Lib_K_Relay.Networking.Packets.Server;
using Lib_K_Relay.Utilities;
using Lib_K_Relay.GameData;
using Lib_K_Relay.GameData.DataStructures;

namespace DyeSkinFaker
{
	public partial class FrmConfig : Form
	{
		public Client _c;
		public FrmConfig(Client c)
		{
			InitializeComponent();
			_c = c;
		}

		private void Config_Load(object sender, EventArgs e)
		{
			Dictionary<int, string> LargeDyesData = new Dictionary<int,string>();



			comboBox1.DataSource = new BindingSource(DyeSkinFaker.LargeDyes.OrderBy(pair => pair.Value), null);
			comboBox1.DisplayMember = "Value";
			comboBox1.ValueMember = "Key";
			comboBox1.SelectedValue = Config.Default.LargeDye;

			comboBox2.DataSource = new BindingSource(DyeSkinFaker.SmallDyes.OrderBy(pair => pair.Value), null);
			comboBox2.DisplayMember = "Value";
			comboBox2.ValueMember = "Key";
			comboBox2.SelectedValue = Config.Default.SmallDye;

			comboBox3.DataSource = new BindingSource(DyeSkinFaker.Skins.OrderBy(pair => pair.Value), null);
			comboBox3.DisplayMember = "Value";
			comboBox3.ValueMember = "Key";
			comboBox3.SelectedValue = Config.Default.Skin;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (comboBox1.SelectedValue != null && comboBox1.SelectedText != null)
				Config.Default.LargeDye = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;
			else
				Config.Default.LargeDye = 0;

			if (comboBox2.SelectedValue != null && comboBox2.SelectedText != null)
				Config.Default.SmallDye = ((KeyValuePair<int, string>)comboBox2.SelectedItem).Key;
			else
				Config.Default.SmallDye = 0;

			if (comboBox3.SelectedValue != null && comboBox3.SelectedText != null)
				Config.Default.Skin = ((KeyValuePair<int, string>)comboBox3.SelectedItem).Key;
			else
				Config.Default.Skin = 0;

		//	Config.Default.LargeDye = large;
		//	Config.Default.SmallDye = small;
		//	Config.Default.Skin = skin;

			Config.Default.Save();

			//DyeSkinFaker.OnChange(_c);

			this.Close();
		}
	}
}
