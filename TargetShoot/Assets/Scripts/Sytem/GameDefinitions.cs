using System.Collections.Generic;

namespace GameDefinition
{
    public enum SceneType
    {
        Transition_Internal,
        Title,
        Game,
    }
    public enum FieldTargetType
    {
        ScoreTarget,
        CharaTarget,
    }
    public enum GameMode
    {
        Survie,
        Arcade,
    }
    public static class GameConstant
    {
        public const int ApplicationVersion = 1;

        // 物理関連
        public const float MoveLimit = 3f;
        public const float MoveSpeed = 2.3f;
        public const float TargetPlateDistance = 0.5f;
        public const float TargetMoveBaseSpeed = 0.2f;
        public const float BulletSpeed = 5f;
        public const float TargetDispearLimitHeight = -2f;

        // ゲームバランス
        public const int HighScoreTargetProbability = 10;
        public const int DownTargetProbability = 20;
        public const int MaxProbability = 100;
        public const int FrameTargetProbability = 15;

        public const int MatchTime = 90;

        // スコアー関連
        public const int FireCost = 1;
        public const int DefaultScore = 100;
        public const float TargetReviveInterval = 2f;
        public const int BulletComboBonusScoreTimes = 2;
        public const int ScoreComboOnCatchTargetStep = 2;

        public const int ComboLastSec = 3;

        public static float GetComboTimes(int combo)
        {
            if(combo >= 10)
            {
                return 3f;
            }
            else if(combo >= 8)
            {
                return 2.5f;
            }
            else if (combo >= 6)
            {
                return 2.0f;
            }
            else if (combo >= 4)
            {
                return 1.6f;
            }
            else if (combo >= 2)
            {
                return 1.4f;
            }
            else if (combo >= 1)
            {
                return 1.2f;
            }

            return 1;
        }
    }
}