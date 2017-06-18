﻿using Phoenix;
using Phoenix.Communication;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using Project_E.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Project_E.Lib.SpellManager
{
    public delegate void DecodedSpell();
    public class SpellManager
    {
        public event EventHandler<OnSpellDoneArgs> OnSpellDone;
        private byte[] LastData;
        private SettingsGUI Settings;
        DateTime StartCast=DateTime.Now;
        Action Sacrafire;
        Action Bandage;
        public string LastSpell { get; set; }
        //readonly Dictionary<string, int> SpellsDelays = new Dictionary<string, int> {
        //    { "Fireball", 2060 }, { "Flame", 4100 }, { "Meteor", 6750 }, { "Lightning", 3550 },
        //    { "Bolt", 3100 }, { "frostbolt", 3100 },, { "Harm", 1500 }, { "Mind", 3450 }, { "Invis", 3300 }/*-150*/ };

        public static Dictionary<string, int> SpellDelays = new Dictionary<string, int>()
        {
            { "18", 2060 },         // Fireball
            { "52", 4100 },         // Flame Strike
            { "55", 6750 },         // Meteor
            { "40", 3550 },         // Lighting
            { "42", 3100 },         // Energy Bolt
            { ".frostbolt", 3100 },  // Frost
            { ".necrobolt", 3100 },  // Necrobolt
            { "12", 1500 },         // Harm
            { "37", 3500 },         // Mindblast
            { "44", 3300 }          // Invis
        };


        public SpellManager(SettingsGUI settings, Action sacrafire, Action bandage)
        {
            Core.RegisterClientMessageCallback(0x12, SpellReq);
            Core.RegisterClientMessageCallback(0xAD, OnCustomSpell);
            Core.RegisterServerMessageCallback(0x1C, OnSpellFizz);
            Settings = settings;
            Sacrafire = sacrafire;
            Bandage = bandage;

        }


        private void DecodedSpell()
        {
            if (Settings.KlerikShaman == 1 && World.Player.Mana < (World.Player.MaxMana - short.Parse(Settings.Sacrafire ?? "40")))
                Sacrafire();
            if (World.Player.Hits < (World.Player.MaxHits - 7))
                Bandage();
            if (Aliases.GetObject("SpellTarget") != 0)
            {
                UO.Attack(Aliases.GetObject("SpellTarget"));
                UO.WaitTargetObject(Aliases.GetObject("SpellTarget"));
            }
            Core.SendToServer(LastData);
            Aliases.SetObject("SpellTarget", 0);
            StartCast = DateTime.Now;
        }

        public CallbackResult SpellReq(byte[]data , CallbackResult Prev)//0x12
        {
            PacketReader pr = new PacketReader(data);
            pr.Skip(1);
            int length = pr.ReadUInt16();
            string spell = "";
            if (pr.ReadByte() == 0x56)
            {
                spell = pr.ReadAnsiString(length - 4);

                LastSpell = spell;
                LastData = data;
                DecodedSpell ds = new DecodedSpell(DecodedSpell);
                ds.BeginInvoke(null, null);
                return CallbackResult.Eat;


            }


            return CallbackResult.Normal;
        }

        private CallbackResult OnCustomSpell(byte[] data, CallbackResult prevState)//0xAD
        {
            UnicodeSpeechRequest a = new UnicodeSpeechRequest(data);
            if(SpellDelays.ContainsKey(a.Text))
            {

                LastSpell = a.Text;
                LastData = data;
                DecodedSpell ds = new DecodedSpell(DecodedSpell);
                ds.BeginInvoke(null, null);
                return CallbackResult.Eat;
            }
            return CallbackResult.Normal;
        }

        private CallbackResult OnSpellFizz(byte[] data, CallbackResult prev)//0x1C
        {
            AsciiSpeech a = new AsciiSpeech(data);
            if(a.Text.ToLowerInvariant().Contains("kouzlo se nezdarilo."))
            {

                EventHandler<OnSpellDoneArgs> temp = OnSpellDone;
                if (temp != null)
                {
                    foreach (EventHandler<OnSpellDoneArgs> ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, new OnSpellDoneArgs() { Fizzed = true, Spell = LastSpell }, null, null);
                    }
                }
            }

            return CallbackResult.Normal;
        }

        public class OnSpellDoneArgs : EventArgs
        {
            public string Spell { get; set; }
            public bool Fizzed { get; set; }
        }
    }
}
