using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using Project_E.GUI;
using Project_E.Lib;
using Project_E.Lib.DrinkManager;
using Project_E.Lib.EquipSet;
using Project_E.Lib.GameWindow;
using Project_E.Lib.Healing;
using Project_E.Lib.Runes;
using Project_E.Lib.Skills;
using Project_E.Lib.SpellManager;
using Project_E.Lib.WallManager;
using Project_E.Lib.WeaponsSet;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
// TODO Resource manager
namespace Project_E
{
    public class Main
    {
        #region Declare Libs

        DrinkManager DM;
        WallManager WM;
        GameWindowSize GWS;
        GameWIndoSizeDATA GWSDATA;
        System.Timers.Timer Wait2CharLoad;
        public EreborGUI EreborGUI;

        public Abilities AB;
        public Skills SK;
        public Facilitation FA;
        public Targeting TRG;
        public Watcher WT;
        public SettingsGUI SGUI;
        public AutoHeal AH;
        public Autolot Autolot;
        public SpellManager SM;


        #endregion


        #region Singelton
        private static Main instance = new Main();
        private int TabIndex;


        static Main() { }

        private Main()
        {
            // GameWindow Settings
           // MessageBox.Show("dd");
            XmlSerializeHelper<GameWIndoSizeDATA>.Load("WindowSize", out GWSDATA, false);
            GWS = new GameWindowSize(GWSDATA);
            Wait2CharLoad = new System.Timers.Timer(100);
            Wait2CharLoad.Elapsed += CharLoad;
            Wait2CharLoad.Start();

        }
        public static Main Instance { get { return instance; } }
        #endregion


        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private void CharLoad(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (World.Player.Name == null) return;
            Wait2CharLoad.Stop();
            setEQ();

            #region Initialize Classes
            DM = new DrinkManager();
            WM = new WallManager();

            AB = new Abilities();
            FA = new Facilitation();
            TRG = new Targeting();
            WT = new Watcher();


            XmlSerializeHelper<SettingsGUI>.Load(World.Player.Name, out SGUI, true);
            if (SGUI == null) SGUI = new SettingsGUI();
            SK = new Skills(SGUI);

            // TODO AH - nutno predavat tolik atrb ?!
            AH = new AutoHeal(SGUI.HealedPlayers, SGUI.KlerikShaman == 0 ? ".heal" : ".samheal",
                SGUI.KlerikShaman == 0 ? ".enlightment" : ".improvement", SGUI.Weapons.ActualWeapon, WT, SGUI, SGUI.KlerikShaman == 0 ? 86 : 90,SK.HarmSelf);
            SM = new SpellManager(SGUI, AB.Sacrafire,AH.Bandage);
            EreborGUI = EreborGUI.LastInstance;
            #endregion

            #region GUI Init-Refresh
            // Initialize GUI Values;
            EreborGUI.BeginInvoke(new MethodInvoker(delegate
            {
                EreborGUI.GW_Height = GWSDATA.Height.ToString();
                EreborGUI.GW_Width = GWSDATA.Width.ToString();

            }));


            // Refresh GUI - all lists
            EreborGUI.Invoke(new MethodInvoker(delegate
            {
                EreborGUI.TabControl.SelectedIndex = 2;
                EreborGUI.TabControl.SelectedIndex = 0;
                // Runes
                EreborGUI.Runes.Nodes.Clear();
                SGUI.Runes.FillTreeView(EreborGUI.Runes);
                EreborGUI.Weapons.Refresh();

                // Weapons
                EreborGUI.Weapons.Items.Clear();
                foreach (WeaponSet t in SGUI.Weapons.weapons)
                {
                    ListViewItem lvitem = new ListViewItem(t.Name);
                    if (t.Weapon == SGUI.Weapons.ActualWeapon.Weapon && t.Shield == SGUI.Weapons.ActualWeapon.Shield)
                    {
                        lvitem.SubItems.Add("Actual Weapon");
                    }
                    else
                    {
                        lvitem.SubItems.Add("- - -");
                    }
                    EreborGUI.Weapons.Items.Add(lvitem);
                }
                EreborGUI.Weapons.Refresh();

                // Healed
                EreborGUI.HealedPlayers.Items.Clear();
                foreach (Patient t in SGUI.HealedPlayers)
                {
                    t.Character.Click();
                    UO.Wait(200);
                    ListViewItem lvitem = new ListViewItem(t.Character.Serial.ToString());
                    lvitem.SubItems.Add(t.Character.Name ?? "-");
                    lvitem.SubItems.Add(t.Equip.ToString());

                    EreborGUI.HealedPlayers.Items.Add(lvitem);
                }

                EreborGUI.HealedPlayers.Refresh();

                // Track Ignored
                EreborGUI.TrackIgnore.Items.Clear();
                foreach (string t in SGUI.Tracking.Ignored)
                {
                    ListViewItem lvitem = new ListViewItem(t);
                    EreborGUI.TrackIgnore.Items.Add(lvitem);
                }
                EreborGUI.TrackIgnore.Refresh();


            }));
            #endregion

            // Register Events
            EreborGUI.OnChanged += EreborGUI_OnChanged;
            WT.HitsChanged += WT_HitsChanged;
            WT.OnParalyze += WT_OnParalyze;
            World.Player.Changed += Player_Changed;
            WT.OnSuccessHit += WT_OnSuccessHit;
            WT.HiddenChanged += WT_HiddenChanged;
         

            UO.PrintInformation("Loaded");
            Wait2CharLoad = null;
            Core.Window.FormClosing += Window_FormClosing;
            Core.Disconnected += Core_Disconnected;
        }

