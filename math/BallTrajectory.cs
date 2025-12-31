using System.Numerics;
using System.Runtime.CompilerServices;
using gameplay.math;
using MemoryPack;

namespace gameplay.ball;

[MemoryPackable]
public partial class BallTrajectory
{
    public const float G = 9.8f;
    public const float NegG = -G;
    public static readonly Vector3E VG = new Vector3E(0, 0, NegG);
    public const float BounceSpeedReduction = 0.7f;
    public const float BounceStopSpeed = 1.5f;
    public const float BallRadius = 0.123f;
    public const float GrounderAcc = 3f;
    //private static float GrounderAcc = 3f;
    //public static float GrounderA { get => GrounderAcc; set => GrounderAcc = value; }

    public struct MotionState
    {
        public Vector3E Position;
        public Vector3E Speed;
        public QuaternionE Rotation;
        public Vector3E RotateAxis;
        public float RotateSpeedRadians;

        [MemoryPackIgnore]
        public bool IsGrounder => Speed.Z.IsNaN();
    }

    public struct MotionKeyFrame
    {
        public float Time;
        public MotionState State;
    }

    [MemoryPackInclude]
    private MotionKeyFrame[] _KeyFrames = { new MotionKeyFrame() { Time = 0f } };

    [MemoryPackIgnore]
    public int KeyFrameCount => _KeyFrames.Length;

    [MemoryPackIgnore]
    public ref MotionState InitState
    {
        get
        {
            return ref _KeyFrames[0].State;
        }
    }
    public void AddKeyFrame(float time, in MotionState state)
    {
        int pos = 0;
        for (; pos < _KeyFrames.Length; ++pos)
        {
            var postime = _KeyFrames[pos].Time;
            if (postime.GTApprox(time))
            {
                break;
            }
        }
        if (pos > 0)
        {
            var postime = _KeyFrames[pos - 1].Time;
            if (postime.EQApprox(time))
            {
                _KeyFrames[pos - 1].State = state;
                return;
            }
        }
        var newframes = new MotionKeyFrame[_KeyFrames.Length + 1];
        for (int i = 0; i < pos; ++i)
        {
            newframes[i] = _KeyFrames[i];
        }
        newframes[pos] = new MotionKeyFrame() { Time = time, State = state };
        for (int i = pos; i < _KeyFrames.Length; ++i)
        {
            newframes[i + 1] = _KeyFrames[i];
        }
        _KeyFrames = newframes;
    }

