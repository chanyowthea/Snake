using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstValue
{
    // game value
    /// <summary>
    /// default ratio of strong body length. 
    /// </summary>
    public const float _DefaultStrongRatio = 0.3333f;

    public const int _OneBodyScores = 8;
    public const int _StrongBodyScores = (int)(_OneBodyScores * 1.25f);
    public const int _HeadScores = _OneBodyScores * 2;
    public const int _DefaultBodyLength = 2;
    public const int _MinusScorePerDie = _OneBodyScores * _DefaultBodyLength * 2;
    public const int _ScoreUnit = 1;
    public const float _KillScoresRatio = 0.2f;
    public const float _BodyUnitSize = 0.5f;
    public const float _RaceTime0 = 10f;

    // render value
    public const int _FoodMaskSortingLayer = 400;
    public const int _BodyMaskSortingLayer = 900;

    // system value
    public const int _MaxLoopTime = 100;
}
