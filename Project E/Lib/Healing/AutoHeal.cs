﻿using Phoenix;
using Phoenix.WorldData;
using Project_E.GUI;
using Project_E.Lib.WeaponsSet;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using static Project_E.Lib.Watcher;

namespace Project_E.Lib.Healing
{
    public class AutoHeal
    {
        public bool Running = false;
        bool CrystalOn = false;
        bool BandageDone = true;
        public bool MusicDone = true;
        bool RessurectDone = true;
        bool Paralyze = false;
        int PatientMinHits;
        DateTime StartBandage = DateTime.Now;
        Action<bool> SelfHarm;
        string HealCmd;
        string CrystalCmd;
        HealedPlayers HP;
        WeaponSet Weapon;
        BackgroundWorker bw;
        Watcher ev;
        SettingsGUI Settings;




        public AutoHeal(HealedPlayers HealedPlayers, string HealCmd, string CrystalCmd, WeaponSet ActualWeapon, Watcher watcher, SettingsGUI settings, int PatientMinHP,Action<bool> selfHarm)
        {
            HP = HealedPlayers;
            this.HealCmd = HealCmd;
            this.CrystalCmd = CrystalCmd;
            Weapon = ActualWeapon;
            ev = watcher;
            Settings = settings;
            PatientMinHits = PatientMinHP;
            SelfHarm = selfHarm;
            ev.OnBandageDone += Ev_OnBandageDone;
        }

        private void GetStatuses()
        {
            foreach (var p in HP)
            {
                if (p.Character.Distance < 11) p.Character.RequestStatus(50);
            }
        }

        public void Start()
        {
            UO.PrintInformation("Debug Heal ON");
            Running = true;
            //bw = new BackgroundWorker();
            //bw.WorkerSupportsCancellation = true;
            //bw.DoWork += Bw_DoWork;


            ev.OnCrystalChange += Ev_OnCrystalOn;
            ev.OnMusicDone += Ev_OnMusicDone;
            ev.OnParalyze += Ev_OnParalyze;
            ev.OnRessurectionDone += Ev_OnRessurectionDone;
            HealRunning(ref Running);
           // bw.RunWorkerAsync();
        }


        public void Stop()
        {
            UO.PrintInformation("Debug Heal OFF");
            Running = false;
            //if (bw != null)
            //{
            //    bw.CancelAsync();
            //    bw.DoWork -= Bw_DoWork;
            //}

            ev.OnCrystalChange -= Ev_OnCrystalOn;
            ev.OnMusicDone -= Ev_OnMusicDone;
            ev.OnParalyze -= Ev_OnParalyze;
            ev.OnRessurectionDone -= Ev_OnRessurectionDone;
        }

        #region Events Handle
        private void Ev_OnRessurectionDone(object sender, EventArgs e)
        {
            RessurectDone = true;
        }

        private void Ev_OnParalyze(object sender, EventArgs e)
        {
            Paralyze = true;
        }

        private void Ev_OnMusicDone(object sender, EventArgs e)
        {
            MusicDone = true;
        }

        private void Ev_OnCrystalOn(object sender, OnCrystalChangeArgs e)
        {
            CrystalOn = e.On;
        }

        private void Ev_OnBandageDone(object sender, EventArgs e)
        {
            BandageDone = true;
        }

        #endregion


        private void HealRunning(ref bool On)
        {
            Patient temp;
            while (On)
            {
                UO.Wait(100);
                if (Paralyze && Settings.AutoHarm)
                {
                    SelfHarm(false);
                    Paralyze = false;
                }
                if (World.Player.Hits < short.Parse(Settings == null ? "80" : Settings.Hits2Bandage ?? "80"))
                {
                    Bandage();
                }
                if (World.Player.Hidden || !RessurectDone || Paralyze) continue;
                temp = HP.GetPatient(PatientMinHits);
                if (temp == null)
                {
                    if (CrystalOn) UO.Say(CrystalCmd);
                    GetStatuses();
                }
                else
                {
                    if (!MusicDone && temp.Character.Hits > 65)
                    {
                        temp = null;
                    }
                    else
                    {
                        if (BandageDone)
                            Bandage(temp);
                        else
                        {
                            if ((DateTime.Now - StartBandage).TotalSeconds > 7)
                            {
                                BandageDone = true;
                                UO.Print("Error");
                            }
                        }
                        temp = null;
                    }
                }
            }
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Patient temp;
            while (!bw.CancellationPending)
            {
                Thread.Sleep(100);
                if (Paralyze && Settings.AutoHarm)
                {
                    SelfHarm(false);
                    Paralyze = false;
                }
                if (World.Player.Hits < short.Parse(Settings == null ? "80" : Settings.Hits2Bandage ?? "80"))
                {
                    Bandage();
                }
                if (World.Player.Hidden || !RessurectDone || Paralyze) continue;
                temp = HP.GetPatient(PatientMinHits);
                if (temp == null)
                {
                    if (CrystalOn) UO.Say(CrystalCmd);
                    GetStatuses();
                }
                else
                {
                    if (!MusicDone && temp.Character.Hits > 65)
                    {
                        temp = null;
                    }
                    else
                    {
                        if (BandageDone)
                            Bandage(temp);
                        else
                        {
                            if ((DateTime.Now - StartBandage).TotalSeconds > 7)
                            {
                                BandageDone = true;
                                UO.Print("Error");
                            }
                        }
                        temp = null;
                    }
                }
            }
        }

        private void Bandage(Patient temp)
        {
            if (!BandageDone || temp.Character.Hits > PatientMinHits || temp.Character.Hits > 100 || temp.Character.Hits < 1 || temp.Character.Distance > 6) return;
            BandageDone = false;
            StartBandage = DateTime.Now;
            if (!CrystalOn) UO.Say(CrystalCmd);
            temp.Heal(HealCmd);
            UO.Wait(200);
            if (Settings.KlerikShaman == 1 && CrystalOn)
            {
                UO.Say(CrystalCmd);
            }
            if (Weapon == null ? false : true)
            {
                Weapon.Equip();
            }

        }

        public void Bandage()
        {
            if (!BandageDone || World.Player.Hits==World.Player.MaxHits) return;

            BandageDone = false;
            StartBandage = DateTime.Now;
            UO.Say(HealCmd + "15");
            if (Weapon == null ? false : true)
            {
                Weapon.Equip();
            }
            if(Running)GetStatuses();
        }

        public void Res()
        {
            RessurectDone = false;
            UOItem bandage;
            if (Settings.KlerikShaman == 1)
                bandage = new UOItem(World.Player.Backpack.AllItems.FindType(0x0E20));
            else
                bandage = new UOItem(World.Player.Backpack.AllItems.FindType(0x0E21));
            World.FindDistance = 3;
            Journal.Clear();
            foreach (UOItem corps in World.Ground.Where(x => x.Graphic == 0x2006).ToList())
            {
                corps.WaitTarget();
                bandage.Use();
                UO.Wait(200);
                if (Journal.Contains("Jako Priest nemuzes ozivovat")) continue;
                if (Journal.Contains("Duch neni ve "))
                {
                    UO.Say(" dej WAR ");
                    return;
                }
                if (Journal.Contains("Necht se navrati "))
                {
                    UO.Say("Resuju ");
                    return;
                }

            }
        }
    }
}

