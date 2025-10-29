using System;
using GameDefinition;
using R3;
using UnityEngine;
public class MatchModel : IModel
{
    public GameMode CurrentGameModel;
    public bool IsSurvieMode => CurrentGameModel == GameMode.Survie;
    public bool IsArcadeMode => CurrentGameModel == GameMode.Arcade;
    public void Reset()
    {
    }
    public void OnMatchStart()
    {

    }
    public void OnMatchFinished(long result)
    {

    }
}