        private void Core_Disconnected(object sender, EventArgs e)
        {
            XmlSerializeHelper<SettingsGUI>.Save(World.Player.Name, SGUI, true);

            XmlSerializeHelper<GameWIndoSizeDATA>.Save("WindowSize", GWSDATA, false);
            Core.Window.FormClosing -= Window_FormClosing;
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            XmlSerializeHelper<SettingsGUI>.Save(World.Player.Name, SGUI, true);

            XmlSerializeHelper<GameWIndoSizeDATA>.Save("WindowSize", GWSDATA, false);


        }

        private void setEQ()
        {
            UO.Wait(800);
            World.Player.Click();
            UO.Wait(800);
            World.Player.WaitTarget();
            UO.Say(".setequip15");

        }
        private void WT_HiddenChanged(object sender, Watcher.HiddenChangedArgs e)
        {
            if(!e.State)
            {
                SK.hidoff();
                UO.Say(".resync");
                UO.PrintWarning("Resynced");
            }
        }

        private void WT_OnSuccessHit(object sender, EventArgs e)
        {
            if(SGUI.HitBandage)
            {
                AH.Bandage();
            }
            if (SGUI.HitTracking)
            {
                SGUI.Tracking.Track(2);
            }
        }

        private void Player_Changed(object sender, ObjectChangedEventArgs e)
        {
            if(SGUI.CorpsesHide)
            {
                World.FindDistance = 10;
                foreach(var it in World.Ground.Where(x=>x.Graphic==0x2006).ToList())
                {
                    UO.Hide(it);
                }
            }

            string tmp = PrintState();
            if (tmp != Client.Text)
                Client.Text = tmp;
            // TODO predelat na form

        }

        private string PrintState()
        {
            UOPlayer p = World.Player;
            string temp = "";
            temp = "HP: " + p.Hits + "/" + p.MaxHits //11
                + "  ||  Mana: " + p.Mana + "/" + p.MaxMana//19
                + "  ||  Stamina: " + p.Stamina + "/" + p.MaxStamina//22
                + "  ||  Sila: " + p.Strenght //15
                + "  ||  Stealth: " + p.Skills["Stealth"].RealValue / 10 //18
                + "  ||  Weight: " + p.Weight + "/" + p.MaxWeight//21
                + "  ||  Armor: " + p.Armor //16
                + "  ||  Gold: " + p.Gold;//20  =109-->110+110=220 124-->125+125=290

            if (World.GetCharacter(Aliases.GetObject("laststatus")).Distance < 20 && World.GetCharacter(Aliases.GetObject("laststatus")).Hits > -1)
            {
                if (temp.Length < 145)
                {
                    for (int i = 0; i < 145 - temp.Length; i++)
                    {
                        temp += " ";
                    }
                    temp += "                              ";//40 znaku
                    temp += "                              ";//40 znaku
                }
                temp += World.GetCharacter(Aliases.GetObject("laststatus")).Name
                    + ": "
                    + World.GetCharacter(Aliases.GetObject("laststatus")).Hits
                    + "/"
                    + World.GetCharacter(Aliases.GetObject("laststatus")).MaxHits
                    + "   Distance: "
                    + World.GetCharacter(Aliases.GetObject("laststatus")).Distance;

            }
            return temp;
        }
        private void WT_OnParalyze(object sender, EventArgs e)
        {
            if (SGUI.AutoHarm && !AH.Running) SK.HarmSelf(false);
        }

