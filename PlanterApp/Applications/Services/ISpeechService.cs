using System;
using System.Collections.Generic;

namespace PlanterApp.Applications.Services
{
    internal class SpeechEventArgs : EventArgs
    {
        public string Command { get; set; }
    }

    internal interface ISpeechService : IDisposable
    {
        event EventHandler<SpeechEventArgs> PlantSelected;
        event EventHandler<SpeechEventArgs> ApplicationCommand;

        void Initialize(List<int> ids);
        void Speak(string text);
        void Start();
        void Stop();
    }
}
