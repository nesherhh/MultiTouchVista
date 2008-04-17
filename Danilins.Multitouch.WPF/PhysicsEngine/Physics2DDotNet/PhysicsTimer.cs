#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion




#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Threading;


namespace Physics2DDotNet
{

    /// <summary>
    /// The State of a PhysicsTimer
    /// </summary>
    public enum TimerState
    {
        NotStarted,
        /// <summary>
        /// The PhysicsTimer is Paused.
        /// </summary>
        Paused,
        /// <summary>
        /// The PhysicsTimer's calls to the Callback are on time.
        /// </summary>
        Normal,
        /// <summary>
        /// The PhysicsTimer's calls to the Callback are behind schedule.
        /// </summary>
        Slow,
        /// <summary>
        /// The PhysicsTimer's calls to the Callback are delayed to be on time.
        /// </summary>
        Fast,
        /// <summary>
        /// The PhysicsTimer is Disposed.
        /// </summary>
        Disposed,
    }
    /// <summary>
    /// A Callback used by the PhysicsTimer
    /// </summary>
    /// <param name="dt">The change in time.</param>
    public delegate void PhysicsCallback(Scalar dt);

    /// <summary>
    /// A class to update the PhysicsEngine at regular intervals.
    /// </summary>
    public sealed class PhysicsTimer : IDisposable
    {
        #region static
        static int threadCount;
        #endregion
        #region fields
        bool isBackground;
        bool isDisposed;
        bool isRunning;

        TimerState state;
        Scalar targetInterval;
        PhysicsCallback callback;
        AutoResetEvent waitHandle;
        Thread engineThread; 
        #endregion
        #region constructors
        /// <summary>
        /// Creates a new PhysicsTimer Instance.
        /// </summary>
        /// <param name="callback">The callback to call.</param>
        /// <param name="targetDt">The target change in time.</param>
        public PhysicsTimer(PhysicsCallback callback, Scalar targetInterval)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }
            if (targetInterval <= 0) { throw new ArgumentOutOfRangeException("targetInterval"); }
            this.isBackground = true;
            this.state = TimerState.NotStarted;
            this.targetInterval = targetInterval;
            this.callback = callback;
            this.waitHandle = new AutoResetEvent(true);
        } 
        #endregion
        #region properties
        /// <summary>
        /// Gets or sets a value indicating whether or not the thread that runs the time is a background thread.
        /// </summary>
        public bool IsBackground
        {
            get { return isBackground; }
            set
            {
                if (isDisposed) { throw new ObjectDisposedException(typeof(PhysicsTimer).Name); }
                if (isBackground ^ value)
                {
                    isBackground = value;
                    if (engineThread != null)
                    {
                        engineThread.IsBackground = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets and Sets if the PhysicsTimer is currently calling the Callback.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                if (isDisposed) { throw new ObjectDisposedException(typeof(PhysicsTimer).Name); }
                if (this.isRunning ^ value)
                {
                    this.isRunning = value;
                    if (value)
                    {
                        if (this.engineThread == null)
                        {
                            this.engineThread = new Thread(EngineProcess);
                            this.engineThread.IsBackground = isBackground;
                            this.engineThread.Name = string.Format("PhysicsEngine Thread: {0}", Interlocked.Increment(ref threadCount));
                            this.engineThread.Start();
                        }
                        else
                        {
                            waitHandle.Set();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets and Sets the desired Interval between Callback calls.
        /// </summary>
        public Scalar TargetInterval
        {
            get { return targetInterval; }
            set
            {
                if (isDisposed) { throw new ObjectDisposedException(this.ToString()); }
                if (value <= 0) { throw new ArgumentOutOfRangeException("value"); }
                this.targetInterval = value;
            }
        }
        /// <summary>
        /// Gets the current State of the PhysicsTimer.
        /// </summary>
        public TimerState State
        {
            get { return state; }
        }
        /// <summary>
        /// Gets and Sets the current Callback that will be called.
        /// </summary>
        public PhysicsCallback Callback
        {
            get { return callback; }
            set
            {
                if (isDisposed) { throw new ObjectDisposedException(this.ToString()); }
                if (value == null) { throw new ArgumentNullException("value"); }
                callback = value;
            }
        } 
        #endregion
        #region methods
        /// <summary>
        /// Stops the Timer 
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                isRunning = false;
                waitHandle.Set();
                waitHandle.Close();
                state = TimerState.Disposed;
            }
        }
        void EngineProcess()
        {
            Scalar desiredDt = targetInterval * 1000;
            DateTime lastRun = DateTime.Now;
            Scalar extraDt = 0;
            while (!isDisposed)
            {
                if (isRunning)
                {
                    DateTime now = DateTime.Now;
                    Scalar dt = (Scalar)(now.Subtract(lastRun).TotalMilliseconds);
                    Scalar currentDt = extraDt + dt;
                    if (currentDt < desiredDt)
                    {
                        state = TimerState.Fast;
                        int sleep = (int)Math.Ceiling(desiredDt - currentDt);
                        waitHandle.WaitOne(sleep, false);
                    }
                    else
                    {
                        extraDt = currentDt - desiredDt;
                        if (extraDt > desiredDt)
                        {
                            extraDt = desiredDt;
                            state = TimerState.Slow;
                        }
                        else
                        {
                            state = TimerState.Normal;
                        }
                        lastRun = now;
                        callback(targetInterval);
                    }
                }
                else
                {
                    state = TimerState.Paused;
                    waitHandle.WaitOne();
                    lastRun = DateTime.Now;
                    extraDt = 0;
                }
            }
            state = TimerState.Disposed;
        } 
        #endregion
    }
}