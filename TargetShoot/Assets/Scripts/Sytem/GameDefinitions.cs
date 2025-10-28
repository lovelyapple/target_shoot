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
    public static class GameConstant
    {
        public const int ApplicationVersion = 1;
        public const float MoveLimit = 3f;
        public const float MoveSpeed = 2f;
        public const float TargetPlateDistance = 0.5f;
        public const int HighScoreTargetProbability = 10;
        public const int DownTargetProbability = 20;
        public const int MaxProbability = 100;
        public const float TargetMoveBaseSpeed = 0.2f;
        public const float BulletSpeed = 5f;
        public const int DefaultScore = 50;

        public const float TargetDispearLimitHeight = -2f;
        public const float TargetReviveInterval = 2f;
    }
}