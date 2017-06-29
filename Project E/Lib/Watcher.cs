using Phoenix;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Linq;
using System.ComponentModel;
using System.Threading;

namespace Project_E.Lib
{
    public class Watcher
    {
        public event EventHandler OnMusicDone;
        public event EventHandler<OnCrystalChangeArgs> OnCrystalChange;
        public event EventHandler OnBandageDone;
        public event EventHandler OnSuccessHit;
        public event EventHandler OnRessurectionDone;
        public event EventHandler OnParalyze;
        public event EventHandler<HiddenChangedArgs> HiddenChanged;
        public event EventHandler<HitsChangedArgs> HitsChanged;


        readonly string[] musicDoneCalls = { " have no musical instrument ", "uklidneni se povedlo.", " neni co uklidnovat!", "uklidnovani nezabralo.", "tohle nemuzes ", "you play poorly.", "oslabeni uspesne.", "oslabeni se nepovedlo.", " tve moznosti." };
        readonly string[] crystalOnCalls = { " v normalnim stavu." ," schopen rychleji lecit ", " schopen lecit lepe.", " navracena spotrebovana magenergie.", " kouzlit za mene many." };
        readonly string[] bandageDoneCalls = { " byl uspesne osetren", "leceni se ti nepovedlo.", "prestal krvacet", " neni zranen.", "nevidis cil.","cil je moc daleko." };
        readonly string[] onHitCalls = { " cil krvaci.", "skvely zasah!", "kriticky zasah!", "vysavas staminu", "vysavas zivoty!" };
        readonly string[] ressurectionCalls = { "duch neni ve ", "ozivil jsi", "ozivila jsi" };
        readonly string[] onParaCalls = { "nohama ti projela silna bolest", "citis, ze se nemuzes hybat.", " crying awfully." };
        readonly UOPlayer Player;

        private bool RessDon;
        private bool CrystalOn;
        private bool Paralyze;
        private bool Onhit;
        private bool BandageDon;
        private bool MusicDon;
        private bool HiddenState;
        private short Hits;
        private bool Poison;
        //private BackgroundWorker bw;
        private bool CrystalState;
        System.Timers.Timer bw;

        public Watcher()
        {
            Core.RegisterServerMessageCallback(0x1C, OnCallsA);
            Core.RegisterServerMessageCallback(0x1C, OnCallsB);
            Player = World.Player;
            bw = new System.Timers.Timer(250);
            bw.Elapsed += Bw_Elapsed;
            bw.Start();
            //bw = new BackgroundWorker();
            //bw.WorkerSupportsCancellation = true;
            //bw.DoWork += Bw_DoWork;
            //bw.RunWorkerAsync();
        }

        private void Bw_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Player.Hidden != HiddenState)
            {
                EventHandler<HiddenChangedArgs> temp = HiddenChanged;
                if (temp != null)
                {
                    foreach (EventHandler<HiddenChangedArgs> ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, new HiddenChangedArgs() { State = Player.Hidden }, null, null);
                    }
                }

