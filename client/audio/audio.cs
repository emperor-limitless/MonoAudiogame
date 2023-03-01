using System;
using System.Collections.Generic;
using System.IO;
using Synthizer;
namespace Audio
{
    public static class Cache
    {
        public static Dictionary<string, Synthizer.Buffer> buffers = new();
        public static Dictionary<string, StreamHandle> streams = new();
    }
    public class Sound
    {
        public readonly int type;
        public Context ctx;
        public Source src;
        public Generator gen;
        public Synthizer.Buffer buf;
        public StreamHandle sh;
        internal bool streamed;
        internal bool paused = false;
        public Sound(Context ctx, int type = 0)
        {
            this.ctx = ctx;
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
                buf = Cache.buffers[filename];
            }
            else
            {
                buf = new();
                buf.FromFile(filename);
                Cache.buffers.Add(filename, buf);
            }
            gen = new BufferGenerator(ctx);
            BufferGenerator gen2 = gen as BufferGenerator;
            gen2.Buffer.Value = buf;
            if (type == 0)
            {
                src = new DirectSource(ctx);
            }
            else if (type == 1)
            {
                src = new Source3D(ctx);
            }
            gen.ConfigDeleteBehavior(true, 0);
            src.ConfigDeleteBehavior(true, 0);
            src.AddGenerator(gen);
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
                sh = Cache.streams[filename];
            }
            else
            {
                sh = new();
                sh.FromFile(filename);
                Cache.streams.Add(filename, sh);
            }
            gen = new StreamingGenerator();
            StreamingGenerator gen2 = gen as StreamingGenerator;
            gen2.FromStreamHandle(ctx, sh);
            if (type == 0)
            {
                src = new DirectSource(ctx);
            }
            else if (type == 1)
            {
                src = new Source3D(ctx);
            }
            gen.ConfigDeleteBehavior(true, 0);
            src.ConfigDeleteBehavior(true, 0);
            src.AddGenerator(gen);
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
                if (streamed)
                {
                    StreamingGenerator gen2 = gen as StreamingGenerator;
                    return gen2.Looping.Value;
                }
                else
                {
                    BufferGenerator gen2 = gen as BufferGenerator;
                    return gen2.Looping.Value;
                }
            }
            set
            {
                if (!IsActive())
                {
                    return;
                }
                if (streamed)
                {
                    StreamingGenerator gen2 = gen as StreamingGenerator;
                    gen2.Looping.Value = value;
                }
                else
                {
                    BufferGenerator gen2 = gen as BufferGenerator;
                    gen2.Looping.Value = value;
                }
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
            gen.Destroy();
            src.Destroy();
            src = null;
            gen = null;
            return true;
        }
        public void Pause()
        {
            paused = true;
            src.Pause();
        }
        public void Play()
        {
            paused = false;
            src.Play();
        }
        public bool IsActive()
        {
            if (gen == null) return false;
            if (src == null) return false;
            if (ctx == null) return false;
            return true;
        }
        public double Volume
        {
            get
            {
                if (!IsActive())
                {
                    return 0;
                }
                return src.Gain.Value;
            }
            set
            {
                if (!IsActive())
                {
                    return;
                }
                if (value > 1.0) value = 1.0;
                if (value < 0.1) value = 0.1;
                src.Gain.Value = value;
            }
        }
        public ValueTuple<double, double, double> Position
        {
            get
            {
                if (!IsActive() || type != 1)
                {
                    return (0, 0, 0);
                }
                var src2 = src as Source3D;
                return src2.Position.Value;
            }
            set
            {
                if (!IsActive() || type != 1)
                {
                    return;
                }
                var src2 = src as Source3D;
                src2.Position.Value = value;
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
                if (streamed)
                {
                    var gen2 = gen as StreamingGenerator;
                    return gen2.PlaybackPosition.Value;
                }
                else
                {
                    var gen2 = gen as BufferGenerator;
                    return gen2.PlaybackPosition.Value;
                }
            }
            set
            {
                if (!IsActive())
                {
                    return;
                }
                if (streamed)
                {
                    var gen2 = gen as StreamingGenerator;
                    gen2.PlaybackPosition.Value = value;
                }
                else
                {
                    var gen2 = gen as BufferGenerator;
                    gen2.PlaybackPosition.Value = value;
                }
            }
        }
        public bool Playing()
        {
            return PlaybackPosition <= buf.GetLengthInSeconds() - 0.005;
        }
    }
    public class SoundPool
    {
        List<Sound> sounds = new();
        Context ctx = new();
        uint count = 3;
        public string Path = "sounds";
        bool hrtf;
        public bool Hrtf
        {
            get
            {
                return hrtf;
            }
            set
            {
                hrtf = value;
                if (value == true)
                {
                    ctx.DefaultDistanceModel.Value = (int)DistanceModel.Linear;
                    ctx.DefaultRolloff.Value = 2.0;
                    ctx.DefaultPannerStrategy.Value = (int)PannerStrategy.Hrtf;
                }
                else
                {
                    ctx.DefaultPannerStrategy.Value = (int)PannerStrategy.Stereo;
                }
            }
        }
        public ValueTuple<double, double, double> Position
        {
            get
            {
                return ctx.Position.Value;
            }
            set
            {
                ctx.Position.Value = value;
            }
        }
        public Sound Play(string filename, bool looping = false, bool stream = false)
        {
            Sound s = new(ctx);
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
        public Sound Play3d(string filename, double x = 0.0, double y = 0.0, double z = 0.0, bool looping = false, bool stream = false)
        {
            Sound s = new(ctx, 1);
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
