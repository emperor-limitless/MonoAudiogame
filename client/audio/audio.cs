using System;
using System.Collections.Generic;
using System.IO;
using SoLoud;
namespace Audio
{
    public static class Cache
    {
        public static Dictionary<string, Wav> buffers = new();
        public static Dictionary<string, WavStream> streams = new();
    }
    public class Sound
    {
        public readonly int type;
        public Soloud soloud;
        public uint handle { get; private set; }
        public float x;
        public float y;
        public float z;
        public Wav wav;
        public WavStream wavStream;
        internal bool streamed;
        internal bool paused = false;
        public Sound(Soloud soloud, int type = 0)
        {
            this.soloud = soloud;
            this.type = type;
        }
        public bool Load(string filename)
        {
            if (IsActive())
            {
                Destroy();
            }
            if (!File.Exists(filename))
            {
                return false;
            }
            streamed = false;
            if (Cache.buffers.ContainsKey(filename))
            {
                wav = Cache.buffers[filename];
            }
            else
            {
                wav = new();
                wav.load(filename);
                Cache.buffers.Add(filename, wav);
            }
            if (type == 0)
            {
                handle = soloud.play(wav);
            }
            else if (type == 1)
            {
                handle = soloud.play3d(wav, 0, 0, 0, 0, 0, 0);
            }
            return true;
        }
        public bool Stream(string filename)
        {
            if (IsActive())
            {
                Destroy();
            }
            if (!File.Exists(filename))
            {
                return false;
            }
            streamed = true;
            if (Cache.streams.ContainsKey(filename))
            {
                wavStream = Cache.streams[filename];
            }
            else
            {
                wavStream = new();
                wavStream.load(filename);
                Cache.streams.Add(filename, wavStream);
            }
            if (type == 0)
            {
                handle = soloud.play(wavStream);
            }
            else if (type == 1)
            {
                handle = soloud.play3d(wavStream, 0, 0, 0, 0, 0, 0);
            }
            return true;
        }
        public bool Looping
        {
            get
            {
                if (!IsActive())
                {
                    return false;
                }
                return soloud.getLooping(handle) == 1 ? true : false;
            }
            set
            {
                if (!IsActive())
                {
                    return;
                }
                soloud.setLooping(handle, value ? 1 : 0);
            }
        }
        public bool Destroy()
        {
            if (!IsActive())
            {
                return false;
            }
            streamed = false;
            paused = false;
            soloud.stop(handle);
            handle = 0;
            return true;
        }
        public void Pause()
        {
            paused = true;
            soloud.setPause(handle, 1);
        }
        public void Play()
        {
            paused = false;
            soloud.setPause(handle, 0);
        }
        public bool IsActive()
        {
            if (soloud == null || soloud.isValidVoiceHandle(handle) <= 0)
            {
                return false;
            }
            return true;
        }
        public float Volume
        {
            get
            {
                if (!IsActive())
                {
                    return 0;
                }
                return soloud.getVolume(handle);
            }
            set
            {
                if (!IsActive())
                {
                    return;
                }
                if (value > 1.0f) value = 1.0f;
                if (value < 0.1f) value = 0.1f;
                soloud.setVolume(handle, value);
            }
        }
        public ValueTuple<float, float, float> Position
        {
            get
            {
                if (!IsActive() || type != 1)
                {
                    return (0, 0, 0);
                }
                return (x, y, z);
            }
            set
            {
                if (!IsActive() || type != 1)
                {
                    return;
                }
                (this.x, this.y, this.z) = value;
                soloud.set3dSourcePosition(handle, this.x, this.y, this.z);
            }
        }
        public double PlaybackPosition
        {
            get
            {
                if (!IsActive())
                {
                    return 0;
                }
                return soloud.getStreamPosition(handle);
            }
            set
            {
                if (!IsActive())
                {
                    return;
                }
                soloud.seek(handle, value * 1000);
            }
        }
        public bool Playing()
        {
            return PlaybackPosition <= wav.getLength() - 0.005;
        }
    }
    public class SoundPool
    {
        List<Sound> sounds = new();
        Soloud soloud = new();
        uint count = 3;
        public string Path = "sounds";
        float x;
        float y;
        float z;
        public SoundPool()
        {
            soloud.init();
        }
        ~SoundPool()
        {
            soloud.deinit();
        }
        bool hrtf;
        public bool Hrtf
        {
            get
            {
                return hrtf;
            }
            set
            {
                //hrtf = value;
                if (value == true)
                {
                }
                else
                {
                }
            }
        }
        public ValueTuple<float, float, float> Position
        {
            get
            {
                return (x, y, z);
            }
            set
            {
                (x, y, z) = value;
                soloud.set3dListenerPosition(x, y, z); ;
            }
        }
        public Sound Play(string filename, bool looping = false, bool stream = false)
        {
            Sound s = new(soloud);
            if (stream)
            {
                s.Stream($"{Path}/{filename}");
            }
            else
            {
                s.Load($"{Path}/{filename}");
            }
            if (looping)
            {
                s.Looping = looping;
            }
            sounds.Add(s);
            Clean();
            return s;
        }
        public Sound Play3d(string filename, float x = 0.0f, float y = 0.0f, float z = 0.0f, bool looping = false, bool stream = false)
        {
            Sound s = new(soloud, 1);
            if (stream)
            {
                s.Stream($"{Path}/{filename}");
            }
            else
            {
                s.Load($"{Path}/{filename}");
            }
            if (looping)
            {
                s.Looping = looping;
            }
            s.Position = (x, y, z);
            sounds.Add(s);
            Clean();
            return s;
        }
        public void Clean()
        {
            count -= 1;
            if (count == 0)
            {
                count = 3;
                for (int i = 0; i < sounds.Count; i++)
                {
                    if (!sounds[i].IsActive() || sounds[i].Playing() || sounds[i].paused) continue;
                    sounds[i].Destroy();
                    sounds.Remove(sounds[i]);
                    continue;
                }
            }
        }
    }
}