    public (float time, MotionState state) GetKeyFrameAt(int pos)
    {
        if (pos >= 0 && pos < _KeyFrames.Length)
        {
            ref var keyframe = ref _KeyFrames[pos];
            return (keyframe.Time, keyframe.State);
        }
        return (float.NaN, default);
    }
    public ref MotionState GetKeyStateAt(int pos, out float time)
    {
        if (pos >= 0 && pos < _KeyFrames.Length)
        {
            ref var keyframe = ref _KeyFrames[pos];
            time = keyframe.Time;
            return ref keyframe.State;
        }
        time = float.NaN;
        return ref Unsafe.NullRef<MotionState>();
    }
    public ref MotionState GetKeyStateAt(int pos)
    {
        return ref GetKeyStateAt(pos, out _);
    }
    public int FindKeyFrame(float time)
    {
        int pos = 0;
        for (; pos < _KeyFrames.Length; ++pos)
        {
            ref var keyframe = ref _KeyFrames[pos];
            var postime = keyframe.Time;
            if (postime.EQApprox(time))
            {
                return pos;
            }
            else if (postime > time)
            {
                break;
            }
        }
        return -1;
    }
    public bool TryGetKeyFrame(float time, out MotionState state)
    {
        var pos = FindKeyFrame(time);
        if (pos >= 0)
        {
            ref var keyframe = ref _KeyFrames[pos];
            state = keyframe.State;
            return true;
        }
        state = default;
        return false;
    }
    public ref MotionState GetKeyFrame(float time)
    {
        var pos = FindKeyFrame(time);
        if (pos >= 0)
        {
            return ref _KeyFrames[pos].State;
        }
        return ref Unsafe.NullRef<MotionState>();
    }
    public (bool found, MotionState state) TryRemoveKeyFrame(float time)
    {
        var pos = FindKeyFrame(time);
        if (pos >= 0)
        {
            var removed = _KeyFrames[pos].State;
            var newframes = new MotionKeyFrame[_KeyFrames.Length - 1];
            for (int i = 0; i < pos; ++i)
            {
                newframes[i] = _KeyFrames[i];
            }
            for (int i = pos; i < newframes.Length; ++i)
            {
                newframes[i] = _KeyFrames[i + 1];
            }
            _KeyFrames = newframes;
            return (true, removed);
        }
        return (false, default);
    }
    public void RemoveKeyFrame(float time)
    {
        var pos = FindKeyFrame(time);
        if (pos >= 0)
        {
            var newframes = new MotionKeyFrame[_KeyFrames.Length - 1];
            for (int i = 0; i < pos; ++i)
            {
                newframes[i] = _KeyFrames[i];
            }
            for (int i = pos; i < newframes.Length; ++i)
            {
                newframes[i] = _KeyFrames[i + 1];
            }
            _KeyFrames = newframes;
        }
    }
    public int FindActiveKeyState(float time)
    {
        int pos = 1;
        for (; pos < _KeyFrames.Length; ++pos)
        {
            ref var keyframe = ref _KeyFrames[pos];
            var postime = keyframe.Time;
            if (postime.GTApprox(time))
            {
                break;
            }
        }
        return pos - 1;
    }
    public ref MotionState GetActiveKeyState(float time, out float keyFrameTime)
    {
        var pos = FindActiveKeyState(time);
        ref var keyframe = ref _KeyFrames[pos];
        keyFrameTime = keyframe.Time;
        return ref keyframe.State;
    }
    public ref MotionState GetActiveKeyState(float time)
    {
        return ref GetActiveKeyState(time, out _);
    }

    public BallTrajectory(in MotionState initState)
    {
        InitState = initState;
    }

    [MemoryPackConstructor]
    public BallTrajectory(MotionKeyFrame[] keyFrames)
    {
        _KeyFrames = keyFrames;
    }

    public static implicit operator BallTrajectory(in MotionState initState)
    {
        return new BallTrajectory(initState);
    }
    public static explicit operator MotionState(BallTrajectory trajectory)
    {
        return trajectory.InitState;
    }
    public BallTrajectory(Vector3E initpos, Vector3E initspeed)
    {
        InitState.Position = initpos;
        InitState.Speed = initspeed;
    }

