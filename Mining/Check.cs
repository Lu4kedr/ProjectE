using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mining
{
	public class Check
	{

		public event EventHandler OnAfk;
		public event EventHandler OnNoOre;
		public event EventHandler OnSkillDelay;
		public event EventHandler OnMaxedWeight;
		public event EventHandler<OnOreAddedArgs> OnOreAdded;


		BackgroundWorker bw;
		readonly string[] calls = { "You put ", "Nevykopala jsi nic ","Odstranila jsi zaval!", "Odstranil jsi zaval!","Nepovedlo se ti odstranit zaval.", "Jeste nemuzes pouzit skill",                  // 0-4, 5
								   "There is no ore ", "too far", "Try mining","Tam nedosahnes.","Jsi prilis daleko.",                    // 6-10
									"afk", "AFK", "kontrola", "GM", "gm", "Je spatne videt." };     // 11-16





		private void Bw_DoWork(object sender, DoWorkEventArgs e)
		{
			while (!bw.CancellationPending)
			{
				Checking();

				Thread.Sleep(200);
			}
		}

		private void Checking()
		{
			EventHandler temp;
			// Check AFK
			if (Journal.Contains(true, calls[15], calls[11], calls[12], calls[13], calls[14]))
			{
				temp = OnAfk;
				if (temp != null)
				{
					foreach (EventHandler ev in temp.GetInvocationList())
					{
						ev.BeginInvoke(this, null, null, null);
					}
				}

			}
			//// Check CK/Monster
			//foreach (var ch in World.Characters)
			//{
			//	if (ch.Notoriety > Notoriety.Criminal && ch.Notoriety < Notoriety.Invulnerable) 
			//	{
			//		if (Humanoid.Any(x => x == ch.Model))
			//		{
			//			var tmp = EnemyAppeared;
			//			if (tmp != null)
			//			{
			//				foreach (EventHandler<EnemyAppearedArgs> ev in tmp.GetInvocationList())
			//				{
			//					ev.BeginInvoke(this, new EnemyAppearedArgs() { Enemy = ch, CK = true, TopMonster = false }, null, null);
			//				}
			//			}
			//		}
			//		else
			//		{
			//			ch.Click();
			//			UO.Wait(200);
			//			if (TopMonster.Any(x => x == ch.Name.ToLowerInvariant()))
			//			{
			//				var tmp = EnemyAppeared;
			//				if (tmp != null)
			//				{
			//					foreach (EventHandler<EnemyAppearedArgs> ev in tmp.GetInvocationList())
			//					{
			//						ev.BeginInvoke(this, new EnemyAppearedArgs() { Enemy = ch, CK = false, TopMonster = true }, null, null);
			//					}
			//				}
			//			}
			//			else
			//			{
			//				var temp1 = EnemyAppeared;
			//				if (temp1 != null)
			//				{
			//					foreach (EventHandler<EnemyAppearedArgs> ev in temp1.GetInvocationList())
			//					{
			//						ev.BeginInvoke(this, new EnemyAppearedArgs() { Enemy = ch, CK = false, TopMonster = false }, null, null);
			//					}
			//				}
			//			}
			//		}



			//	}
			//}
			// Check Light
			if (Journal.Contains(true, calls[15]) || World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A18))
			{
				World.Player.Layers[Layer.LeftHand].Use();
				UO.Wait(200);
				if (World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A15)) World.Player.Backpack.AllItems.FindType(0x0A18).Use();

			}

			// No Ore
			if (Journal.Contains(true, calls[6], calls[7], calls[8], calls[9], calls[10]))
			{
				temp = OnNoOre;
				if (temp != null)
				{
					foreach (EventHandler ev in temp.GetInvocationList())
					{
						ev.BeginInvoke(this, null, null, null);
					}
				}
			}

			// Skill delay
			if (Journal.Contains(true, calls[5]))
			{
				temp = OnSkillDelay;
				if (temp != null)
				{
					foreach (EventHandler ev in temp.GetInvocationList())
					{
						ev.BeginInvoke(this, null, null, null);
					}
				}
			}

			// Check Weight
			if (World.Player.Weight > (World.Player.Strenght * 4 + 15))
			{
				temp = OnMaxedWeight;
				if (temp != null)
				{
					foreach (EventHandler ev in temp.GetInvocationList())
					{
						ev.BeginInvoke(this, null, null, null);
					}
				}
			}

			// Incoming Ore  
			if (Journal.Contains(true, calls[0]))//, calls[1], calls[2], calls[3], calls[4]))
			{
				string type = "_";


				if (Journal.Contains(true, " Copper "))
				{
					type = "Copper";

				}
				else
				if (Journal.Contains(true, " Iron "))
				{
					type = "Iron";
				}
				else
				if (Journal.Contains(true, " Kremicity "))
				{
					type = "Kremicity";
				}
				else
				if (Journal.Contains(true, " Verite "))
				{
					type = "Verite";

				}
				else
				if (Journal.Contains(true, " Valorite "))
				{
					type = "Valorite";
				}
				else
				if (Journal.Contains(true, " Obsidian "))
				{
					type = "Obsidian";
				}
				else
				if (Journal.Contains(true, " Adamantium "))
				{
					type = "Adamantium";
				}
				var temp2 = OnOreAdded;
				if (temp2 != null && type!="_")
				{
					foreach (EventHandler<OnOreAddedArgs> ev in temp2.GetInvocationList())
					{
						ev.BeginInvoke(this, new OnOreAddedArgs() { Type = type }, null, null);
					}
				}

			}
			Journal.Clear();
			Journal.ClearAll();

		}

		internal void Stop()
		{
			bw.CancelAsync();
			bw.DoWork -= Bw_DoWork;
		}

		public void Start()
		{
			bw = new BackgroundWorker();
			bw.WorkerSupportsCancellation = true;
			bw.DoWork += Bw_DoWork;
			bw.RunWorkerAsync();
		}
	}
}
