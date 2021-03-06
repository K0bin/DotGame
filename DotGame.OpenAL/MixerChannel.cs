﻿using DotGame.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Text;

namespace DotGame.OpenAL
{
    public class MixerChannel : AudioObject, IMixerChannel
    {
        public IEffect Effect {
            get { return effect; }
            set
            {
                if (value == null)
                {
                    effect = null;
                    if (ID != -1)
                        AudioDeviceInternal.Efx.BindEffectToAuxiliarySlot(ID, 0);
                }
                else
                {
                    effect = AudioDeviceInternal.Cast<Effect>(value, "Effect");
                    if (ID != -1)
                        AudioDeviceInternal.Efx.BindEffectToAuxiliarySlot(ID, effect.ID);
                }
            }
        }
        private Effect effect;
        public float WetGain { get { return Get(EfxAuxiliaryf.EffectslotGain); } set { Set(EfxAuxiliaryf.EffectslotGain, value); } }

        internal readonly int ID;

        internal MixerChannel(AudioDevice audioDevice, string name) : base(audioDevice)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            if (AudioDevice.Capabilities.SupportsEfx)
            {
                ID = AudioDeviceInternal.Efx.GenAuxiliaryEffectSlot();
                DotGame.OpenAL.AudioDevice.CheckALError();
                AudioDeviceInternal.Efx.AuxiliaryEffectSlot(ID, EfxAuxiliaryi.EffectslotAuxiliarySendAuto, 1);
            }
            else
            {
                ID = -1;
            }
        }

        private void Set(EfxAuxiliaryf param, float value)
        {
            if (ID == -1)
                return;

            AudioDeviceInternal.Efx.AuxiliaryEffectSlot(ID, param, value);
            DotGame.OpenAL.AudioDevice.CheckALError();
        }

        private float Get(EfxAuxiliaryf param)
        {
            if (ID == -1)
                return 0;

            float value;
            AudioDeviceInternal.Efx.GetAuxiliaryEffectSlot(ID, param, out value);
            DotGame.OpenAL.AudioDevice.CheckALError();
            return value;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is IMixerChannel)
                return Equals((IMixerChannel)obj);
            return false;
        }

        /// <inheritdoc/>
        public bool Equals(IMixerChannel other)
        {
            if (other is MixerChannel)
                return ID == ((MixerChannel)other).ID;
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + ID.GetHashCode();
                return hash;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[MixerChannel ID: ");
            builder.Append(ID);
            builder.Append(", WetGain: ");
            builder.Append(WetGain);
            builder.Append(", Effect: ");
            builder.Append(effect);
            builder.Append("]");

            return builder.ToString();
        }
    }
}