        private short LastHitDecrease;


        private void WT_HitsChanged(object sender, Watcher.HitsChangedArgs e)
        {
            if (!e.gain) LastHitDecrease = e.amount;
            if(SGUI.AutoDrink)
            {
                if ((World.Player.Hits + 10) <= LastHitDecrease) UO.Say(".potionheal");
                if (World.Player.Hits <= short.Parse(SGUI.Hits2Drink ?? "50")) UO.Say(".potionheal");
            }
        }


        // GUI functions
        private void EreborGUI_OnChanged(object sender, OnChangedArgs e)
        {

            if (sender is Button)
            {
                switch (((Button)sender).Name)
                {
                    case "btn_LotOnOff":
                        Button tmp = EreborGUI.LotOnOff;
                        if (tmp.BackColor == Color.Red)
                        {
                            EreborGUI.Invoke(new MethodInvoker(delegate
                            {
                                EreborGUI.LotOnOff.BackColor = Color.Green;
                                EreborGUI.LotOnOff.Text = "Lot On";
                            }));
                            Autolot = new Autolot();
                            Autolot.Start();
                        }
                        else
                        {
                            EreborGUI.Invoke(new MethodInvoker(delegate
                            {
                                EreborGUI.LotOnOff.BackColor = Color.Red;
                                EreborGUI.LotOnOff.Text = "Lot Off";
                            }));
                            Autolot.Dispose();
                            Autolot = null;
                        }
                        break;
                    case "btn_SetLotBackpack":
                        UO.PrintInformation("Zamer Lotovaci Backpack");
                        SGUI.LotBackpack = new UOItem(UIManager.TargetObject());
                        SetForegroundWindow(Client.HWND);
                        break;
                    case "btn_SetCarvTool":
                        UO.PrintInformation("Zamer nuz");
                        SGUI.CarvTool = new UOItem(UIManager.TargetObject());
                        SetForegroundWindow(Client.HWND);
                        break;
                    case "btn_SetItem1":
                        UO.PrintInformation("Zamer Item");
                        UOItem it = new UOItem(UIManager.TargetObject());
                        SetForegroundWindow(Client.HWND);
                        SGUI.Extend1 = it.Graphic;
                        break;
                    case "btn_SetItem2":
                        UO.PrintInformation("Zamer Item");
                        UOItem it2 = new UOItem(UIManager.TargetObject());
                        SetForegroundWindow(Client.HWND);
                        SGUI.Extend2 = it2.Graphic;
                        break;
                    case "btn_ClearHotkeys":
                        SGUI.SwitchabeHotkeys.Clear();
                        break;
                    case "btn_AddHotkey":
                        SGUI.SwitchabeHotkeys.Add();

                        break;
                    case "btn_SetPoison":
                        UO.PrintInformation("Zamer Poison");
                        UOItem pois = new UOItem(UIManager.TargetObject());
                        SetForegroundWindow(Client.HWND);
                        pois.Click();
                        UO.Wait(200);
                        EreborGUI.Invoke(new MethodInvoker(delegate
                        {
                            EreborGUI.Poison.Text = pois.Name;
                        }));
                        SGUI.Poison = pois.Color;
                        break;
                    case "btn_0":
                        switch (e.TabControlSelectedID)
                        {
                            // Runes  
                            case 0:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    if (EreborGUI.Runes.SelectedNode != null)
                                    {
                                        foreach (Rune r in SGUI.Runes.Runes.Where(run => run.Id.ToString() == EreborGUI.Runes.SelectedNode.Tag.ToString()))
                                        {
                                            SGUI.Runes.findRune(r);
                                            r.RecallSvitek();
                                        }
                                        SetForegroundWindow(Client.HWND);
                                    }
                                }));
                                break;
                            // Weapons
                            case 1:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.Weapons.Items.Clear();
                                    foreach (WeaponSet t in SGUI.Weapons.weapons)
                                    {
                                        ListViewItem lvitem = new ListViewItem(t.Name);
                                        if (t.Weapon == SGUI.Weapons.ActualWeapon.Weapon && t.Shield == SGUI.Weapons.ActualWeapon.Shield)
                                        {
                                            lvitem.SubItems.Add("Actual Weapon");
                                        }
                                        else
                                        {
                                            lvitem.SubItems.Add("- - -");
                                        }
                                        EreborGUI.Weapons.Items.Add(lvitem);
                                    }
                                    EreborGUI.Weapons.Refresh();
                                }));
                                break;
                            // Equips
                            case 2:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    if (EreborGUI.Equips.SelectedItems.Count > 0)
                                    {
                                        SGUI.Equips.equipy[EreborGUI.Equips.SelectedItems[0].Index].DressOnly();
                                        SetForegroundWindow(Client.HWND);
                                    }
                                }));

                                break;
                            // Healed
                            case 3:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.HealedPlayers.Items.Clear();
                                    foreach (Patient t in SGUI.HealedPlayers)
                                    {
                                        t.Character.Click();
                                        UO.Wait(100);
                                        ListViewItem lvitem = new ListViewItem(t.Character.Serial.ToString());
                                        lvitem.SubItems.Add(t.Character.Name ?? "-");
                                        lvitem.SubItems.Add(t.Equip.ToString());

                                        EreborGUI.HealedPlayers.Items.Add(lvitem);
                                    }

                                    EreborGUI.HealedPlayers.Refresh();
                                }));
                                break;
                            // Track Ingored
                            case 4:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.TrackIgnore.Items.Clear();
                                    foreach (string t in SGUI.Tracking.Ignored)
                                    {
                                        ListViewItem lvitem = new ListViewItem(t);
                                        EreborGUI.TrackIgnore.Items.Add(lvitem);
                                    }
                                    EreborGUI.TrackIgnore.Refresh();
                                }));
                                break;

                        }
                        break;
                    case "btn_1":
                        switch (e.TabControlSelectedID)
                        {
                            // Runes
                            case 0:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.Runes.Nodes.Clear();
                                    SGUI.Runes.FillTreeView(EreborGUI.Runes);
                                    EreborGUI.Weapons.Refresh();
                                }));
                                break;
                            // Weapons
                            case 1:
                                break;
                            // Equips
                            case 2:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.Equips.Items.Clear();
                                    foreach (EqSet t in SGUI.Equips.equipy)
                                    {
                                        ListViewItem lvitem = new ListViewItem(t.SetName);
                                        lvitem.SubItems.Add(t.set.Count.ToString());

                                        EreborGUI.Equips.Items.Add(lvitem);
                                    }

                                    EreborGUI.Equips.Refresh();
                                }));

                                break;
                            // Healed
                            case 3:
                                break;
                            // Track Ingored
                            case 4:
                                break;

                        }
                        break;
                    case "btn_2":
                        switch (e.TabControlSelectedID)
                        {
                            // Runes
                            case 0:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    if (EreborGUI.Runes.SelectedNode != null)
                                    {
                                        foreach (Rune r in SGUI.Runes.Runes.Where(run => run.Id.ToString() == EreborGUI.Runes.SelectedNode.Tag.ToString()))
                                        {
                                            SGUI.Runes.findRune(r);
                                            r.Recall();
                                        }
                                        SetForegroundWindow(Client.HWND);
                                    }
                                }));
                                break;
                            // Weapons
                            case 1:
                                SetForegroundWindow(Client.HWND);
                                SGUI.Weapons.Add();


                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.Weapons.Items.Clear();
                                    foreach (WeaponSet t in SGUI.Weapons.weapons)
                                    {
                                        ListViewItem lvitem = new ListViewItem(t.Name);
                                        if (t.Weapon == SGUI.Weapons.ActualWeapon.Weapon && t.Shield == SGUI.Weapons.ActualWeapon.Shield)
                                        {
                                            lvitem.SubItems.Add("Actual Weapon");
                                        }
                                        else
                                        {
                                            lvitem.SubItems.Add("- - -");
                                        }
                                        EreborGUI.Weapons.Items.Add(lvitem);
                                    }
                                    EreborGUI.Weapons.Refresh();
                                }));
                                break;
                            // Equips
                            case 2:

                                UO.PrintInformation("Zamer bath kam odlozit aktualni Equip");
                                SetForegroundWindow(Client.HWND);
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    if (EreborGUI.Equips.SelectedItems.Count > 0)
                                    {
                                        SGUI.Equips.equipy[EreborGUI.Equips.SelectedItems[0].Index].Dress(new UOItem(UIManager.TargetObject()));
                                    }
                                }));
                                break;
                            // Healed
                            case 3:
                                UO.PrintInformation("Zamer Hrace");
                                SetForegroundWindow(Client.HWND);
                                SGUI.HealedPlayers.Add(new UOCharacter(UIManager.TargetObject()));
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.HealedPlayers.Items.Clear();
                                    foreach (Patient t in SGUI.HealedPlayers)
                                    {
                                        t.Character.Click();
                                        UO.Wait(200);
                                        ListViewItem lvitem = new ListViewItem(t.Character.Serial.ToString());
                                        lvitem.SubItems.Add(t.Character.Name ?? "-");
                                        lvitem.SubItems.Add(t.Equip.ToString());

                                        EreborGUI.HealedPlayers.Items.Add(lvitem);
                                    }

                                    EreborGUI.HealedPlayers.Refresh();
                                }));
                                break;
                            // Track Ingored
                            case 4:
                                UO.PrintInformation("Zamer koho ignorovat");
                                SetForegroundWindow(Client.HWND);
                                SGUI.Tracking.Add();


                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.TrackIgnore.Items.Clear();
                                    foreach (string t in SGUI.Tracking.Ignored)
                                    {
                                        ListViewItem lvitem = new ListViewItem(t);
                                        EreborGUI.TrackIgnore.Items.Add(lvitem);
                                    }
                                    EreborGUI.TrackIgnore.Refresh();
                                }));
                                break;

                        }
                        break;
                    case "btn_3":
                        switch (e.TabControlSelectedID)
                        {
                            // Runes
                            case 0:
                                SetForegroundWindow(Client.HWND);
                                SGUI.Runes.GetRunes();
                                break;
                            // Weapons
                            case 1:
                                break;
                            // Equips
                            case 2:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.Equips.Items.Clear();
                                    SetForegroundWindow(Client.HWND);
                                    SGUI.Equips.Add();
                                    foreach (EqSet t in SGUI.Equips.equipy)
                                    {
                                        ListViewItem lvitem = new ListViewItem(t.SetName);
                                        lvitem.SubItems.Add(t.set.Count.ToString());

                                        EreborGUI.Equips.Items.Add(lvitem);
                                    }

                                    EreborGUI.Equips.Refresh();
                                }));
                                break;
                            // Healed
                            case 3:
                                break;
                            // Track Ingored
                            case 4:
                                break;

                        }
                        break;
                    case "btn_4":
                        switch (e.TabControlSelectedID)
                        {
                            // Runes
                            case 0:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    if (EreborGUI.Runes.SelectedNode != null)
                                    {
                                        foreach (Rune r in SGUI.Runes.Runes.Where(run => run.Id.ToString() == EreborGUI.Runes.SelectedNode.Tag.ToString()))
                                        {
                                            SGUI.Runes.findRune(r);
                                            r.Gate();
                                        }
                                        SetForegroundWindow(Client.HWND);
                                    }
                                }));
                                break;
                            // Weapons
                            case 1:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    if (EreborGUI.Weapons.SelectedItems.Count > 0)
                                    {
                                        SGUI.Weapons.Remove(EreborGUI.Weapons.SelectedItems[0].Index);
                                    }
                                }));

                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.Weapons.Items.Clear();
                                    foreach (WeaponSet t in SGUI.Weapons.weapons)
                                    {
                                        ListViewItem lvitem = new ListViewItem(t.Name);
                                        if (t.Weapon == SGUI.Weapons.ActualWeapon.Weapon && t.Shield == SGUI.Weapons.ActualWeapon.Shield)
                                        {
                                            lvitem.SubItems.Add("Actual Weapon");
                                        }
                                        else
                                        {
                                            lvitem.SubItems.Add("- - -");
                                        }
                                        EreborGUI.Weapons.Items.Add(lvitem);
                                    }
                                    EreborGUI.Weapons.Refresh();
                                }));
                                break;
                            // Equips
                            case 2:
                                break;
                            // Healed
                            case 3:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    if (EreborGUI.HealedPlayers.SelectedItems.Count > 0)
                                    {
                                        SGUI.HealedPlayers.Remove(EreborGUI.HealedPlayers.SelectedItems[0].Index);
                                    }
                                }));
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.HealedPlayers.Items.Clear();
                                    foreach (Patient t in SGUI.HealedPlayers)
                                    {
                                        t.Character.Click();
                                        UO.Wait(200);
                                        ListViewItem lvitem = new ListViewItem(t.Character.Serial.ToString());
                                        lvitem.SubItems.Add(t.Character.Name ?? "-");
                                        lvitem.SubItems.Add(t.Equip.ToString());

                                        EreborGUI.HealedPlayers.Items.Add(lvitem);
                                    }

                                    EreborGUI.HealedPlayers.Refresh();
                                }));
                                break;
                            // Track Ingored
                            case 4:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    if (EreborGUI.TrackIgnore.SelectedItems.Count > 0)
                                    {
                                        SGUI.Tracking.Remove(EreborGUI.TrackIgnore.SelectedItems[0].Index);
                                    }
                                }));

                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {
                                    EreborGUI.TrackIgnore.Items.Clear();
                                    foreach (string t in SGUI.Tracking.Ignored)
                                    {
                                        ListViewItem lvitem = new ListViewItem(t);
                                        EreborGUI.TrackIgnore.Items.Add(lvitem);
                                    }
                                    EreborGUI.TrackIgnore.Refresh();
                                }));
                                break;

                        }
                        break;
                    case "btn_5":
                        switch (e.TabControlSelectedID)
                        {
                            // Runes
                            case 0:
                                break;
                            // Weapons
                            case 1:
                                break;
                            // Equips
                            case 2:
                                EreborGUI.Invoke(new MethodInvoker(delegate
                                {

                                    if (EreborGUI.Equips.SelectedItems.Count > 0)
                                    {
                                        SGUI.Equips.Remove(EreborGUI.Equips.SelectedItems[0].Index);
                                    }

                                    EreborGUI.Equips.Items.Clear();
                                    foreach (EqSet t in SGUI.Equips.equipy)
                                    {
                                        ListViewItem lvitem = new ListViewItem(t.SetName);
                                        lvitem.SubItems.Add(t.set.Count.ToString());

                                        EreborGUI.Equips.Items.Add(lvitem);
                                    }

                                    EreborGUI.Equips.Refresh();
                                }));
                                break;
                            // Healed
                            case 3:
                                break;
                            // Track Ingored
                            case 4:
                                break;

                        }
                        break;

                }

            }
            if (sender is CheckBox)
            {
                // AutoMorf
                if (((CheckBox)sender).Name == "chb_AutoMorf")
                {
                    if (SGUI.AutoMorf)
                    {
                        Core.RegisterServerMessageCallback(0x77, automorf);
                        UO.Print("Morf ON");
                    }
                    else
                    {
                        Core.UnregisterServerMessageCallback(0x77, automorf);
                        UO.Print("Morf OFF");
                    }
                }

                // StoodUp
                if (((CheckBox)sender).Name == "chb_PrintStoods")
                {
                    if (SGUI.PrintStoodUp)
                    {
                        Core.RegisterServerMessageCallback(0x6E, onStoodUp);
                        UO.Print("Naprahy ON");
                    }
                    else
                    {
                        Core.UnregisterServerMessageCallback(0x6E, onStoodUp);
                        UO.Print("Naprahy Off");
                    }
                }

            }

            // Save values out of SettingsGUI

            EreborGUI.Invoke(new MethodInvoker(delegate
            {
                GWSDATA.Height = int.Parse(EreborGUI.GW_Height);
                GWSDATA.Width = int.Parse(EreborGUI.GW_Width);
            }));
            TabIndex = e.TabControlSelectedID;

            // Save to xml on change
            // TODO Save btn

           // XmlSerializeHelper<SettingsGUI>.Save(World.Player.Name, SGUI, true);

          //  XmlSerializeHelper<GameWIndoSizeDATA>.Save("WindowSize", GWSDATA, false);


        }


        CallbackResult automorf(byte[] data, CallbackResult prev)
        {

            PacketReader reader = new PacketReader(data);
            reader.Skip(5);
            ushort model = reader.ReadUInt16();
            if ((model == 0x000A) || (model == 0x0009)) model = 39;//demon-summ
            if ((model == 0x00AD)) model = 11;//elder spider
            ByteConverter.BigEndian.ToBytes((ushort)model, data, 5);
            return CallbackResult.Normal;

        }

        CallbackResult onStoodUp(byte[] data, CallbackResult prev)
        {

            PacketReader p = new PacketReader(data);
            p.Skip(1);
            uint serial = p.ReadUInt32();
            ushort action = p.ReadUInt16();
            if ((action == 26 || action == 11) && serial == World.Player.Serial && new UOCharacter(Aliases.LastAttack).Distance < 3)
            {
                UO.Print(SpeechFont.Normal, 0x0076, "Naprah na " + new UOCharacter(Aliases.LastAttack).Name);
            }

            return CallbackResult.Normal;
        }



    }

}