    public static MotionState CalculateState(in MotionState curstate, float aftertime)
    {
        MotionState newstate = curstate;
        newstate.Position += 0.5f * VG * aftertime * aftertime + newstate.Speed * aftertime;
        newstate.Speed += VG * aftertime;
        newstate.Rotation = newstate.Rotation.Concatenate((newstate.RotateAxis, newstate.RotateSpeedRadians * aftertime));
        return newstate;
    }
    public static MotionState CalculateStateGrounder(in MotionState curstate, float aftertime)
    {
        MotionState newstate = curstate;
        var curspeed = curstate.Speed.WithZ(0f);
        var numspeed = curspeed.Length();
        var normspeed = curspeed / numspeed;
        var maxtime = numspeed / GrounderAcc;
        var ballradius = curstate.Position.Z;
        if (aftertime >= maxtime)
        {
            newstate.Speed = Vector3E.Zero;
            newstate.Position += curspeed * maxtime * 0.5f;
            var dist = numspeed * maxtime * 0.5f;
            if (ballradius.NEApprox(0f))
            {
                newstate.Rotation = newstate.Rotation.Concatenate((Vector3E.UnitZ & normspeed, dist / ballradius));
            }
        }
        else
        {
            var deltaSpeed = normspeed * GrounderAcc * aftertime;
            newstate.Speed -= deltaSpeed;
            newstate.Speed.Z = 0f;
            newstate.Position += 0.5f * (curspeed + curspeed - deltaSpeed) * aftertime;
            var dist = (newstate.Position - curstate.Position).Length();
            if (ballradius.NEApprox(0f))
            {
                newstate.Rotation = newstate.Rotation.Concatenate((Vector3E.UnitZ & normspeed, dist / ballradius));
            }
        }
        return newstate;
    }
    public MotionState CalculateState(float aftertime)
    {
        var keypos = FindActiveKeyState(aftertime);
        ref MotionState keyState = ref GetKeyStateAt(keypos, out float keyTime);
        if (keyState.IsGrounder)
        {
            MotionState newstate;
            if (((Vector2E)keyState.Speed).EQApprox(Vector2E.Zero))
            {
                newstate = keyState;
                newstate.Speed.Z = 0f;
            }
            else
            {
                newstate = CalculateStateGrounder(in keyState, aftertime - keyTime);
            }
            return newstate;
        }
        else
        {
            return CalculateState(in keyState, aftertime - keyTime);
        }
    }
    public static (float bouncetime, MotionState bounced) CalculateBounce(in MotionState curstate, float reduction = BounceSpeedReduction, float ballradius = BallRadius)
    {
        var bouncetime = CalculateTimeToGround(in curstate, ballradius);
        var bouncestate = CalculateState(in curstate, bouncetime);
        bouncestate.Speed *= new Vector3E(reduction, reduction, -reduction);
        return (bouncetime, bouncestate);
    }
    public BallTrajectory AppendFreeBounce(float reduction = BounceSpeedReduction, float ballradius = BallRadius, float stopspeed = BounceStopSpeed)
    {
        if (_KeyFrames[_KeyFrames.Length - 1].State.IsGrounder)
        {
            return this;
        }
        while (true)
        {
            ref var last = ref _KeyFrames[_KeyFrames.Length - 1];
            var (bouncetime, bouncestate) = CalculateBounce(in last.State, reduction, ballradius);
            if (bouncestate.Speed.Length().LEApprox(stopspeed))
            {
                bouncestate.Speed = new Vector3E(0f, 0f, float.NaN);
                AddKeyFrame(last.Time + bouncetime, bouncestate);
                break;
            }
            else
            {
                AddKeyFrame(last.Time + bouncetime, bouncestate);
            }
        }
        return this;
    }
    public BallTrajectory TruncateToGrounder(float aftertime = 0f, float forcePosZTo = float.NaN)
    {
        var keypos = FindActiveKeyState(aftertime);
        ref MotionState keyState = ref GetKeyStateAt(keypos, out float keyTime);
        if (keyTime.EQApprox(aftertime))
        {
            var newframes = new MotionKeyFrame[keypos + 1];
            for (int i = 0; i <= keypos; ++i)
            {
                newframes[i] = _KeyFrames[i];
            }
            _KeyFrames = newframes;
            keyState = ref newframes[keypos].State;
        }
        else
        {
            var newframes = new MotionKeyFrame[keypos + 2];
            for (int i = 0; i <= keypos; ++i)
            {
                newframes[i] = _KeyFrames[i];
            }
            newframes[keypos + 1].Time = aftertime;
            _KeyFrames = newframes;
            var grounderinitstate = CalculateState(in keyState, aftertime - keyTime);
            keyState = ref newframes[keypos + 1].State;
            keyState = grounderinitstate;
        }
        keyState.Speed.Z = float.NaN;
        if (!float.IsNaN(forcePosZTo))
        {
            keyState.Position.Z = forcePosZTo;
        }
        return this;
    }

