using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace _15TileGame
{
    public static class AudioManager
    {
        private static Song backgroundMusic;                            
        private static Dictionary<string, SoundEffect> soundEffectDict; //Dictionary that uses the string name of the sound
                                                                        //  as the index to return sound effects

        private static bool muted = false;          //Whether audio (music & sound) is currently muted
        private const float maxMusicVolume = 0.7f;  //The default bg music volume when audio is not muted

        public static void LoadAudioContent(ContentManager content)
        {
            //Load the background music from content
            backgroundMusic = content.Load<Song>("Audio/backgroundMusic");

            //Load all sound effects into the dictionary from content with their corresponding names
            soundEffectDict = new Dictionary<string, SoundEffect>()
            {
                { "tileMove", content.Load<SoundEffect>("Audio/tileMove") },
                { "tileCannotMove", content.Load<SoundEffect>("Audio/tileCannotMove") },
                { "tileBreak", content.Load<SoundEffect>("Audio/tileBreak") },
                { "buttonClick", content.Load<SoundEffect>("Audio/buttonClick") },
                { "buttonHover", content.Load<SoundEffect>("Audio/buttonHover") },
                { "puzzleSolved", content.Load<SoundEffect>("Audio/puzzleSolved") }
            };

            //Start playing background music at the default volume, setting it to loop
            MediaPlayer.Volume = maxMusicVolume;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);
        }

        public static void PlaySoundEffectWithName(string name)
        {
            //Don't bother playing the sound if audio is muted
            if (muted)
                return;

            //If the dictionary contains a sound effect with the key
            //  of the given name, play this sound effect.
            if (soundEffectDict.ContainsKey(name))
            {
                soundEffectDict[name].Play();
            }
        }

        public static bool ToggleAudio()
        {
            //Flip the muted bool to toggle audio on/off
            muted = !muted;
            //If muted, set music volume to 0. Otherwise set it to the default max value
            if (muted)
            {
                MediaPlayer.Volume = 0f;
            }
            else
            {
                MediaPlayer.Volume = maxMusicVolume;
            }
            return muted;
        }
    }
}
