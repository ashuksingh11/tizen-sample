using System;
using Tizen.Applications;
using Tizen.Multimedia;
using Tizen.System;

namespace MultiTimer.Services
{
    public static class AlertService
    {
        private static Player _player;
        private static bool _initialized;

        private static void EnsureInitialized()
        {
            if (_initialized)
                return;

            try
            {
                _player = new Player();
                string resPath = Application.Current.DirectoryInfo.Resource;
                _player.SetSource(new MediaUriSource(resPath + "alarm.wav"));
                _player.PrepareAsync().Wait();
                _initialized = true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("MultiTimer", $"AlertService init failed: {ex.Message}");
            }
        }

        public static async void PlayAlert()
        {
            // Play vibration
            try
            {
                var feedback = new Feedback();
                try
                {
                    feedback.Play(FeedbackType.Vibration, "Timer");
                }
                catch
                {
                    try
                    {
                        feedback.Play(FeedbackType.Vibration, "General");
                    }
                    catch (Exception ex)
                    {
                        Tizen.Log.Warn("MultiTimer", $"Vibration failed: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Warn("MultiTimer", $"Feedback init failed: {ex.Message}");
            }

            // Play sound
            try
            {
                EnsureInitialized();
                if (_player == null)
                    return;

                if (_player.State == PlayerState.Playing)
                {
                    _player.Stop();
                }

                if (_player.State == PlayerState.Ready || _player.State == PlayerState.Paused)
                {
                    _player.Start();
                }
                else if (_player.State == PlayerState.Idle)
                {
                    await _player.PrepareAsync();
                    _player.Start();
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("MultiTimer", $"Sound playback failed: {ex.Message}");
            }
        }

        public static void StopAlert()
        {
            try
            {
                if (_player != null && _player.State == PlayerState.Playing)
                {
                    _player.Stop();
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Warn("MultiTimer", $"StopAlert failed: {ex.Message}");
            }

            try
            {
                var feedback = new Feedback();
                feedback.Stop();
            }
            catch (Exception ex)
            {
                Tizen.Log.Warn("MultiTimer", $"Stop vibration failed: {ex.Message}");
            }
        }
    }
}