    public static MotionState CalculateInitStateByTime(Vector3E initpos, Vector3E targetpos, float motiontime)
    {
        var trans = targetpos - initpos;
        var speed = trans / motiontime;
        speed += new Vector3E(0, 0, 0.5f * G * motiontime);
        var initstate = new MotionState() { Position = initpos, Speed = speed };
        SetThrowRotation(ref initstate);
        return initstate;
    }
    public static MotionState CalculateInitStateByPitchRadians(Vector3E initpos, Vector3E targetpos, float pitchRadians)
    {
        var trans = targetpos - initpos;
        float distH = ((Vector2E)trans).Length();
        float distV = trans.Z;
        float speedH = distH / MathF.Sqrt((distV - distH * MathF.Tan(pitchRadians)) * 2 / NegG);
        float speedV = MathF.Tan(pitchRadians) * speedH;
        var speedXY = (((Vector2E)trans).Normalize() * speedH);
        var speed = new Vector3E(speedXY, speedV);
        var initstate = new MotionState() { Position = initpos, Speed = speed };
        SetThrowRotation(ref initstate);
        return initstate;
    }
    public static MotionState CalculateInitStateOver(Vector3E initpos, Vector3E targetpos, float distHorizontal, float height)
    {
        var x1 = distHorizontal;
        var y1 = height - initpos.Z;
        var x2 = ((Vector2E)(targetpos - initpos)).Length();
        var y2 = targetpos.Z - initpos.Z;
        var vx = MathF.Sqrt((NegG * (x1 * x2 * (x2 - x1)) / (2f * (x1 * y2 - x2 * y1))));
        var vy = y2 * vx / x2 - x2 * NegG / (2f * vx);
        var vh = ((Vector2E)(targetpos - initpos)).Normalize() * vx;
        var speed = new Vector3E(vh, vy);
        var initstate = new MotionState() { Position = initpos, Speed = speed };
        SetThrowRotation(ref initstate);
        return initstate;
    }
    public static MotionState CalculateInitStateThrough(Vector3E initpos, Vector3E targetpos, Vector3E passpos)
    {
        var dir2p = (Vector2E)(passpos - initpos);
        var dist = dir2p.Length();
        var height = passpos.Z;
        return CalculateInitStateOver(initpos, targetpos, dist, height);
    }
    public static MotionState CalculateInitStateByTimeAndBounceOnce(Vector3E initpos, Vector3E targetpos, float motiontime, out float bouncetime, float reduction = BounceSpeedReduction, float ballradius = BallRadius)
    {
        float initz = initpos.Z - ballradius;
        float tarz = targetpos.Z - ballradius;
        var a = 0.5f * (reduction + 1) * G;
        var b = -(1f + 0.5f * reduction) * G * motiontime;
        var c = initz * reduction + tarz + 0.5f * G * motiontime * motiontime;
        var d = -reduction * initz * motiontime;
        float t0 = EquationSolver.SolveCubicNearest(a, b, c, d, motiontime / 2f);
        float speedV = -initz / t0 + 0.5f * G * t0;
        Vector2E speedH = ((Vector2E)targetpos - (Vector2E)initpos) / (t0 + reduction * (motiontime - t0));
        bouncetime = t0;
        var initstate = new MotionState() { Position = initpos, Speed = new Vector3E(speedH, speedV) };
        SetThrowRotation(ref initstate);
        return initstate;
    }
    public static MotionState CalculateInitStateByTimeAndBounceOnce(Vector3E initpos, Vector3E targetpos, float motiontime, float reduction = BounceSpeedReduction, float ballradius = BallRadius)
    {
        return CalculateInitStateByTimeAndBounceOnce(initpos, targetpos, motiontime, out _, reduction, ballradius);
    }
    public static MotionState CalculateInitStateGrounder(Vector2E initpos, Vector2E targetpos, float motiontime, float ballradius = BallRadius)
    {
        var normspeed = (targetpos - initpos).Normalize();
        var speed = (targetpos - initpos) / motiontime + 0.5f * GrounderAcc * motiontime * normspeed;
        var initstate = new MotionState() { Position = new Vector3E(initpos, ballradius), Speed = new Vector3E(speed, float.NaN) };
        SetThrowRotation(ref initstate);
        return initstate;
    }