                HiddenState = Player.Hidden;
            }

            if (Player.Hits != Hits || Player.Poisoned != Poison)
            {
                HitsChangedArgs args = new HitsChangedArgs(Player.Hits < Hits ? false : true, (short)Math.Abs(Hits - Player.Hits), Player.Poisoned);
                Hits = Player.Hits;
                Poison = Player.Poisoned;

                EventHandler<HitsChangedArgs> temp = HitsChanged;
                if (temp != null)
                {
                    foreach (EventHandler<HitsChangedArgs> ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, args, null, null);
                    }
                }
            }

            if (RessDon)
            {
                RessDon = false;
                EventHandler temp = OnRessurectionDone;
                if (temp != null)
                {
                    foreach (EventHandler ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, null, null, null);
                    }
                }

            }

            if (MusicDon)
            {
                MusicDon = false;
                EventHandler temp = OnMusicDone;
                if (temp != null)
                {
                    foreach (EventHandler ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, null, null, null);
                    }
                }

            }

            if (BandageDon)
            {
                BandageDon = false;
                EventHandler temp = OnBandageDone;
                if (temp != null)
                {
                    foreach (EventHandler ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, null, null, null);
                    }
                }
            }

            if (Onhit)
            {
                Onhit = false;
                EventHandler temp = OnSuccessHit;
                if (temp != null)
                {
                    foreach (EventHandler ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, null, null, null);
                    }
                }
            }

            if (CrystalOn)
            {
                CrystalOn = false;
                EventHandler<OnCrystalChangeArgs> temp = OnCrystalChange;
                if (temp != null)
                {
                    foreach (EventHandler<OnCrystalChangeArgs> ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, new OnCrystalChangeArgs() { On = CrystalState }, null, null);
                    }
                }
            }

            if (Paralyze)
            {
                Paralyze = false;
                EventHandler temp = OnParalyze;
                if (temp != null)
                {
                    foreach (EventHandler ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, null, null, null);
                    }
                }
            }
        }


        ///// <summary>
        ///// Periodicaly check and fires events.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Bw_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    while (!bw.CancellationPending)
        //    {
        //        if (Player.Hidden != HiddenState)
        //        {
        //            EventHandler<HiddenChangedArgs> temp = HiddenChanged;
        //            if (temp != null)
        //            {
        //                foreach (EventHandler<HiddenChangedArgs> ev in temp.GetInvocationList())
        //                {
        //                    ev.BeginInvoke(this, new HiddenChangedArgs() { State=Player.Hidden}, null, null);
        //                }
        //            }

        //            HiddenState = Player.Hidden;
        //        }

        //        if (Player.Hits != Hits || Player.Poisoned != Poison)
        //        {
        //            HitsChangedArgs args = new HitsChangedArgs(Player.Hits < Hits ? false : true, (short)Math.Abs(Hits - Player.Hits), Player.Poisoned);
        //            Hits = Player.Hits;
        //            Poison = Player.Poisoned;

        //            EventHandler<HitsChangedArgs> temp = HitsChanged;
        //            if (temp != null)
        //            {
        //                foreach (EventHandler<HitsChangedArgs> ev in temp.GetInvocationList())
        //                {
        //                    ev.BeginInvoke(this, args, null, null);
        //                }
        //            }
        //        }

        //        if (RessDon)
        //        {
        //            RessDon = false;
        //            EventHandler temp = OnRessurectionDone;
        //            if (temp != null)
        //            {
        //                foreach (EventHandler ev in temp.GetInvocationList())
        //                {
        //                    ev.BeginInvoke(this, null, null, null);
        //                }
        //            }

        //        }

        //        if (MusicDon)
        //        {
        //            MusicDon = false;
        //            EventHandler temp = OnMusicDone;
        //            if (temp != null)
        //            {
        //                foreach (EventHandler ev in temp.GetInvocationList())
        //                {
        //                    ev.BeginInvoke(this, null, null, null);
        //                }
        //            }

        //        }

        //        if (BandageDon)
        //        {
        //            BandageDon = false;
        //            EventHandler temp = OnBandageDone;
        //            if (temp != null)
        //            {
        //                foreach (EventHandler ev in temp.GetInvocationList())
        //                {
        //                    ev.BeginInvoke(this, null, null, null);
        //                }
        //            }
        //        }

        //        if (Onhit)
        //        {
        //            Onhit = false;
        //            EventHandler temp = OnSuccessHit;
        //            if (temp != null)
        //            {
        //                foreach (EventHandler ev in temp.GetInvocationList())
        //                {
        //                    ev.BeginInvoke(this, null, null, null);
        //                }
        //            }
        //        }

        //        if (CrystalOn)
        //        {
        //            CrystalOn = false;
        //            EventHandler<OnCrystalChangeArgs> temp = OnCrystalChange;
        //            if (temp != null)
        //            {
        //                foreach (EventHandler<OnCrystalChangeArgs> ev in temp.GetInvocationList())
        //                {
        //                    ev.BeginInvoke(this, new OnCrystalChangeArgs() { On = CrystalState }, null, null);
        //                }
        //            }
        //        }

        //        if (Paralyze)
        //        {
        //            Paralyze = false;
        //            EventHandler temp = OnParalyze;
        //            if (temp != null)
        //            {
        //                foreach (EventHandler ev in temp.GetInvocationList())
        //                {
        //                    ev.BeginInvoke(this, null, null, null);
        //                }
        //            }
        //        }

        //        Thread.Sleep(200);
        //    }
        //}



        public class HitsChangedArgs : EventArgs
        {

            private bool Gain;
            private short Amount;
            private bool Poison;

            public HitsChangedArgs(bool gain, short amount, bool poison)
            {
                Gain = gain;
                Amount = amount;
                Poison = poison;
            }

            public bool gain { get { return Gain; } }
            public bool poison { get { return Poison; } }
            public short amount { get { return Amount; } }
        }

        public class OnCrystalChangeArgs : EventArgs
        {
            public bool On { get; set; }
        }

        public class HiddenChangedArgs : EventArgs
        {
            public bool State { get; set; }
        }


        CallbackResult OnCallsA(byte[] data, CallbackResult prev)
        {
            AsciiSpeech speech = new AsciiSpeech(data);
            foreach (string s in ressurectionCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    RessDon = true;
                    return CallbackResult.Normal;
                }
            }

            foreach (string s in musicDoneCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    MusicDon = true;
                    return CallbackResult.Normal;

                }
            }

            foreach (string s in bandageDoneCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    BandageDon = true;
                    return CallbackResult.Normal;

                }
            }

            return CallbackResult.Normal;
        }



        CallbackResult OnCalls(byte[] data, CallbackResult prev)
        {
            var tmp = 0;
            AsciiSpeech speech = new AsciiSpeech(data);
            if (musicDoneCalls.Any(x => speech.Text.ToLowerInvariant().Contains(x))) tmp = 1;
            if (crystalOnCalls.Any(x => speech.Text.ToLowerInvariant().Contains(x))) tmp = 2;
            if (bandageDoneCalls.Any(x => speech.Text.ToLowerInvariant().Contains(x))) tmp = 3;
            if (onHitCalls.Any(x => speech.Text.ToLowerInvariant().Contains(x))) tmp = 4;
            if (ressurectionCalls.Any(x => speech.Text.ToLowerInvariant().Contains(x))) tmp = 5;
            if (onParaCalls.Any(x => speech.Text.ToLowerInvariant().Contains(x))) tmp = 6;
            switch(tmp)
            {
                case 0:
                    return CallbackResult.Normal;
                case 1:
                    MusicDon = true;
                    break;
                case 2:
                    CrystalOn = true;
                    if (crystalOnCalls[0] == (crystalOnCalls.First(x=>speech.Text.ToLowerInvariant().Contains(x))))
                        CrystalState = false;
                    else CrystalState = true;
                    break;
                case 3:
                    BandageDon = true;
                    break;
                case 4:
                    Onhit = true;
                    break;
                case 5:
                    RessDon = true;
                    break;
                case 6:
                    Paralyze = true;
                    break;

            }

            return CallbackResult.Normal;
        }





        CallbackResult OnCallsB(byte[] data, CallbackResult prev)
        {
            AsciiSpeech speech = new AsciiSpeech(data);

            foreach (string s in onHitCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    Onhit = true;
                    return CallbackResult.Normal;

                }
            }

            foreach (string s in crystalOnCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    CrystalOn = true;
                    if (crystalOnCalls[0] == s)
                        CrystalState = false;
                    else CrystalState = true;
                    return CallbackResult.Normal;

                }
            }
            foreach (string s in onParaCalls)
            {

                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    Paralyze = true;
                    return CallbackResult.Normal;

                }
            }

            return CallbackResult.Normal;
        }

    }
}
