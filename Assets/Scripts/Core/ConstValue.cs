using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstValue
{
    /// <summary>
    /// default ratio of strong body length. 
    /// </summary>
    public const float _DefaultStrongRatio = 0.3333f;
    public const int _OneBodyScores = 8;
    public const int _StrongBodyScores = (int)(_OneBodyScores * 1.25f);
    public const int _HeadScores = _OneBodyScores * 2;
    public const int _DefaultBodyLength = 2;
    public const int _MinusScorePerDie = _OneBodyScores * _DefaultBodyLength * 5;
    public const int _ScoreUnit = 1;
}
