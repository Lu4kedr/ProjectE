using Phoenix;
using Phoenix.Communication.Packets;
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
        //public bool Running = false;
        bool CrystalOn = false;
        bool BandageDone = true;
        public bool MusicDone = true;
        bool RessurectDone = true;
        bool Paralyze = false;
        int PatientMinHits;
        DateTime StartBandage = DateTime.Now;
        Action<bool,bool> SelfHarm;
        string HealCmd;
        string CrystalCmd;
        HealedPlayers HP;
        Weapons Weapon;
        Watcher ev;
        SettingsGUI Settings;
        private bool healRun = false;
        Patient temp;
        int ParaX, ParaY;
        private DateTime ResTime;


        readonly string[] crystalOnCalls = { " v normalnim stavu.", " schopen rychleji lecit ", " schopen lecit lepe.", " navracena spotrebovana magenergie.", " kouzlit za mene many." };
        readonly string[] bandageDoneCalls = { " byl uspesne osetren", "leceni se ti nepovedlo.", "prestal krvacet", " neni zranen.", "nevidis cil.", "cil je moc daleko." };
        readonly string[] ressurectionCalls = { "duch neni ve ", "ozivil jsi", "ozivila jsi" };
        readonly string[] musicDoneCalls = { " have no musical instrument ", "uklidneni se povedlo.", " neni co uklidnovat!", "uklidnovani nezabralo.", "tohle nemuzes ", "you play poorly.", "oslabeni uspesne.", "oslabeni se nepovedlo.", " tve moznosti." };

        public bool ShamanCast { get;  set; }

        public bool HealRun
        {
            get
            {
                return healRun;
            }

            set
            {
                healRun = value;
            }
        }

        public AutoHeal(HealedPlayers HealedPlayers, string HealCmd, string CrystalCmd, Weapons weapons, Watcher watcher, SettingsGUI settings, int PatientMinHP,Action<bool,bool> selfHarm)
        {
            HP = HealedPlayers;
            this.HealCmd = HealCmd;
            this.CrystalCmd = CrystalCmd;
            Weapon = weapons;
            ev = watcher;
            Settings = settings;
            PatientMinHits = PatientMinHP;
            SelfHarm = selfHarm;
            //ev.OnBandageDone += Ev_OnBandageDone;

            //ev.OnCrystalChange += Ev_OnCrystalOn;
            //ev.OnMusicDone += Ev_OnMusicDone;
            ev.OnParalyze += Ev_OnParalyze;
            //ev.OnRessurectionDone += Ev_OnRessurectionDone;


        }



        public void AutoHealing(ref bool Runn)
        {
            if (Runn)
            {
                HealRun = true;
                Core.RegisterServerMessageCallback(0x1C, OnCalls);
                UO.PrintInformation("Autoheal On");
            }
            try
            {
                while (Runn)
                {
                    UO.Wait(200);


                    if (World.Player.Hidden || !RessurectDone || Paralyze || ShamanCast)
                    {
                        if (Paralyze && Settings.AutoHarm)
                        {
                            Paralyze = false;
                            SelfHarm(false,false);
                        }
                        if (Paralyze && (World.Player.X != ParaX || World.Player.Y != ParaY))
                        {
                            Paralyze = false;
                        }
                        if(!RessurectDone)
                        {
                            if(DateTime.Now-ResTime>TimeSpan.FromSeconds(10))
                            {
                                RessurectDone = true;
                            }
                        }
                        continue;
                    }
                    if (World.Player.Hits < short.Parse(Settings == null ? "80" : Settings.Hits2Bandage ?? "80"))
                    {
                        Bandage();
                    }
                    //UO.PrintInformation("Hleadam koho bandit");
                    temp = HP.GetPatient(PatientMinHits);
                    if (temp == null)
                    {
                        if (CrystalOn) UO.Say(CrystalCmd);
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
                                if ((DateTime.Now - StartBandage).TotalSeconds > 5)
                                {
                                    BandageDone = true;
                                    UO.Print("Error - Too long bandage");
                                }
                            }
                            temp = null;
                        }
                    }
                }
            }
            catch (Exception ex) { UO.PrintError(ex.Message); }
            finally
            {
                HealRun = false;
                Core.UnregisterServerMessageCallback(0x1C, OnCalls);
                UO.PrintError("Heal Off");
            }
        }

        private void GetStatuses()
        {
            foreach (var p in HP)
            {
                if (p.Character.Distance < 11) p.Character.RequestStatus(50);
            }
        }



        //#region Events Handle
        //private void Ev_OnRessurectionDone(object sender, EventArgs e)
        //{
        //    RessurectDone = true;
        //}

        private void Ev_OnParalyze(object sender, EventArgs e)
        {
            Paralyze = true;
            ParaX = World.Player.X;
            ParaY = World.Player.Y;
        }

        //private void Ev_OnMusicDone(object sender, EventArgs e)
        //{
        //    MusicDone = true;
        //}

        //private void Ev_OnCrystalOn(object sender, OnCrystalChangeArgs e)
        //{
        //    CrystalOn = e.On;
        //}

        //private void Ev_OnBandageDone(object sender, EventArgs e)
        //{
        //    BandageDone = true;
        //}

        //#endregion






        private void Bandage(Patient temp)
        {
            if (!BandageDone || temp.Character.Hits < 1 || temp.Character.Distance > 6 || temp.Character.Hits > PatientMinHits || temp.Character.Hits > 100 ) return;
            BandageDone = false;
            StartBandage = DateTime.Now;
            if (!CrystalOn) UO.Say(CrystalCmd);
            UO.Wait(50);
            temp.Heal(HealCmd);
            UO.Wait(200);
            if (Settings.KlerikShaman == 1 && CrystalOn)
            {
                UO.Say(CrystalCmd);
            }
            if (Weapon.ActualWeapon == null ? false : true)
            {
                Weapon.ActualWeapon.Equip();
            }

        }

        public void Bandage()
        {
            if(((DateTime.Now - StartBandage).TotalSeconds > 6))
                BandageDone = true;
            if (!BandageDone || World.Player.Hits == World.Player.MaxHits)
            {
                if (Weapon == null ? false : true && new UOItem(Weapon.ActualWeapon.Weapon).Layer == Layer.None)
                {
                    Weapon.ActualWeapon.Equip();
                }
                return;
            }
            if (Weapon == null ? false : true && new UOItem(Weapon.ActualWeapon.Weapon).Layer == Layer.None)
            {
                Weapon.ActualWeapon.Equip();
            }

            BandageDone = false;
            StartBandage = DateTime.Now;
            UO.Say(HealCmd + "15");
            UO.Wait(200);
            if (Weapon == null ? false : true && new UOItem(Weapon.ActualWeapon.Weapon).Layer == Layer.None)
            {
                Weapon.ActualWeapon.Equip();
            }
            
            if (HealRun) GetStatuses();
        }

        public void Res()
        {
            ResTime = DateTime.Now;
            bool first = true;
            UOItem bandage;
            if (Settings.KlerikShaman == 1)
                bandage = new UOItem(World.Player.Backpack.AllItems.FindType(0x0E20));
            else
                bandage = new UOItem(World.Player.Backpack.AllItems.FindType(0x0E21));
            World.FindDistance = 3;
            Journal.Clear();
            foreach (UOItem corps in World.Ground.Where(x => x.Graphic == 0x2006).ToList())
            {
                if (first)
                {
                    RessurectDone = false;
                    UO.Wait(200);
                    first = false;
                }
                corps.WaitTarget();
                bandage.Use();
                UO.Wait(200);
                if (Journal.Contains("Jako Priest nemuzes ozivovat")) continue;
                if (Journal.Contains("Jako Shaman nemuzes takhle ozivovat.")) continue;

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




        CallbackResult OnCalls(byte[] data, CallbackResult prev)
        {
            AsciiSpeech speech = new AsciiSpeech(data);
            foreach (string s in ressurectionCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    RessurectDone = true;
                    return CallbackResult.Normal;
                }
            }

            foreach (string s in musicDoneCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    MusicDone = true;
                    return CallbackResult.Normal;

                }
            }

            foreach (string s in bandageDoneCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    BandageDone = true;
                    return CallbackResult.Normal;

                }
            }
            foreach (string s in crystalOnCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    if (crystalOnCalls[0] == s)
                        CrystalOn = false;
                    else CrystalOn = true;
                    return CallbackResult.Normal;

                }
            }

            return CallbackResult.Normal;
        }
    }
}

