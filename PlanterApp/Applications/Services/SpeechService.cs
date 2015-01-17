using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace PlanterApp.Applications.Services
{
    //http://msdn.microsoft.com/en-us/library/hh361633

    //[Export(typeof(ISpeechService))]
    //internal class SpeechService : ISpeechService
    //{
    //    private const string GRAMMAR_APPLICATION = "application";
    //    private const string APPLICATION_COMMAND_ACCEPT = "accept";
    //    private const string APPLICATION_COMMAND_REPEAT = "repeat";

    //    private const string GRAMMAR_PLANT = "plant";


    //    private PromptBuilder _promptBuilder;
    //    private SpeechSynthesizer _synthesizer;
    //    private SpeechRecognitionEngine _recognizer;

    //    public event EventHandler<SpeechEventArgs> PlantSelected;
    //    public event EventHandler<SpeechEventArgs> ApplicationCommand;
    //    //private string _currentPhrase;
    //    //private EventHandler<PlantSelectedEventArgs> _pendingAction;
    //    //private PlantSelectedEventArgs _pendingActionArgs;

    //    public SpeechService()
    //    {
    //    }

    //    public void Initialize(List<int> ids)
    //    {
    //        if (ids == null || ids.Count == 0)
    //        {
    //            return;
    //        }

    //        var appGrammar = GetApplicationGrammar();
    //        var plantGrammar = GetPlantGrammar(ids);

    //        if (_recognizer != null)
    //        {
    //            _recognizer.Dispose();
    //        }

    //        _recognizer = new SpeechRecognitionEngine();
    //        _recognizer.LoadGrammar(appGrammar);
    //        _recognizer.LoadGrammar(plantGrammar);
    //        _recognizer.SpeechRecognized += OnSpeechRecognized;
    //        _recognizer.SetInputToDefaultAudioDevice();
    //    }

    //    private Grammar GetApplicationGrammar()
    //    {
    //        var grammarBuilder = new GrammarBuilder();
    //        grammarBuilder.Append(new Choices(APPLICATION_COMMAND_ACCEPT, APPLICATION_COMMAND_REPEAT));
    //        return new Grammar(grammarBuilder) { Name = GRAMMAR_APPLICATION };
    //    }

    //    private Grammar GetPlantGrammar(List<int> ids)
    //    {
    //        var grammarBuilder = new GrammarBuilder();
    //        grammarBuilder.Append("Plant");

    //        var plantChoices = new Choices();
    //        foreach (var id in ids)
    //        {
    //            plantChoices.Add(id.ToString());
    //        }
    //        grammarBuilder.Append(plantChoices);

    //        return new Grammar(grammarBuilder) { Name = GRAMMAR_PLANT };
    //    }

    //    public void Speak(string text)
    //    {
    //        if (string.IsNullOrEmpty(text))
    //        {
    //            return;
    //        }

    //        if (_promptBuilder == null)
    //        {
    //            _promptBuilder = new PromptBuilder();
    //        }
    //        _promptBuilder.ClearContent();
    //        _promptBuilder.AppendText(text);

    //        if (_synthesizer == null)
    //        {
    //            _synthesizer = new SpeechSynthesizer();
    //        }

    //        _synthesizer.Speak(_promptBuilder);
    //    }

    //    public void Start()
    //    {
    //        if (_recognizer != null)
    //        {
    //            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
    //        }
    //    }

    //    public void Stop()
    //    {
    //        //_recognizer.Dispose();
    //        //_recognizer.UnloadAllGrammars();
    //        //_recognizer.RecognizeAsyncCancel();

    //        if (_synthesizer != null)
    //        {
    //            _synthesizer.SpeakAsyncCancelAll();
    //        }

    //        if (_recognizer != null)
    //        {
    //            _recognizer.RecognizeAsyncStop();
    //        }
    //    }

    //    private void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    //    {
    //        if (e.Result.Confidence < 0.7)
    //        {
    //            return;
    //        }
            
    //        EventHandler<SpeechEventArgs> handler = null;
    //        SpeechEventArgs args = null;

    //        if (e.Result.Grammar.Name == GRAMMAR_APPLICATION)
    //        {
    //            handler = PlantSelected;
    //            args = new SpeechEventArgs { Command = e.Result.Words[1].Text };
                
    //            //HandleApplicationCommand(e.Result.Text);
    //        }
    //        else
    //        {
    //            if (e.Result.Grammar.Name == GRAMMAR_PLANT || e.Result.Words.Count == 2)
    //            {
    //                HandlePlantSelection(e.Result);
    //            }

    //            Speak(_currentPhrase);
    //        }

    //        //_recognizer.EmulateRecognizeAsync(e.Result.Text);
    //    }

    //    //private void HandlePlantSelection(RecognitionResult result)
    //    //{
    //    //    var callback = PlantSelected;

    //    //    int id;

    //    //    if (callback != null && int.TryParse(result.Words[1].Text, out id))
    //    //    {
    //    //        _pendingAction = callback;
    //    //        _pendingActionArgs = new PlantSelectedEventArgs { Id = id };
    //    //    }

    //    //    _currentPhrase = result.Text;
    //    //}

    //    private void HandleApplicationCommand(string command)
    //    {
    //        switch (command)
    //        {
    //            case APPLICATION_COMMAND_ACCEPT:
    //                if (_pendingAction != null && _pendingActionArgs != null)
    //                {
    //                    _pendingAction(this, _pendingActionArgs);

    //                    Speak("Command accepted");

    //                    _pendingAction = null;
    //                    _pendingActionArgs = null;
    //                }
    //                break;
    //            case APPLICATION_COMMAND_REPEAT:
    //                Speak(_currentPhrase);
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        Stop();

    //        if (_synthesizer != null)
    //        {
    //            _synthesizer.Dispose();
    //            _synthesizer = null;
    //        }

    //        if (_recognizer != null)
    //        {
    //            _recognizer.Dispose();
    //            _recognizer = null;
    //        }
    //    }
    //}
}