    public static MotionState CalculateInitStateBankShot(Vector3E initpos, Vector3E rimpos, PlaneV boardPlane, float pitchRadians, float reduction = BounceSpeedReduction, float ballradius = BallRadius)
    {
        PlaneV boardHitPlane = boardPlane;
        float d1 = boardHitPlane.Normal.Dot((Vector2E)initpos) - boardHitPlane.D;
        if (d1 < 0)
        {
            boardHitPlane.Normal = -boardHitPlane.Normal;
            boardHitPlane.D = -boardHitPlane.D;
            d1 = -d1;
        }
        if (d1 > ballradius)
        {
            boardHitPlane.D += ballradius;
            d1 -= ballradius;
        }
        float d2 = boardHitPlane.Normal.Dot((Vector2E)rimpos) - boardHitPlane.D;
        if (d2 < 0)
        { // the start pos and the target pos are on different sides of the board.
            return CalculateInitStateByPitchRadians(initpos, rimpos, pitchRadians);
        }

        var rimmirror = rimpos.Reflect(boardHitPlane.Normal) + 2 * boardHitPlane.D * (Vector3E)boardHitPlane.Normal;
        var hdv = (Vector2E)(rimmirror - initpos);
        var hdir = hdv.Normalize();
        float l1, l2;
        if (d1.EQApprox(0f) && d2.EQApprox(0f))
        {
            l1 = 0f; l2 = 0f;
        }
        else
        {
            var hd = hdv.Length();
            float sd_1 = 1f / (d1 + d2);
            float hd_sd_1 = hd * sd_1;
            l1 = d1 * hd_sd_1;
            l2 = d2 * hd_sd_1;
        }

        float h0 = initpos.Z;
        float h2 = rimpos.Z;
        float t = MathF.Tan(pitchRadians);
        float r_1 = 1f / reduction;
        float v = MathF.Sqrt((l1 * l1 + 2f * l1 * l2 + l2 * l2 * r_1 * r_1) * G * 0.5f / (h0 - h2 + t * (l1 + l2)));
        return new MotionState() { Position = initpos, Speed = new Vector3E(hdir * v, t * v) };
    }

    public static BallTrajectory Create(Vector3E initpos, Vector3E initspeed)
    {
        return new BallTrajectory(initpos, initspeed);
    }
    public static BallTrajectory CreateByTime(Vector3E initpos, Vector3E targetpos, float motiontime)
    {
        var init = CalculateInitStateByTime(initpos, targetpos, motiontime);
        return new BallTrajectory(in init);
    }
    public static BallTrajectory CreateByPitchRadians(Vector3E initpos, Vector3E targetpos, float pitchRadians)
    {
        var init = CalculateInitStateByPitchRadians(initpos, targetpos, pitchRadians);
        return new BallTrajectory(in init);
    }
    public static BallTrajectory CreateOver(Vector3E initpos, Vector3E targetpos, float distHorizontal, float height)
    {
        var init = CalculateInitStateOver(initpos, targetpos, distHorizontal, height);
        return new BallTrajectory(in init);
    }
    public static BallTrajectory CreateThrough(Vector3E initpos, Vector3E targetpos, Vector3E passpos)
    {
        var init = CalculateInitStateThrough(initpos, targetpos, passpos);
        return new BallTrajectory(in init);
    }
    public static BallTrajectory CreateByTimeAndBounceOnce(Vector3E initpos, Vector3E targetpos, float motiontime, float reduction = BounceSpeedReduction, float ballradius = BallRadius)
    {
        MotionState initState = CalculateInitStateByTimeAndBounceOnce(initpos, targetpos, motiontime, out float bouncetime, reduction, ballradius);
        var traj = new BallTrajectory(in initState);
        var bouncems = CalculateState(in initState, bouncetime);
        bouncems.Speed *= new Vector3E(reduction, reduction, -reduction);
        traj.AddKeyFrame(bouncetime, in bouncems);
        return traj;
    }
    public static BallTrajectory CreateGrounder(Vector2E initpos, Vector2E targetpos, float motiontime, float ballradius = BallRadius)
    {
        var init = CalculateInitStateGrounder(initpos, targetpos, motiontime, ballradius);
        return new BallTrajectory(in init);
    }
    public static BallTrajectory CreateGrounder(Vector2E initpos, Vector2E initspeed, float ballradius = BallRadius)
    {
        return Create(new Vector3E(initpos, ballradius), new Vector3E(initspeed, float.NaN));
    }

