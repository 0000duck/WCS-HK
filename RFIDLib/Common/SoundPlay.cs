using System;
using System.Collections.Generic;
using System.Media;
using System.Text;

namespace RFIDLib
{
    public class SoundPlay
    {
        SoundPlayer sPlayer = new SoundPlayer();

        public void playSuccess()
        {
            //string path = Environment.CurrentDirectory + "\\SoundSource\\success.wav";
            //sPlayer.SoundLocation = path;
            //sPlayer.Load();
            //sPlayer.Play();
        }

        public void playFail()
        {
            //string path = Environment.CurrentDirectory + "\\SoundSource\\fail.wav";
            //sPlayer.SoundLocation = path;
            //sPlayer.Load();
            //sPlayer.Play();
        }

    }
}