using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using System;
using System.Linq;

namespace Project_E
{
    [RuntimeObject]
    public class Initialize
    {
        private int x = 1;
        Main i;
        public Initialize()
        {
            i = Main.Instance;
        }




        [ServerMessageHandler(0x22)]
        public CallbackResult OnWalkRequestSucceeded(byte[] data, CallbackResult prevResult)
        {
            if (World.Player.Hidden)
            {
                if (prevResult < CallbackResult.Sent)
                {
                    if (x % 5 == 0) UO.Print(0x011C, "Stealth : {0}", x);
                    x++;
                }
            }
            else
            {
                x = 1;
            }
            return CallbackResult.Normal;
        }

        [ServerMessageHandler(0x65)]//weather
        public CallbackResult Filter(byte[] data, CallbackResult prevResult) { return CallbackResult.Eat; }


        [ServerMessageHandler(0x4F)]
        public CallbackResult OnSunLight(byte[] data, CallbackResult prevResult)
        {
            if (prevResult < CallbackResult.Sent)
            {
                if (data[1] > 18)//max 31-tma
                {
                    byte[] newData = new byte[2];
                    newData[0] = 0x4F;
                    newData[1] = (byte)17;
                    Core.SendToClient(newData);

                    // UO.Print(0x015C, "Light level fixed.");
                    return CallbackResult.Sent;
                }
            }
            return CallbackResult.Normal;
        }

        [ServerMessageHandler(0x11)]
        public CallbackResult OnNextTarget(byte[] data, CallbackResult prevResult)
        {
            PacketReader reader = new PacketReader(data);
            if (reader.ReadByte() != 0x11) throw new Exception("Invalid packet passed to OnNextTarget method.");
            ushort blockSize = reader.ReadUInt16();
            uint serial = reader.ReadUInt32();
            if (serial == Aliases.Self)
            {
                return CallbackResult.Normal;
            }
            if (i.SGUI.HealedPlayers.ToList().Find(x => x.Character.Serial == serial) != null) return CallbackResult.Normal;
            Aliases.SetObject("laststatus", serial);
            UOCharacter cil = World.GetCharacter(serial);
            if (cil.MaxHits == -1)
            {
                cil.RequestStatus(50);
                return CallbackResult.Normal;
            }
            else
            {
                ushort color = 0;
                string not = cil.Notoriety.ToString();
                switch (not)
                {

                    case "Criminal":
                        color = 0x0026;
                        break;

                    case "Enemy":
                        color = 0x0031;
                        break;

                    case "Guild":
                        color = 0x0B50;
                        break;

                    case "Innocent":
                        color = 0x0058;
                        break;

                    case "Murderer":
                        color = 0x0026;
                        break;

                    case "Neutral":
                        color = 0x03BC;
                        break;
                    case "Unknown":
                        color = 0x03BC;
                        break;
                    default:
                        color = Phoenix.Env.DefaultInfoColor;
                        break;
                }
                UO.Print(color, "{0} : {1}/{2} ({3})", cil.Name, cil.Hits, cil.MaxHits, cil.Distance);
                return CallbackResult.Normal;
            }
        }

        [ServerMessageHandler(0xa1)]
        public CallbackResult onHpChanged(byte[] data, CallbackResult prevResult)//0xa1
        {
            UOCharacter character = new UOCharacter(Phoenix.ByteConverter.BigEndian.ToUInt32(data, 1));
            if (character.Serial == World.Player.Serial) return CallbackResult.Normal;
            ushort maxHits = 100; // Nejvyssi HITS bez nakouzleni
            ushort hits = Phoenix.ByteConverter.BigEndian.ToUInt16(data, 7);
            ushort[] color = new ushort[4];
            color[0] = 0x0026;//red
            color[2] = 0x0175;//green
            color[1] = 0x099;//yellow
            color[3] = 0x0FAB;//fialova - enemy;
            int col = 0;

            if (character.Hits - hits < -4 || character.Hits - hits > 4)
            {
                if (character.Hits > hits)
                {
                    if (character.Poisoned) col = 2;
                    else col = 0;
                }
                else
                {
                    if (character.Poisoned) col = 2;
                    else col = 1;
                }

                if ((character.Model == 0x0190 || character.Model == 0x0191))
                {
                    character.Print(color[col], "{2} [{0} HP] {1}", ((maxHits / 100) * hits), (hits - character.Hits), character.Name);
                }


                if (character.Serial == Aliases.LastAttack)
                    character.Print(color[3], "[{0} HP] {1}", ((maxHits / 100) * hits), (hits - character.Hits));

            }
            return CallbackResult.Normal;
        }
    }
}