    public static BallTrajectory CreateBankShot(Vector3E initpos, Vector3E rimpos, PlaneV boardPlane, float pitchRadians, float reduction = BounceSpeedReduction, float ballradius = BallRadius)
    {
        var init = CalculateInitStateBankShot(initpos, rimpos, boardPlane, pitchRadians, reduction, ballradius);
        var traj = new BallTrajectory(in init);
        var hittime = CalculateTimeToPlane(in init, boardPlane, ballradius);
        var hitstate = CalculateState(in init, hittime);
        hitstate.Speed = hitstate.Speed.Reflect(boardPlane.Normal) * reduction;
        traj.AddKeyFrame(hittime, in hitstate);
        return traj;
    }

    public static (float, float) CalculateTimeToHeight2(in MotionState state, float height)
    {
        float vz = state.Speed.Z;
        float pz = state.Position.Z - height;
        var sqrtdelta = MathF.Sqrt(vz * vz + 2f * G * pz);
        if (float.IsNaN(sqrtdelta))
        {
            return (float.NaN, float.NaN);
        }
        return ((vz - sqrtdelta) / G, (vz + sqrtdelta) / G);
    }
    public static float CalculateTimeToHeight(in MotionState state, float height)
    {
        var (x1, x2) = CalculateTimeToHeight2(in state, height);
        if (x1.LEApprox(0))
        {
            return x2;
        }
        else
        {
            return x1;
        }
    }
    public static float CalculateTimeToGround(in MotionState state, float ballradius = BallRadius)
    {
        return CalculateTimeToHeight(in state, ballradius);
    }
    public static (float, float) CalculateTimeToPlane2(in MotionState state, Plane plane)
    {
        float d = plane.D - (state.Position | plane.Normal);
        float v = state.Speed | plane.Normal;
        float a = VG | plane.Normal;
        if (a.EQApprox(0f))
        { // Vertical Plane
            var t = d / v;
            return (t, t);
        }
        else
        {
            var sqrtdelta = MathF.Sqrt(v * v + 2f * a * d);
            if (float.IsNaN(sqrtdelta))
            {
                return (float.NaN, float.NaN);
            }
            var x1 = (-sqrtdelta - v) / a;
            var x2 = (sqrtdelta - v) / a;
            return (MathF.Min(x1, x2), MathF.Max(x1, x2));
        }
    }
    public static float CalculateTimeToPlane(in MotionState state, Plane plane, float ballradius = BallRadius)
    {
        if (ballradius.NEApprox(0f))
        {
            float d = plane.D - (state.Position | plane.Normal);
            if (d < -ballradius)
            {
                plane.D += ballradius;
            }
            else if (d > ballradius)
            {
                plane.D -= ballradius;
            }
        }

        var (x1, x2) = CalculateTimeToPlane2(in state, plane);
        if (x1.LEApprox(0))
        {
            return x2;
        }
        else
        {
            return x1;
        }
    }
    public static float CalculateTimeToPosition(in MotionState state, Vector3E target, float epsilon = NumberF.ComparisonEpsilon)
    {
        static bool PassPosition(in MotionState state, Vector3E target, float time, float epsilon)
        {
            var lastZ = state.Position.Z + state.Speed.Z * time + 0.5f * NegG * time * time;
            if (NumberF.NEApprox(lastZ, target.Z, epsilon))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        var dir3 = target - state.Position;
        var dir2 = (Vector2E)dir3;
        if (NumberF.NEApprox(dir2 & (Vector2E)state.Speed, 0f, epsilon))
        {
            return float.NaN;
        }
        float len2 = dir2.Length();
        if (len2.EQApprox(0f, 0.01f))
        {
            var planen = dir3.Normalize();
            var planed = target | planen;
            var plane = new Plane(planen, planed);
            var (time1, time2) = CalculateTimeToPlane2(in state, plane);
            if (time1.LEApprox(0))
            {
                if (PassPosition(in state, target, time2, epsilon))
                {
                    return time2;
                }
                else
                {
                    return float.NaN;
                }
            }
            else
            {
                if (PassPosition(in state, target, time1, epsilon))
                {
                    return time1;
                }
                else if (PassPosition(in state, target, time2, epsilon))
                {
                    return time2;
                }
                else
                {
                    return float.NaN;
                }
            }
        }
        else
        {
            float speed2 = ((Vector2E)state.Speed).Length();
            var time = len2 / speed2;
            var lastZ = state.Position.Z + state.Speed.Z * time + 0.5f * NegG * time * time;
            if (NumberF.NEApprox(lastZ, target.Z, epsilon))
            {
                return float.NaN;
            }
            else
            {
                return time;
            }
        }
    }

    public float CalculateTimeToHeight(float height)
    {
        for (int i = 0; i < _KeyFrames.Length; ++i)
        {
            ref var keyFrame = ref _KeyFrames[i];
            var time = CalculateTimeToHeight(in keyFrame.State, height);
            if (!float.IsNaN(time) && (i == _KeyFrames.Length - 1 || _KeyFrames[i].Time.GTApprox(time)))
            {
                return time;
            }
        }
        return float.NaN;
    }
    public float CalculateTimeToGround(float ballradius = BallRadius)
    {
        for (int i = 0; i < _KeyFrames.Length; ++i)
        {
            ref var keyFrame = ref _KeyFrames[i];
            var time = CalculateTimeToGround(in keyFrame.State, ballradius);
            if (!float.IsNaN(time) && (i == _KeyFrames.Length - 1 || _KeyFrames[i].Time.GTApprox(time)))
            {
                return time;
            }
        }
        return float.NaN;
    }
    public float CalculateTimeToPlane(Plane plane, float ballradius = BallRadius)
    {
        for (int i = 0; i < _KeyFrames.Length; ++i)
        {
            ref var keyFrame = ref _KeyFrames[i];
            var time = CalculateTimeToPlane(in keyFrame.State, plane, ballradius);
            if (!float.IsNaN(time) && (i == _KeyFrames.Length - 1 || _KeyFrames[i + 1].Time.GTApprox(time)))
            {
                return time + keyFrame.Time;
            }
        }
        return float.NaN;
    }
    public float CalculateTimeToPosition(Vector3E target, float epsilon = NumberF.ComparisonEpsilon)
    {
        for (int i = 0; i < _KeyFrames.Length; ++i)
        {
            ref var keyFrame = ref _KeyFrames[i];
            var time = CalculateTimeToPosition(in keyFrame.State, target, epsilon);
            if (!float.IsNaN(time) && (i == _KeyFrames.Length - 1 || _KeyFrames[i].Time.GTApprox(time)))
            {
                return time;
            }
        }
        return float.NaN;
    }

    public BallTrajectory WithRotation(QuaternionE rotInit = default, Vector3E rotAxis = default, float rotSpeedRadians = 0)
    {
        if (rotInit != default(QuaternionE))
        {
            InitState.Rotation = rotInit;
        }
        if (rotAxis != default(Vector3E) && rotAxis.EQApprox(rotAxis) && !float.IsNaN(rotSpeedRadians))
        {
            InitState.RotateAxis = rotAxis;
            InitState.RotateSpeedRadians = rotSpeedRadians;
        }
        return this;
    }
    public static (Vector3E rotAxis, float rotSpeedRadians) CalculateThrowRotation(in MotionState initState)
    {
        var speedH = ((Vector2E)initState.Speed).Length();
        var speedV = initState.Speed.Z;
        var pitchangle = NumberF.Atan2(speedV, speedH);
        var axis1 = initState.Speed & Vector3E.UnitZ;
        axis1 = axis1.Normalize();
        var axis2 = axis1 & initState.Speed;
        axis2 = axis2.Normalize();
        var axisfinal = axis1 * 12f * pitchangle + axis2 * 3f * pitchangle;
        var rspeedfinal = axisfinal.Length();
        return (axisfinal / rspeedfinal, rspeedfinal);
    }
    public static ref MotionState SetThrowRotation(ref MotionState initState)
    {
        var (rotAxis, rotSpeed) = CalculateThrowRotation(in initState);
        initState.RotateAxis = rotAxis;
        initState.RotateSpeedRadians = rotSpeed;
        return ref initState;
    }
    public BallTrajectory WithThrowRotation()
    {
        SetThrowRotation(ref InitState);
        return this;
    }
}
