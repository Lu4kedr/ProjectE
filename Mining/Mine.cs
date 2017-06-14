using Mining.PathFinding;
using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using Project_E;
using Project_E.Lib.Runes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Mining
{
	public delegate void Run(bool recall);
	public class Mine
	{

		#region Singelton
		private static Mine instance = new Mine();


		static Mine() { }

		private Mine()
		{

			Initialize();
		}
		public static Mine Instance { get { return instance; } }
		#endregion

		Settings Settings;
		Check Check;
		private string AlarmPath = Core.Directory + @"\afk.wav";
		private Graphic Ore = 0x19B7;
		private Dictionary<string, UOColor> Material = new Dictionary<string, UOColor>() { { "Copper", 0x099A }, { "Iron", 0x0763 }, { "Kremicity", 0x0481 }, { "Verite", 0x097F }, { "Valorite", 0x0985 }, { "Obsidian", 0x09BD }, { "Adamantium", 0x0026 } };
		private Movement mov;
		private SearchParameters searchParams;
		private PathFinder pathfinder;
		DateTime StartMine = DateTime.Now;
		UOItem Weapon;
		UOItem PickAxe;
		private Point ActualPosition;
		private bool EmptyField=false;
		private bool MineRun=false;
		private bool MaxedWeight = false;
		private bool SkillDelay=false;
		private bool EnemyAppeared=false;
		UOCharacter Enemy;
		private bool CK=false;
		private bool TopMonster=false;
		private UOCharacter LastEnemy;

		public void Initialize()
		{
			mov = new Movement();
			searchParams = new SearchParameters(new Point(), new Point(), null);
			XmlSerializeHelper<Settings>.Load("Mining", out Settings, false);
			if (Settings == null)
			{
				Settings = new Settings();
			}
			StartMine = DateTime.Now;
			Check = new Check();
			GUI.Mining.LastInstance.OnCHanged += LastInstance_OnCHanged;

			GUI.Mining.LastInstance.BeginInvoke(new MethodInvoker(delegate
			{
				GUI.Mining.LastInstance.Mines.Items.Clear();
				foreach (var it in Instance.Settings.Maps)
				{
					GUI.Mining.LastInstance.Mines.Items.Add(it.Name);
				}
			}));
			Core.Window.FormClosing += Window_FormClosing;
            Core.Disconnected += Core_Disconnected;
		}

        private void Core_Disconnected(object sender, EventArgs e)
        {
            XmlSerializeHelper<Settings>.Save("Mining", Instance.Settings, false);
            Core.Window.FormClosing -= Window_FormClosing;
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
		{
			XmlSerializeHelper<Settings>.Save("Mining", Instance.Settings, false);

		}

		public void Start()
		{
			CK = false;
			TopMonster = false;
			if (Instance.Settings.ActualMapIndex == 0)
			{
				UO.PrintError("Vyber Mapu");
				return;
			}

			Check.Start();
			MineRun = true;
			Check.EnemyAppeared += Check_EnemyAppeared;
			Check.OnAfk += Check_OnAfk;
			Check.OnMaxedWeight += Check_OnMaxedWeight;
			Check.OnNoOre += Check_OnNoOre;
			Check.OnOreAdded += Check_OnOreAdded;
			Check.OnSkillDelay += Check_OnSkillDelay;

			if (Instance.Settings.Maps[Instance.Settings.ActualMapIndex].Fields[0].Distance > 200)
			{
				int tmpMapInx = Instance.Settings.ActualMapIndex;
				Instance.Settings.ActualMapIndex = 0;
				MoveTo(Instance.Settings.RunePositionX, Instance.Settings.RunePositionY);
				Instance.Settings.ActualMapIndex = tmpMapInx;
				Recall(1); 
			}
			while (MineRun)
			{
				MineHere(MoveToClosestExploitable(), 0);
				if(World.Player.Backpack.AllItems.FindType(Ore, Material["Obsidian"]).Amount >= Instance.Settings.MaxObs ||
					World.Player.Backpack.AllItems.FindType(Ore, Material["Adamantium"]).Amount >= Instance.Settings.MaxAda)
				{
					// TODO Battle bug
					// TODO Check resource nez zacne
					// TODO Fast run dul

					ActualPosition = new Point(World.Player.X, World.Player.Y);
					Recall(0);
					Unload();
					Recall(1);
					MoveTo(ActualPosition);
					MaxedWeight = false;
				}

				if (CK || TopMonster)
				{
					Stop();
					UOItem pr;
					Mining.Run r = new Mining.Run(Run);
					if (World.Player.Backpack.AllItems.FindType(Ore, Material["Iron"]).Amount > 10 && CK) 
					{

						r.BeginInvoke(true, null, null);
						for(int i=0;i< (World.Player.Backpack.AllItems.FindType(Ore, Material["Iron"]).Amount>100?50:100);i++)
						{
							if (UO.Dead) break;
							pr = new UOItem(World.Player.Backpack.AllItems.FindType(Ore, Material["Iron"]));
							if (pr.Amount > 5)
							{
								pr.Move(1, World.Player.Backpack, 20, 20);
							}
						}
					}
					r.BeginInvoke(false, null, null);
					return;
				}
				UO.Wait(200);
				if (Instance.Settings.AutoRemoveRocks)
				{
					int tmp = 3300 - (int)(DateTime.Now - StartMine).TotalMilliseconds;
					if (tmp > 0)
					{
						Thread.Sleep(tmp);
					}
					Instance.Settings.Maps[Instance.Settings.ActualMapIndex].RemoveNearObstacles(MineHere);
				}
			}
		}
		private void Run(bool recall)
		{
			DateTime drink;
			while (true)
			{
				if (Enemy.Distance > 5)
				{
					UO.Say(".potioninvis");
					drink = DateTime.Now;
					if (recall)
					{
						Recall(0);
						UO.TerminateAll();
					}
					else
					{
						UO.Wait(300);
						if (!World.Player.Hidden)
						{
							while (!World.Player.Hidden)
							{
								moveXField(10);
								if (DateTime.Now - drink > TimeSpan.FromSeconds(21) && Enemy.Distance > 3)
								{
									UO.Say(".potioninvis");
									UO.TerminateAll();
								}
							}
						}
					}
				}
			}

		}

		public void Stop()
		{
			Check.Stop();
			MineRun = false;
			Check.EnemyAppeared -= Check_EnemyAppeared;
			Check.OnAfk -= Check_OnAfk;
			Check.OnMaxedWeight += Check_OnMaxedWeight;
			Check.OnNoOre -= Check_OnNoOre;
			Check.OnOreAdded -= Check_OnOreAdded;
			Check.OnSkillDelay -= Check_OnSkillDelay;
			
		}
		private void Check_OnSkillDelay(object sender, EventArgs e)
		{
			SkillDelay = true;
		}

		private void Check_OnOreAdded(object sender, OnOreAddedArgs e)
		{

			Instance.Settings.WeightProgressbar = World.Player.Weight;
			switch(e.Type)
			{
				case "Copper":
					if (Instance.Settings.SkipCopper) EmptyField = true;
					if (Instance.Settings.DropCopper)
					{
						EmptyField = true;
						World.Player.Backpack.AllItems.FindType(Ore, Material["Copper"]).DropHere(ushort.MaxValue);
					}
					
						break;
				case "Iron":
					if (Instance.Settings.SkipIron) EmptyField = true;
					Instance.Settings.AIron++;
					Instance.Settings.TIron++;
					break;
				case "Kremicity":
					if (Instance.Settings.SkipSilicon) EmptyField = true;
					Instance.Settings.ASilicon++;
					Instance.Settings.TSilicon++;
					break;
				case "Verite":
					if (Instance.Settings.SkipVerite) EmptyField = true;
					Instance.Settings.AVerite++;
					Instance.Settings.TVerite++;
					break;
				case "Valorite":
					Instance.Settings.AValorite++;
					Instance.Settings.TValorite++;
					break;
				case "Obsidian":
					Instance.Settings.AObsidian++;
					Instance.Settings.TObsidian++;
					break;
				case "Adamantium":
					Instance.Settings.AAdamantium++;
					Instance.Settings.TAdamantium++;
					break;
			}

		}

		private void Check_OnNoOre(object sender, EventArgs e)
		{
			EmptyField = true;
		}

		private void Check_OnMaxedWeight(object sender, EventArgs e)
		{
			MaxedWeight = true;
		}

		private void Check_OnAfk(object sender, EventArgs e)
		{
			Check.OnAfk -= Check_OnAfk;
			System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
			my_wave_file.Play();
			UO.Wait(200);
			Check.OnAfk += Check_OnAfk;
		}

		private void Check_EnemyAppeared(object sender, EnemyAppearedArgs e)
		{
			if (EnemyAppeared) return;
			if (LastEnemy != Enemy)
			{
				if (e.CK)
				{
					CK = true;
				}
				if (e.TopMonster)
				{
					TopMonster = true;
				}
				EnemyAppeared = true;
				Enemy = new UOCharacter(e.Enemy);
				Enemy.Click();
				UO.Wait(100);

				LastEnemy = Enemy;
				if((Enemy.Name=="Kryska" || Enemy.Name == "Troll") && !Instance.Settings.KryskaTrollAlarm)
				{

				}
				else
				{
					System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
					my_wave_file.Play();
				}

			}
		}

		private void MineHere(MineField mf, int Try)
		{
			try
			{
				if (mf == null || CK || TopMonster) return;
				if (Try == 0)
				{

					EmptyField = false;
				}
				if (EnemyAppeared)
				{

					try
					{
						UO.Say(Instance.Settings.FightSay);
						Battle b = new Battle(MoveTo, moveXField, Recall, Enemy, Weapon);
						b.Kill();
					}
					catch (Exception ex) { UO.PrintError(ex.Message); }
					UO.Say(Instance.Settings.FightSay);
					EnemyAppeared = false;
				}
				if (SkillDelay)
				{
					UO.Wait(5000);
					SkillDelay = false;
				}
				int tmp = 3300 - (int)(DateTime.Now - StartMine).TotalMilliseconds;
				if (tmp > 0)
				{
					Thread.Sleep(tmp);
				}
				if (EmptyField)
				{
					mf.State = MineFieldState.Empty;
					EmptyField = false;
					return;
				}
				if (!CHeckTools() || MaxedWeight)
				{
					ActualPosition = new Point(World.Player.X, World.Player.Y);
					Recall(0);
					Unload();
					Recall(1);
					MoveTo(ActualPosition);
					MaxedWeight = false;
				}

				if (Instance.Settings.UseCrystal && Try == 0)
				{
					UO.Say(".vigour");
					UO.WaitTargetTile(World.Player.X, World.Player.Y, World.Player.Z, 0);
					PickAxe.Use();
					StartMine = DateTime.Now;
					Journal.WaitForText(true, 1500, "Nasla jsi lepsi material!", "Nasel jsi lepsi material!");

					UO.Say(".vigour");
					MineHere(mf, Try + 1);
				}
				else
				{
					UO.WaitTargetTile(World.Player.X, World.Player.Y, World.Player.Z, 0);
					PickAxe.Use();
					StartMine = DateTime.Now;
					UO.Wait(100);
					MineHere(mf, Try + 1);
				}
			}
			catch { }
		}

		private void MineHere(UOItem item, int Try)
		{
			try
			{
				if (item == null || CK || TopMonster) return;
				if (Try == 0)
				{

					EmptyField = false;
				}
				if (EnemyAppeared)
				{
					EnemyAppeared = false;
					Battle b = new Battle(MoveTo, moveXField, Recall, Enemy, Weapon);
					b.Kill();
				}
				if (SkillDelay)
				{
					UO.Wait(5000);
					SkillDelay = false;
				}
				int tmp = 3300 - (int)(DateTime.Now - StartMine).TotalMilliseconds;
				if (tmp > 0)
				{
					Thread.Sleep(tmp);
				}

				if (!CHeckTools() || MaxedWeight)
				{
					ActualPosition = new Point(World.Player.X, World.Player.Y);
					Recall(0);
					Unload();
					Recall(1);
					MoveTo(ActualPosition);
					MaxedWeight = false;
				}



				item.WaitTarget();
				PickAxe.Use();
				StartMine = DateTime.Now;
				UO.Wait(100);

				MineHere(item, Try + 1);
			}
			catch { UO.PrintError("Zaval odstranen"); }
		}


		private void Unload()
		{

            int tmpMapIndex = Instance.Settings.ActualMapIndex;
			UOItem dltmp = new UOItem(Settings.DoorLeft);
			UOItem drtmp = new UOItem(Settings.DoorRight);
			if (dltmp.Graphic == Settings.DoorLeftClosedGraphic) dltmp.Use();
			if (drtmp.Graphic == Settings.DoorRightClosedGraphic) drtmp.Use();

			Instance.Settings.ActualMapIndex = 0;
			MoveTo(Instance.Settings.HousePositionX, Instance.Settings.HousePositionY);

			Serial tmp;
			openBank(14);
			UOItem box = new UOItem(Instance.Settings.OreBox);
			box.Use();
			UO.Wait(200);
			UOItem gemBox = new UOItem(Instance.Settings.GemBox);
			gemBox.Use();
			UO.Wait(200);
			UOItem resourceBox = new UOItem(Instance.Settings.ResourceBox);
			resourceBox.Use();

			World.Player.Backpack.AllItems.FindType(Ore, Material["Copper"]).Move(ushort.MaxValue, box);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Iron"]).Move(ushort.MaxValue, box);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Verite"]).Move(ushort.MaxValue, box);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Valorite"]).Move(ushort.MaxValue, box);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Kremicity"]).Move(ushort.MaxValue, box);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Obsidian"]).Move(ushort.MaxValue, box);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Adamantium"]).Move(ushort.MaxValue, box);
			UO.Wait(100);
			CHeckTools();
			if (PickAxe.Serial == 0)
			{

				tmp = resourceBox.AllItems.FindType(0x1406).Exist
						? resourceBox.AllItems.FindType(0x1406).Serial : resourceBox.AllItems.FindType(0x1407).Exist
						? resourceBox.AllItems.FindType(0x1407).Serial : 0;
				if (tmp == 0)
				{
					UO.PrintError("Nemas krumpac");
					UO.TerminateAll();
				}
				else
				{
					new UOItem(tmp).Move(1, World.Player.Backpack);
				}
			}

			if (Weapon == 0)
			{
				tmp = resourceBox.AllItems.FindType(0x0E85).Exist
				? resourceBox.AllItems.FindType(0x0E85).Serial : resourceBox.AllItems.FindType(0x0E86).Exist
				? resourceBox.AllItems.FindType(0x0E86).Serial : 0;
				if (tmp == 0)
				{
					UO.PrintError("Nemas Zbran");
					UO.TerminateAll();
				}
				else
				{
					new UOItem(tmp).Move(1, World.Player.Backpack);
				}
			}
			for (ushort i = 0x0F0F; i < 0x0F31; i++)
			{
				World.Player.Backpack.AllItems.FindType(i).Move(ushort.MaxValue, gemBox);
			}
			// Mramor
			while (World.Player.Backpack.AllItems.FindType(0x1363).Amount > 0)
				World.Player.Backpack.AllItems.FindType(0x1363).Move(ushort.MaxValue, gemBox);

			
			SelfFeed();
			resourceBox.Use();
			if (World.Player.Backpack.AllItems.FindType(0x1F4C).Amount < 6)
				resourceBox.AllItems.FindType(0x1F4C).Move(7, World.Player.Backpack);

			// GH
			if (World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0160).Amount < 1)
				resourceBox.AllItems.FindType(0x0F0E, 0x0160).Move(2, World.Player.Backpack);

			// Invisky
			if (World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0447).Amount < 1)
				resourceBox.AllItems.FindType(0x0F0E, 0x0447).Move(2, World.Player.Backpack);

			MoveTo(Instance.Settings.RunePositionX, Instance.Settings.RunePositionY);
			Instance.Settings.ActualMapIndex = tmpMapIndex;

			Instance.Settings.AIron = 0;
			Instance.Settings.ASilicon = 0;
			Instance.Settings.AVerite = 0;
			Instance.Settings.AValorite = 0;
			Instance.Settings.AObsidian = 0;
			Instance.Settings.AAdamantium = 0;
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



		private bool CHeckTools()
		{
			uint tmp;
			if (Settings.Weapon!=0)
			{
				Weapon = new UOItem(Settings.Weapon);
			}
			else
			{

				if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x1406) || World.Player.Layers[Layer.RightHand].Graphic.Equals(0x1407))
					tmp = World.Player.Layers[Layer.RightHand];
				else tmp = World.Player.Backpack.AllItems.FindType(0x1406).Exist
				? World.Player.Backpack.AllItems.FindType(0x1406).Serial
				: World.Player.Backpack.AllItems.FindType(0x1407).Exist
				? World.Player.Backpack.AllItems.FindType(0x1407).Serial : 0;

				if (tmp == 0) return false;
				else Weapon = new UOItem(tmp);
				tmp = 0;
			}

			if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E85) ||
				World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E86))
			{
				tmp = World.Player.Layers[Layer.RightHand];
			}

			else tmp = World.Player.Backpack.AllItems.FindType(0x0E85).Exist
					? World.Player.Backpack.AllItems.FindType(0x0E85).Serial : World.Player.Backpack.AllItems.FindType(0x0E86).Exist
					? World.Player.Backpack.AllItems.FindType(0x0E86).Serial : 0;
			if (tmp != 0)
			{
				PickAxe = new UOItem(tmp);
				return true;
			}
			return false;
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
							(a => a.Name == Instance.Settings.Maps[Instance.Settings.ActualMapIndex].Name))
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



		private MineField MoveToClosestExploitable()
		{
			Instance.Settings.Maps[Instance.Settings.ActualMapIndex].FindObstacles();
			Instance.Settings.Maps[Instance.Settings.ActualMapIndex].Fields.Sort((a, b) => a.Distance.CompareTo(b.Distance));
			MineField tmp;
			try
			{
				tmp = Instance.Settings.Maps[Instance.Settings.ActualMapIndex].Fields.First(x => x.State == MineFieldState.Unknown);
				MoveTo(tmp.Location);
			}
			catch
			{
				tmp = null;
				UO.PrintError("Nenalezen tezitelbne pole");
			}
			return tmp;
		}


		public void MoveTo(int X, int Y)
		{
			foreach (Point p in GetWay(new Point(World.Player.X, World.Player.Y), new Point(X, Y)))
			{
				mov.moveToPosition(p);
			}
		}


		public void MoveTo(Point location)
		{
			foreach (Point p in GetWay(new Point(World.Player.X, World.Player.Y), location))
			{
				mov.moveToPosition(p);
			}
		}


		private List<Point> GetWay(Point StartPosition, Point EndPosition)
		{
			searchParams = new SearchParameters(StartPosition, EndPosition, Instance.Settings.Maps[Instance.Settings.ActualMapIndex]);
			pathfinder = new PathFinder(searchParams);
			return pathfinder.FindPath();
		}


		public void moveXField(int distance)
		{
			Instance.Settings.Maps[Instance.Settings.ActualMapIndex].FindObstacles();
			Instance.Settings.Maps[Instance.Settings.ActualMapIndex].Fields.Sort((a, b) => a.Distance.CompareTo(b.Distance));
			MineField tmp;
			try
			{
				tmp = Instance.Settings.Maps[Instance.Settings.ActualMapIndex].Fields.First(x => x.Distance >= distance);
				MoveTo(tmp.Location);
			}
			catch
			{
				tmp = null;
				UO.PrintError("Nenalezen tezitelbne pole");
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




		private void LastInstance_OnCHanged(object sender, EventArgs e)
		{
			if (sender is Button)
			{

				switch (((Button)sender).Name)
				{

					case "btn_ResourceBox":
						UO.PrintInformation("Zamer Baglik/Truhlicku.. ze ktere se budou brat Krumpace, Mace, Recally");
						Instance.Settings.ResourceBox = new UOItem(UIManager.TargetObject());
						break;
					case "btn_OreBox":
						UO.PrintInformation("Zamer Baglik/Truhlicku.. do ktere se bude vykladat natezene ore");
						Instance.Settings.OreBox = new UOItem(UIManager.TargetObject());
						break;
					case "btn_GemBox":
						UO.PrintInformation("Zamer Baglik/Truhlicku.. do ktere se budou vykladat natezene drahokamy a Mramor");
						Instance.Settings.GemBox = new UOItem(UIManager.TargetObject());
						break;
					case "btn_BankPos":
						UO.PrintInformation("Aktualni pozive bude pouzita pro Otevirani Banky");
						UO.PrintWarning("V dosahu 4 poli by mely lezet rybi steaky!");
						Instance.Settings.HousePositionX = World.Player.X;
						Instance.Settings.HousePositionY = World.Player.Y;
						break;
					case "btn_RecallPos":
						UO.PrintInformation("Aktualni pozive bude pouzita pro Recallovani zpet do Dolu");
						UO.PrintWarning("V dosahu musi byt truhlicka s Runami a Runy musi byt nacteni v GUI");
						Instance.Settings.RunePositionX = World.Player.X;
						Instance.Settings.RunePositionY = World.Player.Y;
						break;
					case "btn_SetDoor":
						UO.PrintError("Zamer leve zavrene dvere");
						UOItem tmpd = new UOItem(UIManager.TargetObject());
						Instance.Settings.DoorLeft = tmpd ?? new UOItem(0x0);
						Instance.Settings.DoorLeftClosedGraphic = tmpd.Graphic;
						UO.PrintInformation("Zamer prave zavrene dvere");
						tmpd = new UOItem(UIManager.TargetObject());
						Instance.Settings.DoorRight = tmpd ?? new UOItem(0x0);
						Instance.Settings.DoorRightClosedGraphic = tmpd.Graphic;
						break;
					case "btn_SetWeapon":
						UO.PrintInformation(" Pokud nepouzivas Mace k boji zamer zbran");
						UOItem temp = new UOItem(UIManager.TargetObject());
						if (temp != null)
						{
							Instance.Settings.Weapon = temp;
						}
						break;
					case "btn_SelectMine":
						Mining.GUI.Mining.LastInstance.Invoke(new MethodInvoker(delegate
						{

							if (GUI.Mining.LastInstance.Mines.SelectedIndex >= 0)
							{
								Instance.Settings.ActualMapIndex = GUI.Mining.LastInstance.Mines.SelectedIndex;

								UO.PrintInformation("Zvolen dul {0}", Instance.Settings.Maps[Instance.Settings.ActualMapIndex].Name);

							}
						}));
						break;
					case "btn_StartMine":
						UO.Say(",mine");

						break;

				}
			}
		}


	}
}
