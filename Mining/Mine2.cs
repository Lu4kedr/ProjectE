using Mining.PathFinding;
using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using Project_E;
using Project_E.Lib.Runes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace Mining
{
	public delegate void Work();
	[RuntimeObject]
	public class Mine2
	{
		private string[] calls = { "You put ", "Nevykopala jsi nic ","Odstranila jsi zaval!", "Odstranil jsi zaval!","Nepovedlo se ti odstranit zaval.", "Jeste nemuzes pouzit skill",                  // 0-4, 5
								   " There is no ore", "too far", "Try mining","Tam nedosahnes.",                    // 6-9
									"afk", "AFK", "kontrola", "GM", "gm", "Je spatne videt." };     // 10-15


		private string path = Core.Directory + @"\Profiles\XML\";
		private string AlarmPath = Core.Directory + @"\afk.wav";// "C:\\afk.wav";
		private Graphic Ore = 0x19B7;
		private Dictionary<string, UOColor> Material = new Dictionary<string, UOColor>() { { "Copper", 0x099A }, { "Iron", 0x0763 }, { "Kremicity", 0x0481 }, { "Verite", 0x097F }, { "Valorite", 0x0985 }, { "Obsidian", 0x09BD }, { "Adamantium", 0x0026 } };
		private int[] MaterialsCount = { 0, 0, 0, 0, 0, 0, 0 };
		private DateTime StartMine = DateTime.Now;
		private Movement mov;
		private SearchParameters searchParams;
		public Settings Settings;
		private System.Timers.Timer t;
		private PathFinder pathfinder;
		private UOItem pickAxe;
		private UOItem mace;
		private UOItem Weapon;

		public uint PickAxe
		{
			get
			{
				uint tmp;
				if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E85) | World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E86))
					tmp = World.Player.Layers[Layer.RightHand];

				else tmp = World.Player.Backpack.AllItems.FindType(0x0E85).Exist
						? World.Player.Backpack.AllItems.FindType(0x0E85).Serial : World.Player.Backpack.AllItems.FindType(0x0E86).Exist
						? World.Player.Backpack.AllItems.FindType(0x0E86).Serial : 0;
				if (tmp != 0)
					pickAxe = new UOItem(tmp);
				return tmp;
			}

			set
			{
				pickAxe = new UOItem(value);
			}
		}

		public uint Mace
		{
			get
			{
				uint tmp;
				if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x1406) | World.Player.Layers[Layer.RightHand].Graphic.Equals(0x1407))
					tmp = World.Player.Layers[Layer.RightHand];
				else tmp = World.Player.Backpack.AllItems.FindType(0x1406).Exist
				? World.Player.Backpack.AllItems.FindType(0x1406).Serial
				: World.Player.Backpack.AllItems.FindType(0x1407).Exist
				? World.Player.Backpack.AllItems.FindType(0x1407).Serial : 0;
				if (tmp != 0)
					mace = new UOItem(tmp);
				return tmp;
			}

			set
			{
				mace = new UOItem(value);
			}
		}

		public Mine2()
		{
			t = new System.Timers.Timer(200);
			t.Elapsed += T_Elapsed;
			t.Start();
		}

		private void T_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (World.Player.Name != null && GUI.Mining.LastInstance!=null)
			{
				t.Elapsed-= T_Elapsed;
				t.Stop();
				Initialize();

			}
		}

		public void Initialize()
		{
           // MessageBox.Show("Dd");
			mov = new Movement();
			searchParams = new SearchParameters(new Point(), new Point(), null);
			XmlSerializeHelper<Settings>.Load("Mining", out Settings, false);
			if(Settings==null)
			{
				Settings = new Settings();
			}
			Mining.GUI.Mining.LastInstance.OnCHanged += LastInstance_OnCHanged;


            Mining.GUI.Mining.LastInstance.Invoke(new MethodInvoker(delegate
            {
                GUI.Mining.LastInstance.Mines.Items.Clear();
                foreach (var it in Settings.Maps)
                {
                    GUI.Mining.LastInstance.Mines.Items.Add(it.Name);
                }
            }));

        }

        private void LastInstance_OnCHanged(object sender, EventArgs e)
		{
			if (sender is Button)
			{

				XmlSerializeHelper<Settings>.Save("Mining", Settings, false);
				switch (((Button)sender).Name)
				{

					case "btn_ResourceBox":
						UO.PrintInformation("Zamer Baglik/Truhlicku.. ze ktere se budou brat Krumpace, Mace, Recally");
						Settings.ResourceBox = new UOItem(UIManager.TargetObject());
						break;
					case "btn_OreBox":
						UO.PrintInformation("Zamer Baglik/Truhlicku.. do ktere se bude vykladat natezene ore");
						Settings.OreBox = new UOItem(UIManager.TargetObject());
						break;
					case "btn_GemBox":
						UO.PrintInformation("Zamer Baglik/Truhlicku.. do ktere se budou vykladat natezene drahokamy a Mramor");
						Settings.GemBox = new UOItem(UIManager.TargetObject());
						break;
					case "btn_BankPos":
						UO.PrintInformation("Aktualni pozive bude pouzita pro Otevirani Banky");
						UO.PrintWarning("V dosahu 4 poli by mely lezet rybi steaky!");
						Settings.HousePositionX = World.Player.X;
						Settings.HousePositionY = World.Player.Y;
						break;
					case "btn_RecallPos":
						UO.PrintInformation("Aktualni pozive bude pouzita pro Recallovani zpet do Dolu");
						UO.PrintWarning("V dosahu musi byt truhlicka s Runami a Runy musi byt nacteni v GUI");
						Settings.RunePositionX = World.Player.X;
						Settings.RunePositionY = World.Player.Y;
						break;
					case "btn_SetDoor":
						UO.PrintError("Zamer leve zavrene dvere");
						UOItem tmpd = new UOItem(UIManager.TargetObject());
						Settings.DoorLeft = tmpd ?? new UOItem(0x0);
						Settings.DoorLeftClosedGraphic = tmpd.Graphic;
						UO.PrintInformation("Zamer prave zavrene dvere");
						tmpd = new UOItem(UIManager.TargetObject());
						Settings.DoorRight = tmpd ?? new UOItem(0x0);
						Settings.DoorRightClosedGraphic = tmpd.Graphic;
						break;
					case "btn_SetWeapon":
						UO.PrintInformation(" Pokud nepouzivas Mace k boji zamer zbran");
						UOItem temp = new UOItem(UIManager.TargetObject());
						if(temp!=null)
						{
							Settings.Weapon = temp;
						}
						break;
					case "btn_SelectMine":
						Mining.GUI.Mining.LastInstance.Invoke(new MethodInvoker(delegate
						{

							if (GUI.Mining.LastInstance.Mines.SelectedIndex >= 0)
							{
                                Settings.ActualMapIndex = GUI.Mining.LastInstance.Mines.SelectedIndex;

                                UO.PrintInformation("Zvolen dul {0}", Settings.Maps[Settings.ActualMapIndex].Name);

							}
						}));
						break;
					case "btn_StartMine":
						UO.Say(",mine");

						break;

				}
			}
		}

		[Command,BlockMultipleExecutions]
		public void AddMap(string Name)
		{
			var tmp = new Map();
			tmp.Record(Name, Settings.Maps.Count());
			Settings.Maps.Add(tmp);
		}


		/// <summary>
		/// Check AFK, Attack, Weight etc..
		/// </summary>
		/// <returns>True - MineField is empty</returns>
		public bool Check()
		{
			try
			{
				bool rtrnTmp = false;
				// Check AFK
				if (Journal.Contains(true, calls[10], calls[11], calls[12], calls[13], calls[14]))
				{
					System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
					my_wave_file.PlaySync();
				}
				// Check CK/Monster
				foreach (var ch in World.Characters)
				{
					if (ch.Notoriety > Notoriety.Criminal)
					{
						System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
						my_wave_file.Play();
						Battle b = new Battle(MoveTo,moveXField,Recall,ch,new UOItem(Settings.Weapon==0?Mace==0?0:Mace:Settings.Weapon)); // TODO Battle
                        b.Kill();
                    }
				}

				// Check Light
				if (Journal.Contains(true, calls[15]) || World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A18))
				{
					World.Player.Layers[Layer.LeftHand].Use();
					UO.Wait(200);
					if (World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A15)) World.Player.Backpack.AllItems.FindType(0x0A18).Use();

				}
                // TODO stamina
				// Check stamina
				//if (World.Player.Stamina < 10)
				//{
				//	while (World.Player.Stamina < 50)
				//	{
				//		Check();
				//		UO.Wait(200);
				//	}
				//}

				// No Ore
				if (Journal.Contains(true, calls[6], calls[7], calls[8], calls[9]))
					rtrnTmp = true;

				// Skill delay
				if (Journal.Contains(true, calls[5]))
				{
					UO.Wait(5000);
				}

				// Check Weight
				if (World.Player.Weight > (World.Player.Strenght * 4 + 15))
				{
					Unload();
					return false;
				}

				// Incoming Ore  
				if (Journal.Contains(true, calls[0], calls[1], calls[2], calls[3], calls[4]))
				{

					if (Journal.Contains(true, " zaval!"))
					{
						rtrnTmp = true;
					}
					if (Journal.Contains(true, "Copper "))
					{
						if (Settings.SkipCopper)
							rtrnTmp = true;
						if (Settings.DropCopper)
						{
							World.Player.Backpack.AllItems.FindType(Ore, Material["Copper"]).DropHere(ushort.MaxValue);
							rtrnTmp = true;
						}
					}
					if (Journal.Contains(true, "Iron "))
					{
						if (Settings.SkipIron)
							rtrnTmp = true;
                        GUI.Mining.LastInstance.Invoke(new MethodInvoker(delegate
                        {
                            int a = (int.Parse(GUI.Mining.LastInstance.AIron ?? "0") + 1);
                            int b = (int.Parse(GUI.Mining.LastInstance.TIron ?? "0") + 1);
                            GUI.Mining.LastInstance.AIron = a.ToString();
                            GUI.Mining.LastInstance.TIron = b.ToString();
                        }));
                    }
					if (Journal.Contains(true, "Kremicity "))
					{
						if (Settings.SkipSilicon)
							rtrnTmp = true;
						//Settings.ASilicon++;
						//Settings.TSilicon++;

					}
					if (Journal.Contains(true, "Verite "))
					{
						if (Settings.SkipVerite)
							rtrnTmp = true;
						//Settings.AVerite++;
						//Settings.TVerite++;
					}
					if (Journal.Contains(true, "Valorite "))
					{
						//Settings.AValorite++;
						//Settings.TValorite++;
					}
					if (Journal.Contains(true, "Obsidian "))
					{
						//Settings.AObsidian++;
						//Settings.TObsidian++;
					}
					if (Journal.Contains(true, "Adamantium "))
					{
						//Settings.AAdamantium++;
						//Settings.TAdamantium++;
					}



				}
				Journal.Clear();
				if (rtrnTmp)
				{
					// Check amount of Best materials
					if (Settings.AObsidian >= Settings.MaxObs)
					{
						Unload();
					}
					else
					if (Settings.AAdamantium >= Settings.MaxAda)
					{
						Unload();
					}
				}
				return rtrnTmp;
			}
			catch { return true; };




		}  

		[Command("mine"),BlockMultipleExecutions]
		public void Work()
		{
			if(Settings.ActualMapIndex==0)
			{
				UO.PrintError("Vyber Mapu");
				return;
			}
			if(Settings.Maps[Settings.ActualMapIndex].Fields[0].Distance>100)
			{
				MoveTo(Settings.RunePositionX, Settings.RunePositionY);
				Recall(1); // TODO nejede
			}
			while (true)
			{
				MineHere(MoveToClosestExploitable(), 0);
				UO.Wait(200);
				if (Settings.AutoRemoveRocks)
				{
					while (DateTime.Now - StartMine < TimeSpan.FromMilliseconds(3300))
					{
						UO.Wait(100);
					}
					Settings.Maps[Settings.ActualMapIndex].RemoveNearObstacles(MineHere);
				}
			}

		}




		public void Unload()
		{
			Point ActualPosition = new Point(World.Player.X, World.Player.Y);
			int tmpMapIndex = Settings.ActualMapIndex;
			Recall(0);
			UOItem dltmp = new UOItem(Settings.DoorLeft);
			UOItem drtmp = new UOItem(Settings.DoorRight);
			if (dltmp.Graphic == Settings.DoorLeftClosedGraphic) dltmp.Use();
			if (drtmp.Graphic == Settings.DoorRightClosedGraphic) drtmp.Use();
			StockUp();
			Settings.ActualMapIndex = tmpMapIndex;
			Recall(1);
			MoveTo(ActualPosition);

		}


		private void Recall(int v)
		{

			int x = World.Player.X;
			int y = World.Player.Y;
			UO.Warmode(false);
			switch (v)
			{
				case 0:
					while (World.Player.X == x | World.Player.Y == y)
					{
						while (World.Player.Mana < 20)
						{
							UO.UseSkill(StandardSkill.Meditation);
							UO.Wait(2500);
						}
						UO.WaitTargetSelf();
						UO.Say(".recallhome");
						Journal.WaitForText(true, 10000, "Kouzlo se nezdarilo.");
						Journal.ClearAll();
						UO.Wait(200);
					}
					break;
				case 1:

					while (World.Player.X == x || World.Player.Y == y)
					{
						while (World.Player.Mana < 20)
						{
							UO.UseSkill(StandardSkill.Meditation);
							UO.Wait(2500);
						}

						foreach (Rune r in Main.Instance.SGUI.Runes.Runes.Where
							(a => a.Name == Settings.Maps[Settings.ActualMapIndex].Name))
						{
							Main.Instance.SGUI.Runes.findRune(r);
							r.RecallSvitek();
						}


						Journal.ClearAll();
						UO.Wait(500);
						Journal.WaitForText(true, 10000, "Kouzlo se nezdarilo.");
						UO.Wait(200);
					}
					break;
			}


		}


		private void StockUp()
		{
			Settings.ActualMapIndex = 0;
			MoveTo(Settings.HousePositionX, Settings.HousePositionY);
			MoveOre_Feed_GetRecall();
			MoveTo(Settings.RunePositionX, Settings.RunePositionY);

		}


		private void MoveOre_Feed_GetRecall() // TODO resource
		{
			Serial tmp;
			openBank(14);
			UOItem box = new UOItem(Settings.OreBox);
			box.Use();
			UO.Wait(200);
			new UOItem(Settings.GemBox).Use();
			World.Player.Backpack.AllItems.FindType(Ore, Material["Copper"]).Move(ushort.MaxValue, Settings.OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Iron"]).Move(ushort.MaxValue, Settings.OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Verite"]).Move(ushort.MaxValue, Settings.OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Valorite"]).Move(ushort.MaxValue, Settings.OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Kremicity"]).Move(ushort.MaxValue, Settings.OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Obsidian"]).Move(ushort.MaxValue, Settings.OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Adamantium"]).Move(ushort.MaxValue, Settings.OreBox);
			UO.Wait(100);
			if (PickAxe == 0)
			{

				tmp = box.AllItems.FindType(0x1406).Exist
						? box.AllItems.FindType(0x1406).Serial : box.AllItems.FindType(0x1407).Exist
						? box.AllItems.FindType(0x1407).Serial : 0;
				if (tmp == 0)
				{
					UO.PrintError("Nemas krumpac");
					UO.TerminateAll();
				}
			}

			if (Mace == 0)
			{
				tmp = box.AllItems.FindType(0x0E85).Exist
				? box.AllItems.FindType(0x0E85).Serial : box.AllItems.FindType(0x0E86).Exist
				? box.AllItems.FindType(0x0E86).Serial : 0;
				if (tmp == 0)
				{
					UO.PrintError("Nemas Zbran");
					UO.TerminateAll();
				}
			}
			for (ushort i = 0x0F0F; i < 0x0F31; i++)
			{
				World.Player.Backpack.AllItems.FindType(i).Move(ushort.MaxValue, Settings.GemBox);
			}
			// Mramor
			while (World.Player.Backpack.AllItems.FindType(0x1363).Amount > 0)
				World.Player.Backpack.AllItems.FindType(0x1363).Move(ushort.MaxValue, Settings.OreBox);



			SelfFeed();
			box.Use();
			if (World.Player.Backpack.AllItems.FindType(0x1F4C).Amount < 4)
				box.AllItems.FindType(0x1F4C).Move(7, World.Player.Backpack);

		}


		private void SelfFeed()
		{
			World.FindDistance = 4;
			World.Ground.FindType(0x097B).Use();
			UO.Wait(100);
			World.Ground.FindType(0x097B).Use();
			UO.Wait(100);
			World.Ground.FindType(0x097B).Use();
			UO.Wait(100);
			World.Ground.FindType(0x097B).Use();
			UO.Wait(100);
		}


		private void MineHere(MineField mf, int Try)
		{
			if (mf == null)
			{
				return;
			}

			while (DateTime.Now - StartMine < TimeSpan.FromMilliseconds(3300))
			{
				UO.Wait(50);
				if (Check())
				{
					mf.State = MineFieldState.Empty;
					return;
				}
			}

			if (!CheckTools()) return;


			if (Settings.UseCrystal && Try == 0)
			{
				UO.Say(".vigour");
				UO.WaitTargetTile(World.Player.X, World.Player.Y, World.Player.Z, 0);
				pickAxe.Use();
				StartMine = DateTime.Now;
				Journal.WaitForText(true, 1500, "Nasla jsi lepsi material!", "Nasel jsi lepsi material!");
				UO.Say(".vigour");
				MineHere(mf, Try + 1);
			}
			else
			{
				UO.WaitTargetTile(World.Player.X, World.Player.Y, World.Player.Z, 0);
				pickAxe.Use();
				StartMine = DateTime.Now;
				UO.Wait(100);
				MineHere(mf, Try + 1);
			}
		}

		private bool CheckTools()
		{
			Mace = Mace;
			PickAxe = PickAxe;
			Weapon = new UOItem(Settings.Weapon);

			if (Mace == 0 && Weapon.Serial==default(uint))
			{
				Unload();
				return false;
			}
			if (PickAxe == 0)
			{
				Unload();
				return false;
			}
			return true;
		}

		private void MineHere(UOItem item, int Try)
		{
			if (item == null)
			{
				return;
			}

			while (DateTime.Now - StartMine < TimeSpan.FromMilliseconds(3200))
			{
				UO.Wait(50);
				if (!Settings.AutoRemoveRocks) return;
				if (Check())
				{
					return;
				}
			}

			if (!CheckTools()) return;

			{
				try
				{
					item.WaitTarget();
					pickAxe.Use();
				}
				catch { }
				StartMine = DateTime.Now;
				UO.Wait(100);
				MineHere(item, Try + 1);
			}
		}




		private void openBank(int equip)
		{
			Core.UnregisterServerMessageCallback(0xB0, onGumpBank);
			Core.RegisterServerMessageCallback(0xB0, onGumpBank);
			UO.Say(".equip{0}", equip);
			UO.Wait(600);
			Core.UnregisterServerMessageCallback(0xB0, onGumpBank);
		}


		private CallbackResult onGumpBank(byte[] data, CallbackResult prevResult)
		{
			byte cmd = 0xB1; //1 byte
			uint ID, gumpID;
			uint buttonID = 9; //4 byte
			uint switchCount = 0;
			uint textCount = 0;

			PacketReader pr = new PacketReader(data);
			if (pr.ReadByte() != 0xB0) return CallbackResult.Normal;
			pr.ReadInt16();
			ID = pr.ReadUInt32();
			gumpID = pr.ReadUInt32();


			PacketWriter reply = new PacketWriter();
			reply.Write(cmd);
			reply.WriteBlockSize();
			reply.Write(ID);
			reply.Write(gumpID);
			reply.Write(buttonID);
			reply.Write(switchCount);
			reply.Write(textCount);

			Core.SendToServer(reply.GetBytes());
			return CallbackResult.Sent;
		}
	}
}

