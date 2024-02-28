﻿using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;

namespace LethalVocabulary;

public class SpeechRecognizer {
    public static readonly HashSet<string> DefaultCurseWordsSet = new() {
        "fuck", "fucker", "fucking", "motherfucker",
        "shit", "shitter", "shitting", "bullshit",
        "bitch", "bitching",
        "ass", "asshole",
        "cock", "dick",
        "bastard", "pussy", "cunt", "crap"
    };

    private readonly SpeechRecognitionEngine _recognizer = new();
    private Grammar _triggerGrammar;

    public SpeechRecognizer () {
        _recognizer.SetInputToDefaultAudioDevice();
    }

    public void AddSpeechRecognizedHandler (EventHandler<SpeechRecognizedEventArgs> eventHandler) {
        _recognizer.SpeechRecognized += eventHandler;
    }

    public void LoadAndStart (HashSet<string> triggerWords) {
        _triggerGrammar = new Grammar(CreateSrgs(triggerWords));
        _recognizer.LoadGrammar(_triggerGrammar);
        _recognizer.RecognizeAsync(RecognizeMode.Multiple);
    }

    public void StopAndUnload () {
        _recognizer.RecognizeAsyncStop();
        _recognizer.UnloadGrammar(_triggerGrammar);
    }

    public static SrgsDocument CreateSrgs (HashSet<string> triggerWords) {
        SrgsRule rule = new("Rule");
        SrgsOneOf words = new(new SrgsItem("word"));

        foreach (string word in triggerWords) words.Add(new SrgsItem(word));

        rule.Add(new SrgsItem(1, 1, SrgsRuleRef.Dictation));
        rule.Add(new SrgsItem(1, 1, words));
        rule.Add(new SrgsItem(0, 1, SrgsRuleRef.Dictation));

        return new SrgsDocument {
            Rules = { rule },
            Root = rule
        };
    }
}